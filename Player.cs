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
            Gold = 100_000;

            // Инициализация гильдий
            foreach (Guild g in FormMain.Config.Guilds)
            {
                Guilds.Add(new PlayerGuild(this, g));
            }

            // Инициализация зданий
            foreach (Building b in FormMain.Config.Buildings)
            {
                Buildings.Add(new PlayerBuilding(this, b));
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

/*            for (int i = 0; i < ExternalBuildings.Count(); i++)
            {
                foreach (BuildingOfPlayer bp in ExternalBuildings[i])
                {
                    for (int r = 0; r < FormMain.Config.Resources.Count(); r++)
                        Resources[r] += bp.Building.IncomeResources[r];
                }
            }*/
            //Resources[0] += 1000;
        }

        internal Lobby Lobby { get; }
        internal string Name { get; }
        internal int Position { get; }
        internal Fraction Fraction { get; }
        internal List<PlayerGuild> Guilds { get; } = new List<PlayerGuild>();
        internal List<PlayerBuilding> Buildings { get; } = new List<PlayerBuilding>();
        internal TypePlayer TypePlayer { get; }
        internal Chieftain Chieftain { get; }
        internal List<Squad> Squads { get; } = new List<Squad>();
        internal int Gold { get; set; }
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
