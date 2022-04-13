using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс информации о поражении
    internal sealed class LoseInfo
    {
        public LoseInfo(int day, Player opponent)
        {
            Day = day;
            Opponent = opponent;
        }

        internal int Day { get; }
        internal Player Opponent { get; }
    }

    // Класс игрока лобби
    internal abstract class Player : BattleParticipant
    {
        private Construction Castle;

        private bool startBonusApplied = false;

        // TODO Вынести константы в конфигурацию игры
        internal const int MAX_FLAG_EXCLUSIVE = 1;// Максимальное число флагов с максимальным
        internal const int MAX_FLAG_HIGH = 2;// Максимальное число флагов с высоким приоритетом
        internal const int MAX_FLAG_COUNT = 5;// Максимальное число активных флагов

        private List<UnitOfQueueForBuy> queueShopping = new List<UnitOfQueueForBuy>();

        public Player(Lobby lobby, DescriptorPlayer player, int playerIndex) : base(player, lobby)
        {
            Descriptor = player;
            PlayerIndex = playerIndex;
            PositionInLobby = playerIndex + 1;

            Initialization = true;

            //
            BaseResources = new ListBaseResources(lobby.TypeLobby.BaseResources);
            if (Descriptor.TypePlayer == TypePlayer.Computer)
                BaseResources[FormMain.Descriptors.Gold.Number].Quantity = 100_000;
            ResourceGold = BaseResources[FormMain.Descriptors.Gold.Number];

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

            PercentCorruption = 10;
            ChangeCorruption = 1;

            CurrentLevelTax = FormMain.Descriptors.DefaultLevelTax;

            // Настраиваем постоянные бонусы
            if (lobby.TypeLobby.VariantPersistentBonus > 0)
            {
                VariantPersistentBonus = new List<DescriptorPersistentBonus>[(int)TypePersistentBonus.Other + 1];
                for (int i = 0; i < VariantPersistentBonus.GetLength(0); i++)
                    VariantPersistentBonus[i] = new List<DescriptorPersistentBonus>();

                // Сначала добавляем все бонусы в списки
                foreach (DescriptorPersistentBonus dpb in FormMain.Descriptors.PersistentBonuses)
                {
                    VariantPersistentBonus[(int)dpb.Type].Add(dpb);
                }

                for (int i = 0; i < VariantPersistentBonus.GetLength(0); i++)
                {
                    Assert(VariantPersistentBonus[i].Count >= lobby.TypeLobby.VariantPersistentBonus);

                    while (VariantPersistentBonus[i].Count > lobby.TypeLobby.VariantPersistentBonus)
                    {
                        VariantPersistentBonus[i].RemoveAt(lobby.Rnd.Next(VariantPersistentBonus[i].Count));
                    }
                }
            }

            // Настраиваем варианты бонусов типов героев
            VariantsBonusedTypeSimpleHero = new List<DescriptorCreature>();
            VariantsBonusedTypeTempleHero = new List<DescriptorCreature>();

            List<DescriptorCreature> listSimpleHero = new List<DescriptorCreature>();
            List<DescriptorCreature> listTempleHero = new List<DescriptorCreature>();
            foreach (DescriptorCreature dc in FormMain.Descriptors.Creatures)
            {
                if (dc.CategoryCreature == CategoryCreature.Hero)
                {
                    switch (dc.TypeHero)
                    {
                        case TypeHero.Base:
                        case TypeHero.Advanced:
                            listSimpleHero.Add(dc);
                            break;
                        case TypeHero.Temple:
                            listTempleHero.Add(dc);
                            break;
                        default:
                            DoException($"Неизвестный тип героя: {dc.TypeHero}");
                            break;
                    }
                }
            }

            while (VariantsBonusedTypeSimpleHero.Count < lobby.TypeLobby.VariantsUpSimpleHero)
            {
                int idx = lobby.Rnd.Next(listSimpleHero.Count);
                VariantsBonusedTypeSimpleHero.Add(listSimpleHero[idx]);
                listSimpleHero.RemoveAt(idx);
            }

            while (VariantsBonusedTypeTempleHero.Count < lobby.TypeLobby.VariantsUpTempleHero)
            {
                int idx = lobby.Rnd.Next(listTempleHero.Count);
                VariantsBonusedTypeTempleHero.Add(listTempleHero[idx]);
                listTempleHero.RemoveAt(idx);
            }

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
            foreach (DescriptorConstruction tck in FormMain.Descriptors.Constructions)
            {
                if (tck.IsInternalConstruction)
                    new Construction(this, tck, null);
            }

            // Инициализация окрестностей
            foreach (TypeLobbyConstructionSettings cs in lobby.TypeLobby.Neighbourhood)
            {
                for (int i = 0; i < cs.Quantity; i++)
                {
                    Construction c = new Construction(this, cs.Construction, null);
                    Neighbourhood.Add(c);
                }
            }

            //
            Location l;

            Locations = new Location[lobby.TypeLobby.MapHeight, lobby.TypeLobby.MapWidth];

            /*foreach (TypeLobbyLocationSettings ls in lobby.TypeLobby.Locations)
            {
                l = new Location(this, ls);
                Debug.Assert(Locations[l.Settings.Coord.Y, l.Settings.Coord.X] is null);
                Locations[l.Settings.Coord.Y, l.Settings.Coord.X] = l;

                if (l.Settings.ID == lobby.TypeLobby.LocationCapital.ID)
                {
                    Debug.Assert(LocationCapital is null);
                    LocationCapital = l;
                }
            }*/

            CurrentLocation = LocationCapital;

            // Инициализация логов
            ScoutRandomLair(lobby.TypeLobby.StartScoutedLairs, true);

            //
            Castle = GetPlayerConstruction(FormMain.Descriptors.FindConstruction(FormMain.Config.IDConstructionCastle));
            Castle.Gold = Gold;
            Graveyard = GetPlayerConstruction(FormMain.Descriptors.FindConstruction(FormMain.Config.IDCityGraveyard));

            LevelGreatness = 1;
            PointGreatnessForNextLevel = 100;

            Hero king = Castle.HireHero(FormMain.Descriptors.FindCreature("King"), null);
            Hero advisor = Castle.HireHero(FormMain.Descriptors.FindCreature("Advisor"), null);
            Hero captain = Castle.HireHero(FormMain.Descriptors.FindCreature("Captain"), null);
            Hero treasurer = Castle.HireHero(FormMain.Descriptors.FindCreature("Treasurer"), null);

            //
            /*AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("PotionOfMana"), 10, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Regeneration"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("Protection"), 1, true));
            AddItem(new PlayerItem(FormMain.Config.FindItem("ImpProtection"), 2, true));*/

            ValidateHeroes();

            //LairCaptured(FormMain.Config.FindConstruction("WolfsDen"));
            //LairCaptured(FormMain.Config.FindConstruction("WolfsDen"));

            StartBonus GenerateStartBonus()
            {
                int restAttempts = 100;
                bool needRegenerate;
                while (restAttempts > 0)
                {
                    StartBonus newSb = GenerateNew(lobby.TypeLobby.PointStartBonus);
                    needRegenerate = false;

                    if (newSb.QuantityElements() > FormMain.Config.MaxElementInStartBonus)
                        needRegenerate = true;
                    else
                    {
                        // Ищем, есть ли такой же бонус
                        foreach (StartBonus b in VariantsStartBonuses)
                            if (b.Equals(newSb))
                            {
                                needRegenerate = true;
                                break;
                            }
                    }
                    
                    if (!needRegenerate)
                        return newSb;

                    restAttempts--;
                }

                throw new Exception("Не удалось подобрать уникальный бонус.");

                StartBonus GenerateNew(int points)
                {
                    foreach (StartBonus csb in FormMain.Descriptors.StartBonuses)
                    {
                        csb.ClearQuantity();
                    }

                    StartBonus sb = new StartBonus();
                    List<StartBonus> listBonuses = new List<StartBonus>();

                    while (sb.Points < points)
                    {
                        // Выбираем случайный бонус из списка доступных, чтобы хватило оставшихся очков
                        listBonuses.Clear();
                        listBonuses.AddRange(FormMain.Descriptors.StartBonuses.Where(b => ((b.CurrentQuantity == -1) || (b.CurrentQuantity < b.MaxQuantity)) && (b.Points <= (points - sb.Points))));
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
            ExtraLevelUp = 0;
            ExtraResearch = 0;

            foreach (Construction pc in Constructions)
                pc.PrepareTurn();

            List<Hero> listForDelete = new List<Hero>();

            foreach (Hero h in CombatHeroes)
            {
                if (h.NeedMoveToAbode != null)
                {
                    Debug.Assert(h.Abode.Heroes.IndexOf(h) != -1);
                    Debug.Assert(h.NeedMoveToAbode.Heroes.IndexOf(h) == -1);

                    h.Abode.Heroes.Remove(h);
                    h.NeedMoveToAbode.Heroes.Add(h);
                    h.NeedMoveToAbode = null;
                }

                if (!h.IsLive)
                {
                    listForDelete.Add(h);
                }

            }

            // Убираем мертвых героев из своих списков
            foreach (Hero h in listForDelete)
            {
                Debug.Assert(AllHeroes.IndexOf(h) != -1);
                Debug.Assert(CombatHeroes.IndexOf(h) != -1);

                AllHeroes.Remove(h);
                CombatHeroes.Remove(h);
            }

            // Если первое число недели, то выплачиваем жалованье
            if (Lobby.Day == 1)
            {
                int earnedSalary;
                foreach (Hero h in AllHeroes)
                {
                    earnedSalary = h.SalaryPerWeek();
                    if (earnedSalary > 0)
                    {
                        SpendGold(earnedSalary);
                        h.AddGold(earnedSalary);
                    }
                }
            }

            Builders = Castle.TypeConstruction.Levels[Castle.Level].BuildersPerDay;
            if (Lobby.Turn == 1)
                Builders += Lobby.TypeLobby.StartBuilders;
            FreeBuilders = Builders;

            SetTaskForHeroes();
        }

        internal abstract void DoTurn();
        internal abstract void EndTurn();
        internal virtual void CalcTurn()
        {
            queueShopping.Clear();

            // Собираем очередь из героев на посещение сооружений
            foreach (Construction pc in Constructions)
            {
                if (pc.Level > 0)
                    pc.PrepareQueueShopping(queueShopping);
            }

            // Выполняем покупки
            foreach (UnitOfQueueForBuy u in queueShopping)
            {
                //if (u.Hero.CounterConstructionForBuy > 0)
                //    u.Hero.DoShopping(u.Construction);
            }
        }

        //
        protected void ScoutRandomLair(int scoutLaires, bool needNotice)
        {
            return;
            if (scoutLaires > 0)
            {
                foreach (Location l in Locations)
                {
                    scoutLaires = ScoutLayer(l, scoutLaires);
                    if (scoutLaires == 0)
                        break;
                }
            }

            int ScoutLayer(Location ll, int maxScout)
            {
                List<Construction> lairs = new List<Construction>();
                for (int y = 0; y < ll.Lairs.GetLength(0); y++)
                    for (int x = 0; x < ll.Lairs.GetLength(1); x++)
                        if (ll.Lairs[y, x].Hidden)
                            lairs.Add(ll.Lairs[y, x]);

                int scouting = Math.Min(maxScout, lairs.Count);
                int restScouting = maxScout - scouting;
                int index;
                for (int i = 0; i < scouting; i++)
                {
                    index = Lobby.Rnd.Next(lairs.Count);
                    lairs[index].Unhide(needNotice);
                    lairs.RemoveAt(index);
                }

                return restScouting;
            }
        }

        private void CreateExternalConstructions(DescriptorConstruction typeConstruction, int level, Location location, int quantity, TypeNoticeForPlayer typeNotice)
        {
            Debug.Assert((typeConstruction.Category == CategoryConstruction.External) || (typeConstruction.Category == CategoryConstruction.BasePlace) || (typeConstruction.Category == CategoryConstruction.Place));
            Debug.Assert(level <= typeConstruction.MaxLevel);
            //Debug.Assert(typeConstruction.TypePlaceForConstruct.ID == FormMain.Config.IDEmptyPlace);

            /*if (quantity > 0)
            {
                // Собираем список пустых мест
                List<Construction> listEmptyPlaces = new List<Construction>();
                for (int y = 0; y < location.Lairs.GetLength(0); y++)
                    for (int x = 0; x < location.Lairs.GetLength(1); x++)
                        if (location.Lairs[y, x].TypeConstruction.ID == location.Settings.DefaultConstruction.ID)
                            listEmptyPlaces.Add(location.Lairs[y, x]);

                Debug.Assert(quantity <= listEmptyPlaces.Count);

                // 
                int index;
                while (quantity > 0)
                {
                    index = Lobby.Rnd.Next(listEmptyPlaces.Count);
                    Construction empty = listEmptyPlaces[index];
                    Construction pc = new Construction(this, typeConstruction, level, empty.X, empty.Y, empty.Location, typeNotice);
                    location.Lairs[pc.Y, pc.X] = pc;
                    listEmptyPlaces.RemoveAt(index);
                    quantity--;
                }

                Lobby.Layer.UpdateNeighborhoods();
            }*/
        }

        // Расчет после завершения хода игроком
        internal void CalcFinalityTurn()
        {
            // Убеждаемся, что у нас не сломалось соответствие флагов
            foreach (Location l in Locations)
            {
                foreach (Construction lc in l.Lairs)
                {
                    if (lc != null)
                    {
                        if (lc.PriorityFlag != PriorityExecution.None)
                            Debug.Assert(ListFlags.IndexOf(lc) != -1);
                        else
                            Debug.Assert(ListFlags.IndexOf(lc) == -1);
                    }
                }
            }

            // Расчет флагов на логова
            List<Construction> tempListLair = ListFlags.ToList();// Работаем с копией списка, так как текущий будет меняться по мере обработки флагов
            int maxSteps = FormMain.Config.MaxDurationBattleWithMonster * FormMain.Config.StepsInSecond;

            foreach (Construction pl in tempListLair)
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
                        pl.PreparingForBattle();

                        //Debug.Assert(p.TargetLair.CombatHeroes.Count > 0);

                        bool showForPlayer = false;// Player.TypePlayer == TypePlayer.Human;
                        b = new Battle(this, pl, Lobby.Turn, Lobby.Rnd.Next(), maxSteps, showForPlayer);

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

                            if (!pl.TypeConstruction.IsOurConstruction)
                                LairCaptured(pl.TypeConstruction);
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

                    if (this is PlayerHuman h)
                        h.AddEvent(new VCEventExecuteFlag(typeFlag, pl.TypeConstruction, pl.Destroyed ? null : pl, (b is null) || (b.Winner == this), b));
                }
            }
        }

        private void LairCaptured(DescriptorConstruction dc)
        {
            Debug.Assert(!dc.IsOurConstruction);

            if (destroyedLair.ContainsKey(dc))
                destroyedLair[dc]++;
            else
                destroyedLair.Add(dc, 1);
        }

        internal int LairsDestroyed(DescriptorConstruction dc)
        {
            Debug.Assert(!dc.IsOurConstruction);

            return destroyedLair.ContainsKey(dc) ? destroyedLair[dc] : 0;
        }

        internal int TypeConstructionBuilded(DescriptorTypeConstruction typeConstruction)
        {
            int builded = 0;
            foreach (Construction c in Constructions)
            {
                if ((c.TypeConstruction.TypeConstruction == typeConstruction) && (!c.Hidden) && (c.Level > 0))
                    builded++;
            }

            return builded;
        }

        internal void CalcResultTurn()
        {
            if (IsLive == true)
            {
                ReceivedResource(new ListBaseResources(Income()));

                ValidateHeroes();

                QuantityHeroes = CombatHeroes.Count();
            }
        }

        private void ValidateHeroes()
        {
            foreach (Construction pb in Constructions)
                pb.ValidateHeroes();
        }

        internal DescriptorPlayer Descriptor { get; }
        internal int PlayerIndex { get; }
        internal int PositionInLobby { get; set; }
        internal bool Initialization { get; }
        internal int LevelGreatness { get; }// Уровень величия
        internal int PointGreatness { get; private set; }// Очков величия
        internal int PointGreatnessForNextLevel { get; }// Очков величия до следующего уровня
        internal List<Construction> Constructions { get; } = new List<Construction>();
        internal List<Construction> Neighbourhood { get; } = new List<Construction>();// Окрестности
        internal int LevelCastle => Castle.Level;
        internal List<Hero> AllHeroes { get; } = new List<Hero>();

        internal DescriptorLevelTax CurrentLevelTax { get; set; }// Текущий уровень налогов
        internal int Gold { get => BaseResources[FormMain.Descriptors.Gold.Number].Quantity; }// Текущее количество золота
        internal int GreatnessCollected { get; private set; }// Собрано величия за игру
        internal ListBaseResources BaseResources { get; }// Базовые ресурсы
        internal ListBaseResources BaseResourcesCollected { get; } = new ListBaseResources();// Собрано базовых ресурсов
        internal BaseResource ResourceGold { get; }// Ресурс - золото

        internal List<DescriptorCreature> VariantsBonusedTypeSimpleHero { get; }// Варианты типов простых героев для выбора постоянного бонуса
        internal List<DescriptorCreature> VariantsBonusedTypeTempleHero { get; }// Варианты храмовников для выбора постоянного бонуса
        internal DescriptorCreature SelectedBonusSimpleHero { get; set; }
        internal DescriptorCreature SelectedBonusTempleHero { get; set; }

        //
        internal List<VCNoticeForPlayer> ListNoticesForPlayer { get; } = new List<VCNoticeForPlayer>();// Список событий в графстве

        // Локации
        internal Location[,] Locations { get; }
        internal Location LocationCapital { get; }
        internal Location CurrentLocation { get; set; }// Текущая выбранная локация

        //
        internal int PercentCorruption { get; set; }//
        internal int ChangeCorruption { get; set; }
        internal int MinPercentCorruption { get; }
        internal int MaxPercentCorruption { get; }

        // Информация о поражениях и вылете из лобби
        internal List<LoseInfo> LoseInfo { get; } = new List<LoseInfo>();
        internal int CurrentLoses { get; private set; }// Текущее количество поражений
        internal int MaxLoses { get; private set; }// Максимальное количество поражений
        internal int DayOfEndGame { get; private set; }// День вылета из лобби
        internal int SkippedBattles { get; set; }// Сколько битв было пропущено (про причине нечетного количества игроков)
        internal bool SkipBattle { get; set; }// Битва на этому ходу будет пропущена

        internal int Builders { get; private set; }
        internal int FreeBuilders { get; private set; }
        internal List<Entity> QueueBuilding { get; } = new List<Entity>();// Очередь строительства

        internal List<DescriptorPersistentBonus>[] VariantPersistentBonus { get; }
        internal List<DescriptorPersistentBonus> PersistentBonuses { get; } = new List<DescriptorPersistentBonus>();
        internal List<StartBonus> VariantsStartBonuses { get; }// Варианты стартовых бонусов

        internal int ExtraLevelUp { get; private set; }
        internal int ExtraResearch { get; private set; }

        internal int QuantityHeroes { get; private set; }

        internal Item[] Warehouse = new Item[FormMain.Config.WarehouseMaxCells];// Предметы на складе игрока

        internal List<ConstructionSpell> ConstructionSpells { get; } = new List<ConstructionSpell>();// Все заклинания игрока

        // Перки от сооружений
        internal List<(Construction, DescriptorPerk)> listPerksFromConstruction = new List<(Construction, DescriptorPerk)>();

        // Логова
        internal List<Construction> ListFlags { get; } = new List<Construction>();
        internal Dictionary<PriorityExecution, int> QuantityFlags { get; } = new Dictionary<PriorityExecution, int>();
        internal int LairsScouted { get; private set; }
        internal int LairsShowed { get; private set; }

        //
        internal Construction Graveyard { get; }// Кладбище игрока

        // Статистика
        internal Dictionary<DescriptorConstruction, int> destroyedLair = new Dictionary<DescriptorConstruction, int>();

        // Визуальные контролы
        private Player opponent;// Убрать это
        internal Player Opponent { get { return opponent; } set { if (value != this) { if (opponent != value) { opponent = value; UpdateOpponent(); } } else new Exception("Нельзя указать оппонентов самого себя."); } }
        internal Construction FlagAttackToOpponent { get; private set; }

        // Читинг
        internal bool CheatingIgnoreRequirements { get; set; }
        internal bool CheatingIgnoreBaseResources { get; set; }
        internal bool CheatingIgnoreBuilders { get; set; }
        internal bool CheatingInstantlyBuilding { get; set; }
        internal bool CheatingInstantlyResearch { get; set; }
        internal bool CheatingInstantlyHire { get; set; }

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
                FlagAttackToOpponent = opponent.GetPlayerConstruction(FormMain.Descriptors.FindConstruction(FormMain.Config.IDConstructionCastle));
                FlagAttackToOpponent.AttackToCastle();

                Debug.Assert(ListFlags.IndexOf(FlagAttackToOpponent) == -1);
                ListFlags.Insert(0, FlagAttackToOpponent);
            }

            Lobby.Layer.LairsWithFlagChanged();
        }

        internal Construction GetPlayerConstruction(DescriptorConstruction b)
        {
            Debug.Assert(b != null);

            foreach (Construction pb in Constructions)
            {
                if (pb.TypeConstruction == b)
                    return pb;
            }

            throw new Exception("У игрока " + GetName() + " сооружение " + b.ID + " не найдено.");
        }

        internal void AddHero(Hero ph)
        {
            Debug.Assert(CombatHeroes.IndexOf(ph) == -1);
            Debug.Assert(AllHeroes.IndexOf(ph) == -1);

            AllHeroes.Add(ph);
            if (ph.TypeCreature.CategoryCreature == CategoryCreature.Hero)
                AddCombatHero(ph);

            UpdatePerksFromConstructionForHero(ph);

            SetTaskForHeroes();

            if (Descriptor.TypePlayer == TypePlayer.Human)
                Lobby.Layer.ListHeroesChanged();
        }

        internal void Constructed(Construction pb)
        {
            //Debug.Assert(pb.CheckRequirements());

            //SpendResource(pb.CostBuyOrUpgrade());
            if (pb.TypeConstruction.Levels[pb.Level + 1].GetCreating() != null)
                if (!CheatingIgnoreBuilders)
                    FreeBuilders -= pb.TypeConstruction.Levels[pb.Level + 1].GetCreating().Builders;
            AddGreatness(pb.TypeConstruction.Levels[pb.Level + 1].GreatnessByConstruction);

            Debug.Assert(FreeBuilders >= 0);
        }

        internal int Income()
        {
            int income = 0;

            foreach (Construction pb in Constructions)
            {
                income += pb.Income();
            }

            return income;
        }

        // Поиск слота для предмета
        internal int FindSlotWithItem(DescriptorItem item)
        {
            for (int i = 0; i < Warehouse.Length; i++)
            {
                if ((Warehouse[i] != null) && (Warehouse[i].Descriptor == item))
                    return i;
            }

            return -1;
        }

        private int FindSlotForItem(DescriptorItem item)
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

        internal void AddItem(Item pi)
        {
            Debug.Assert(pi.Quantity > 0);

            int numberCell = FindSlotForItem(pi.Descriptor);
            if (numberCell >= 0)
                AddItem(pi, numberCell);
        }

        internal void AddItem(Item pi, int numberCell)
        {
            if (Warehouse[numberCell] != null)
            {
                Debug.Assert(Warehouse[numberCell].Quantity > 0);

                if (Warehouse[numberCell].Descriptor == pi.Descriptor)
                {
                    Warehouse[numberCell].Quantity += pi.Quantity;
                    pi.Quantity = 0;
                }
            }
            else
            {
                Warehouse[numberCell] = new Item(this, pi.Descriptor, pi.Quantity);
                pi.Quantity = 0;
            }
        }

        internal void MoveItem(int fromSlot, int toSlot)
        {
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(fromSlot != toSlot);

            Item tmp = null;
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

        internal void SellItem(Item pi)
        {

        }

        internal void GiveItemToHero(int fromSlot, Hero ph, int quantity, int toSlot)
        {
            Debug.Assert(ph.Construction.Player == this);
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[fromSlot].Quantity >= quantity);

            ph.AcceptItem(Warehouse[fromSlot], quantity, toSlot);
            if (Warehouse[fromSlot].Quantity == 0)
                Warehouse[fromSlot] = null;
        }

        internal void GiveItemToHero(int fromSlot, Hero ph, int quantity)
        {
            Debug.Assert(ph.Construction.Player == this);
            Debug.Assert(Warehouse[fromSlot] != null);
            Debug.Assert(Warehouse[fromSlot].Quantity >= quantity);

            ph.AcceptItem(Warehouse[fromSlot], quantity);
            if (Warehouse[fromSlot].Quantity == 0)
                Warehouse[fromSlot] = null;
        }

        internal bool GetItemFromHero(Hero ph, int fromSlot)
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
        internal void GetItemFromHero(Hero ph, int fromSlot, int toSlot)
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
        internal Item TakeItemFromWarehouse(int fromCell, int quantity)
        {
            Debug.Assert(quantity > 0);
            Debug.Assert(Warehouse[fromCell] != null);
            Debug.Assert(Warehouse[fromCell].Quantity > 0);
            Debug.Assert(Warehouse[fromCell].Quantity >= quantity);

            Item pi;

            // Если забирают всё, то возвращаем ссылку на этот предмет и убираем его у себя, иначе делим предмет
            if (Warehouse[fromCell].Quantity == quantity)
            {
                pi = Warehouse[fromCell];
                Warehouse[fromCell] = null;
            }
            else
            {
                pi = new Item(this, Warehouse[fromCell].Descriptor, quantity);
                Warehouse[fromCell].Quantity -= quantity;
            }

            return pi;
        }

        internal bool CheckRequiredResources(ListBaseResources reqResources)
        {
            if (CheatingIgnoreBaseResources)
                return true;

            return BaseResources.ResourcesEnough(reqResources);
        }

        internal bool CheckRequireBuilders(int needBuilders)
        {
            if (CheatingIgnoreBuilders)
                return true;

            return FreeBuilders >= needBuilders;
        }

        internal bool CheckRequirements(List<DescriptorRequirement> list)
        {
            if (CheatingIgnoreRequirements)
                return true;

            foreach (DescriptorRequirement r in list)
            {
                if (!r.CheckRequirement(this))
                    return false;
            }

            return true;
        }

        internal void TextRequirements(List<DescriptorRequirement> listReq, List<TextRequirement> listTextReq)
        {
            Construction pb;

            foreach (DescriptorRequirement r in listReq)
            {
                listTextReq.Add(r.GetTextRequirement(this));
            }
        }

        internal override void PreparingForBattle()
        {
            base.PreparingForBattle();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep1Name(GetName());
            panelHint.AddStep4Level($"{PositionInLobby} место", GetImageIndex());
            panelHint.AddStep5Description("Уровень Замка: " + LevelCastle.ToString() + Environment.NewLine
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

            List<Hero> freeHeroes = new List<Hero>();// Список свободных героев

            // Сначала сбрасываем всем состояние
            foreach (Hero ph in CombatHeroes)
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
                    Hero ph = CombatHeroes[i] as Hero;
                    freeHeroes.Remove(ph);
                    FlagAttackToOpponent.AddAttackingHero(ph);
                    ph.SetState(NameStateCreature.BattleWithPlayer);
                }
            }

            if (freeHeroes.Count == 0)
                return;

            if (CountActiveFlags() > 0)
            {
                foreach (Construction pl in ListFlags.Where(pl => (pl != null) && (pl.TypeFlag == TypeFlag.Scout)))
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

                        foreach (Construction pl in ListFlags.Where(pl => (pl != null) && ((pl.TypeFlag == TypeFlag.Attack) || (pl.TypeFlag == TypeFlag.Defense))))
                            if (pl != null)
                            {
                                heroesToFlag = Math.Min(freeHeroes.Count, heroesPerFlag);

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
            foreach (Construction pl in ListFlags)
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
            foreach (Construction pl in ListFlags)
            {
                if (pl == null)
                    QuantityFlags[PriorityExecution.None]++;
                else
                    QuantityFlags[pl.PriorityFlag]++;
            }
        }

        internal void AddFlag(Construction lair)
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

        internal void UpPriorityFlag(Construction lair)
        {
            int idx = ListFlags.IndexOf(lair);
            Debug.Assert(idx != -1);
            QuantityFlags[lair.PriorityFlag - 1]--;
            QuantityFlags[lair.PriorityFlag]++;

            CheckFlags();
        }


        internal void RemoveFlag(Construction lair)
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

        internal void RemoveLair(Construction l)
        {
            Debug.Assert(l != null);
            Debug.Assert(l.Location.Lairs[l.Y, l.X] != null);
            Debug.Assert(l.Location.Lairs[l.Y, l.X] == l);

            l.Location.Lairs[l.Y, l.X] = null;

            if (Lobby.Layer.PlayerObjectIsSelected(l))
                Lobby.Layer.SelectPlayerObject(null);
        }

        internal void ApplyReward(Construction l)
        {
            if (l.TypeConstruction.Reward != null)
            {
                ReceivedResource(l.TypeConstruction.Reward.Cost);
                AddGreatness(l.TypeConstruction.Reward.Greatness);
            }

            if (l.TypeConstruction.HiddenReward != null)
            {
                ReceivedResource(l.TypeConstruction.HiddenReward.Cost);
                AddGreatness(l.TypeConstruction.HiddenReward.Greatness);
            }
        }

        protected void ApplyStartBonus(StartBonus sb)
        {
            BaseResources.AddResources(sb.BaseResources);
            foreach (BaseResource br in sb.BaseResources)
            {
                if (br.Quantity > 0)
                    AddNoticeForPlayer(br, TypeNoticeForPlayer.ReceivedBaseResource);
            }

            Builders += sb.Builders;
            FreeBuilders += sb.Builders;
            CreateExternalConstructions(FormMain.Descriptors.FindConstruction(FormMain.Config.IDPeasantHouse), 1, LocationCapital, sb.PeasantHouse, TypeNoticeForPlayer.Build);
            DescriptorConstruction holyPlace = FormMain.Descriptors.FindConstruction(FormMain.Config.IDHolyPlace);
            CreateExternalConstructions(holyPlace, holyPlace.DefaultLevel, LocationCapital, sb.HolyPlace, TypeNoticeForPlayer.Explore);
            ScoutRandomLair(sb.Scouting, true);

            startBonusApplied = true;

            if (GetTypePlayer() == TypePlayer.Human)
                Lobby.Layer.ShowPlayerNotices();
        }

        internal void AddLose()
        {
            Debug.Assert(CurrentLoses < MaxLoses);
            Debug.Assert(LoseInfo[CurrentLoses] is null);
            Debug.Assert(!(opponent is null));
            Debug.Assert(IsLive);
            Debug.Assert(DayOfEndGame == 0);

            LoseInfo[CurrentLoses] = new LoseInfo(Lobby.Turn, opponent);
            CurrentLoses++;

            if (CurrentLoses == MaxLoses)
            {
                IsLive = false;
                DayOfEndGame = Lobby.Turn;
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

        internal void SpendResource(ListBaseResources res)
        {
            if (res != null)
            {
                if (!CheatingIgnoreBaseResources)
                {
                    for (int i = 0; i < BaseResources.Count; i++)
                    {
                        Debug.Assert(BaseResources[i].Quantity >= 0);
                        Debug.Assert(BaseResources[i].Quantity >= res[i].Quantity);
                        Debug.Assert(res[i].Quantity >= 0);

                        BaseResources[i].Quantity -= res[i].Quantity;
                    }
                }

                UpdateResourceInCastle();
            }
        }

        internal void SpendGold(int gold)
        {
            Assert(gold >= 0);

            if (gold > 0)
            {
                if (!CheatingIgnoreBaseResources)
                {                    
                    Debug.Assert(ResourceGold.Quantity >= 0);
                    Debug.Assert(ResourceGold.Quantity >= gold);
                    ResourceGold.Quantity -= gold;
                }

                UpdateResourceInCastle();
            }
        }

        internal void ReturnResource(ListBaseResources res)
        {
            if (!CheatingIgnoreBaseResources)
            {
                for (int i = 0; i < BaseResources.Count; i++)
                {
                    Debug.Assert(BaseResources[i].Quantity >= 0);
                    Debug.Assert(BaseResources[i].Quantity <= Lobby.TypeLobby.MaxBaseResources[i].Quantity);
                    Debug.Assert(res[i].Quantity >= 0);

                    BaseResources[i].Quantity += AllowAddBaseResource(res[i]);
                }
            }

            UpdateResourceInCastle();
        }

        internal void ReceivedResource(ListBaseResources res)
        {
            for (int i = 0; i < BaseResources.Count; i++)
            {
                Debug.Assert(BaseResources[i].Quantity >= 0);
                Debug.Assert(BaseResources[i].Quantity <= Lobby.TypeLobby.MaxBaseResources[i].Quantity);
                Debug.Assert(res[i].Quantity >= 0, $"Поступление ресурса {res[i].Descriptor.ID}: {res[i].Quantity}");

                int addValue = AllowAddBaseResource(res[i]);
                BaseResources[i].Quantity += addValue;
                BaseResourcesCollected[i].Quantity += addValue;
            }

            UpdateResourceInCastle();
        }

        private int AllowAddBaseResource(BaseResource r)
        {
            return BaseResources[r.Descriptor.Number].Quantity + r.Quantity <= Lobby.TypeLobby.MaxBaseResources[r.Descriptor.Number].Quantity
                ? r.Quantity : Lobby.TypeLobby.MaxBaseResources[r.Descriptor.Number].Quantity - BaseResources[r.Descriptor.Number].Quantity;
        }

        private void UpdateResourceInCastle()
        {
            if (Castle != null)
                Castle.Gold = BaseResources.ValueGold();
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

        internal void SelectRandomPersistentBonus()
        {
            // Применяем случайные постоянные бонусы
            for (int i = 0; i < VariantPersistentBonus.GetLength(0); i++)
                PersistentBonuses.Add(VariantPersistentBonus[i][Lobby.Rnd.Next(VariantPersistentBonus[i].Count)]);

            SelectedBonusSimpleHero = VariantsBonusedTypeSimpleHero[Lobby.Rnd.Next(VariantsBonusedTypeSimpleHero.Count)];
            SelectedBonusTempleHero = VariantsBonusedTypeTempleHero[Lobby.Rnd.Next(VariantsBonusedTypeTempleHero.Count)];
        }

        internal StartBonus GetRandomStartBonus()
        {
            return VariantsStartBonuses[Lobby.Rnd.Next(VariantsStartBonuses.Count)];
        }

        internal int PointGreatnessPerDay()
        {
            int g = 0;

            foreach (Construction pc in Constructions)
                if (pc.Level > 0)
                    g += pc.GreatnessPerDay();

            return g;
        }

        internal bool CanBuildTypeConstruction(DescriptorConstruction type)
        {
            // Сначала проверяем наличие ресурсов
            if (!BaseResources.ResourcesEnough(type.Levels[1].GetCreating().CostResources))
                return false;

            // Проверяем наличие очков строительства
            if (type.Levels[1].GetCreating().Builders > FreeBuilders)
                return false;

            // Проверяем требования к зданиям
            return CheckRequirements(type.Levels[1].GetCreating().Requirements);
        }

        internal List<TextRequirement> GetTextRequirementsBuildTypeConstruction(DescriptorConstruction type)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            TextRequirements(type.Levels[1].GetCreating().Requirements, list);

            return list;
        }

        internal void PrepareHintForBuildTypeConstruction(PanelHint panelHint, DescriptorConstruction type)
        {
            panelHint.AddStep2Header(type.Name);
            panelHint.AddStep4Level("Уровень 1");
            panelHint.AddStep5Description(type.Description);
            //panelHint.AddStep6Income(type.Levels[1].Income);
            panelHint.AddStep8Greatness(type.Levels[1].GreatnessByConstruction, type.Levels[1].GreatnessPerDay);
            panelHint.AddStep9PlusBuilders(type.Levels[1].BuildersPerDay);
            panelHint.AddStep10DaysBuilding(-1, type.Levels[1].GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirementsBuildTypeConstruction(type));
            panelHint.AddStep12Gold(BaseResources, type.Levels[1].GetCreating().CostResources);
            panelHint.AddStep13Builders(type.Levels[1].GetCreating().Builders, FreeBuilders >= type.Levels[1].GetCreating().Builders);
        }

        //
        internal override string GetName() => Descriptor.Name;
        internal override Player GetPlayer() => this;
        internal override TypePlayer GetTypePlayer() => Descriptor.TypePlayer;
        internal override int GetImageIndexAvatar() => Descriptor.ImageIndex;

        // Реализация интерфейса
        internal override int GetImageIndex()
        {
            return GetImageIndexAvatar();
        }
        internal override bool GetNormalImage()
        {
            return IsLive;
        }

        internal override string GetLevel()
        {
            return LevelGreatness.ToString();
        }

        internal override void ShowInfo(int selectPage = -1)
        {

        }

        internal override void HideInfo()
        {

        }

        internal Construction FindConstruction(string ID)
        {
            foreach (Construction c in Constructions)
            {
                if (c.TypeConstruction.ID == ID)
                    return c;
            }

            throw new Exception($"У игрока {GetName()} не найдено сооружение с ID = {ID}.");
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {

        }

        internal void AddPerkFromConstruction(Construction c, DescriptorPerk dp)
        {
            Debug.Assert(c.Player == this);
            Debug.Assert(c.Level > 0);

            foreach ((Construction, DescriptorPerk) p in listPerksFromConstruction)
            {
                Debug.Assert(p.Item2.ID != dp.ID);

            }

            listPerksFromConstruction.Add((c, dp));

            foreach (Hero h in CombatHeroes)
            {
                h.AddPerk(dp, c);
            }
        }

        internal void RemovePerkFromConstruction(Construction c, DescriptorPerk dp)
        {
            Debug.Assert(c.Player == this);
            Debug.Assert(c.Level > 0);

            if (!listPerksFromConstruction.Remove((c, dp)))
                throw new Exception($"Перк {dp.ID} сооружения {c.TypeConstruction.ID} не был в списке.");

            foreach (Hero h in CombatHeroes)
            {
                h.RemovePerk(dp);
            }
        }

        internal void UpdatePerksFromConstructionForHero(Hero h)
        {
            Debug.Assert(h.IsLive);

            foreach ((Construction, DescriptorPerk) p in listPerksFromConstruction)
            {
                h.AddPerk(p.Item2, p.Item1);
            }

            h.PerksChanged();
        }

        internal void RecalcPerksHeroes()
        {
            foreach (Hero h in CombatHeroes)
            {
                h.PerksChanged();
            }
        }

        internal void AddFreeBuilder()
        {
            FreeBuilders++;
        }

        internal void UseFreeBuilder(int builders)
        {
            Debug.Assert(FreeBuilders > 0);
            Debug.Assert(builders >= 0);

            FreeBuilders -= builders;

            Debug.Assert(FreeBuilders >= 0);
        }

        internal void UnuseFreeBuilders(int builders)
        {
            Debug.Assert(builders >= 0);

            FreeBuilders += builders;

            Debug.Assert(FreeBuilders >= 0);
        }

        internal void AddExtraLevelUp()
        {
            Debug.Assert(ExtraLevelUp >= 0);

            ExtraLevelUp++;
        }

        internal void UseExtraLevelUp()
        {
            Debug.Assert(ExtraLevelUp > 0);

            ExtraLevelUp--;
        }

        internal void AddExtraResearch()
        {
            Debug.Assert(ExtraResearch >= 0);

            ExtraResearch++;
        }

        internal void UseExtraResearch()
        {
            Debug.Assert(ExtraResearch > 0);

            ExtraResearch--;
        }

        internal void UnhideAll()
        {
            foreach (Location l in Locations)
            {
                foreach (Construction lc in l.Lairs)
                {
                    if (lc.Hidden)
                    {
                        lc.Unhide(false);
                    }
                }
            }
        }

        internal void AddNoticeForPlayer(Entity entity, TypeNoticeForPlayer typeNotice)
        {
            if (GetTypePlayer() == TypePlayer.Human)
            { 
                ListNoticesForPlayer.Add(new VCNoticeForPlayer(entity, typeNotice));
            }
        }

        internal void RemoveNoticeForPlayer(VCNoticeForPlayer e)
        {
            Debug.Assert(ListNoticesForPlayer.IndexOf(e) != -1);

            ListNoticesForPlayer.Remove(e);
        }

        internal void AddEntityToQueueBuilding(Entity e)
        {
            Debug.Assert(QueueBuilding.IndexOf(e) == -1);

            QueueBuilding.Add(e);
        }

        internal void RemoveEntityToQueueBuilding(Entity e)
        {
            Debug.Assert(QueueBuilding.IndexOf(e) != -1);

            QueueBuilding.Remove(e);
        }
    }

    internal sealed class UnitOfQueueForBuy
    {
        public UnitOfQueueForBuy(Hero hero, Construction construction, int priority)
        {
            Hero = hero;
            Construction = construction;
            Priority = priority;
        }

        internal Hero Hero { get; }
        internal Construction Construction { get; }
        internal int Priority { get; }
    }
}