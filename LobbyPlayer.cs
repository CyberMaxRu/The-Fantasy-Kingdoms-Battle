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
        private int gold;

        private bool startBonusApplied = false;

        // TODO Вынести константы в конфигурацию игры
        internal const int MAX_FLAG_EXCLUSIVE = 1;// Максимальное число флагов с максимальным
        internal const int MAX_FLAG_HIGH = 2;// Максимальное число флагов с высоким приоритетом
        internal const int MAX_FLAG_COUNT = 5;// Максимальное число активных флагов

        public LobbyPlayer(Lobby lobby, Player player, int playerIndex) : base(lobby)
        {
            Player = player;
            PlayerIndex = playerIndex;
            PositionInLobby = playerIndex + 1;

            Initialization = true;

            // Создаем справочик количества приоритетов флагов
            foreach (PriorityExecution pe in Enum.GetValues(typeof(PriorityExecution)))
            {
                QuantityFlags.Add(pe, 0);
            }

            // Настраиваем игрока согласно настройкам лобби
            SetQuantityFlags(lobby.TypeLobby.StartQuantityFlags);

            CurrentLoses = 0;
            MaxLoses = lobby.TypeLobby.MaxLoses;
            for (int i = 0; i < MaxLoses; i++)
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
            foreach (TypeConstruction tck in FormMain.Config.TypeConstructions)
            {
                if (tck.IsInternalConstruction)
                    new PlayerConstruction(this, tck);
            }

            // Инициализация логов
            Lairs = new PlayerConstruction[lobby.TypeLobby.LairsLayers, lobby.TypeLobby.LairsHeight, lobby.TypeLobby.LairsWidth];

            GenerateLairs();
            ScoutRandomLair(lobby.TypeLobby.StartScoutedLairs);

            //
            Gold = Lobby.TypeLobby.Gold;
            if (Player.TypePlayer == TypePlayer.Computer)
                Gold = 100_000;

            Castle = GetPlayerConstruction(FormMain.Config.FindTypeConstruction(FormMain.Config.IDConstructionCastle));
            Castle.Gold = Gold;

            LevelGreatness = 1;
            PointGreatnessForNextLevel = 100;

            PlayerHero king = Castle.HireHero(FormMain.Config.FindTypeCreature("King"));
            PlayerHero advisor = Castle.HireHero(FormMain.Config.FindTypeCreature("Advisor"));
            PlayerHero captain = Castle.HireHero(FormMain.Config.FindTypeCreature("Captain"));
            PlayerHero treasurer = Castle.HireHero(FormMain.Config.FindTypeCreature("Treasurer"));

            //
            /*AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfMana"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Regeneration"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Protection"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ImpProtection"), 2, true));*/

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
                    foreach (StartBonus csb in FormMain.Config.StartBonuses)
                    {
                        csb.ClearQuantity();
                    }

                    StartBonus sb = new StartBonus();
                    List<StartBonus> listBonuses = new List<StartBonus>();

                    while (sb.Points < points)
                    {
                        // Выбираем случайный бонус из списка доступных, чтобы хватило оставшихся очков
                        listBonuses.Clear();
                        listBonuses.AddRange(FormMain.Config.StartBonuses.Where(b => ((b.CurrentQuantity == -1) || (b.CurrentQuantity < b.MaxQuantity)) && (b.Points <= (points - sb.Points))));
                        Debug.Assert(listBonuses.Count > 0);
                        sb.AddBonus(listBonuses[lobby.Rnd.Next(listBonuses.Count)]);
                    }

                    return sb;
                }
            }

            Initialization = false;
        }

        internal virtual void PrepareTurn()
        {
            UpdateBuildersNextDay();

            foreach (PlayerConstruction pc in Constructions)
                if (pc.Level > 0)
                    pc.PrepareTurn();

            Builders = BuildersAtNextDay;
            if (Lobby.Day == 1)
                Builders += Lobby.TypeLobby.StartBuilders;
            FreeBuilders = Builders;

            SetTaskForHeroes();
        }

        internal abstract void DoTurn();
        internal abstract void EndTurn();
        internal virtual void AfterEndTurn()
        {
            foreach (PlayerConstruction pc in Constructions)
                if (pc.Level > 0)
                    pc.AfterEndTurn();
        }

        private void UpdateBuildersNextDay()
        {
            BuildersAtNextDay = Castle.TypeConstruction.Levels[Castle.Level].BuildersPerDay;
        }

        //
        protected void ScoutRandomLair(int scoutLaires)
        {
            if (scoutLaires > 0)
            {
                for (int i = 0; i < Lairs.GetLength(0); i++)
                {
                    scoutLaires = ScoutLayer(i, scoutLaires);
                    if (scoutLaires == 0)
                        break;
                }
            }

            int ScoutLayer(int layer, int maxScout)
            {
                List<PlayerConstruction> lairs = new List<PlayerConstruction>();
                for (int y = 0; y < Lairs.GetLength(1); y++)
                    for (int x = 0; x < Lairs.GetLength(2); x++)
                        if (Lairs[layer, y, x].Hidden)
                            lairs.Add(Lairs[layer, y, x]);

                int scouting = Math.Min(maxScout, lairs.Count);
                int restScouting = maxScout - scouting;
                int index;
                for (int i = 0; i < scouting; i++)
                {
                    index = Lobby.Rnd.Next(lairs.Count);
                    lairs[index].Unhide();
                    lairs.RemoveAt(index);
                }

                return restScouting;
            }
        }

        private void CreateExternalConstructions(TypeConstruction typeConstruction, int level, int layer, int quantity)
        {
            Debug.Assert((typeConstruction.Category == CategoryConstruction.External) || (typeConstruction.Category == CategoryConstruction.BasePlace) || (typeConstruction.Category == CategoryConstruction.Place));
            Debug.Assert(level <= typeConstruction.MaxLevel);
            //Debug.Assert(typeConstruction.TypePlaceForConstruct.ID == FormMain.Config.IDEmptyPlace);

            if (quantity > 0)
            {
                // Собираем список пустых мест
                List<PlayerConstruction> listEmptyPlaces = new List<PlayerConstruction>();
                for (int y = 0; y < Lairs.GetLength(1); y++)
                    for (int x = 0; x < Lairs.GetLength(2); x++)
                        if (Lairs[layer, y, x].TypeConstruction.ID == FormMain.Config.IDEmptyPlace)
                            listEmptyPlaces.Add(Lairs[layer, y, x]);

                Debug.Assert(quantity <= listEmptyPlaces.Count);

                // 
                int index;
                while (quantity > 0)
                {
                    index = Lobby.Rnd.Next(listEmptyPlaces.Count);
                    PlayerConstruction empty = listEmptyPlaces[index];
                    PlayerConstruction pc = new PlayerConstruction(this, typeConstruction, level, empty.X, empty.Y, empty.Layer);
                    Lairs[pc.Layer, pc.Y, pc.X] = pc;
                    listEmptyPlaces.RemoveAt(index);
                    quantity--;
                }

                Program.formMain.UpdateNeighborhood();
            }
        }

        // Расчет после завершения хода игроком
        internal void CalcFinalityTurn()
        {
            // Убеждаемся, что у нас не сломалось соответствие флагов
            foreach (PlayerConstruction pl in Lairs)
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
            List<PlayerConstruction> tempListLair = ListFlags.ToList();// Работаем с копией списка, так как текущий будет меняться по мере обработки флагов
            int maxSteps = FormMain.Config.MaxDurationBattleWithMonster * FormMain.Config.StepsInSecond;

            foreach (PlayerConstruction pl in tempListLair)
            {
                Battle b = null;
                WindowBattle formBattle;
                TypeFlag typeFlag;

                if ((pl != null) && (pl.listAttackedHero.Count > 0) && (pl.TypeFlag != TypeFlag.Battle))
                {
                    Debug.Assert((pl.TypeFlag == TypeFlag.Scout) || (pl.TypeFlag == TypeFlag.Attack) || (pl.TypeFlag == TypeFlag.Defense));

                    typeFlag = pl.TypeFlag;

                    if (pl.TypeFlag == TypeFlag.Scout)
                    {
                        pl.DoScout();
                    }
                    else if (pl.TypeFlag == TypeFlag.Attack)
                    {
                        // У Сокровища монстров может не быть. Но бой посчитать надо
                        //Debug.Assert(pl.Monsters.Count > 0);

                        PreparingForBattle();

                        // Включить, когда ИИ может выбирать цель
                        pl.Participant.PreparingForBattle();

                        //Debug.Assert(p.TargetLair.CombatHeroes.Count > 0);

                        bool showForPlayer = false;// Player.TypePlayer == TypePlayer.Human;
                        b = new Battle(this, pl.Participant, Lobby.Day, Lobby.Rnd.Next(), maxSteps, showForPlayer);

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
                    else if (pl.TypeFlag == TypeFlag.Defense)
                    {
                        pl.DoDefense();
                    }
                    else
                        throw new Exception("Неизвестный флаг: " + pl.TypeFlag.ToString());

                    if (this is LobbyPlayerHuman h)
                        h.AddEvent(new VCEventExecuteFlag(typeFlag, pl.TypeConstruction, pl.Destroyed ? null : pl, (b is null) || (b.Winner == this), b));
                }
            }
        }

        internal void CalcResultTurn()
        {
            if (IsLive == true)
            {
                IncomeGold(Income());

                ValidateHeroes();

                QuantityHeroes = CombatHeroes.Count();
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
        internal bool Initialization { get; }
        internal int LevelGreatness { get; }// Уровень величия
        internal int PointGreatness { get; private set; }// Очков величия
        internal int PointGreatnessForNextLevel { get; }// Очков величия до следующего уровня
        internal List<PlayerConstruction> Constructions { get; } = new List<PlayerConstruction>();
        internal int LevelCastle => Castle.Level;
        internal List<PlayerHero> AllHeroes { get; } = new List<PlayerHero>();
        internal int Gold { get => gold; private set { gold = value; if (Castle != null) Castle.Gold = gold; } }// Текущее количество золота
        internal int GoldCollected { get; private set; }// Собрано золота за игру
        internal int GreatnessCollected { get; private set; }// Собрано величия за игру

        // Информация о поражениях и вылете из лобби
        internal List<LoseInfo> LoseInfo { get; } = new List<LoseInfo>();
        internal int CurrentLoses { get; private set; }// Текущее количество поражений
        internal int MaxLoses { get; private set; }// Максимальное количество поражений
        internal int DayOfEndGame { get; private set; }// День вылета из лобби
        internal int SkippedBattles { get; set; }// Сколько битв было пропущено (про причине нечетного количества игроков)
        internal bool SkipBattle { get; set; }// Битва на этому ходу будет пропущена

        internal int Builders { get; private set; }
        internal int FreeBuilders { get; private set; }
        internal int BuildersAtNextDay { get; private set; }
        internal List<StartBonus> VariantsStartBonuses { get; }// Варианты стартовых бонусов

        internal int QuantityHeroes { get; private set; }

        internal PlayerItem[] Warehouse = new PlayerItem[FormMain.Config.WarehouseMaxCells];// Предметы на складе игрока

        // Логова
        internal PlayerConstruction[,,] Lairs { get; }
        internal List<PlayerConstruction> ListFlags { get; } = new List<PlayerConstruction>();
        internal Dictionary<PriorityExecution, int> QuantityFlags { get; } = new Dictionary<PriorityExecution, int>();
        internal int LairsScouted { get; private set;}
        internal int LairsShowed { get; private set; }

        // Визуальные контролы
        internal PanelPlayer Panel { get; set; }
        private LobbyPlayer opponent;// Убрать это
        internal LobbyPlayer Opponent { get { return opponent; } set { if (value != this) { if (opponent != value) { opponent = value; UpdateOpponent(); } } else new Exception("Нельзя указать оппонентов самого себя."); } }
        internal PlayerConstruction FlagAttackToOpponent { get; private set; }

        private void UpdateOpponent()
        {
            if (opponent is null)
            {
                Debug.Assert(FlagAttackToOpponent != null);
                Debug.Assert(ListFlags.IndexOf(FlagAttackToOpponent) != -1);

                ListFlags.Remove(FlagAttackToOpponent);
                FlagAttackToOpponent = null;
            }
            else
            {
                Debug.Assert(FlagAttackToOpponent is null);
                FlagAttackToOpponent = opponent.GetPlayerConstruction(FormMain.Config.FindTypeConstruction(FormMain.Config.IDConstructionCastle));
                FlagAttackToOpponent.AttackToCastle();

                Debug.Assert(ListFlags.IndexOf(FlagAttackToOpponent) == -1);
                ListFlags.Insert(0, FlagAttackToOpponent);
            }

            Program.formMain.LairsWithFlagChanged();
        }

        internal PlayerConstruction GetPlayerConstruction(TypeConstruction b)
        {
            Debug.Assert(b != null);

            foreach (PlayerConstruction pb in Constructions)
            {
                if (pb.TypeConstruction == b)
                    return pb;
            }

            throw new Exception("У игрока " + GetName() + " сооружение " + b.ID + " не найдено.");
        }

        internal void AddHero(PlayerHero ph)
        {
            Debug.Assert(CombatHeroes.IndexOf(ph) == -1);
            Debug.Assert(AllHeroes.IndexOf(ph) == -1);

            AllHeroes.Add(ph);
            if ((ph.TypeCreature.ID != "King") && (ph.TypeCreature.ID != "Advisor") && (ph.TypeCreature.ID != "Captain") && (ph.TypeCreature.ID != "Treasurer"))
                AddCombatHero(ph);

            SetTaskForHeroes();

            if (Player.TypePlayer == TypePlayer.Human)
                Program.formMain.ListHeroesChanged();
        }

        internal void Constructed(PlayerConstruction pb)
        {
            Debug.Assert(pb.CheckRequirements());

            SpendGold(pb.CostBuyOrUpgrade());
            FreeBuilders -= pb.TypeConstruction.Levels[pb.Level + 1].Builders;
            AddGreatness(pb.TypeConstruction.Levels[pb.Level + 1].GreatnessByConstruction);

            Debug.Assert(FreeBuilders >= 0);
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

        internal override void PreparingForBattle()
        {
            base.PreparingForBattle();
        }

        internal void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(
                Player.Name, $"{PositionInLobby} место",
                "Уровень Замка: " + LevelCastle.ToString() + Environment.NewLine
                    + "Героев: " + QuantityHeroes.ToString() + Environment.NewLine
                    + " " + Environment.NewLine
                    + "Поражений: " + CurrentLoses.ToString()
                    + (DayOfEndGame > 0 ? Environment.NewLine + "Поражение в лобби: " + DayOfEndGame.ToString() + " день" : ""));
        }

        // Метод по распределению задач героев
        internal void SetTaskForHeroes()
        {
            if (CombatHeroes.Count == 0)
                return;

            List<PlayerHero> freeHeroes = new List<PlayerHero>();// Список свободных героев

            // Сначала сбрасываем всем состояние
            foreach (PlayerHero ph in CombatHeroes)
            {
                if ((ph.StateCreature.ID == NameStateCreature.DoAttackFlag.ToString())
                    || (ph.StateCreature.ID == NameStateCreature.DoScoutFlag.ToString())
                    || (ph.StateCreature.ID == NameStateCreature.DoDefenseFlag.ToString())
                    || (ph.StateCreature.ID == NameStateCreature.BattleWithPlayer.ToString()))
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
            // Но первым делом отбираем героев на битву с другим игроком

            if (Lobby.IsDayForBattleBetweenPlayers() && !SkipBattle)
            {
                Debug.Assert(FlagAttackToOpponent != null);

                int takeHeroes = Math.Min(Lobby.TypeLobby.MaxHeroesForBattle, freeHeroes.Count);                    
                for (int i = 0; i < takeHeroes; i++)
                {
                    PlayerHero ph = CombatHeroes[i] as PlayerHero;
                    freeHeroes.Remove(ph);
                    FlagAttackToOpponent.AddAttackingHero(ph);
                    ph.SetState(NameStateCreature.BattleWithPlayer);
                }
            }

            if (freeHeroes.Count == 0)
                return;

            if (CountActiveFlags() > 0)
            {
                foreach (PlayerConstruction pl in ListFlags.Where(pl => (pl != null) && (pl.TypeFlag == TypeFlag.Scout)))
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

                        foreach (PlayerConstruction pl in ListFlags.Where(pl => (pl != null) && ((pl.TypeFlag == TypeFlag.Attack) || (pl.TypeFlag == TypeFlag.Defense))))
                            if (pl != null)
                            {
                                heroesToFlag = Math.Min(Math.Min(freeHeroes.Count, heroesPerFlag), pl.TypeConstruction.MaxHeroes);

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
            foreach (PlayerConstruction pl in ListFlags)
                if (pl != null)
                    count++;

            return count;
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
            foreach (PlayerConstruction pl in ListFlags)
            {
                if (pl == null)
                    QuantityFlags[PriorityExecution.None]++;
                else
                    QuantityFlags[pl.PriorityFlag]++;
            }
        }

        internal void AddFlag(PlayerConstruction lair)
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

        internal void UpPriorityFlag(PlayerConstruction lair)
        {
            int idx = ListFlags.IndexOf(lair);
            Debug.Assert(idx != -1);
            QuantityFlags[lair.PriorityFlag - 1]--;
            QuantityFlags[lair.PriorityFlag]++;

            CheckFlags();
        }


        internal void RemoveFlag(PlayerConstruction lair)
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
            if (FlagAttackToOpponent != null)
            {
                q++;
                qNonNone++;
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

        internal void RemoveLair(PlayerConstruction l)
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
            int idxCell;
            int idxTypeLair;
            for (int layer = 0; layer < Lobby.TypeLobby.LairsLayers; layer++)
            {
                List<TypeConstruction> lairs = new List<TypeConstruction>();
                lairs.AddRange(Lobby.Lairs[layer]);
                List<Point> cells = GetCells();
                Debug.Assert(cells.Count <= lairs.Count);

                while (cells.Count > 0)
                {
                    // Берем случайную ячейку
                    idxCell = Lobby.Rnd.Next(cells.Count);
                    // Берем случайное логово
                    idxTypeLair = Lobby.Rnd.Next(lairs.Count);

                    // Помещаем в нее логово
                    Debug.Assert(Lairs[layer, cells[idxCell].Y, cells[idxCell].X] == null);
                    Lairs[layer, cells[idxCell].Y, cells[idxCell].X] = new PlayerConstruction(this, lairs[idxTypeLair], lairs[idxTypeLair].DefaultLevel, cells[idxCell].X, cells[idxCell].Y, layer);
                    
                    cells.RemoveAt(idxCell);// Убираем ячейку из списка доступных
                    lairs.RemoveAt(idxTypeLair);// Убираем тип логова из списка доступных
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

        internal void ApplyReward(PlayerConstruction l)
        {
            if (l.TypeConstruction.TypeReward != null)
            {
                IncomeGold(l.TypeConstruction.TypeReward.Gold);
                AddGreatness(l.TypeConstruction.TypeReward.Greatness);
            }

            if (l.TypeConstruction.HiddenReward != null)
            {
                IncomeGold(l.TypeConstruction.HiddenReward.Gold);
                AddGreatness(l.TypeConstruction.HiddenReward.Greatness);
            }
        }

        protected void ApplyStartBonus(StartBonus sb)
        {
            IncomeGold(sb.Gold);
            PointGreatness += sb.Greatness;
            Builders += sb.Builders;
            FreeBuilders += sb.Builders;
            CreateExternalConstructions(FormMain.Config.FindTypeConstruction(FormMain.Config.IDPeasantHouse), 1, 0, sb.PeasantHouse);
            TypeConstruction holyPlace = FormMain.Config.FindTypeConstruction(FormMain.Config.IDHolyPlace);
            CreateExternalConstructions(holyPlace, holyPlace.DefaultLevel, 0, sb.HolyPlace);
            TypeConstruction tradePost = FormMain.Config.FindTypeConstruction(FormMain.Config.IDTradePost);
            CreateExternalConstructions(tradePost, tradePost.DefaultLevel, 0, sb.TradePlace);
            ScoutRandomLair(sb.Scouting);

            startBonusApplied = true;
        }

        internal void AddLose()
        {
            Debug.Assert(CurrentLoses < MaxLoses);
            Debug.Assert(LoseInfo[CurrentLoses] is null);
            Debug.Assert(!(opponent is null));
            Debug.Assert(IsLive);
            Debug.Assert(DayOfEndGame == 0);

            LoseInfo[CurrentLoses] = new LoseInfo(Lobby.Day, opponent);
            CurrentLoses++;

            if (CurrentLoses == MaxLoses)
            {
                IsLive = false;
                DayOfEndGame = Lobby.Day;
            }
        }

        internal static int TypeFlagToImageIndex(TypeFlag typeFlag)
        {
            switch (typeFlag)
            {
                case TypeFlag.Scout:
                    return FormMain.Config.Gui48_FlagScout;
                case TypeFlag.Defense:
                    return FormMain.Config.Gui48_FlagDefense;
                case TypeFlag.Attack:
                    return FormMain.Config.Gui48_FlagAttack;
                default:
                    throw new Exception("Неизвестный тип флага: " + typeFlag.ToString() + ".");
            }
        }

        internal void SpendGold(int gold)
        {
            Debug.Assert(gold >= 0);
            Debug.Assert(Gold >= gold);

            Gold -= gold;
        }

        internal void ReturnGold(int gold)
        {
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= Lobby.TypeLobby.MaxGold);
            Debug.Assert(gold >= 0);

            Gold += AllowAddGold(gold);
        }

        internal void IncomeGold(int gold)
        {
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= Lobby.TypeLobby.MaxGold);
            Debug.Assert(gold >= 0, $"Доход: {gold}");

            int addGold = AllowAddGold(gold);
            Gold += addGold;
            GoldCollected += addGold;
        }

        private int AllowAddGold(int gold)
        {
            return Gold + gold <= Lobby.TypeLobby.MaxGold ? gold : Lobby.TypeLobby.MaxGold - Gold;
        }

        internal void AddGreatness(int greatness)
        {
            Debug.Assert(greatness >= 0);

            if (greatness > 0)
            {
                PointGreatness += greatness;
                GreatnessCollected += greatness;
            }
        }

        internal abstract void PlayerIsWin();

        // Интерфейс
        internal virtual void SelectStartBonus()
        {
            Debug.Assert(!startBonusApplied);
            Debug.Assert(VariantsStartBonuses.Count > 0);
        }

        internal int PointGreatnessPerDay()
        {
            int g = 0;

            foreach (PlayerConstruction pc in Constructions)
                if (pc.Level > 0)
                    g += pc.GreatnessPerDay();

            return g;
        }

        internal bool CanBuildTypeConstruction(TypeConstruction type)
        {
            // Сначала проверяем наличие золота
            if (Gold < type.Levels[1].Cost)
                return false;

            // Проверяем наличие очков строительства
            if (type.Levels[1].Builders > FreeBuilders)
                return false;

            // Проверяем требования к зданиям
            return CheckRequirements(type.Levels[1].Requirements);
        }

        internal List<TextRequirement> GetTextRequirementsBuildTypeConstruction(TypeConstruction type)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            TextRequirements(type.Levels[1].Requirements, list);

            return list;
        }

        internal void PrepareHintForBuildTypeConstruction(TypeConstruction type)
        {
            Program.formMain.formHint.AddStep1Header(type.Name, "Уровень 1", type.Description);
            Program.formMain.formHint.AddStep2Income(type.Levels[1].Income);
            Program.formMain.formHint.AddStep3Greatness(type.Levels[1].GreatnessByConstruction, type.Levels[1].GreatnessPerDay);
            Program.formMain.formHint.AddStep35PlusBuilders(type.Levels[1].BuildersPerDay);
            Program.formMain.formHint.AddStep3Requirement(GetTextRequirementsBuildTypeConstruction(type));
            Program.formMain.formHint.AddStep4Gold(type.Levels[1].Cost, Gold >= type.Levels[1].Cost);
            Program.formMain.formHint.AddStep5Builders(type.Levels[1].Builders, FreeBuilders >= type.Levels[1].Builders);
        }

        //
        internal override string GetName() => Player.Name;
        internal override LobbyPlayer GetPlayer() => this;
        internal override TypePlayer GetTypePlayer() => Player.TypePlayer;
        internal override int GetImageIndexAvatar() => Player.ImageIndex;

        // Реализация интерфейса
        BitmapList ICell.BitmapList() => Program.formMain.imListObjects48;
        int ICell.ImageIndex()
        {
            return GetImageIndexAvatar();
        }
        bool ICell.NormalImage() => IsLive;
        int ICell.Level() => LevelGreatness;
        int ICell.Quantity() => 0;
        string ICell.Cost() => null;
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