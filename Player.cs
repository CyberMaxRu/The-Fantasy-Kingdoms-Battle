using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal enum TypePlayer { Human, Computer };
    // Класс игрока
    internal sealed class Player : ICell
    {
        private ResultBattle resultLastBattle;
        private PlayerBuilding Castle;

        public Player(Lobby lobby, int index, string name, TypePlayer typePlayer)
        {
            Lobby = lobby;
            Name = name;
            TypePlayer = typePlayer;
            Wins = 0;
            Loses = 0;
            IsLive = true;
            PlayerIndex = index;
            ImageIndexAvatar = typePlayer == TypePlayer.Computer ? PlayerIndex : Program.formMain.Settings.IndexAvatar();
            ResultLastBattle = ResultBattle.None;

            // Настраиваем игрока согласно настройкам лобби
            DurabilityCastle = Lobby.TypeLobby.DurabilityCastle;

            // Инициализация зданий
            foreach (Building b in FormMain.Config.Buildings)
            {
                Buildings.Add(new PlayerBuilding(this, b));
            }

            // Инициализация логов
            foreach (Lair l in FormMain.Config.Lairs)
            {
                Lairs.Add(new PlayerLair(this, l));
            }

            Castle = GetPlayerBuilding(FormMain.Config.FindBuilding(FormMain.Config.IDBuildingCastle));
            Gold = Lobby.TypeLobby.Gold;
            if (TypePlayer == TypePlayer.Computer)
                Gold = 100_000;

            // Настройка ячеек героев
            CellHeroes = new PlayerHero[FormMain.Config.HeroRows, FormMain.Config.HeroInRow];

            //
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfMana"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Regeneration"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Protection"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ImpProtection"), 2, true));

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
            GetPlayerBuilding(FormMain.Config.FindBuilding("GuildMage")).BuyOrUpgrade();

            HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildWarrior")));
            HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildHunter")));
            HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildCleric")));
            HireAllHero(GetPlayerBuilding(FormMain.Config.FindBuilding("GuildMage")));

            void HireAllHero(PlayerBuilding bp)
            {
                if (bp.Heroes.Count < bp.MaxHeroes())
                {
                    //int needHire = 49;
                    int needHire = FormMain.Rnd.Next(2) + 1;

                    for (int x = 0; x < needHire; x++)
                    //                for (; bp.Heroes.Count() < bp.MaxHeroes();)
                    {
                        if (bp.Heroes.Count == bp.MaxHeroes())
                            break;
                        if (CombatHeroes.Count == Lobby.TypeLobby.MaxHeroes)
                            break;
                        bp.HireHero();
                    }
                }
            }
        }

        internal void CalcResultTurn()
        {
            if (IsLive == true)
            {
                Gold += Income();

                ValidateHeroes();
                CalcBuilders();

                QuantityHeroes = CombatHeroes.Count();

                if (DurabilityCastle <= 0)
                {
                    IsLive = false;
                    DayOfDie = Lobby.Turn;
                }
            }
        }

        private void CalcBuilders()
        {
            TotalBuilders = 0;
            foreach (PlayerHero ph in AllHeroes)
                if (ph.ClassHero.CanBuild == true)
                    TotalBuilders++;

            FreeBuilders = TotalBuilders;
        }

        private void ValidateHeroes()
        {
            foreach (PlayerBuilding pb in Buildings)
                pb.ValidateHeroes();
        }

        internal Lobby Lobby { get; }
        internal string Name { get; set; }
        internal int ImageIndexAvatar { get; set; }
        internal int PlayerIndex { get; }
        internal int PositionInLobby { get; set; }
        internal int DurabilityCastle { get; set; }
        internal int LastBattleDamageToCastle { get; set; }
        internal List<PlayerBuilding> Buildings { get; } = new List<PlayerBuilding>();
        internal int LevelCastle => Castle.Level;
        internal List<PlayerHero> CombatHeroes { get; } = new List<PlayerHero>();
        internal List<PlayerHero> AllHeroes { get; } = new List<PlayerHero>();
        internal PlayerHero[,] CellHeroes;
        internal TypePlayer TypePlayer { get; }
        internal int Gold { get => Castle.Gold; set { Castle.Gold = value; } }
        internal int TotalBuilders { get; private set; }
        internal int FreeBuilders { get; set; }
        internal int[] Resources { get; }
        internal bool IsLive { get; private set; }
        internal int DayOfDie { get; private set; }

        internal int QuantityHeroes { get; private set; }

        internal PlayerItem[] Warehouse = new PlayerItem[FormMain.Config.WarehouseMaxCells];// Предметы на складе игрока

        // Логова
        internal List<PlayerLair> Lairs { get; } = new List<PlayerLair>();
        internal PlayerLair TargetLair { get; set; }// Логово для атаки

        // Статистика по боям
        internal int Wins { get; set; }
        internal int Loses { get; set; }
        internal int Draws { get; set; }
        internal int Streak { get; set; }
        internal ResultBattle ResultLastBattle 
        { get { return resultLastBattle; }
            set
            {
                switch (value)
                {
                    case ResultBattle.Win:
                        if (resultLastBattle == ResultBattle.Win)
                            Streak++;
                        else
                            Streak = 1;

                        Wins++;

                        break;
                    case ResultBattle.Lose:
                        if (resultLastBattle == ResultBattle.Lose)
                            Streak++;
                        else
                            Streak = 1;

                        Loses++;

                        break;
                    case ResultBattle.Draw:
                        if (resultLastBattle == ResultBattle.Draw)
                            Streak++;
                        else
                            Streak = 1;

                        Draws++;

                        break;
                    case ResultBattle.None:
                        break;
                    default:
                        throw new Exception("Неизвестный результат боя.");
                }

                resultLastBattle = value;
            }
        }

        // Визуальные контролы
        internal PanelPlayer Panel { get; set; }
        private Player opponent;// Убрать это
        internal Player Opponent { get { return opponent; } set { if (value != this) opponent = value; else new Exception("Нельзя указать оппонентов самого себя."); } }
        internal bool BattleCalced { get; set; }
        internal List<Battle> HistoryBattles { get; } = new List<Battle>();

        internal PlayerBuilding GetPlayerBuilding(Building b)
        {
            Debug.Assert(b != null);

            foreach (PlayerBuilding pb in Buildings)
            {
                if (pb.Building == b)
                    return pb;
            }

            throw new Exception("У игрока здание " + b.ID + " не найдено.");
        }

        internal void AddHero(PlayerHero ph)
        {
            Debug.Assert(CombatHeroes.IndexOf(ph) == -1);
            Debug.Assert(AllHeroes.IndexOf(ph) == -1);

            AllHeroes.Add(ph);
            CombatHeroes.Add(ph);

            if (ph.Building.Building.CategoryBuilding != CategoryBuilding.Castle)
            {
                RearrangeHeroes();
            }
        }

        private void RearrangeHeroes()
        {
            // Очищаем все координаты героев
            foreach (PlayerHero ph in CellHeroes)
            {
                if (ph != null)
                {
                    CellHeroes[ph.CoordInPlayer.Y, ph.CoordInPlayer.X] = null;
                    ph.CoordInPlayer = new Point(-1, -1);
                }
            }

            // Проставляем координаты для героев
            foreach (PlayerHero ph in CombatHeroes.OrderBy(ph => ph.Priority()))
                SetPosForHero(ph);
        }

        private void SetPosForHero(PlayerHero ph)
        {
            // Ищем место в ячейках героев
            int coordY = -1;
            int coordX = 0;
            List<int> positions = new List<int>();

            // Сначала ищем ячейку согласно категории героя
            // Для этого ищем линию со свободными ячейками для категории героя, начиная с первой
            // Пытаемся разместить его в середине линии, а затем в стороны от середины
            for (int x = CellHeroes.GetLength(1) - 1; x >= 0; x--)
            {
                coordX = x;
                positions.Clear();

                for (int y = 0; y < CellHeroes.GetLength(0); y++)
                    if (CellHeroes[y, x] == null)
                    {
                        positions.Add(y);
                    }

                if (positions.Count > 0)
                {
                    int centre = (int)Math.Truncate(CellHeroes.GetLength(0) / 2.0 + 0.5) - 1;
                    if (positions.IndexOf(centre) != -1)
                    {
                        coordY = centre;
                    }
                    else
                    {
                        int shift = 1;
                        for (; ; shift++)
                        {
                            if (positions.IndexOf(centre - shift) != -1)
                            {
                                coordY = centre - shift;
                                break;
                            }

                            if (positions.IndexOf(centre + shift) != -1)
                            {
                                coordY = centre + shift;
                                break;
                            }

                            if (shift == centre)
                                break;
                        }
                    }
                }

                if (coordY != -1)
                    break;

            }

            Debug.Assert(coordY != -1);
            Debug.Assert(CellHeroes[coordY, coordX] == null);

            CellHeroes[coordY, coordX] = ph;
            ph.CoordInPlayer = new Point(coordX, coordY);
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
                Warehouse[numberCell] = new PlayerItem(pi.Item, pi.Quantity, true);
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
            /*Debug.Assert(ph.Building.Player == this);
            Debug.Assert(ph.Slots[fromSlot] != null);
            Debug.Assert(ph.Slots[fromSlot].Quantity > 0);

            // Ищем слот для предмета
            int toSlot = FindSlotForItem(ph.Slots[fromSlot].Item);
            if (toSlot == -1)
                return false;

            GetItemFromHero(ph, fromSlot, toSlot);*/
            return true;
        }
        internal void GetItemFromHero(PlayerHero ph, int fromSlot, int toSlot)
        {
            /*Debug.Assert(ph.Building.Player == this);
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
            ph.ValidateCell(fromSlot);*/
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
                pi = new PlayerItem(Warehouse[fromCell].Item, quantity, true);
                Warehouse[fromCell].Quantity -= quantity;
            }

            return pi;
        }

        internal bool CheckRequirements(List<Requirement> list)
        {
            PlayerBuilding pb;
            foreach (Requirement r in list)
            {
                pb = GetPlayerBuilding(r.Building);
                if (r.Level > pb.Level)
                    return false;
            }

            return true;
        }

        internal void TextRequirements(List<Requirement> listReq, List<TextRequirement> listTextReq)
        {           
            PlayerBuilding pb;

            foreach (Requirement r in listReq)
            {
                pb = GetPlayerBuilding(r.Building);
                listTextReq.Add(new TextRequirement(r.Level <= pb.Level, pb.Building.Name + (r.Level > 1 ? " " + r.Level + " уровня" : "")));
            }
        }

        internal void SpendGold(int gold)
        {
            Gold -= gold;

            Debug.Assert(Gold >= 0);
        }

        internal void MakeAlive()
        {
            IsLive = true;
            DayOfDie = 0;
            DurabilityCastle = 1;
        }

        // Реализация интерфейса
        PanelEntity ICell.Panel { get; set; }
        ImageList ICell.ImageList() => Program.formMain.ilPlayerAvatars;
        int ICell.ImageIndex() => ImageIndexAvatar;
        bool ICell.NormalImage() => IsLive;
        int ICell.Value() => Castle.Level;
        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(
                Name,
                "Место №" + PositionInLobby.ToString(),
                "Уровень Замка: " + LevelCastle.ToString() + Environment.NewLine
                    + "Прочность Замка " + DurabilityCastle.ToString() + "/" + Lobby.TypeLobby.DurabilityCastle.ToString() + Environment.NewLine
                    + "Героев: " + QuantityHeroes.ToString() + Environment.NewLine
                    + Environment.NewLine
                    + "Побед: " + Wins.ToString() + Environment.NewLine
                    + "Ничьих: " + Draws.ToString() + Environment.NewLine
                    + "Поражений: " + Loses.ToString() + Environment.NewLine
                    + (IsLive ? "" : Environment.NewLine + "Игрок покинул лобби на " + DayOfDie + " ходу"));
        }

        void ICell.Click(PanelEntity pe)
        { 

        }
    }
}