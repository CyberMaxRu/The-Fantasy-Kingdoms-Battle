using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum TypePlayer { Human, Computer };
    // Класс игрока
    internal sealed class Player
    {
        public Player(Lobby lobby, int pos, string name, TypePlayer typePlayer)
        {
            Lobby = lobby;
            Name = name;
            TypePlayer = typePlayer;
            Wins = 0;
            Loses = 0;
            IsLive = true;
            Position = pos;
            ImageIndexAvatar = Position - 1;

            // Настраиваем игрока согласно настройкам лобби
            DurabilityCastle = Lobby.TypeLobby.DurabilityCastle;
            Gold = Lobby.TypeLobby.Gold;

            // Инициализация зданий
            foreach (Building b in FormMain.Config.Buildings)
            {
                Buildings.Add(new PlayerBuilding(this, b));
            }
            
            // Настройка ячеек героев
            CellHeroes = new PlayerHero[Config.HERO_ROWS, Config.HERO_IN_ROW];

            //
            AddItem(new PlayerItem(FormMain.Config.FindItem("Sword1"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Sword2"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Bow1"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Bow2"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ArmourWarrior1"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ArmourWarrior2"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ArmourArcher1"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ArmourArcher2"), 4));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfMana"), 10));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Regeneration"), 1));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Protection"), 1));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ImpProtection"), 2));

            ValidateHeroes();
            CalcBuilders();
        }

        internal void DoTurn()
        {
            Debug.Assert(TypePlayer == TypePlayer.Computer);
            Debug.Assert(IsLive == true);

            // Здесь расчет хода для ИИ
            // Покупаем четыре гильдии и строим 16 героев. На этом пока всё
            GetPlayerBuilding(FormMain.Config.FindBuilding("GuildWarrior")).BuyOrUpgrade();
            GetPlayerBuilding(FormMain.Config.FindBuilding("GuildRogue")).BuyOrUpgrade();
            GetPlayerBuilding(FormMain.Config.FindBuilding("GuildHunter")).BuyOrUpgrade();
            GetPlayerBuilding(FormMain.Config.FindBuilding("GuildCleric")).BuyOrUpgrade();
            HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildWarrior")));

            //HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildWarrior")));
            //HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildRogue")));
            //HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildHunter")));
            //HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildCleric")));

            void HireAllHero(PlayerBuilding bp)
            {
                if (bp.Heroes.Count < bp.MaxHeroes())
                {
                    for (int x = 0; x < Math.Min(Config.HERO_IN_ROW, bp.MaxHeroes()); x++)
                    //                for (; bp.Heroes.Count() < bp.MaxHeroes();)
                    {
                        bp.HireHero();
                    }
                }
            }
        }

        internal void CalcResultTurn()
        {
            if (IsLive == true)
            {
                if (DurabilityCastle > 0)
                {
                    Gold += Income();

                    ValidateHeroes();
                    CalcBuilders();
                }
                else
                {
                    IsLive = false;
                }
            }
        }

        private void CalcBuilders()
        {
            TotalBuilders = 0;
            foreach (PlayerBuilding pb in Buildings)
                if ((pb.Building.TrainedHero != null) && (pb.Building.TrainedHero.CanBuild == true))
                    TotalBuilders += pb.Heroes.Count;

            FreeBuilders = TotalBuilders;
        }

        private void ValidateHeroes()
        {
            foreach (PlayerBuilding pb in Buildings)
                pb.ValidateHeroes();
        }

        internal Lobby Lobby { get; }
        internal string Name { get; }
        internal int ImageIndexAvatar { get; }
        internal int Position { get; }
        internal int PositionInLobby { get; set; }
        internal int DurabilityCastle { get; set; }
        internal List<PlayerBuilding> Buildings { get; } = new List<PlayerBuilding>();
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();
        internal PlayerHero[,] CellHeroes;
        internal TypePlayer TypePlayer { get; }
        internal int Gold { get; set; }
        internal int TotalBuilders { get; private set; }
        internal int FreeBuilders { get; set; }
        internal int[] Resources { get; }
        internal int Wins { get; set; }
        internal int Loses { get; set; }
        internal int Draws { get; set; }
        internal ResultBattle ResultLastBattle { get; set; }
        internal bool IsLive { get; private set; }

        internal PlayerItem[] Warehouse = new PlayerItem[FormMain.WH_MAX_SLOTS];// Предметы на складе игрока
        internal PanelAboutPlayer PanelAbout { get; set; }
        internal PanelPlayer Panel { get; set; }
        private Player opponent;// Убрать это
        internal Player Opponent { get { return opponent; } set { if (value != this) opponent = value; else new Exception("Нельзя указать оппонентов самого себя."); } }
        internal bool BattleCalced { get; set; }
        internal List<Battle> HistoryBattles { get; } = new List<Battle>();

        internal PlayerBuilding GetPlayerBuilding(Building b)
        {
            foreach (PlayerBuilding pb in Buildings)
            {
                if (pb.Building == b)
                    return pb;
            }

            throw new Exception("У игрока здание " + b.ID + " не найдено.");
        }

        internal void AddHero(PlayerHero ph)
        {
            Debug.Assert(Heroes.IndexOf(ph) == -1);

            Heroes.Add(ph);

            if (ph.Building.Building.CategoryBuilding != CategoryBuilding.Castle)
            {
                // Ищем место в ячейках героев
                int line = 0;
                int pos = -1;
                List<int> positions = new List<int>();

                // Сначала ищем ячейку согласно категории героя
                // Для этого ищем линию со свободными ячейками для категории героя, начиная с первой
                // Пытаемся разместить его в середине линии, а затем в стороны от середины
                for (int y = 0; y < CellHeroes.GetLength(0); y++)
                {
                    line = y;
                    positions.Clear();

                    for (int x = 0; x < CellHeroes.GetLength(1); x++)
                        if (CellHeroes[y, x] == null)
                        {
                            positions.Add(x);
                        }

                    if (positions.Count > 0)
                    {
                        int centre = (int)Math.Truncate(CellHeroes.GetLength(1) / 2.0 + 0.5) - 1;
                        if (positions.IndexOf(centre) != -1)
                        {
                            pos = centre;
                        }
                        else
                        {
                            int shift = 1;
                            for (; ; shift++)
                            {
                                if (positions.IndexOf(centre - shift) != -1)
                                {
                                    pos = centre - shift;
                                    break;
                                }

                                if (positions.IndexOf(centre + shift) != -1)
                                {
                                    pos = centre + shift;
                                    break;
                                }

                                if (shift == centre)
                                    break;
                            }
                        }
                    }

                    if (pos != -1)
                        break;
        
                }

                Debug.Assert(pos != -1);
                Debug.Assert(CellHeroes[line, pos] == null);

                CellHeroes[line, pos] = ph;
                ph.CoordInPlayer = new Point(pos, line);
            }
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

        // Поиск слота для предмета
        internal int FindSlotWithItem(Item item)
        {
            for (int i = 0; i < Warehouse.Length; i++)
            {
                if ((Warehouse[i] != null) && (Warehouse[i].Item == item))
                    return i;
            }

            return -1;
        }

        private int FindSlotForItem(Item item)
        {
            // Сначала ищем, есть ли такой предмет в слоте
            int number = FindSlotWithItem(item);
            if (number != -1)
                return number;
            
            // Ищем первый свободный слот
            for (int i = 0; i < Warehouse.Length; i++)
            {
                if (Warehouse[i] == null)
                    return i;
            }

            return -1;
        }

        internal void AddItem(PlayerItem pi)
        {
            Debug.Assert(pi.Quantity > 0);

            int numberCell = FindSlotForItem(pi.Item);
            if (numberCell >= 0)
                AddItem(pi, numberCell);
        }

        internal void AddItem(PlayerItem pi, int numberCell)
        {
            if (Warehouse[numberCell] != null)
            {
                Debug.Assert(Warehouse[numberCell].Quantity > 0);

                if (Warehouse[numberCell].Item == pi.Item)
                {
                    Warehouse[numberCell].Quantity += pi.Quantity;
                    pi.Quantity = 0;
                }
            }
            else
            {
                Warehouse[numberCell] = new PlayerItem(pi.Item, pi.Quantity);
                pi.Quantity = 0;
            }
        }

        internal void MoveItem(int fromSlot, int toSlot)
        {
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(fromSlot != toSlot);

            PlayerItem tmp = null;
            if (Warehouse[toSlot] != null)
                tmp = Warehouse[toSlot];
            Warehouse[toSlot] = Warehouse[fromSlot];
            Warehouse[fromSlot] = tmp;
        }

        internal void SellItem(int slot)
        {
            Debug.Assert(Warehouse[slot] != null);

            Warehouse[slot] = null;
        }

        internal void SellItem(PlayerItem pi)
        {

        }

        internal void GiveItemToHero(int fromSlot, PlayerHero ph, int quantity, int toSlot)
        {
            Debug.Assert(ph.Building.Player == this);
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[fromSlot].Quantity >= quantity);

            ph.AcceptItem(Warehouse[fromSlot], quantity, toSlot);
            if (Warehouse[fromSlot].Quantity == 0)
                Warehouse[fromSlot] = null;
        }

        internal void GiveItemToHero(int fromSlot, PlayerHero ph, int quantity)
        {
            Debug.Assert(ph.Building.Player == this);
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[fromSlot].Quantity >= quantity);

            ph.AcceptItem(Warehouse[fromSlot], quantity);
            if (Warehouse[fromSlot].Quantity == 0)
                Warehouse[fromSlot] = null;
        }

        internal bool GetItemFromHero(PlayerHero ph, int fromSlot)
        {
            Debug.Assert(ph.Building.Player == this);
            Debug.Assert(ph.Slots[fromSlot] != null);
            Debug.Assert(ph.Slots[fromSlot].Quantity > 0);

            // Ищем слот для предмета
            int toSlot = FindSlotForItem(ph.Slots[fromSlot].Item);
            if (toSlot == -1)
                return false;

            GetItemFromHero(ph, fromSlot, toSlot);
            return true;
        }
        internal void GetItemFromHero(PlayerHero ph, int fromSlot, int toSlot)
        {
            Debug.Assert(ph.Building.Player == this);
            Debug.Assert(ph.Slots[fromSlot] != null);
            Debug.Assert(toSlot >= 0);

            if (Warehouse[toSlot] != null)
            {
                if (Warehouse[toSlot].Item == ph.Slots[fromSlot].Item)
                {
                    Warehouse[toSlot].Quantity += ph.Slots[fromSlot].Quantity;
                    ph.Slots[fromSlot].Quantity = 0;
                }
                else
                    return;
            }
            else
                Warehouse[toSlot] = ph.Slots[fromSlot];

            ph.Slots[fromSlot] = null;
            ph.ValidateCell(fromSlot);
        }

        // Забираем указанное количество предметов из ячейки
        internal PlayerItem TakeItemFromWarehouse(int fromCell, int quantity)
        {
            Debug.Assert(quantity > 0);
            Debug.Assert(Warehouse[fromCell] != null);
            Debug.Assert(Warehouse[fromCell].Quantity > 0);
            Debug.Assert(Warehouse[fromCell].Quantity >= quantity);

            PlayerItem pi;

            // Если забирают всё, то возвращаем ссылку на этот предмет и убираем его у себя, иначе делим предмет
            if (Warehouse[fromCell].Quantity == quantity)
            {
                pi = Warehouse[fromCell];
                Warehouse[fromCell] = null;
            }
            else
            {
                pi = new PlayerItem(Warehouse[fromCell].Item, quantity);
                Warehouse[fromCell].Quantity -= quantity;
            }

            return pi;
        }
    }
}
