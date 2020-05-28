using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс здания игрока
    internal sealed class PlayerBuilding
    {
        public PlayerBuilding(Player p, Building b)
        {
            Player = p;
            Building = b;

            Level = b.DefaultLevel;
        }

        internal Player Player { get; }
        internal Building Building { get; }
        internal int Level { get; private set; }
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();

        internal void UpdatePanel()
        {
            Building.Panel.ShowData(this);
        }

        internal void BuyOrUpgrade()
        {
            if ((Level < Building.MaxLevel) && (Player.Gold >= CostBuyOrUpgrade()) && (CheckRequirements() == true))
            {
                Debug.Assert(Player.Gold >= CostBuyOrUpgrade());
                Debug.Assert(Player.FreeBuilders >= Builders());

                Player.Gold -= CostBuyOrUpgrade();
                Player.FreeBuilders -= Builders();
                Level++;
                ValidateHeroes();
            }
        }

        internal void ValidateHeroes()
        {
            if ((Building.TrainedHero != null) && (Building.TrainedHero.Cost == 0))
            {
                if (Heroes.Count() < MaxHeroes())
                {
                    for (; Heroes.Count() < MaxHeroes();)
                    {
                        HireHero();
                    }
                }
            }
        }

        internal bool CanLevelUp()
        {
            return Level < Building.MaxLevel;
        }

        internal int CostBuyOrUpgrade()
        {
            return CanLevelUp() == true ? Building.Levels[Level + 1].Cost : 0;
        }

        internal int Builders()
        {
            return CanLevelUp() == true ? Building.Levels[Level + 1].Builders : 0;
        }

        internal bool CheckRequirements()
        {
            // Сначала проверяем наличие золота
            if (Player.Gold < Building.Levels[Level + 1].Cost)
                return false;

            // Проверяем наличие строителей
            if (Player.FreeBuilders < Building.Levels[Level + 1].Builders)
                return false;

            // Проверяем требования к зданиям
            PlayerBuilding pb;
            foreach (Requirement r in Building.Levels[Level + 1].Requirements)
            {
                pb = Player.GetPlayerBuilding(r.Building);
                if (r.Level > pb.Level)
                    return false;
            }

            return true; 
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            if (Level == Building.MaxLevel)
                return null;

            List <TextRequirement> list = new List<TextRequirement>();
            PlayerBuilding pb;

            foreach (Requirement r in Building.Levels[Level + 1].Requirements)
            {
                pb = Player.GetPlayerBuilding(r.Building);
                list.Add(new TextRequirement(r.Level <= pb.Level, pb.Building.Name + (r.Level > 1 ? " " + r.Level + " уровня" : "")));
            }

            return list;
        }

        internal int Income()
        {
            int coef;
            switch (Building.TypeIncome)
            {
                case TypeIncome.None:
                    coef = 0;
                    break;
                case TypeIncome.Persistent:
                    coef = 1;
                    break;
                case TypeIncome.PerHeroes:
                    coef = Heroes.Count;
                    break;
                default:
                    throw new Exception("Неизвестный тип дохода.");
            }

            return Level > 0 ? Building.Levels[Level].Income * coef : 0;
        }

        internal bool DoIncome()
        {
            return Building.Levels[1].Income > 0;
        }

        internal int IncomeNextLevel()
        {
            return Level < Building.MaxLevel ? Building.Levels[Level + 1].Income : 0;
        }

        internal PlayerHero HireHero()
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            Debug.Assert(Player.Gold >= Building.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this);
            Heroes.Add(h);
            Player.AddHero(h);

            if (Building.TrainedHero.Cost > 0)
            {
                Player.Gold -= Building.TrainedHero.Cost;
                Program.formMain.ShowGold();
            }

            return h;
        }

        internal bool CanTrainHero()
        {
            Debug.Assert(Heroes.Count <= MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count <= Player.Lobby.TypeLobby.MaxHeroes);

            return (Level > 0) && (Player.Gold >= Building.TrainedHero.Cost) && (Heroes.Count < MaxHeroes()) && (Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
        }

        internal bool MaxHeroesAtPlayer()
        {
            return Player.CombatHeroes.Count == Player.Lobby.TypeLobby.MaxHeroes;
        }

        internal List<TextRequirement> GetTextRequirementsHire()
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Level == 0)
                list.Add(new TextRequirement(false, Building.CategoryBuilding == CategoryBuilding.Guild ? "Гильдия не построена" : "Храм не построен"));

            if ((Level > 0) && (Heroes.Count == MaxHeroes()))
                list.Add(new TextRequirement(false, Building.CategoryBuilding == CategoryBuilding.Guild ? "Гильдия заполнена" : "Храм заполнен"));

            if (MaxHeroesAtPlayer())
                list.Add(new TextRequirement(false, "Достигнуто максимальное количество героев в королевстве"));

            return list;
        }      

        internal int MaxHeroes()
        {
            return Level > 0 ? Building.Levels[Level].MaxHeroes : 0;
        }
    }
}