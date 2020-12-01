using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum StateHeroInBattle { Melee, Archery, Cast, Drink, Healing, Rest, Resurrection, Tumbstone, Dead, 
        PrepareMove, Move, None }// Состояние героя в бою

    internal sealed class HeroInBattle
    {
        private int countAction;// Счетчик действия
        private int timeAction;// Какое количество времени выполнения действие
        private bool inRollbackAfterAction;// Герой во время отката после выполнения действия        
        private HeroInBattle lastAttackedHero;
        private BattlefieldTile currentTile;

        public HeroInBattle(Battle b, PlayerHero ph, Point coord)
        {
            Battle = b;
            CurrentTile = b.Battlefield.Tiles[coord.Y, coord.X];            
            PlayerHero = ph;
            IsLive = true;

            Parameters = new HeroParameters(ph.ParametersWithAmmunition);

            Debug.Assert(Parameters.Health > 0);
            Debug.Assert(Parameters.Mana > 0);
            Debug.Assert(Parameters.Stamina > 0);

            CurrentHealth = Parameters.Health;
            CurrentMana = Parameters.Mana;
            CurrentStamina = Parameters.Stamina;

            State = StateHeroInBattle.None;

            LastTarget = default;
        }

        internal PlayerHero PlayerHero { get; }
        internal Player Player => PlayerHero.Player;
        internal HeroParameters Parameters { get; }
        internal Battle Battle { get; }
        internal bool IsLive { get; set; }
        internal StateHeroInBattle State { get; private set; }
        internal HeroInBattle Target { get; private set; }
        internal Point LastTarget { get; private set; }
        internal int CurrentHealth { get; set; }
        internal int CurrentMana { get; set; }
        internal int CurrentStamina { get; set; }
        internal int ReceivedDamage { get; private set; }

        //
        internal Point Coord { get { return new Point(currentTile.X, currentTile.Y); } }
        internal BattlefieldTile CurrentTile
        {
            get { return currentTile; }
            set
            {
                Debug.Assert(value != null);

                if (currentTile != null)
                    currentTile.Unit = null;

                currentTile = value;
                currentTile.Unit = this;
            }
        }
        internal BattlefieldTile DestinationForMove { get; set; }
        internal BattlefieldTile TileForMove { get; set; }
        internal List<BattlefieldTile> PathToDestination;
        //internal int MoveStepPaa

        // Делает шаг битвы
        internal void DoStepBattle(Battle b)
        {
            Debug.Assert(IsLive == true);
            Debug.Assert(CurrentTile != null);

            if (inRollbackAfterAction == false)
            {
                switch (State)
                {
                    case StateHeroInBattle.None:
                        Debug.Assert(Target == null);
                        Debug.Assert(countAction == 0);
                        Debug.Assert(CurrentHealth > 0);
                        Debug.Assert(IsLive == true);

                        // Если сейчас ничего не выполняем, ищем, что можно сделать
                        // Сначала атакуем
                        switch (PlayerHero.ClassHero.KindHero.TypeAttack)
                        { 
                            case TypeAttack.Melee:                            
                                if (SearchTargetForMelee() == false)
                                {
                                    // Ищем цель на своей линии
                                    //if (Search)
                                }
                            break;
                            case TypeAttack.Archer:
                            case TypeAttack.Magic:
                                if (SearchTargetForShoot() == false)
                                {
                                    // Ищем цель на своей линии
                                    //if (Search)
                                }
                                break;
                        }

                        // Если целей нет, идем к ней
                        if (Target == null)
                        {
                            if (SearchTargetForMove() == true)
                            {
                                State = StateHeroInBattle.Move;
                                countAction = (int)(TimeMove() * 1.00 * ((TileForMove.X != 0) && (TileForMove.Y != 0) ? 1.4 : 1));
                                timeAction = countAction;
                                inRollbackAfterAction = false;
                                //State = StateHeroInBattle.PrepareMove;
                            }
                        }

                        break;
                    case StateHeroInBattle.Melee:
                    case StateHeroInBattle.Archery:
                    case StateHeroInBattle.Cast:
                        countAction--;

                        if (Target.State != StateHeroInBattle.Tumbstone)
                        {
                            if (countAction == 0)
                            {
                                // Делаем удар по противнику
                                Target.GetDamage(CalcDamageMelee(Target), CalcDamageShoot(Target), CalcDamageMagic(Target));
                                LastTarget = Target.Coord;
                                Target = null;

                                // После удара делаем паузу длиной во время атаки
                                countAction = TimeAttack();
                                inRollbackAfterAction = true;
                            }
                        }
                        else
                        {
                            // Противника уже убили, пропускаем ход
                            LastTarget = Target.Coord;
                            Target = null;
                            State = StateHeroInBattle.None;
                            countAction = timeAction - countAction;
                            timeAction = countAction;
                            inRollbackAfterAction = true;
                        }

                        break;
                    case StateHeroInBattle.Tumbstone:
                        Debug.Assert(Target == null);

                        countAction--;
                        if (countAction == 0)
                        {
                            IsLive = false;
                            State = StateHeroInBattle.Dead;
                            currentTile.Unit = null;
                            currentTile = null;
                        }

                        break;
                    case StateHeroInBattle.Move:
                        Debug.Assert(TileForMove != null);
                        Debug.Assert(TileForMove.Unit == null);

                        countAction--;
                        if (countAction == 0)
                        {
                            Debug.Assert(TileForMove.ReservedForMove == this);

                            // Пришли на тайл
                            Target = null;
                            CurrentTile = TileForMove;
                            TileForMove.ReservedForMove = null;
                            TileForMove = null;

                            if (PathToDestination.Count() > 1)
                            {
                                // Заново осматриваемся
                                State = StateHeroInBattle.None;

                                /*TileForMove = PathToDestination.First();
                                PathToDestination.RemoveAt(0);

                                countAction = TimeMove();
                                timeAction = countAction;*/
                            }
                            else
                            {
                                // Пришли на конечный тайл
                                State = StateHeroInBattle.None;
                            }
                            PathToDestination = null;
                            DestinationForMove = null;
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
                {
                    LastTarget = default;
                    State = StateHeroInBattle.None;
                    inRollbackAfterAction = false;
                }
            }

            bool SearchTargetForMelee()
            {
                Debug.Assert(PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Melee);

                // Ищем, кого атаковать
                List<HeroInBattle> targets = new List<HeroInBattle>();

                foreach (HeroInBattle h in b.ActiveHeroes)
                {
                    // Собираем список вражеских героев вокруг себя
                    if (h.IsLive == true)
                        if (h.Player != Player)
                            if (h.CurrentHealth > 0)
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

            bool SearchTargetForShoot()
            {
                Debug.Assert((PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Archer) || (PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Magic));

                // Если герой, по которому стреляли, жив, атакуем его снова
                if ((lastAttackedHero != null) && (lastAttackedHero.CurrentHealth > 0))
                {
                    Target = lastAttackedHero;
                }
                else
                {
                    // Ищем, кого атаковать
                    List<HeroInBattle> targets = new List<HeroInBattle>();

                    foreach (HeroInBattle h in b.ActiveHeroes)
                    {
                        // Собираем список вражеских героев
                        if (h.Player != Player)
                            if (h.CurrentHealth > 0)
                                targets.Add(h);
                    }

                    if (targets.Count > 0)
                    {
                        Debug.Assert(this != targets[0]);
                        Target = targets[0];// targets[Battle.Rnd.Next(0, targets.Count - 1)];
                    }
                }

                if (Target != null)
                { 
                    if (PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Archer)
                        State = StateHeroInBattle.Archery;
                    else
                        State = StateHeroInBattle.Cast;
                    countAction = TimeAttack();
                    timeAction = countAction;
                    lastAttackedHero = Target;

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

            if (State != StateHeroInBattle.Tumbstone)
            {
                if (CurrentHealth <= 0)
                {
                    LastTarget = default;
                    Target = null;
                    DestinationForMove = null;

                    if (TileForMove != null)
                    {
                        TileForMove.ReservedForMove = null;
                        TileForMove = null;
                    }
                    State = StateHeroInBattle.Tumbstone;
                    countAction = FormMain.Config.StepsHeroInTumbstone;
                    timeAction = countAction;
                    CurrentHealth = 0;
                    inRollbackAfterAction = false;
                }
            }

            ReceivedDamage = 0;

            Debug.Assert(CurrentHealth <= Parameters.Health);
            Debug.Assert(CurrentMana <= Parameters.Mana);
            Debug.Assert(CurrentStamina <= Parameters.Stamina);
        }

        private int TimeAttack()
        {
            int timeAttack = (int)(PlayerHero.Weapon.TimeHit / 100.00 * FormMain.Config.StepsInSecond);

            Debug.Assert(timeAttack > 0);

            return timeAttack;
        }

        private int CalcDamageMelee(HeroInBattle target)
        {
            int delta = Parameters.MaxMeleeDamage - Parameters.MinMeleeDamage;
            int value = FormMain.Rnd.Next(delta);

            int d = Parameters.MaxMeleeDamage;// Parameters.MinMeleeDamage + value;

            return d;
        }

        private int CalcDamageShoot(HeroInBattle target)
        {
            int delta = Parameters.MaxArcherDamage - Parameters.MinArcherDamage;
            int value = FormMain.Rnd.Next(delta);

            int d = Parameters.MaxArcherDamage;// Parameters.MinArcherDamage + value;

            return d;
        }
        private int CalcDamageMagic(HeroInBattle target)
        {
            int d = Parameters.MagicDamage;

            return d;
        }

        internal void GetDamage(int damageMelee, int damageArcher, int damageMagic)
        {
            Debug.Assert((damageMelee > 0) || (damageArcher > 0) || (damageMagic > 0));
            Debug.Assert(State != StateHeroInBattle.Tumbstone);
            Debug.Assert(State != StateHeroInBattle.Dead);
            Debug.Assert(State != StateHeroInBattle.Resurrection);

            ReceivedDamage += damageMelee + damageArcher + damageMagic;
        }

        internal void CalcParameters()
        {
            // Расчет надо делать через модифицированные основные параметры, после которых уже применяются баффы/дебаффы на урон
            // Сейчас для простоты берем уже посчитанные параметры с амуницией
            Parameters.MinMeleeDamage = PlayerHero.ParametersWithAmmunition.MinMeleeDamage;
            Parameters.MaxMeleeDamage = PlayerHero.ParametersWithAmmunition.MaxMeleeDamage;
            Parameters.MinArcherDamage = PlayerHero.ParametersWithAmmunition.MinArcherDamage;
            Parameters.MaxArcherDamage = PlayerHero.ParametersWithAmmunition.MaxArcherDamage;
            Parameters.MagicDamage = PlayerHero.ParametersWithAmmunition.MagicDamage;

            Debug.Assert(Parameters.MinMeleeDamage <= Parameters.MaxMeleeDamage);
            Debug.Assert(Parameters.MinArcherDamage <= Parameters.MaxArcherDamage);
        }

        internal double PercentExecuteAction()
        {
            Debug.Assert(timeAction > 0);
            Debug.Assert(countAction > 0);
            double percent = 1.00 * (timeAction - countAction) / timeAction;

            Debug.Assert(percent >= 0);
            Debug.Assert(percent <= 1);
            return percent;
        }

        internal bool InRollbackAction()
        {
            return inRollbackAfterAction;
        }

        private bool SearchTargetForMove()
        {
            // Ищем ближайшего противника
            // Здесь переделать на перебор всех противников, до которых можно добраться, и учесть их вес
            foreach (HeroInBattle h in Battle.ActiveHeroes)
                if ((h.Player != Player) && (h.currentTile != null) && (h.State != StateHeroInBattle.Tumbstone))
                {
                    if (Battle.Battlefield.Pathfind(CurrentTile, h.currentTile) == true)
                    {
                        PathToDestination = Battle.Battlefield._path;
                        DestinationForMove = PathToDestination.Last();
                        Debug.Assert(DestinationForMove == h.currentTile);
                        TileForMove = PathToDestination.First();
                        Debug.Assert(TileForMove.Unit == null);
                        Debug.Assert(TileForMove.ReservedForMove == null);
                        Debug.Assert(Utils.PointsIsNeighbor(Coord, new Point(TileForMove.X, TileForMove.Y)) == true);

                        Debug.Assert(TileForMove.ReservedForMove == null);
                        TileForMove.ReservedForMove = this;

                        return true;
                    }
                }
            //foreach (BattlefieldTile t in curTile.TilesAround)
            //{
            //    if (SearchAround(t) == true)
            //        return true;
            //}

            return false;

            bool SearchAround(BattlefieldTile tile)
            {
                foreach (BattlefieldTile t in tile.TilesAround)
                {
                    if (t.Unit != null)
                        if (t.Unit.Player != Player)
                            if (t.Unit.IsLive == true)
                            // if ((t.Unit.IsLive == true) && (t.Unit.State != StateHeroInBattle.Move))
                            {
                                // Смотрим, можно ли к нему построить путь
                                if (Battle.Battlefield.Pathfind(CurrentTile, t.Unit.currentTile) == true)
                                {
                                    PathToDestination = Battle.Battlefield._path;
                                    DestinationForMove = PathToDestination.Last();
                                    return true;
                                }
                            }
                }

                return false;
            }
        }

        private void Move()
        {

        }

        private int TimeMove()
        {
            int timeMove = (int)(PlayerHero.ParametersWithAmmunition.SecondsToMove / 100.00 * FormMain.Config.StepsInSecond);
            if (timeMove == 0)
                timeMove = 1 * FormMain.Config.StepsInSecond;
            //timeMove = 3 * FormMain.Config.StepsInSecond;
            Debug.Assert(timeMove > 0);
            return timeMove;
        }
    }
}