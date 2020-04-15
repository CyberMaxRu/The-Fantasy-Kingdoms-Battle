using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal enum TypePlayer { Human, Computer };
    // Класс игрока
    internal sealed class Player
    {
        public Player(string name, Fraction fraction, TypePlayer typePlayer)
        {
            Name = name;
            Fraction = fraction;
            TypePlayer = typePlayer;
            StepsToCastle = 5;
            Wins = 0;
            Loses = 0;

            // Инициализируем ресурсы               
            Resources = new int[FormMain.Config.Resources.Count];
            foreach (Resource r in FormMain.Config.Resources)
            {
                if (r.ID == "Gold")
                {
                    Resources[r.Position] = 10000;
                }
                else
                {
                    Resources[r.Position] = 10;
                }
            }

            // Инициализация зданий
            ExternalBuildings = new List<BuildingOfPlayer>[FormMain.Config.ExternalBuildings.Count];

            foreach (Building b in FormMain.Config.ExternalBuildings)
            {
                ExternalBuildings[b.Position] = new List<BuildingOfPlayer>();
            }
        }

        internal string Name { get; }
        internal Fraction Fraction { get; }
        internal TypePlayer TypePlayer { get; }
        internal int[] Resources { get; }
        internal int Wins { get; }
        internal int Loses { get; }
        internal int StepsToCastle { get; }
        internal List<BuildingOfPlayer>[] ExternalBuildings {get;}

        internal PanelAboutPlayer PanelAbout { get; set; }
    }
}
