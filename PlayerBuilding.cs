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
            Debug.Assert(Level < Building.MaxLevel);
            Debug.Assert(Player.Gold >= CostBuyOrUpgrade());

            if (CheckRequirements() == true)
            {
                Player.Gold -= CostBuyOrUpgrade();
                Level++;
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

        internal bool CheckRequirements()
        {
            // Сначала проверяем наличие золота
            if (Player.Gold < Building.Levels[Level + 1].Cost)
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
            return Level > 0 ? Building.Levels[Level].Income : 0;
        }

        internal PlayerHero HireHero()
        {
            Debug.Assert(Heroes.Count < Building.MaxHeroes);
            Debug.Assert(Player.Heroes.Count < FormMain.MAX_HEROES_AT_PLAYER);
            Debug.Assert(Player.Gold >= Building.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this);
            Heroes.Add(h);
            Player.Heroes.Add(h);
            Player.Gold -= Building.TrainedHero.Cost;

            Program.formMain.ShowGold();

            return h;
        }

        internal bool CanTrainHero()
        {
            Debug.Assert(Level > 0);
            Debug.Assert(Heroes.Count <= Building.MaxHeroes);
            Debug.Assert(Player.Heroes.Count <= FormMain.MAX_HEROES_AT_PLAYER);

            return (Player.Gold >= Building.TrainedHero.Cost) && (Heroes.Count < Building.MaxHeroes) && (Player.Heroes.Count < FormMain.MAX_HEROES_AT_PLAYER);
        }

        internal List<TextRequirement> GetTextRequirementsHire()
        {
            if (Level == 0)
                return null;

            List<TextRequirement> list = new List<TextRequirement>();

            if (Heroes.Count == Building.MaxHeroes)
                list.Add(new TextRequirement(false, "Гильдия заполнена"));

            if (Player.Heroes.Count == FormMain.MAX_HEROES_AT_PLAYER)
                list.Add(new TextRequirement(false, "Достигнуто максимальное количество героев"));

            return list;
        }
    }
}