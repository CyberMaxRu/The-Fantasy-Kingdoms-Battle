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
        public Player(Lobby lobby, string name, Fraction fraction, TypePlayer typePlayer)
        {
            Lobby = lobby;
            Name = name;
            Fraction = fraction;
            TypePlayer = typePlayer;
            StepsToCastle = 5;
            Wins = 0;
            Loses = 0;
            IsLive = true;
            Position = lobby.Players.Count();

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

            //
            if (IsLive == true)
            {
                Squads.Add(new Squad(this, FormMain.Config.FindTypeUnit("Spearman")));
                Squads.Add(new Squad(this, FormMain.Config.FindTypeUnit("Swordsman")));
            }
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

        internal Lobby Lobby { get; }
        internal string Name { get; }
        internal int Position { get; }
        internal Fraction Fraction { get; }
        internal TypePlayer TypePlayer { get; }
        internal Chieftain Chieftain { get; }
        internal List<Squad> Squads { get; } = new List<Squad>();
        internal int[] Resources { get; }
        internal int Wins { get; set; }
        internal int Loses { get; set; }
        internal int Draws { get; set; }
        internal int StepsToCastle { get; }
        internal bool IsLive { get; }
        internal List<BuildingOfPlayer>[] ExternalBuildings {get; }
        internal PanelAboutPlayer PanelAbout { get; set; }
        private Player opponent;
        internal Player Opponent { get { return opponent; } set { if (value != this) opponent = value; else new Exception("Нельзя указать оппонентов самого себя."); } }
        internal bool BattleCalced { get; set; }
        internal List<CourseBattle> HistoryBattles { get; } = new List<CourseBattle>();
    }
}
