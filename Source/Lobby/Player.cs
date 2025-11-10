using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Windows.Media.Animation;
using System.Xml.Linq;

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
        private readonly Construction Castle;

        private bool startBonusApplied = false;

        private bool cheatingIgnoreRequirements;
        private bool cheatingSpeedUpProgressBy10;
        private bool cheatingReduceCostBy10;
        private bool cheatingPointTraditionMore10Times;

        public Player(Lobby lobby, DescriptorPlayer player, int playerIndex) : base(player, lobby, null)
        {
            Descriptor = player;
            PlayerIndex = playerIndex;
            PositionInLobby = playerIndex + 1;

            Initialization = true;

            //
            Gold = lobby.TypeLobby.Gold;
            if (Descriptor.TypePlayer == TypePlayer.Computer)   
                Gold = 100_000;

            PointsForNextTradition = FormMain.Config.CostFirstTradition;

            //TypeTradition1 = Lobby.Settings.Players[playerIndex].TypeTradition1;
            //TypeTradition2 = Lobby.Settings.Players[playerIndex].TypeTradition2;
            //TypeTradition3 = Lobby.Settings.Players[playerIndex].TypeTradition3;

            //AddNoticeForPlayer(FormMain.Config.Gui48_Tradition, TypeTradition1.ImageIndex, "Первый тип традиций", TypeTradition1.Name, Color.Orange);
            //AddNoticeForPlayer(FormMain.Config.Gui48_Tradition, TypeTradition2.ImageIndex, "Второй тип традиций", TypeTradition2.Name, Color.Orange);
            //AddNoticeForPlayer(FormMain.Config.Gui48_Tradition, TypeTradition3.ImageIndex, "Третий тип традиций", TypeTradition3.Name, Color.Orange);

            // Настраиваем игрока согласно настройкам лобби
            CurrentLoses = 0;
            MaxLoses = lobby.TypeLobby.MaxLoses;
            for (int i = 0; i < MaxLoses; i++)
                LoseInfo.Add(null);

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

            // Инициализация сооружений города
            foreach (ConfigConstruction cc in lobby.TypeLobby.ConfigCityConstructions)
            {
                Construction c = new Construction(this, cc.Descriptor);
                c.X = cc.Coord.X;
                c.Y = cc.Coord.Y;

                if (cc.Level > 0)
                    c.LevelUp(cc.Level);
            }

            foreach (DescriptorConstruction tck in FormMain.Descriptors.Constructions)
            {
            //    if (tck.IsInternalConstruction)
            //        new Construction(this, tck);
            }

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

            //
            Castle = GetPlayerConstruction(FormMain.Descriptors.FindConstruction(FormMain.Config.IDConstructionCastle));
            Castle.Gold = Gold;

            LevelGreatness = 1;
            PointGreatnessForNextLevel = 100;

           
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

        internal void ReceiveResources()
        {
            /*
            // Получаем ресурсы с добычи
            ListBaseResources lbs = new ListBaseResources();
            List<Construction> lc = new List<Construction>();
            lc.AddRange(Constructions);
            foreach (Construction pc in lc)// Коллекция меняется при замене объекта
            {
                // Прибавляем ресурсы
                if ((pc.Level > 0) && (pc.MiningBaseResources || pc.ProvideBaseResources))
                {
                    foreach (ConstructionBaseResource cbs in pc.IncomeBaseResources)
                        lbs[cbs.DescriptorBaseResource.Number] += cbs.Quantity;
                }
            }

            ReceivedResource(lbs);*/
        }

        internal virtual void PrepareTurn(bool beginOfDay)
        {
            //
            List<Creature> listForDelete = new List<Creature>();

            foreach (Creature h in CombatHeroes)
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
            foreach (Creature h in listForDelete)
            {
                Debug.Assert(AllHeroes.IndexOf(h) != -1);
                Debug.Assert(CombatHeroes.IndexOf(h) != -1);

                AllHeroes.Remove(h);
                CombatHeroes.Remove(h);
            }

            SetTaskForHeroes();
        }

        internal override bool ProperName() => true;
        internal override string GetTypeEntity() => Descriptor.GetTypeEntity();

        // Ход любого игрока. Сначала делаем все расчеты тика, а потом включается ИИ или сам игрок
        internal virtual void DoTick(bool startNewDay)
        {
            // Двигаем прогресс в очереди действий
            // Делаем это из игрока, так как нам нужна строгая последовательность действий (одно может зависеть от другого)
            // Так как при выполнении действия они удаляются из очереди, обходим через временный список
            /*List<CellMenuConstruction> listActions = new List<CellMenuConstruction>();
            listActions.AddRange(queueExecuting);
            foreach (CellMenuConstruction cm in listActions)
            {
                cm.Construction.AssertNotDestroyed();
                cm.DoTick();
            }*/

            // Обработка традиции
            if ((NextTradition is null) && AcceptTraditionsAllowed)
            {
                if (ListVariantsTraditions.Count == 0)
                    GenerateVariantsTraditions();

                // Если нет выбранной традиции, извещаем об этом игрока
                if (NoticeForTradition is null)
                {
                    NoticeForTradition = new VCNoticeSelectTradition(this);
                    AddNoticeForPlayer(NoticeForTradition);
                }
            }

            // Делаем расчет, сколько очков традиций должно прибавиться за ход
            if (AcceptTraditionsAllowed)
            {
                PointsTraditionPerTurn = 1;// (int)Math.Truncate(CityParameters[FormMain.Descriptors.IndexCityParameterCitizens] / 100.0) * 10;
                if (cheatingPointTraditionMore10Times)
                    PointsTraditionPerTurn *= 10;
                Assert(PointsTraditionPerTurn > 0);

                // Прибавляем очки традиций
                Assert(PointsTraditionPerTurn > 0);
                double pointsTraditionPerTick = (double)PointsTraditionPerTurn / FormMain.Config.TicksInTurn;
                PointsTraditions += pointsTraditionPerTick;

                // Если очков хватает на следующую традицию и она выбрана, принимаем её
                if ((PointsTraditions >= PointsForNextTradition) && (NextTradition != null))
                {
                    PointsTraditions = Math.Truncate(PointsTraditions - PointsForNextTradition);
                    PointsForNextTradition = (int)Math.Truncate(PointsForNextTradition * FormMain.Config.CoefForNextTradition);

                    AcceptTradition();
                }

                if (PointsForNextTradition > (int)PointsTraditions)
                    RestTimeForNextTradition = Program.formMain.CalcRestTime(PointsForNextTradition * 1000 - (int)PointsTraditions * 1000, (int)(pointsTraditionPerTick * 1000));
                else
                    RestTimeForNextTradition = 0;
            }
            else
            {
                PointsTraditionPerTurn = 0;
                RestTimeForNextTradition = -1;
            }

            // Делаем тик у сооружений            
            foreach (Construction c in Constructions)
            {
                c.DoTick(startNewDay);

                if (c.IncomeResources > 0)
                {
                    ReceivedResource(c.IncomeResources);
                    c.IncomeResources = 0;
                }
            }

            // Делаем тик у извещений
            for (int i = 0; i < ListNoticesForPlayer.Count; )
            {
                if (ListNoticesForPlayer[i].AutoHide)
                {
                    if (ListNoticesForPlayer[i].CounterForBeginHide > 0)
                    {
                        ListNoticesForPlayer[i].CounterForBeginHide--;
                        i++;
                    }
                    else if (ListNoticesForPlayer[i].CounterForRemove > 0)
                    {
                        ListNoticesForPlayer[i].CounterForRemove--;
                        i++;
                    }
                    else
                    {
                        ListNoticesForPlayer[i].CloseSelf();
                    }
                }
                else
                    i++;
            }
            
            List<Construction> lc = new List<Construction>();
            lc.AddRange(Constructions);
            foreach (Construction pc in lc)// Коллекция меняется при замене объекта
            {
                pc.PrepareNewDay();
            }

            //RebuildQueueBuilding();// Перестраиваем очередь строительства согласно текущим параметрам

            // Обновляем данные для следующего тика, 
            foreach (Construction c in Constructions)
            {
                c.UpdateAfterTick();
            }
        }

        // Определяет варианты выбора следующей традиции
        private void GenerateVariantsTraditions()
        {
            Assert(AcceptTraditionsAllowed);

            ListVariantsTraditions.Clear();
            List<(DescriptorTradition, int)> listAllVariants = new List<(DescriptorTradition, int)>();
            int level;

            // Сначала заполняем список всеми возможными вариантами
            foreach (DescriptorTradition td in FormMain.Descriptors.Traditions)
            {
                level = 1;

                // Если уровень традиции достигнут максимального, пропускаем её
                if (ListTraditions.ContainsKey(td))
                    if (ListTraditions[td] < FormMain.Config.MaxLevelTradition)
                        level = ListTraditions[td] + 1;
                    else
                        continue;

                listAllVariants.Add((td, level));
            }

            // Если нельзя принять больше традиций, ставим флаг об окончании доступных традиций и выходим
            if (listAllVariants.Count == 0)
            {
                AcceptTraditionsAllowed = false;
                return;
            }

            // Всегда добавляем варианты для трех основных традиций города
            ApplyTypeTradition(TypeTradition1);
            ApplyTypeTradition(TypeTradition2);
            ApplyTypeTradition(TypeTradition3);

            // Добиваем варианты до 6 позиций
            int i;
            while ((ListVariantsTraditions.Count < 6) && (listAllVariants.Count > 0))
            {
                i = Lobby.Rnd.Next(listAllVariants.Count);
                ListVariantsTraditions.Add(listAllVariants[i].Item1, listAllVariants[i].Item2);
                listAllVariants.RemoveAt(i);
            }

            void ApplyTypeTradition(DescriptorTypeTradition tt)
            {
                List<(DescriptorTradition, int)> traditions = new List<(DescriptorTradition, int)>();
                foreach ((DescriptorTradition, int) t in listAllVariants)
                {
                    if (t.Item1.TypeTradition == tt)
                        traditions.Add((t.Item1, t.Item2));
                }

                if (traditions.Count == 0)
                    return;

                int j = Lobby.Rnd.Next(traditions.Count);
                ListVariantsTraditions.Add(traditions[j].Item1, traditions[j].Item2);
                listAllVariants.Remove((traditions[j].Item1, traditions[j].Item2));
            }
        }

        internal virtual void CalcDay()
        {
            // Собираем очередь из героев на посещение сооружений
            foreach (Construction pc in Constructions)
            {
            }

            // Выполняем покупки
        }

        private void CreateExternalConstructions(DescriptorConstruction typeConstruction, int level, int quantity, TypeNoticeForPlayer typeNotice)
        {
            Debug.Assert(typeConstruction.Category == CategoryConstruction.Place);
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
            /*foreach (Location l in Locations)
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
            }*/

            // Расчет флагов на логова
            /*List<Construction> tempListLair = ListFlags.ToList();// Работаем с копией списка, так как текущий будет меняться по мере обработки флагов
            int maxSteps = FormMain.Config.MaxDurationBattleWithMonster * FormMain.Config.StepsInSecond;

            foreach (Construction pl in tempListLair)
            {
                Battle b = null;
                WindowBattle formBattle;

                if ((pl != null) && (pl.ComponentObjectOfMap.ListHeroesForFlag.Count > 0) && (pl.ComponentObjectOfMap.TypeFlag != TypeFlag.Battle))
                {
                    Debug.Assert((pl.ComponentObjectOfMap.TypeFlag == TypeFlag.Scout) || (pl.ComponentObjectOfMap.TypeFlag == TypeFlag.Attack) || (pl.ComponentObjectOfMap.TypeFlag == TypeFlag.Defense));

                    typeFlag = pl.ComponentObjectOfMap.TypeFlag;

                    if (pl.ComponentObjectOfMap.TypeFlag == TypeFlag.Scout)
                    {
                        //pl.DoScout();
                    }
                    else if (pl.ComponentObjectOfMap.TypeFlag == TypeFlag.Attack)
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
                            if (!pl.Descriptor.IsOurConstruction)
                                LairCaptured(pl.Descriptor);
                        }
                        else
                        {

                        }
                    }
                    else if (pl.ComponentObjectOfMap.TypeFlag == TypeFlag.Defense)
                    {
                    }
                    else
                        throw new Exception("Неизвестный флаг: " + pl.ComponentObjectOfMap.TypeFlag.ToString());

                    if (this is PlayerHuman h)
                        h.AddEvent(new VCEventExecuteFlag(typeFlag, pl.Descriptor, pl.Destroyed ? null : pl, (b is null) || (b.Winner == this), b));
                }
            
            }*/
        }

        internal int TypeConstructionBuilded(DescriptorTypeConstruction typeConstruction)
        {
            int builded = 0;
            foreach (Construction c in Constructions)
            {
                if ((c.Descriptor.TypeConstruction == typeConstruction) && (c.Level > 0))
                    builded++;
            }

            return builded;
        }

        internal void CalcResultTurn()
        {
            if (IsLive == true)
            {
                ValidateHeroes();

                QuantityHeroes = CombatHeroes.Count();
            }
        }

        private void ValidateHeroes()
        {
            foreach (Construction pb in Constructions)
                pb.ValidateHeroes();
        }

        internal new DescriptorPlayer Descriptor { get; }
        internal int PlayerIndex { get; }
        internal int PositionInLobby { get; set; }
        internal bool Initialization { get; }
        internal int LevelGreatness { get; }// Уровень величия
        internal int PointGreatness { get; private set; }// Очков величия
        internal int PointGreatnessForNextLevel { get; }// Очков величия до следующего уровня
        internal List<Construction> Constructions { get; } = new List<Construction>();
        internal int LevelCastle => Castle.Level;

        internal List<Creature> AllHeroes { get; } = new List<Creature>();
        internal List<Creature> FreeHeroes { get; } = new List<Creature>();
        internal Dictionary<string, BigEntity> Entities { get; } = new Dictionary<string, BigEntity>();// Все сущности игрока

        internal int Gold { get; set; }// Текущее количество золота
        internal int GreatnessCollected { get; private set; }// Собрано величия за игру

        internal List<DescriptorCreature> VariantsBonusedTypeSimpleHero { get; }// Варианты типов простых героев для выбора постоянного бонуса
        internal List<DescriptorCreature> VariantsBonusedTypeTempleHero { get; }// Варианты храмовников для выбора постоянного бонуса
        internal DescriptorCreature SelectedBonusSimpleHero { get; set; }
        internal DescriptorCreature SelectedBonusTempleHero { get; set; }

        //
        internal List<VCCustomNotice> ListNoticesForPlayer { get; } = new List<VCCustomNotice>();// Список событий во владении

        // Информация о поражениях и вылете из лобби
        internal List<LoseInfo> LoseInfo { get; } = new List<LoseInfo>();
        internal int CurrentLoses { get; private set; }// Текущее количество поражений
        internal int MaxLoses { get; private set; }// Максимальное количество поражений
        internal int DayOfEndGame { get; private set; }// День вылета из лобби
        internal int SkippedBattles { get; set; }// Сколько битв было пропущено (про причине нечетного количества игроков)
        internal bool SkipBattle { get; set; }// Битва на этому ходу будет пропущена

        internal List<DescriptorPersistentBonus>[] VariantPersistentBonus { get; }
        internal List<DescriptorPersistentBonus> PersistentBonuses { get; } = new List<DescriptorPersistentBonus>();
        internal List<StartBonus> VariantsStartBonuses { get; }// Варианты стартовых бонусов

        internal int QuantityHeroes { get; private set; }

        // Перки от сооружений
        internal List<(Construction, DescriptorPerk)> listPerksFromConstruction = new List<(Construction, DescriptorPerk)>();

        // Традиции
        internal Dictionary<DescriptorTradition, int> ListTraditions { get; } = new Dictionary<DescriptorTradition, int>();// Принятые традиции
        internal double PointsTraditions { get; private set; }// Очки традиции
        internal int PointsTraditionPerTurn { get; private set; }// Сколько очков традиций должно прибавиться за сутки (ход)
        internal int PointsForNextTradition { get; private set; }// Очков до принятия следующей традиции
        internal DescriptorTradition NextTradition { get; private set; }// Принимаемая традиция
        internal int NextTraditionLevel { get; private set; }// Уровень принимаемой традиции
        internal Dictionary<DescriptorTradition, int> ListVariantsTraditions { get; } = new Dictionary<DescriptorTradition, int>();// Варианты традиций для выбора
        internal VCNoticeSelectTradition NoticeForTradition { get; set; }// Извещение о необходимости выбора традиции
        internal bool AcceptTraditionsAllowed { get; private set; } = true;// Можно еще принять традиции
        internal int RestTimeForNextTradition { get; private set; }// Сколько секунд осталось до принятия традиции

        internal DescriptorTypeTradition TypeTradition1 { get; private set; }// Главная традиция
        internal DescriptorTypeTradition TypeTradition2 { get; private set; }// Второстепенная традиция
        internal DescriptorTypeTradition TypeTradition3 { get; private set; }// Третьестепенная традиция


        // Визуальные контролы
        internal Player Opponent { get; set; }

        // Читинг
        internal bool CheatingIgnoreRequirements
        {
            get => cheatingIgnoreRequirements;
            set
            {
                if (cheatingIgnoreRequirements != value)
                {
                    cheatingIgnoreRequirements = value;
                    AddNoticeForPlayer(-1, cheatingIgnoreRequirements ? FormMain.Config.Gui48_Cheating : FormMain.Config.Gui48_NoCheating,
                        cheatingIgnoreRequirements ? "Применен читинг:" : "Отменен читинг:", "Игнорировать требования", Color.Orange);
                }
            }
        }
        internal bool CheatingSpeedUpProgressBy10
        {
            get => cheatingSpeedUpProgressBy10;
            set
            {
                if (cheatingSpeedUpProgressBy10 != value)
                {
                    cheatingSpeedUpProgressBy10 = value;
                    AddNoticeForPlayer(-1, cheatingSpeedUpProgressBy10 ? FormMain.Config.Gui48_Cheating : FormMain.Config.Gui48_NoCheating,
                        cheatingSpeedUpProgressBy10 ? "Применен читинг:" : "Отменен читинг:", "Ускорение прогресса в 10 раз", Color.Orange);
                }
            }
        }
        internal bool CheatingReduceCostBy10
        {
            get => cheatingReduceCostBy10;
            set
            {
                if (cheatingReduceCostBy10 != value)
                {
                    cheatingReduceCostBy10 = value;
                    AddNoticeForPlayer(-1, cheatingReduceCostBy10 ? FormMain.Config.Gui48_Cheating : FormMain.Config.Gui48_NoCheating,
                        cheatingReduceCostBy10 ? "Применен читинг:" : "Отменен читинг:", "Стоимость меньше в 10 раз", Color.Orange);
                }
            }
        }

        internal bool CheatingPointsTraditionMore10Times
        {
            get => cheatingPointTraditionMore10Times;
            set
            {
                if (cheatingPointTraditionMore10Times != value)
                {
                    cheatingPointTraditionMore10Times = value;
                    AddNoticeForPlayer(-1, cheatingReduceCostBy10 ? FormMain.Config.Gui48_Cheating : FormMain.Config.Gui48_NoCheating,
                        cheatingPointTraditionMore10Times ? "Применен читинг:" : "Отменен читинг:", "Прирост очков традиций больше в 10 раз", Color.Orange);
                }
            }
        }

        internal Construction GetPlayerConstruction(DescriptorConstruction b, bool mustBeExists = true)
        {
            Debug.Assert(b != null);

            foreach (Construction pb in Constructions)
            {
                if (pb.Descriptor == b)
                    return pb;
            }

            if (!mustBeExists)
                return null;

            throw new Exception("У игрока " + GetName() + " сооружение " + b.ID + " не найдено.");
        }

        internal void AddHero(Creature ph)
        {
            Debug.Assert(CombatHeroes.IndexOf(ph) == -1);
            Debug.Assert(AllHeroes.IndexOf(ph) == -1);
            Debug.Assert(FreeHeroes.IndexOf(ph) == -1);

            AllHeroes.Add(ph);
            if (ph.TypeCreature.CategoryCreature == CategoryCreature.Hero)
            {
                FreeHeroes.Add(ph);
                AddCombatHero(ph);
            }

            UpdatePerksFromConstructionForHero(ph);

            SetTaskForHeroes();

            if (Descriptor.TypePlayer == TypePlayer.Human)
                Lobby.Layer.ListHeroesChanged();
        }

        internal bool CheckRequirements(List<DescriptorRequirement> list)
        {
            foreach (DescriptorRequirement r in list)
            {
                if (!r.CheckRequirement(this))
                    return false;
            }

            return true;
        }

        internal void TextRequirements(List<DescriptorRequirement> listReq, ListTextRequirement listTextReq, Construction inConstruction = null)
        {
            foreach (DescriptorRequirement r in listReq)
            {
                listTextReq.Add(r.GetTextRequirement(this, inConstruction));
            }
        }

        internal override void PreparingForBattle()
        {
            base.PreparingForBattle();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Entity(this);
            panelHint.AddStep4Level($"{PositionInLobby} место");
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

            return;
        }

        protected void ApplyStartBonus(StartBonus sb)
        {
            Gold += sb.Gold;
/*            for (int i = 0; i < sb.BaseResources.Count; i++)
            {
                if (sb.BaseResources[i] > 0)
                {
                    BaseResource bs = new BaseResource(FormMain.Descriptors.BaseResources[i]);
                    bs.Quantity = sb.BaseResources[i];
                    AddNoticeForPlayer(bs, TypeNoticeForPlayer.ReceivedBaseResource);
                }
            }*/

            //DescriptorConstruction holyPlace = FormMain.Descriptors.FindConstruction(FormMain.Config.IDHolyPlace);

            startBonusApplied = true;

            //if (GetTypePlayer() == TypePlayer.Human)
            //    Lobby.Layer.ShowPlayerNotices();
        }

        internal void AddLose()
        {
            Debug.Assert(CurrentLoses < MaxLoses);
            Debug.Assert(LoseInfo[CurrentLoses] is null);
            Debug.Assert(!(Opponent is null));
            Debug.Assert(IsLive);
            Debug.Assert(DayOfEndGame == 0);

            LoseInfo[CurrentLoses] = new LoseInfo(Lobby.Turn, Opponent);
            CurrentLoses++;

            if (CurrentLoses == MaxLoses)
            {
                IsLive = false;
                DayOfEndGame = Lobby.Turn;
            }
        }

        internal void SpendResource(int res)
        {
            if (res != 0)
            {
                Gold -= res;

                UpdateResourceInCastle();
            }
        }

        internal void SpendGold(int gold)
        {
            Assert(gold >= 0);

            if (gold > 0)
            {
                Debug.Assert(Gold >= 0);
                Debug.Assert(Gold >= gold);
                Gold -= gold;

                UpdateResourceInCastle();
            }
        }

        internal void ReturnGold(int gold)
        {
            Assert(gold >= 0);

            if (gold > 0)
            {
                Debug.Assert(Gold >= 0);
                Debug.Assert(Gold >= gold);
                Gold += gold;// Здесь нужен тест на превышение суммы лимита золота

                UpdateResourceInCastle();
            }
        }
        internal void ReturnResource(int res)
        {
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= Lobby.TypeLobby.MaxGold);

            Gold += res;

            UpdateResourceInCastle();
        }

        internal void ReceivedResource(int res)
        {
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= Lobby.TypeLobby.MaxGold);
            Gold += res;

            UpdateResourceInCastle();
        }

        private void UpdateResourceInCastle()
        {
            if (Castle != null)
                Castle.Gold = Gold;
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

        internal ListTextRequirement GetTextRequirementsBuildTypeConstruction(DescriptorConstruction type)
        {
            ListTextRequirement list = new ListTextRequirement();

            TextRequirements(type.Levels[1].ComponentCreating.Requirements, list);

            return list;
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
            base.HideInfo();

        }

        internal Construction FindConstruction(string ID)
        {
            foreach (Construction c in Constructions)
            {
                if (c.Descriptor.ID == ID)
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

            foreach (Creature h in CombatHeroes)
            {
                h.AddPerk(dp, c);
            }
        }

        internal void RemovePerkFromConstruction(Construction c, DescriptorPerk dp)
        {
            Debug.Assert(c.Player == this);
            Debug.Assert(c.Level > 0);

            if (!listPerksFromConstruction.Remove((c, dp)))
                throw new Exception($"Перк {dp.ID} сооружения {c.Descriptor.ID} не был в списке.");

            foreach (Creature h in CombatHeroes)
            {
                h.RemovePerk(dp);
            }
        }

        internal void UpdatePerksFromConstructionForHero(Creature h)
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
            foreach (Creature h in CombatHeroes)
            {
                h.PerksChanged();
            }
        }

        internal void AddNoticeForPlayer(VCCustomNotice notice)
        {
            if (GetTypePlayer() == TypePlayer.Human)
            {
                ListNoticesForPlayer.Add(notice);
            }
        }
        internal void AddNoticeForPlayer(Entity entity, TypeNoticeForPlayer typeNotice, int addParam = 0)
        {
            if (GetTypePlayer() == TypePlayer.Human)
            {
                ListNoticesForPlayer.Add(new VCNoticeForPlayer(entity, typeNotice, addParam));
                //Program.formMain.layerGame.ShowPlayerNotices();
            }
        }

        internal void AddNoticeForPlayer(int imageIndexOwner, int imageIndexEntity, string caption, string text, Color color)
        {
            if (GetTypePlayer() == TypePlayer.Human)
            {
                ListNoticesForPlayer.Add(new VCNoticeForPlayer(imageIndexOwner, imageIndexEntity, caption, text, color));
            }
        }

        internal void RemoveNoticeForPlayer(VCCustomNotice e)
        {
            Debug.Assert(ListNoticesForPlayer.IndexOf(e) != -1);

            ListNoticesForPlayer.Remove(e);
        }

        internal void AddConstruction(Construction c)
        {
            Assert(Constructions.IndexOf(c) == -1);

            Constructions.Add(c);
        }

        internal void AddEntity(BigEntity e)
        {
            Debug.Assert(e.IDEntity.Length > 0);
            Debug.Assert(e != null);

            Entities.Add(e.IDEntity, e);
        }

        internal BigEntity FindEntity(string id)
        {
            Entities.TryGetValue(id, out BigEntity v);
            return v;
        }

        internal BigEntity FindBigEntityInSelfAndLobby(string id)
        {
            Entities.TryGetValue(id, out BigEntity e);
            if (e is null)
                e = Lobby.FindEntity(id);

            EntityAssert(e != null, $"{id} не найден.");
            return e;
        }

        internal string ReplaceIDEntityToName(string text)
        {
            while (text.IndexOf("#") >= 0)
            {
                string begin = text.Substring(text.IndexOf("#") + 1);
                int idxEnd = begin.IndexOf("#");
                Assert(idxEnd > 0);
                string id = begin.Substring(0, idxEnd);
                Assert(id.Length > 0);
                BigEntity e = FindBigEntityInSelfAndLobby(id);
                text = text.Replace($"#{id}#", "{" + e.GetName() + "}");
            }

            return text;
        }

        internal void CalcDaysExecutingInActions()
        {
            foreach (Construction c in Constructions)
                c.CalcDaysExecutingInActions();
        }

        internal double CoefficientExecuting(TypeCreating typeCreating)
        {
            return 1;
        }

        internal void CompPurchase(int originCost, int curCost, TypeCreating typeCreating)
        {
            /*
            if (CheatingReduceCostBy10)
            {

                for (int i = 0; i < originCost.Count; i++)
                {
                    curCost[i] = originCost[i] / 10;
                }
            }
            else
                curCost.SetFromList(originCost);
            */
        }

        internal void SelectTradition(DescriptorTradition td, int level)
        {
            Assert(td != null);
            Assert(level > 0);

            NextTradition = td;
            NextTraditionLevel = level;
        }

        internal void AcceptTradition()
        {
            Assert(NextTradition != null);
            Assert(NextTraditionLevel != 0);

            if (ListTraditions.ContainsKey(NextTradition))
            {
                Assert(ListTraditions[NextTradition] < FormMain.Config.MaxLevelTradition);
                Assert(ListTraditions[NextTradition] == NextTraditionLevel - 1);

                ListTraditions[NextTradition] = NextTraditionLevel;
            }
            else
            {
                Assert(NextTraditionLevel == 1);

                ListTraditions.Add(NextTradition, NextTraditionLevel);
            }

            AddNoticeForPlayer(-1, FormMain.Config.Gui48_Tradition, "Принята традиция", NextTradition.Name + $" ({NextTraditionLevel} ур.)", Color.Orange);

            NextTradition = null;
            NextTraditionLevel = 0;
        }

        internal override string GetIDEntity(DescriptorEntity descriptor) => (descriptor as DescriptorPlayer).ID;
    }

    internal sealed class UnitOfQueueForBuy
    {
        public UnitOfQueueForBuy(Creature hero, Construction construction, int priority)
        {
            Hero = hero;
            Construction = construction;
            Priority = priority;
        }

        internal Creature Hero { get; }
        internal Construction Construction { get; }
        internal int Priority { get; }
    }
}