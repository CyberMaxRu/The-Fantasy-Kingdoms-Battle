using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Класс информации о поражении
    internal sealed class LoseInfo
    {
        public LoseInfo(int day, LobbyPlayer opponent)
        {
            Day = day;
            Opponent = opponent;
        }

        internal int Day { get; }
        internal LobbyPlayer Opponent { get; }
    }

    // Класс игрока лобби
    internal abstract class LobbyPlayer : BattleParticipant, ICell
    {
        private PlayerConstruction Castle;

        // TODO Вынести константы в конфигурацию игры
        internal const int MAX_FLAG_EXCLUSIVE = 1;// Максимальное число флагов с максимальным
        internal const int MAX_FLAG_HIGH = 2;// Максимальное число флагов с высоким приоритетом
        internal const int MAX_FLAG_COUNT = 5;// Максимальное число активных флагов

        public LobbyPlayer(Lobby lobby, Player player, int playerIndex) : base(lobby)
        {
            Player = player;
            PlayerIndex = playerIndex;

            // Создаем справочик количества приоритетов флагов
            foreach (PriorityExecution pe in Enum.GetValues(typeof(PriorityExecution)))
            {
                QuantityFlags.Add(pe, 0);
            }

            // Настраиваем игрока согласно настройкам лобби
            PointConstructionGuild = lobby.TypeLobby.StartPointConstructionGuild;
            PointConstructionEconomic = lobby.TypeLobby.StartPointConstructionEconomic;
            PointConstructionTemple = lobby.TypeLobby.StartPointConstructionTemple;
            PointConstructionTradePost = lobby.TypeLobby.StartPointConstructionTradePost;
            SetQuantityFlags(lobby.TypeLobby.StartQuantityFlags);

            for (int i = 0; i < lobby.TypeLobby.MaxLoses; i++)
                LoseInfo.Add(null);

            // Настраиваем стартовые бонусы
            if (lobby.TypeLobby.VariantStartBonus > 0)
            {
                VariantsStartBonuses = new List<StartBonus>();
                for (int i = 0; i < lobby.TypeLobby.VariantStartBonus; i++)
                {
                    VariantsStartBonuses.Add(GenerateStartBonus());
                }
            }

            // Инициализация зданий
            foreach (TypeConstruction tck in FormMain.Config.TypeConstructionsOfKingdom)
            {
                Constructions.Add(new PlayerConstruction(this, tck));
            }

            // Инициализация логов
            Lairs = new PlayerLair[lobby.TypeLobby.LairsLayers, lobby.TypeLobby.LairsHeight, lobby.TypeLobby.LairsWidth];

            GenerateLairs();
            ScoutRandomLair(lobby.TypeLobby.StartScoutedLairs);

            //

            Castle = GetPlayerConstruction(FormMain.Config.FindTypeEconomicConstruction(FormMain.Config.IDConstructionCastle));

            Gold = Lobby.TypeLobby.Gold;
            if (Player.TypePlayer == TypePlayer.Computer)
                Gold = 100_000;

            LevelGreatness = 1;
            PointGreatnessForNextLevel = 100;

            PlayerHero king = Castle.HireHero(FormMain.Config.FindTypeHero("King"));
            PlayerHero advisor = Castle.HireHero(FormMain.Config.FindTypeHero("Advisor"));
            PlayerHero captain = Castle.HireHero(FormMain.Config.FindTypeHero("Captain"));
            PlayerHero treasurer = Castle.HireHero(FormMain.Config.FindTypeHero("Treasurer"));

            //
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfMana"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Regeneration"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Protection"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ImpProtection"), 2, true));

            ValidateHeroes();

            StartBonus GenerateStartBonus()
            {
                int restAttempts = 100;
                bool needRegenerate;
                while (restAttempts > 0)
                {
                    StartBonus newSb = GenerateNew(lobby.TypeLobby.PointStartBonus);
                    needRegenerate = false;

                    // Ищем, есть ли такой же бонус
                    foreach (StartBonus b in VariantsStartBonuses)
                        if (b.Equals(newSb))
                        {
                            needRegenerate = true;
                            break;
                        }

                    if (!needRegenerate)
                        return newSb;

                    restAttempts--;
                }

                throw new Exception("Не удалось подобрать уникальный бонус.");

                StartBonus GenerateNew(int points)
                {
                    StartBonus sb = new StartBonus();
                    List<StartBonus> listBonuses = new List<StartBonus>();

                    while (sb.Points < points)
                    {
                        // Выбираем случайный бонус из списка доступных, чтобы хватило оставшихся очков
                        listBonuses.Clear();
                        listBonuses.AddRange(FormMain.Config.StartBonuses.Where(b => b.Points <= (points - sb.Points)));
                        Debug.Assert(listBonuses.Count > 0);
                        sb.AddBonus(listBonuses[lobby.Rnd.Next(listBonuses.Count)]);
                    }

                    return sb;
                }
            }
        }

        internal abstract void DoTurn();
        internal abstract void EndTurn();

        //
        protected void ScoutRandomLair(int scoutLaires)
        {
            if (scoutLaires > 0)
            {
                List<PlayerLair> lairs = new List<PlayerLair>();
                for (int y = 0; y < Lairs.GetLength(1); y++)
                    for (int x = 0; x < Lairs.GetLength(2); x++)
                        if (Lairs[0, y, x].Hidden)
                            lairs.Add(Lairs[0, y, x]);

                Debug.Assert(scoutLaires <= lairs.Count);

                for (int i = 0; i < scoutLaires; i++)
                {
                    lairs[i].Unhide();
                    lairs.RemoveAt(i);
                }
            }
        }

        // Расчет после завершения хода игроком
        internal void CalcFinalityTurn()
        {
            // Убеждаемся, что у нас не сломалось соответствие флагов
            foreach (PlayerLair pl in Lairs)
            {
                if (pl != null)
                {
                    if (pl.PriorityFlag != PriorityExecution.None)
                        Debug.Assert(ListFlags.IndexOf(pl) != -1);
                    else
                        Debug.Assert(ListFlags.IndexOf(pl) == -1);
                }
            }

            // Расчет флагов на логова
            List<PlayerLair> tempListLair = ListFlags.ToList();// Работаем с копией списка, так как текущий будет меняться по мере обработки флагов

            foreach (PlayerLair pl in tempListLair)
            {
                Battle b;
                WindowBattle formBattle;

                if ((pl != null) && (pl.listAttackedHero.Count > 0))
                {
                    Debug.Assert((pl.TypeFlag == TypeFlag.Scout) || (pl.TypeFlag == TypeFlag.Attack) || (pl.TypeFlag == TypeFlag.Defense));

                    if (pl.TypeFlag == TypeFlag.Scout)
                    {
                        pl.DoScout();
                    }
                    else if (pl.TypeFlag == TypeFlag.Attack)
                    {
                        Debug.Assert(pl.Monsters.Count > 0);

                        PreparingForBattle();

                        // Включить, когда ИИ может выбирать цель
                        pl.PreparingForBattle();

                        //Debug.Assert(p.TargetLair.CombatHeroes.Count > 0);

                        bool showForPlayer = Player.TypePlayer == TypePlayer.Human;
                        b = new Battle(this, pl, Lobby.Day, Lobby.Rnd, showForPlayer);

                        if (showForPlayer)
                        {
                            formBattle = new WindowBattle(b);
                            formBattle.ShowBattle();
                            formBattle.Dispose();
                        }
                        else
                        {
                            //if (formProgressBattle == null)
                            //    formProgressBattle = new FormProgressBattle();

                            //formProgressBattle.SetBattle(b, 1, 1);
                            b.CalcWholeBattle();
                        }

                        if (b.Winner == this)
                        {
                            // Победил игрок
                            pl.DoCapture();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        pl.DoDefense();
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

                QuantityHeroes = CombatHeroes.Count();

                PointConstructionGuild = Lobby.TypeLobby.PointConstructionGuildPerDay;
                PointConstructionEconomic = Lobby.TypeLobby.PointConstructionEconomicPerDay;
            }
        }

        private void ValidateHeroes()
        {
            foreach (PlayerConstruction pb in Constructions)
                pb.ValidateHeroes();
        }

        internal Player Player { get; }
        internal int PlayerIndex { get; }
        internal int PositionInLobby { get; set; }
        internal int LevelGreatness { get; }// Уровень величия
        internal int PointGreatness { get; set; }// Очков величия
        internal int PointGreatnessForNextLevel { get; }// Очков величия до следующего уровня
        internal List<PlayerConstruction> Constructions { get; } = new List<PlayerConstruction>();
        internal int LevelCastle => Castle.Level;
        internal List<PlayerHero> AllHeroes { get; } = new List<PlayerHero>();
        internal int Gold { get => Castle.Gold; set { Castle.Gold = value; } }

        internal List<LoseInfo> LoseInfo { get; } = new List<LoseInfo>();
        internal int CurrentLoses { get; }// Текущее количество поражений

        internal int PointConstructionGuild { get; private set; }
        internal int PointConstructionEconomic { get; private set; }
        internal int PointConstructionTemple { get; private set; }
        internal int PointConstructionTradePost { get; private set; }
        internal List<StartBonus> VariantsStartBonuses { get; }// Варианты стартовых бонусов

        internal int QuantityHeroes { get; private set; }

        internal PlayerItem[] Warehouse = new PlayerItem[FormMain.Config.WarehouseMaxCells];// Предметы на складе игрока

        // Логова
        internal PlayerLair[,,] Lairs { get; }
        internal List<PlayerLair> ListFlags { get; } = new List<PlayerLair>();
        internal Dictionary<PriorityExecution, int> QuantityFlags { get; } = new Dictionary<PriorityExecution, int>();

        // Визуальные контролы
        internal PanelPlayer Panel { get; set; }
        private LobbyPlayer opponent;// Убрать это
        internal LobbyPlayer Opponent { get { return opponent; } set { if (value != this) opponent = value; else new Exception("Нельзя указать оппонентов самого себя."); } }

        internal PlayerConstruction GetPlayerConstruction(TypeConstruction b)
        {
            Debug.Assert(b != null);

            foreach (PlayerConstruction pb in Constructions)
            {
                if (pb.TypeConstruction == b)
                    return pb;
            }

            throw new Exception("У игрока здание " + b.ID + " не найдено.");
        }

        internal void AddHero(PlayerHero ph)
        {
            Debug.Assert(CombatHeroes.IndexOf(ph) == -1);
            Debug.Assert(AllHeroes.IndexOf(ph) == -1);

            AllHeroes.Add(ph);
            if ((ph.TypeHero.ID != "King") && (ph.TypeHero.ID != "Advisor") && (ph.TypeHero.ID != "Captain") && (ph.TypeHero.ID != "Treasurer"))
                AddCombatHero(ph);

            if (ph.Construction.TypeConstruction.TrainedHero != null)
            {
                RearrangeHeroes();
            }

            SetTaskForHeroes();

            if (Player.TypePlayer == TypePlayer.Human)
                Program.formMain.ListHeroesChanged();
        }

        internal void Constructed(PlayerConstruction pb)
        {
            Debug.Assert(pb.CheckRequirements());

            Gold -= pb.CostBuyOrUpgrade();
            PointConstructionGuild -= pb.TypeConstruction.PointConstructionGuild;
            PointConstructionEconomic -= pb.TypeConstruction.PointConstructionEconomic;
            PointConstructionTemple -= pb.TypeConstruction.PointConstructionTemple;
            PointConstructionTradePost -= pb.TypeConstruction.PointConstructionTradePost;

            Debug.Assert(PointConstructionGuild >= 0);
            Debug.Assert(PointConstructionEconomic >= 0);
            Debug.Assert(PointConstructionTemple >= 0);
            Debug.Assert(PointConstructionTradePost >= 0);
        }

        internal int Income()
        {
            int income = 0;

            foreach (PlayerConstruction pb in Constructions)
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
            Debug.Assert(ph.Construction.Player == this);
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[fromSlot].Quantity >= quantity);

            ph.AcceptItem(Warehouse[fromSlot], quantity, toSlot);
            if (Warehouse[fromSlot].Quantity == 0)
                Warehouse[fromSlot] = null;
        }

        internal void GiveItemToHero(int fromSlot, PlayerHero ph, int quantity)
        {
            Debug.Assert(ph.Construction.Player == this);
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[fromSlot].Quantity >= quantity);

            ph.AcceptItem(Warehouse[fromSlot], quantity);
            if (Warehouse[fromSlot].Quantity == 0)
                Warehouse[fromSlot] = null;
        }

        internal bool GetItemFromHero(PlayerHero ph, int fromSlot)
        {
            /*Debug.Assert(ph.Construction.Player == this);
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
            /*Debug.Assert(ph.Construction.Player == this);
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
            PlayerConstruction pb;
            foreach (Requirement r in list)
            {
                pb = GetPlayerConstruction(r.Construction);
                if (r.Level > pb.Level)
                    return false;
            }

            return true;
        }

        internal void TextRequirements(List<Requirement> listReq, List<TextRequirement> listTextReq)
        {
            PlayerConstruction pb;

            foreach (Requirement r in listReq)
            {
                pb = GetPlayerConstruction(r.Construction);
                listTextReq.Add(new TextRequirement(r.Level <= pb.Level, pb.TypeConstruction.Name + (r.Level > 1 ? " " + r.Level + " уровня" : "")));
            }
        }

        internal void SpendGold(int gold)
        {
            Debug.Assert(gold > 0);
            Debug.Assert(Gold >= gold);

            Gold -= gold;
        }

        internal void ReturnGold(int gold)
        {
            Debug.Assert(Gold > 0);
            Debug.Assert(gold >= 0);

            Gold += gold;
        }

        internal override void PreparingForBattle()
        {
            base.PreparingForBattle();
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(
                Player.Name, "",
                "Уровень Замка: " + LevelCastle.ToString() + Environment.NewLine
                    + "Героев: " + QuantityHeroes.ToString());
        }

        // Метод по распределению задач героев
        internal void SetTaskForHeroes()
        {
            List<PlayerHero> freeHeroes = new List<PlayerHero>();// Список свободных героев

            // Сначала сбрасываем всем состояние
            foreach (PlayerHero ph in CombatHeroes)
            {
                if ((ph.StateCreature.ID == NameStateCreature.DoAttackFlag.ToString())
                    || (ph.StateCreature.ID == NameStateCreature.DoScoutFlag.ToString())
                    || (ph.StateCreature.ID == NameStateCreature.DoDefenseFlag.ToString()))                    
                {
                    ph.ClearState();
                }

                if (ph.StateCreature.ID == NameStateCreature.Nothing.ToString())
                    freeHeroes.Add(ph);
            }

            // Базовый алгоритм такой - идем по уменьшению приоритета, берем рандомных героев, ограничивая максимальным числом
            // Но сейчас всех героев делим поровну между флагами, без привязки к приоритету
            // Но учитываем максимальное число героев на логово
            // Это если речь идет о флаге атаки. На разведку идет ровно один герой
            if ((freeHeroes.Count > 0) && (CountActiveFlags() > 0))
            {
                foreach (PlayerLair pl in ListFlags.Where(pl => (pl != null) && (pl.TypeFlag == TypeFlag.Scout)))
                {
                    pl.AddAttackingHero(freeHeroes[0]);
                    freeHeroes.RemoveAt(0);

                    if (freeHeroes.Count == 0)
                        break;
                }

                if (freeHeroes.Count > 0)
                {
                    int quantityFlagAttack = ListFlags.Where(pl => (pl != null) && ((pl.TypeFlag == TypeFlag.Attack) || (pl.TypeFlag == TypeFlag.Defense))).Count();
                    if (quantityFlagAttack > 0)
                    {
                        int heroesToFlag;
                        int heroesPerFlag = Math.Max(freeHeroes.Count / quantityFlagAttack, 1);

                        foreach (PlayerLair pl in ListFlags.Where(pl => (pl != null) && ((pl.TypeFlag == TypeFlag.Attack) || (pl.TypeFlag == TypeFlag.Defense))))
                            if (pl != null)
                            {
                                heroesToFlag = Math.Min(Math.Min(freeHeroes.Count, heroesPerFlag), pl.TypeLair.MaxHeroes);

                                for (int i = 0; i < heroesToFlag; i++)
                                {
                                    pl.AddAttackingHero(freeHeroes[0]);
                                    freeHeroes.RemoveAt(0);
                                }

                                if (freeHeroes.Count == 0)
                                    break;
                            }
                    }
                }
            }
        }

        private int CountActiveFlags()
        {
            int count = 0;
            foreach (PlayerLair pl in ListFlags)
                if (pl != null)
                    count++;

            return count;
        }

        internal override void HideInfo()
        {
            throw new NotImplementedException();
        }

        internal override void ShowInfo()
        {
            throw new NotImplementedException();
        }

        private void SetQuantityFlags(int quantity)
        {
            Debug.Assert(quantity >= ListFlags.Count);
            Debug.Assert(quantity <= Lobby.TypeLobby.MaxQuantityFlags);

            while (ListFlags.Count < quantity)
            {
                ListFlags.Add(null);
            }

            // Указываем количество свободных флагов
            foreach (PlayerLair pl in ListFlags)
            {
                if (pl == null)
                    QuantityFlags[PriorityExecution.None]++;
                else
                    QuantityFlags[pl.PriorityFlag]++;
            }
        }

        internal void AddFlag(PlayerLair lair)
        {
            // Ищем свободный слот
            for (int i = 0; i < ListFlags.Count; i++)
            {
                if (ListFlags[i] == null)
                {
                    ListFlags[i] = lair;
                    QuantityFlags[PriorityExecution.None]--;
                    QuantityFlags[lair.PriorityFlag]++;
                    CheckFlags();

                    return;
                }
            }

            Debug.Fail("Не найден слот для флага.");
        }

        internal void UpPriorityFlag(PlayerLair lair)
        {
            int idx = ListFlags.IndexOf(lair);
            Debug.Assert(idx != -1);
            QuantityFlags[lair.PriorityFlag - 1]--;
            QuantityFlags[lair.PriorityFlag]++;

            CheckFlags();
        }


        internal void RemoveFlag(PlayerLair lair)
        {
            Debug.Assert(lair.PriorityFlag > PriorityExecution.None);

            int idx = ListFlags.IndexOf(lair);
            Debug.Assert(idx != -1);
            ListFlags[idx] = null;
            
            // Сжимаем флаги
            for (int i = idx; i < ListFlags.Count - 1; i++)
            {
                ListFlags[i] = ListFlags[i + 1];
            }
            ListFlags[ListFlags.Count - 1] = null;
            QuantityFlags[PriorityExecution.None]++;
            QuantityFlags[lair.PriorityFlag]--;

            CheckFlags();
        }

        private void CheckFlags()
        {
            // Проверяем, что количество флагов сходится с количеством слотов
            // И что количество флагов с приоритетами Hight и Exclusive правильное
            int q = 0;
            int qNonNone = 0;
            foreach (PriorityExecution pe in Enum.GetValues(typeof(PriorityExecution)))
            {
                q += QuantityFlags[pe];
                if (pe > PriorityExecution.None)
                    qNonNone += QuantityFlags[pe];
            }

            Debug.Assert(q == ListFlags.Count);
            Debug.Assert(q <= Lobby.TypeLobby.MaxQuantityFlags);
            Debug.Assert(QuantityFlags[PriorityExecution.High] <= 2);
            Debug.Assert(QuantityFlags[PriorityExecution.Exclusive] <= 1);

            Debug.Assert(qNonNone == ListFlags.Where(l => l != null).Count());
        }

        internal bool ExistsFreeFlag()
        {
            return QuantityFlags[PriorityExecution.None] > 0;
        }

        internal void RemoveLair(PlayerLair l)
        {
            Debug.Assert(l != null);
            Debug.Assert(Lairs[l.Layer, l.Y, l.X] != null);
            Debug.Assert(Lairs[l.Layer, l.Y, l.X] == l);

            Lairs[l.Layer, l.Y, l.X] = null;

            if (Program.formMain.PlayerObjectIsSelected(l))
                Program.formMain.SelectPlayerObject(null);
        }

        private void GenerateLairs()
        {
            // Создание рандомных логов монстров согласно настроек типа лобби
            // Для этого сначала создаем логова по минимальному списку,
            // а оставшиеся ячейки - из оставшихся по максимуму
            //List<TypeLair>
            int idx;
            for (int layer = 0; layer < Lobby.TypeLobby.LairsLayers; layer++)
            {
                List<TypeLair> lairs = new List<TypeLair>();
                lairs.AddRange(Lobby.Lairs[layer]);
                List<Point> cells = GetCells();
                Debug.Assert(cells.Count <= lairs.Count);

                while (cells.Count > 0)
                {
                    // Берем случайную ячейку
                    idx = Lobby.Rnd.Next(cells.Count);

                    // Помещаем в нее логово
                    Debug.Assert(Lairs[layer, cells[idx].Y, cells[idx].X] == null);
                    Lairs[layer, cells[idx].Y, cells[idx].X] = new PlayerLair(this, lairs[idx], cells[idx].X, cells[idx].Y, layer);

                    // Убираем ячейку из списка доступных
                    cells.RemoveAt(idx);
                }
            }

            List<Point> GetCells()
            {
                List<Point> l = new List<Point>();
                for (int y = 0; y < Lobby.TypeLobby.LairsHeight; y++)
                    for (int x = 0; x < Lobby.TypeLobby.LairsWidth; x++)
                        l.Add(new Point(x, y));

                return l;
            }
        }

        internal void ApplyReward(PlayerLair l)
        {
            Gold += l.RewardGold;
            PointConstructionTemple += l.RewardPointTemple;
            PointConstructionTradePost += l.RewardPointTradePost;
        }

        internal bool CanBuildTemple()
        {
            if (PointConstructionTemple == 0)
                return false;

            foreach (PlayerConstruction pb in Constructions.Where(p => p.TypeConstruction is TypeTemple))
            {
                if ((pb.Level == 0) && pb.CheckRequirements())
                    return true;
            }

            return false;
        }

        protected void ApplyStartBonus(StartBonus sb)
        {
            Gold += sb.Gold;
            PointGreatness += sb.Greatness;
            PointConstructionGuild += sb.PointConstructionGuild;
            ScoutRandomLair(sb.ScoutPlace);
        }

        // Интерфейс
        internal abstract void SelectStartBonus();

        //

        internal override string GetName() => Player.Name;
        internal override LobbyPlayer GetPlayer() => this;
        internal override TypePlayer GetTypePlayer() => Player.TypePlayer;
        internal override int GetImageIndexAvatar() => Player.GetImageIndexAvatar();

        // Реализация интерфейса
        BitmapList ICell.BitmapList() => Program.formMain.imListObjectsCell;
        int ICell.ImageIndex()
        {
            Debug.Assert(GetImageIndexAvatar() >= Program.formMain.ImageIndexFirstAvatar);
            Debug.Assert(GetImageIndexAvatar() < Program.formMain.ImageIndexFirstAvatar + Program.formMain.AvatarsCount);
            return GetImageIndexAvatar();
        }
        bool ICell.NormalImage() => IsLive;
        int ICell.Level() => LevelGreatness;
        int ICell.Quantity() => 0;
        void ICell.PrepareHint()
        {
            PrepareHint();
        }

        void ICell.Click(VCCell pe)
        {

        
        }

        void ICell.CustomDraw(Graphics g, int x, int y, bool drawState) { }
    }
}