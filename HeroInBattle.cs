using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum StateHeroInBattle { Melee, Shoot, Cast, Drink, Healing, Rest, Dead, Resurrection, None }// Состояние героя в бою

    internal sealed class HeroInBattle
    {
        public HeroInBattle(Battle b, PlayerHero ph, Point coord)
        {
            Battle = b;
            PlayerHero = ph;
            Coord = coord;
            IsLive = true;

            Parameters = new HeroParameters(ph.ParametersWithAmmunition);
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

        // Делает шаг битвы
        internal void DoStepBattle(Battle b)
        {
            Debug.Assert(IsLive == true);

            switch (State)
            {
                case StateHeroInBattle.None:
                    Debug.Assert(Target == null);

                    // Если сейчас ничего не выполняем, ищем, что можно сделать
                    // Сначала атакуем
                    if (SearchTargetForMelee() == false)
                    {

                    }

                    break;
                case StateHeroInBattle.Melee:
                    break;
                default:
                    break;
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
                    Target = targets[0];
                    State = StateHeroInBattle.Melee;

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

        }
    }
}
