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

            // Инициализация храмов
            foreach (Temple t in FormMain.Config.Temples)
            {
                Temples.Add(new PlayerTemple(this, t));
            }

            //
            AddItem(new PlayerItem(FormMain.Config.FindItem("Sword1"), 1), 0);
            AddItem(new PlayerItem(FormMain.Config.FindItem("ArmourWarrior1"), 1), 1);
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

            Gold += Income();
        }

        internal Lobby Lobby { get; }
        internal string Name { get; }
        internal int Position { get; }
        internal Fraction Fraction { get; }
        internal List<PlayerGuild> Guilds { get; } = new List<PlayerGuild>();
        internal List<PlayerBuilding> Buildings { get; } = new List<PlayerBuilding>();
        internal List<PlayerTemple> Temples { get; } = new List<PlayerTemple>();
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();
        internal TypePlayer TypePlayer { get; }
        internal int Gold { get; set; }
        internal int[] Resources { get; }
        internal int Wins { get; set; }
        internal int Loses { get; set; }
        internal int Draws { get; set; }
        internal int StepsToCastle { get; }
        internal bool IsLive { get; }

        internal PlayerItem[] Warehouse = new PlayerItem[FormMain.WH_MAX_SLOTS];// Предметы на складе игрока
        internal List<BuildingOfPlayer>[] ExternalBuildings {get; }
        internal PanelAboutPlayer PanelAbout { get; set; }
        private Player opponent;
        internal Player Opponent { get { return opponent; } set { if (value != this) opponent = value; else new Exception("Нельзя указать оппонентов самого себя."); } }
        internal bool BattleCalced { get; set; }
        internal List<CourseBattle> HistoryBattles { get; } = new List<CourseBattle>();

        internal PlayerBuilding GetPlayerBuilding(Building b)
        {
            foreach (PlayerBuilding pb in Buildings)
            {
                if (pb.Building == b)
                    return pb;
            }

            throw new Exception("У игрока здание " + b.ID + " не найдено.");
        }

        internal int Income()
        {
            int income = 0;

            foreach (PlayerBuilding pb in Buildings)
            {
                income += pb.Income();
            }

            return income;
        }

        internal void AddItem(PlayerItem pi, int nSlot)
        {
            Debug.Assert(Warehouse[nSlot] == null);

            Warehouse[nSlot] = pi;
        }

        internal void MoveItem(int fromSlot, int toSlot)
        {
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[toSlot] == null);
            Debug.Assert(fromSlot != toSlot);

            Warehouse[toSlot] = Warehouse[fromSlot];
            Warehouse[fromSlot] = null;
        }

        internal void SellItem(int slot)
        {
            Debug.Assert(Warehouse[slot] != null);

            Warehouse[slot] = null;
        }
    }
}
