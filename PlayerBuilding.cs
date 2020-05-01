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

        internal void UpdatePanel()
        {
            Building.Panel.ShowData(this);
        }

        internal void BuyOrUpgrade()
        {
            Debug.Assert(Level < Building.MaxLevel);

            if (CheckRequirements() == true)
            {
                Player.Gold -= Building.Levels[Level + 1].Cost;
                Level++;
            }
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
    }
}