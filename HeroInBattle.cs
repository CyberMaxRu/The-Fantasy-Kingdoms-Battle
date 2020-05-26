using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum StateHeroInBattle { Melee, Shoot, Cast, Drink, Healing, Rest, Resurrection, Dead, None }// Состояние героя в бою

    internal sealed class HeroInBattle
    {
        private int countAction;// Счетчик действия
        private int timeAction;// Какое количество времени выполнения действие
        private bool inRollbackAfterAction;// Герой во время отката после выполнения действия

        public HeroInBattle(Battle b, PlayerHero ph, Point coord)
        {
            Battle = b;
            PlayerHero = ph;
            Coord = coord;
            IsLive = true;

            Parameters = new HeroParameters(ph.ParametersWithAmmunition);

            Debug.Assert(Parameters.Health > 0);
            Debug.Assert(Parameters.Mana > 0);
            Debug.Assert(Parameters.Stamina > 0);

            CurrentHealth = Parameters.Health;
            CurrentMana = Parameters.Mana;
            CurrentStamina = Parameters.Stamina;

            State = StateHeroInBattle.None;
        }

        internal PlayerHero PlayerHero { get; }
        internal Player Player => PlayerHero.Player;
        internal HeroParameters Parameters { get; }
        internal Battle Battle { get; }
        internal bool IsLive { get; set; }
        internal Point Coord { get; set; }
        internal StateHeroInBattle State { get; private set; }
        internal HeroInBattle Target { get; private set; }
        internal int CurrentHealth { get; set; }
        internal int CurrentMana { get; set; }
        internal int CurrentStamina { get; set; }
        internal int ReceivedDamage { get; private set; }

        // Делает шаг битвы
        internal void DoStepBattle(Battle b)
        {
            Debug.Assert(IsLive == true);

            if (inRollbackAfterAction == false)
            {
                switch (State)
                {
                    case StateHeroInBattle.None:
                        Debug.Assert(Target == null);
                        Debug.Assert(countAction == 0);

                        // Если сейчас ничего не выполняем, ищем, что можно сделать
                        // Сначала атакуем
                        if (SearchTargetForMelee() == false)
                        {

                        }

                        break;
                    case StateHeroInBattle.Melee:
                        countAction--;

                        if (Target.IsLive == true)
                        {
                            if (countAction == 0)
                            {
                                // Делаем удар по противнику
                                Target.GetDamage(CalcDamageMelee(Target), CalcDamageShoot(Target), CalcDamageMagic(Target));
                                Target = null;

                                // После удара делаем паузу длиной во время атаки
                                countAction = TimeAttack();
                                inRollbackAfterAction = true;
                            }
                        }
                        else
                        {
                            // Противника уже убили, пропускаем ход
                            Target = null;
                            State = StateHeroInBattle.None;
                            countAction = timeAction - countAction;
                            timeAction = countAction;
                            inRollbackAfterAction = true;
                        }

                        break;
                    default:
                        break;
                }
            }
            else
            {
                countAction--;
                if (countAction == 0)
                    State = StateHeroInBattle.None;

            }

            bool SearchTargetForMelee()
            {
                Debug.Assert(PlayerHero.ClassHero.CategoryHero == CategoryHero.Melee);

                // Ищем, кого атаковать
                List<HeroInBattle> targets = new List<HeroInBattle>();

                foreach (HeroInBattle h in b.ActiveHeroes)
                {
                    // Собираем список вражеских героев вокруг себя
                    if (h.Player != Player)
                        if (IsNeighbour(h) == true)
                            targets.Add(h);
                }

                if (targets.Count > 0)
                {
                    Debug.Assert(this != targets[0]);

                    State = StateHeroInBattle.Melee;
                    Target = targets[0];
                    countAction = TimeAttack();
                    timeAction = countAction;

                    return true;
                }
                else
                    return false;
            }

            bool IsNeighbour(HeroInBattle hb)
            {
                Debug.Assert(this != hb);

                return Utils.PointsIsNeighbor(Coord, hb.Coord);
            }
        }

        // Применяем шаг битвы
        internal void ApplyStepBattle()
        {
            CurrentHealth -= ReceivedDamage;

            if (CurrentHealth <= 0)
            {
                IsLive = false;
                CurrentHealth = 0;
                State = StateHeroInBattle.Dead;
            }

            Debug.Assert(CurrentHealth <= Parameters.Health);
            Debug.Assert(CurrentMana <= Parameters.Mana);
            Debug.Assert(CurrentStamina <= Parameters.Stamina);
        }

        private int TimeAttack()
        {
            int timeAttack = PlayerHero.Weapon.Item.TimeHit / 100 * Config.STEPS_IN_SECOND;

            Debug.Assert(timeAttack > 0);

            return timeAttack;
        }

        private int CalcDamageMelee(HeroInBattle target)
        {
            int delta = Parameters.MaxMeleeDamage - Parameters.MinMeleeDamage;
            int value = FormMain.Rnd.Next(delta);

            int d = Parameters.MinMeleeDamage + value;
            return d;
        }

        private int CalcDamageShoot(HeroInBattle target)
        {
            return 0;
        }
        private int CalcDamageMagic(HeroInBattle target)
        {
            return 0;
        }

        internal void GetDamage(int damageMelee, int damageMissile, int damageMagic)
        {
            Debug.Assert(damageMelee >= 0);
            Debug.Assert(damageMissile >= 0);
            Debug.Assert(damageMagic >= 0);

            ReceivedDamage += damageMelee + damageMissile + damageMagic;
        }

        internal void CalcParameters()
        {
            // Расчет надо делать через модифицированные основные параметры, после которых уже применяются баффы/дебаффы на урон
            // Сейчас для простоты берем уже посчитанные параметры с амуницией
            Parameters.MinMeleeDamage = PlayerHero.ParametersWithAmmunition.MinMeleeDamage;
            Parameters.MaxMeleeDamage = PlayerHero.ParametersWithAmmunition.MaxMeleeDamage;

            Debug.Assert(Parameters.MinMeleeDamage <= Parameters.MaxMeleeDamage);
            Debug.Assert(Parameters.MinMissileDamage <= Parameters.MaxMissileDamage);
        }
    }
}