using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
            IsLive = true;

            // Инициализируем ресурсы               
            Resources = new int[FormMain.Config.Resources.Count];
            for (int i = 0; i < fraction.StartResources.Count(); i++)
            {
                Resources[i] = fraction.StartResources[i];
            }

            // Инициализация зданий
            ExternalBuildings = new List<BuildingOfPlayer>[FormMain.Config.ExternalBuildings.Count];

            foreach (Building b in FormMain.Config.ExternalBuildings)
            {
                ExternalBuildings[b.Position] = new List<BuildingOfPlayer>
                {
                    new BuildingOfPlayer(b),
                    new BuildingOfPlayer(b),
                };
            }

            Chieftain = new Chieftain(this);
        }

        internal void DoTurn()
        {
            Debug.Assert(TypePlayer == TypePlayer.Computer);
            Debug.Assert(IsLive == true);
            
            // Здесь расчет хода для ИИ
        }

        internal void CalcResultTurn()
        {
            Debug.Assert(IsLive == true);

            for (int i = 0; i < ExternalBuildings.Count(); i++)
            {
                foreach (BuildingOfPlayer bp in ExternalBuildings[i])
                {
                    for (int r = 0; r < FormMain.Config.Resources.Count(); r++)
                        Resources[r] += bp.Building.IncomeResources[r];
                }
            }
            //Resources[0] += 1000;
        }

        internal string Name { get; }
        internal Fraction Fraction { get; }
        internal TypePlayer TypePlayer { get; }
        internal Chieftain Chieftain { get; }
        internal int[] Resources { get; }
        internal int Wins { get; }
        internal int Loses { get; }
        internal int StepsToCastle { get; }
        internal bool IsLive { get; }
        internal List<BuildingOfPlayer>[] ExternalBuildings {get;}

        internal PanelAboutPlayer PanelAbout { get; set; }
    }
}
