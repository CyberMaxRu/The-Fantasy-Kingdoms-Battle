using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum StateConstruction { None, Work, NotBuild, PreparedBuild, Build, InQueueBuild, NeedRepair, Repair, Destroyed };

    // Класс сооружения у игрока
    internal sealed class Construction : BattleParticipant
    {
        private int gold;

        // Конструктор для городских сооружений, которые создаются в начале миссии
        public Construction(Player p, DescriptorConstruction dc) : base(dc, p.Lobby, p)
        {
            Assert(dc.IsInternalConstruction);

            Descriptor = dc;
            PlayerIsOwner = true;
            PlayerCanOwn = true;
            IsEnemy = false;
            Location = null;
            ComponentObjectOfMap = new ComponentObjectOfMap(this, true);

            TuneByCreate();

            if (dc.DefaultLevel == 1)
                Build(false);

            // Восстановить
            //if (Construction.HasTreasury)
            //    Gold = Construction.GoldByConstruction;

            TuneConstructAfterCreate();
        }

        // Конструктор для сооружений, которые создаются для локации в начале миссии
        public Construction(Location l, TypeLobbyLairSettings ls) : base(ls.DescriptorConstruction, l.Lobby, l.Player)
        {
            Assert(!ls.DescriptorConstruction.IsInternalConstruction);

            Descriptor = ls.DescriptorConstruction;
            PlayerIsOwner = ls.Own;
            PlayerCanOwn = ls.CanOwn;
            IsEnemy = ls.IsEnemy;
            Location = l;
            ComponentObjectOfMap = new ComponentObjectOfMap(this, ls.Visible);
            IDPathToLocation = ls.PathToLocation;

            TuneByCreate();

            if (ls.Resources != null)
                InitialQuantityBaseResources = new ListBaseResources(ls.Resources);

            if (Descriptor.DefaultLevel == 1)
                Build(false);

            TuneConstructAfterCreate();
        }

        // Конструктор для сооружений, которые создаются в процессе игры
        public Construction(Player p, DescriptorConstruction dc, int level, int x, int y, Location location, bool visible, bool own, bool canOwn, bool isEnemy, TypeNoticeForPlayer typeNotice, ListBaseResources initQ = null) : base(dc, p.Lobby, p)
        {
            Assert(!dc.IsInternalConstruction);
            Assert((dc.Category == CategoryConstruction.Lair) || (dc.Category == CategoryConstruction.External) || (dc.Category == CategoryConstruction.Temple)
                || (dc.Category == CategoryConstruction.Place) || (dc.Category == CategoryConstruction.BasePlace) || (dc.Category == CategoryConstruction.ElementLandscape));
            Assert(level <= 1);

            Descriptor = dc;
            X = x;
            Y = y;
            Location = location;
            PlayerIsOwner = own;
            PlayerCanOwn = canOwn;
            IsEnemy = isEnemy;
            InitialQuantityBaseResources = initQ;
            ComponentObjectOfMap = new ComponentObjectOfMap(this, visible);

            TuneByCreate();

            if (level == 1)
                Build(false);

            if (typeNotice != TypeNoticeForPlayer.None)
                Player.AddNoticeForPlayer(this, typeNotice);

            TuneConstructAfterCreate();
        }

        internal new DescriptorConstruction Descriptor { get; }// Описатель сооружения
        internal bool PlayerIsOwner { get; private set; }// Игрок - владелец сооружения
        internal bool PlayerCanOwn { get; private set; }// Игрок может владеть сооружением
        internal bool IsEnemy { get; private set; }// Это сооружение враждебно
        internal int Level { get; private set; }
        internal StateConstruction State { get; private set; }// Состояние сооружения

        // Очередь действий
        internal List<CellMenuConstruction> QueueExecuting { get; } = new List<CellMenuConstruction>();// Очередь действий

        // Постройка/ремонт
        internal bool QueueBlocked { get; set; }// Очередь заблокирована
        internal int[] DayLevelConstructed { get; private set; }// На каком ходу был построено каждый уровень. -1: не построено, 0: до начала игры
        //internal bool InConstructing { get; set; }// Сооружение строится
        internal bool InRepair { get; set; }// Сооружение ремонтируется

        // Исследования
        internal int ResearchPoints { get; private set; }// Всего очков исследования на этот ход
        internal int RestResearchPoints { get; private set; }// Осталось очков исследования

        // Прочность
        internal int CurrentDurability { get; set; }// Текущая прочность сооружения
        internal int MaxDurability { get; private set; }// Максимальная прочность сооружения

        //
        internal int Gold { get => gold; set { Debug.Assert(Descriptor.HasTreasury); gold = value; } }// Казна гильдии
        internal List<Creature> Heroes { get; } = new List<Creature>();

        // Свойства для внешних сооружений
        internal Location Location { get; set; }// Локация, на которой находится сооружение
        internal int X { get; set; }// Позиция по X в слое
        internal int Y { get; set; }// Позиция по Y в слое
        internal int PercentScoutForFound { get; set; }// Процент разведки локации, чтобы найти сооружение
        internal Color SelectedColor { get; private set; }// Цвет рамки при выделении
        internal string IDPathToLocation { get; } = "";//
        internal Location NextLocation { get; private set; }// Дескриптор пути в другую локацию
        internal List<Creature> Monsters { get; } = new List<Creature>();// Монстры текущего уровня

        // Small-сущности в сооружении
        internal List<EntityForConstruction> ListEntities { get; } = new List<EntityForConstruction>();// Все сущности в сооружении
        internal ConstructionVisitSimple CurrentVisit { get; private set; }// Текущее активное посещение сооружения
        internal ConstructionEvent CurrentMassEvent { get; set; }// Текущее мероприятие
        internal ConstructionTournament CurrentTournament { get; set; }// Текущий турнир
        internal List<ConstructionVisit> Visits { get; } = new List<ConstructionVisit>();//
        internal List<ConstructionExtension> Extensions { get; } = new List<ConstructionExtension>();// Дополнения
        internal List<ConstructionImprovement> Improvements { get; } = new List<ConstructionImprovement>();// Улучшения
        internal ConstructionListBaseResources IncomeBaseResources { get; private set; }// Поступление базовых ресурсов
        internal List<ConstructionResource> Resources { get; } = new List<ConstructionResource>();// Ресурсы
        internal List<ConstructionService> Services { get; } = new List<ConstructionService>();// Услуги, доступные в строении
        internal List<ConstructionProduct> Goods { get; } = new List<ConstructionProduct>();// Товары, доступные в строении
        internal List<ConstructionAbility> Abilities { get; } = new List<ConstructionAbility>();// Умения, доступные в строении
        internal List<ConstructionSpell> Spells { get; } = new List<ConstructionSpell>();// Заклинания, доступные в строении

        // Действия
        internal CellMenuConstruction ActionMain { get; private set; }// Основное действие, которое отображается в панели сооружения
        private CellMenuConstructionLevelUp ActionBuildOrLevelUp { get; set; }// Действие для постройки/улучшения сооружения
        private CellMenuConstructionRepair ActionRepair { get; set; }// Действие для ремонта сооружения
        internal CellMenuConstructionBuild CellMenuBuildNewConstruction { get; set; }// Ячейка меню, которая строит новое сооружение на этом месте

        //
        internal List<Creature> Recruits { get; } = new List<Creature>();// Рекруты, готовые к найму

        internal int[] SatisfactionNeeds { get; private set; }// Удовлетворяемые потребности
        internal List<CellMenuConstructionSpell> MenuSpells { get; } = new List<CellMenuConstructionSpell>();
        // 
        internal ListBaseResources InitialQuantityBaseResources { get; }// Исходные значения базовых ресурсов
        internal bool MiningBaseResources { get; private set; }// Сооружение добывает ресурсы
        internal bool ProvideBaseResources { get; private set; }// Сооружение поставляет ресурсы

        internal override string GetIDEntity(DescriptorEntity descriptor)
        {
            if (((DescriptorConstruction)descriptor).IsInternalConstruction)
                return descriptor.ID;
            else
                return base.GetIDEntity(descriptor);
        }
        private void TuneCellMenuBuildOrUpgrade()
        {
            ActionBuildOrLevelUp = null;

            // Сооружение не построено, ищем действие для постройки
            List<CellMenuConstruction> listForDelete = new List<CellMenuConstruction>();

            foreach (CellMenuConstruction cm in Actions)
            {
                if (cm is CellMenuConstructionLevelUp cml)
                {
                    if (cml.Descriptor.Number <= Level)
                        listForDelete.Add(cm);
                    else if (cml.Descriptor.Number == Level + 1)
                    {
                        Debug.Assert(ActionBuildOrLevelUp is null);
                        ActionBuildOrLevelUp = cml;
                    }
                }
            }

            // Удаляем все ячейки, если они относятся к уже построенным уровням
            foreach (CellMenuConstruction cmd in listForDelete)
                Actions.Remove(cmd);

            if (ActionRepair != null)
                ActionMain = ActionRepair;
            else if (ActionBuildOrLevelUp != null)
                ActionMain = ActionBuildOrLevelUp;
            else
                ActionMain = null;
        }

        private void UpdateCurrentIncomeResources()
        {
            if (Level > 0)
            {
                MiningBaseResources = false;
                ProvideBaseResources = false;

                foreach (ConstructionBaseResource cbr in IncomeBaseResources)
                    cbr.Quantity = 0;

                if (InitialQuantityBaseResources != null)
                {
                    MiningBaseResources = Descriptor.Levels[Level].Mining != null;

                    for (int i = 0; i < InitialQuantityBaseResources.Count; i++)
                    {
                        if (InitialQuantityBaseResources[i] > 0)
                        {
                            int coefMining = Descriptor.Levels[Level].Mining != null ? Descriptor.Levels[Level].Mining[i] : 10;
                            int quantity = Convert.ToInt32(InitialQuantityBaseResources[i] * coefMining / 10);
                            Debug.Assert(quantity > 0);
                            IncomeBaseResources[i].Quantity = quantity;
                        }
                    }
                }
                else
                {
                    Debug.Assert(Descriptor.Levels[Level].Mining is null);

                    if (Descriptor.Levels[Level].IncomeResources != null)
                    {
                        ProvideBaseResources = true;
                        int q = 0;

                        for (int i = 0; i < Descriptor.Levels[Level].IncomeResources.Count; i++)
                        {
                            IncomeBaseResources[i].Quantity = Descriptor.Levels[Level].IncomeResources[i];
                            q += Descriptor.Levels[Level].IncomeResources[i];
                        }

                        Debug.Assert(q > 0);
                    }
                }
            }
        }

        internal void UpdateMaxDurability()
        {
            if (Level == 0)
            {
                if (ActionBuildOrLevelUp.ExecutingAction.InQueue)
                {
                    MaxDurability = Descriptor.Levels[1].Durability;
                    CurrentDurability = ActionBuildOrLevelUp.ExecutingAction.AppliedPoints;
                }
                else
                {
                    MaxDurability = 0;
                    CurrentDurability = 0;
                }
            }
            else
            {
                MaxDurability = Descriptor.Levels[Level].Durability;
                CurrentDurability = Descriptor.Levels[Level].Durability;
            }
        }

        internal void Build(bool needNotice)
        {
            if (!Lobby.InPrepareTurn && (Lobby.CurrentPlayer?.GetTypePlayer() == TypePlayer.Human))
                Program.formMain.PlayConstructionComplete();

            if ((Descriptor.Category != CategoryConstruction.Lair) && (Descriptor.Category != CategoryConstruction.ElementLandscape))
            {
                Debug.Assert(Level < Descriptor.MaxLevel);
                //Debug.Assert(CheckRequirements());
                //Debug.Assert(Player.BaseResources.ResourcesEnough(CostBuyOrUpgrade()));

                Player.AddGreatness(Descriptor.Levels[Level + 1].GreatnessByConstruction);

                if (Level > 0)
                {
                    // Убираем перки от сооружения
                    foreach (DescriptorPerk dp in Descriptor.Levels[Level].ListPerks)
                    {
                        Debug.Assert(dp != null, $"У сооружения {GetName()} уровня {Level} перк ссылается на null");
                        Player.RemovePerkFromConstruction(this, dp);
                    }

                    // Убираем товар посещения
                    if (Descriptor.Levels[Level].DescriptorVisit != null)
                    {
                        RemoveProduct(Descriptor.Levels[Level].DescriptorVisit);
                    }
                }
            }

            Level++;
            DayLevelConstructed[Level] = Player.Lobby.CounterDay;
            //InConstructing = false;

            if (Level == 1)
            {
                ValidateHeroes();
                //PrepareTurn();
            }

            //
            UpdateMaxDurability();
            CreateProducts();

            if ((Descriptor.Category != CategoryConstruction.Lair) && (Descriptor.Category != CategoryConstruction.ElementLandscape))
            {
                // Убираем операцию постройки из меню
                CellMenuConstruction cmBuild = null;
                foreach (CellMenuConstruction cm in Actions)
                {
                    if (cm is CellMenuConstructionLevelUp cml)
                        if (cml.Descriptor.Number == Level)
                        {
                            cmBuild = cml;
                            break;
                        }
                }

                if (cmBuild != null)
                {
                    Actions.Remove(cmBuild);
                    Lobby.Layer.UpdateMenu();
                }
            }

            // Обновляем список перков от сооружения
            AddPerksToPlayer();

            // Добавляем товар посещения
            AddVisit();

            // Инициализируем удовлетворяемые потребности
            SatisfactionNeeds = new int[FormMain.Descriptors.NeedsCreature.Count];
            if (Descriptor.Levels[Level].DescriptorVisit != null)
            {
                foreach ((DescriptorNeed, int) need in Descriptor.Levels[Level].DescriptorVisit.ListNeeds)
                {
                    SatisfactionNeeds[need.Item1.Index] = need.Item2;
                }
            }

            //
            Properties = new EntityProperties(this, Descriptor.Levels[Level].Properties);
            if (Descriptor.Levels[Level].Properties != null)
            {
                MainPerk = new Perk(this, Descriptor.Levels[Level].Properties);
                Perks.Add(MainPerk);
            }
            
            Initialize();

            if (needNotice)
                Player.AddNoticeForPlayer(this, Level == 1 ? TypeNoticeForPlayer.Build : TypeNoticeForPlayer.LevelUp);

            TuneCellMenuBuildOrUpgrade();
            UpdateCurrentIncomeResources();
            UpdateState();
        }

        private void AddVisit()
        {
            Debug.Assert(Descriptor.Levels[Level].DescriptorVisit != null);
            ConstructionVisitSimple cpVisit = new ConstructionVisitSimple(this, Descriptor.Levels[Level].DescriptorVisit);
            CurrentVisit = cpVisit;
            AddVisit(cpVisit);
        }

        private void CreateProducts()
        {
            foreach (DescriptorSmallEntity se in Descriptor.Levels[Level].Extensions)
            {
                if (se is DescriptorConstructionExtension dce)
                    AddExtension(new ConstructionExtension(this, dce));
                else if (se is DescriptorResource dr)
                    AddResource(new ConstructionResource(this, dr)); 
                //else if (se is DescriptorItem di)
                //    AddProduct(new ConstructionProduct(this, di));
                else
                    throw new Exception($"Неизвестный товар: {se.ID}");
            }
        }

        internal int GetInterest()
        {
            return CurrentVisit != null ? CurrentVisit.Interest : 0;
        }

        internal void AddPerksToPlayer()
        {
            foreach (DescriptorPerk dp in Descriptor.Levels[Level].ListPerks)
                Player.AddPerkFromConstruction(this, dp);

            Player.RecalcPerksHeroes();
        }

        internal void ValidateHeroes()
        {
            // Восстановить
            /*if ((Construction.TrainedHero != null) && (Construction.TrainedHero.Cost == 0))
            {
                if (Heroes.Count() < MaxHeroes())
                {
                    for (; Heroes.Count() < MaxHeroes();)
                    {
                        HireHero();
                    }
                }
            }*/
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {
            // Рисуем содержимое ячеек
            if (ComponentObjectOfMap.Visible)
            {
                Debug.Assert(Descriptor != null);

                ValidateResearches();
                FillResearches(menu);
            }
            else
            {
                foreach (ConstructionSpell cs in Player.ConstructionSpells)
                {
                    if ((cs.DescriptorSpell.TypeEntity == TypeEntity.Construction) && !cs.DescriptorSpell.Scouted)
                    {
                        CellMenuConstructionSpell cmcs = SearchCellMenuSpell(cs);

                        if (cmcs is null)
                        {
                            cmcs = new CellMenuConstructionSpell(this, cs);
                            MenuSpells.Add(cmcs);
                        }
                        //Assert(!menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Used);                        

                        menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Research = cmcs;
                        menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Used = true;
                    }
                }
            }

        }

        private CellMenuConstructionSpell SearchCellMenuSpell(ConstructionSpell spell)
        {
            foreach (CellMenuConstructionSpell cs in MenuSpells)
            {
                if (cs.Spell == spell)
                    return cs;
            }

            return null;
        }

        internal void ValidateResearches()
        {
            Debug.Assert(Actions != null);

            /*List<ConstructionCellMenu> forRemove = new List<ConstructionCellMenu>();

            foreach (ConstructionCellMenu mc in Researches)
            {
                if (mc.Research.TypeConstruction != null)
                    if (mc.ConstructionForBuild != null)
                    {
                        if (mc.ConstructionForBuild.Level> 0)
                        {
                            forRemove.Add(mc);
                        }
                    }
            }

            foreach (ConstructionCellMenu mc in forRemove)
            {
                Researches.Remove(mc);
            }*/
        }

        internal bool CanLevelUp()
        {
            return Level < Descriptor.MaxLevel;
        }

        internal ListBaseResources CostBuyOrUpgrade()
        {
            return CanLevelUp() == true ? Descriptor.Levels[Level + 1].GetCreating().CostResources : null;
        }

        internal bool CheckLevelRequirements(int level)
        {
            // При постройке храма из меню Святой земли, сюда прилетает 2 уровень
            if (Descriptor.MaxLevel < level)
                return false;

            // Сначала проверяем наличие золота
            if (!Player.CheckRequiredResources(Descriptor.Levels[level].GetCreating().CostResources))
                return false;

            // Проверяем наличие очков строительства
            //if (!Player.CheckRequireBuilders(Descriptor.Levels[level].GetCreating().ConstructionPoints(Player)))
            //    return false;

            // Проверяем, что нет события или турнира
            if (CurrentMassEvent != null)
                return false;
            if (CurrentTournament != null)
                return false;

            // Проверяем требования к зданиям
            return Player.CheckRequirements(Descriptor.Levels[level].GetCreating().Requirements);
        }

        internal List<TextRequirement> GetTextRequirements(int level)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            Player.TextRequirements(Descriptor.Levels[level].GetCreating().Requirements, list);

            if (CurrentMassEvent != null)
                list.Add(new TextRequirement(false, "В сооружении идет мероприятие"));

            if (CurrentTournament != null)
                list.Add(new TextRequirement(false, "В сооружении идет турнир"));

            return list;
        }

        internal int Income()
        {
            return (Level > 0) ? IncomeBaseResources.Gold : 0;
        }

        internal int IncomeForLevel(int level)
        {
            return Descriptor.Levels[level].IncomeResources != null ? Descriptor.Levels[level].IncomeResources.Gold : 0;
        }

        internal int DayBuildingForLevel(int level)
        {
            return Descriptor.Levels[level].GetCreating().DaysProcessing;
        }

        internal int GreatnesAddForLevel(int level)
        {
            return Descriptor.Levels[level].GreatnessByConstruction;
        }

        internal int GreatnesPerDayForLevel(int level)
        {
            return Descriptor.Levels[level].GreatnessPerDay;
        }

        internal int BuildersPerDayForLevel(int level)
        {
            return Descriptor.Levels[level].AddConstructionPoints;
        }

        internal int IncomeNextLevel()
        {
            return Level < Descriptor.MaxLevel ? IncomeForLevel(Level + 1) : 0;
        }

        internal int GreatnessPerDay()
        {
            return Level > 0 ? Descriptor.Levels[Level].GreatnessPerDay : 0;
        }

        internal int BuildersPerDay()
        {
            return Level > 0 ? Descriptor.Levels[Level].AddConstructionPoints : 0;
        }

        internal int GreatnessAddNextLevel()
        {
            return Level < Descriptor.MaxLevel ? GreatnesAddForLevel(Level + 1) : 0;
        }

        internal int GreatnessPerDayNextLevel()
        {
            return Level < Descriptor.MaxLevel ? GreatnesPerDayForLevel(Level + 1) : 0;
        }

        internal int BuildersPerDayNextLevel()
        {
            return Level < Descriptor.MaxLevel ? BuildersPerDayForLevel(Level + 1) : 0;
        }

        internal bool AllowHire()
        {
            if (Level == 0)
                return false;

            if ((Level > 0) && (Heroes.Count == MaxHeroes()))
                return false;

            if (MaxHeroesAtPlayer())
                return false;

            return true;

        }

        internal List<TextRequirement> GetTextRequirementsHire()
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Level == 0)
                list.Add(new TextRequirement(false, Descriptor.GetTextConstructionNotBuilded()));

            if ((Level > 0) && (Heroes.Count == MaxHeroes()))
                list.Add(new TextRequirement(false, Descriptor.GetTextConstructionIsFull()));

            if (MaxHeroesAtPlayer())
                list.Add(new TextRequirement(false, "Достигнуто максимальное количество героев в королевстве"));

            return list;
        }

        internal Creature HireHero(DescriptorCreature th, ListBaseResources cost)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= TypeConstruction.TrainedHero.Cost);

            Creature h = new Creature(this, th, Player, Player, 1);

            if (th.CategoryCreature != CategoryCreature.Citizen)
            {
                if (gold > 0)
                {
                    Player.SpendResource(cost);
                    if (Player.Descriptor.TypePlayer == TypePlayer.Human)
                        Program.formMain.SetNeedRedrawFrame();
                }
            }

            AddHero(h);

            return h;
        }

        internal void AddHero(Creature ph)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);

            Heroes.Add(ph);
            Player.AddHero(ph);
        }

        internal bool MaxHeroesAtPlayer()
        {
            return Player.CombatHeroes.Count == Player.Lobby.TypeLobby.MaxHeroes;
        }

        internal int MaxHeroes()
        {
            return Level > 0 ? Descriptor.Levels[Level].MaxInhabitant : 0;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            Debug.Assert(!Destroyed);

            if (Player == Player.Lobby.CurrentPlayer)
            {

                if (Descriptor.IsOurConstruction)
                {
                    panelHint.AddStep2Entity(this);
                    if (!((Level == 1) && (Descriptor.MaxLevel == 1)))
                        panelHint.AddStep4Level(Level > 0 ? "Уровень " + Level.ToString(): "");
                    panelHint.AddStep45State(GetDataState());
                    panelHint.AddStep5Description(Descriptor.Description + ((Level > 0) && (Heroes.Count > 0) ? Environment.NewLine + Environment.NewLine
                        + (Heroes.Count > 0 ? "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : "") : ""));
                    panelHint.AddStep6Income(Income());
                    panelHint.AddStep8Greatness(0, GreatnessPerDay());
                    panelHint.AddStep9PlusBuilders(BuildersPerDay());
                    panelHint.AddStep9Interest(GetInterest(), false);
                    panelHint.AddStep9ListNeeds(SatisfactionNeeds);
                }
                else
                {
                    if (!ComponentObjectOfMap.Visible)
                    {
                        panelHint.AddStep2Header("Неизвестное место");
                        panelHint.AddStep4Level("Место не разведано");
                        panelHint.AddStep5Description("Установите флаг разведки для отправки героев к месту");
                    }
                    else
                    {
                        panelHint.AddStep2Entity(this);
                        panelHint.AddStep5Description(Descriptor.Description);

                        if (Descriptor.Reward != null)
                        {
                            panelHint.AddStep7Reward(Descriptor.Reward.Cost.Gold);
                            panelHint.AddStep8Greatness(Descriptor.Reward.Greatness, 0);
                        }
                    }
                }

                panelHint.AddStep21BaseResources(IncomeBaseResources, MiningBaseResources || ProvideBaseResources);
            }
        }

        internal override void HideInfo()
        {
            base.HideInfo();

            //Debug.Assert(!Destroyed);// Assert не нужен - если сооружение уничтожено, его надо скрыть

            Lobby.Layer.panelConstructionInfo.Visible = false;
            Lobby.Layer.panelLairInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Debug.Assert(!Destroyed);

            if (Descriptor.IsOurConstruction)
            {
                Lobby.Layer.panelConstructionInfo.Visible = true;
                Lobby.Layer.panelConstructionInfo.Entity = this;
                if (selectPage >= 0)
                    Lobby.Layer.panelConstructionInfo.SelectPage(selectPage);
            }
            else
            {
                Lobby.Layer.panelConstructionInfo.Visible = true;
                Lobby.Layer.panelConstructionInfo.Entity = this;
//                Program.formMain.panelLairInfo.Visible = true;
//                Program.formMain.panelLairInfo.Entity = this;
            }

        }

        internal void PrepareNewDay()
        {
            if (Level > 0)
            {
                Initialize();

                if (Lobby.Turn > 1)
                {
                    if (Descriptor.Levels[Level].GreatnessPerDay > 0)
                        Player.AddGreatness(GreatnessPerDay());
                }

                if (CurrentMassEvent != null)
                {
                    CurrentMassEvent.Counter--;
                    if (CurrentMassEvent.Counter == 0)
                        RemoveProduct(CurrentMassEvent.Descriptor);
                }

                if (CurrentTournament != null)
                {
                    CurrentTournament.Counter--;
                    if (CurrentTournament.Counter == 0)
                        RemoveProduct(CurrentTournament.Descriptor);
                }

                foreach (CellMenuConstruction cm in Actions)
                {
                    cm.PrepareNewDay();
                }

                foreach (CellMenuConstructionSpell cm in MenuSpells)
                {
                    cm.PrepareNewDay();
                }

                if (Descriptor.ID != FormMain.Config.IDCityGraveyard)
                {
                    foreach (Creature h in Heroes)
                    {
                        h.PrepareNewDay();
                    }
                }
            }

            foreach (CellMenuConstruction cmc in Actions)
            {
                cmc.PrepareNewDay();
/*                CellMenuConstruction cm = QueueExecuting[0];
                Debug.Assert(cm.DaysLeft > 0);

                cm.DaysProcessed++;
                cm.DaysLeft--;

                if (cm.DaysLeft == 0)
                {
                    cm.Execute();

                    RemoveCellMenuFromQueue(cm, true, false);
                }*/
            }

            // Пересчитываем стоимости и время выполнения в начале хода, так как эти параметры могли измениться от дефолтных
            CalcPurchasesInActions();
            CalcDaysExecutingInActions();
        }

        internal void PrepareQueueShopping(List<UnitOfQueueForBuy> queue)
        {
            Debug.Assert(Level > 0);

            foreach (Creature h in Heroes)
            {
                if (h.IsLive)
                    h.PrepareQueueShopping(queue);
            }
        }

        internal List<TextRequirement> GetResearchTextRequirements(CellMenuConstruction research)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Descriptor.IsInternalConstruction)
            {
                // Если нет требований, то по умолчанию остается только одно - сооружение должно быть построено
                // Если есть, то не надо писать, что сооружение не построено - иначе не видно, какие там требования
                if (Level == 0)
                    list.Add(new TextRequirement(false, "Сооружение не построено"));

                Player.TextRequirements(research.Descriptor.CreatedEntity.GetCreating().Requirements, list);
            }

            return list;
        }

        internal override int GetQuantity()
        {
            return ComponentObjectOfMap is null ? 0 :ComponentObjectOfMap.ListHeroesForFlag.Count;
        }

        private void CreateMonsters()
        {
            AssertNotDestroyed();
            //Debug.Assert(TypeLair.Monsters.Count > 0);

            Creature lm;
            foreach (DescriptorConstructionLevelLair mll in Descriptor.Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Creature(this, mll.Monster, this, Player, mll.Level);
                    Monsters.Add(lm);
                    AddCombatHero(lm);
                }
            }
        }

        internal ListBaseResources CostScout()
        {
            Debug.Assert(!ComponentObjectOfMap.Visible);
            AssertNotDestroyed();

            return new ListBaseResources(0);
           //return PriorityFlag < PriorityExecution.Exclusive ?
           //     new ListBaseResources(Location.Settings.CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100) : new ListBaseResources();
        }

        private void AssertNotHidden()
        {
            Debug.Assert(ComponentObjectOfMap.Visible, $"Логово {Descriptor.ID} игрока {Player.GetName()} скрыто.");
        }

        internal ListBaseResources CostAttack()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return new ListBaseResources(0);
            //return PriorityFlag < PriorityExecution.Exclusive ?
            //    new ListBaseResources(Location.Settings.CostAttack * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100) : new ListBaseResources();
        }

        internal ListBaseResources CostDefense()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return new ListBaseResources(0);
            //return PriorityFlag < PriorityExecution.Exclusive ?
            //    new ListBaseResources(Location.Settings.CostDefense * Player.Lobby.TypeLobby.CoefFlagDefense[(int)PriorityFlag + 1] / 100) : new ListBaseResources();
        }


        internal override string GetName()
        {
            AssertNotDestroyed();

            if (ComponentObjectOfMap.Visible)
            {
                if (NextLocation is null)
                    return Descriptor.Name;
                else
                    return "Путь в " + NextLocation.Settings.Name;
            }
            else
                return "Неизвестное место";
        }

        internal override string GetTypeEntity() => Descriptor.TypeConstruction.Name;

        internal Color GetColorCaption()
        {
            return ComponentObjectOfMap.Visible ? Color.MediumAquamarine : FormMain.Config.ColorMapObjectCaption(false);

            switch (TypeAction())
            {
                case TypeFlag.Scout:
                    return Color.LimeGreen;
                case TypeFlag.Attack:
                    return Color.OrangeRed;
                case TypeFlag.Defense:
                    return Color.DodgerBlue;
                default:
                    throw new Exception($"Неизвестный тип действия: {TypeAction()}");
            }
        }

        internal ListBaseResources RequiredGold()
        {
            AssertNotDestroyed();

            switch (TypeAction())
            {
                case TypeFlag.Scout:
                    return CostScout();
                case TypeFlag.Attack:
                    return CostAttack();
                case TypeFlag.Defense:
                    return CostDefense();
                default:
                    throw new Exception($"Неизвестный тип действия: {TypeAction()}");
            }
        }

        internal List<TextRequirement> GetRequirements()
        {
            AssertNotDestroyed();

            List<TextRequirement> list = new List<TextRequirement>();

            return list;
        }

        internal TypeFlag TypeAction()
        {
            AssertNotDestroyed();

            if (!ComponentObjectOfMap.Visible)
                return TypeFlag.Scout;
            if (Descriptor.Category == CategoryConstruction.Lair)
                return TypeFlag.Attack;
            if (Descriptor.Category == CategoryConstruction.External)
                return TypeFlag.Defense;
            if (Descriptor.ID == FormMain.Config.IDConstructionCastle)
                return TypeFlag.Battle;
            return TypeFlag.None;
        }

        internal void AttackToCastle()
        {
            Debug.Assert(Descriptor.ID == FormMain.Config.IDConstructionCastle);
            ComponentObjectOfMap.TypeFlag = TypeFlag.Battle;
        }

        internal void CancelFlag()
        {
            Debug.Assert(ComponentObjectOfMap.TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            //Player.ReturnResource(Cashback());
            ComponentObjectOfMap.DropFlag();
        }

        internal void Unhide(bool needNotice)
        {
            Debug.Assert(Descriptor.Category != CategoryConstruction.Guild);
            Debug.Assert(Descriptor.Category != CategoryConstruction.Economic);
            Debug.Assert(Descriptor.Category != CategoryConstruction.Temple);
            Debug.Assert(Descriptor.Category != CategoryConstruction.Military);
            //Debug.Assert(TypeConstruction.Category != CategoryConstruction.External);
            Debug.Assert(!ComponentObjectOfMap.Visible);
            Debug.Assert(ComponentObjectOfMap.TypeFlag == TypeFlag.None);
            Debug.Assert(!Destroyed);

            ComponentObjectOfMap.Visible = true;
            if (needNotice)
                Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.Explore);

            if (NextLocation != null)
            {
                NextLocation.Visible = true;

                if (needNotice)
                    Player.AddNoticeForPlayer(NextLocation, TypeNoticeForPlayer.FoundLocation);
            }
        }

        // Сооружение уничтожено 
        internal void Destroy()
        {
            AssertNotDestroyed();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);

            // Если сооружение было выбрано, очищаем ссылку
            Lobby.Layer.ObjectDestroyed(this);
            Destroyed = true;
        }

        // Логово захвачено
        internal void DoCapture()
        {
            AssertNotHidden();
            Debug.Assert(ComponentObjectOfMap.TypeFlag == TypeFlag.Attack);
            Debug.Assert(ComponentObjectOfMap.ListHeroesForFlag.Count > 0);

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            ComponentObjectOfMap.DropFlag();

            Player.ApplyReward(this);
            Destroy();

            // Ставим тип места, который должен быть после зачистки
            Debug.Assert(!(Descriptor.TypePlaceForConstruct is null));

            Construction pl = new Construction(Player, Descriptor.TypePlaceForConstruct, Descriptor.DefaultLevel, X, Y, Location, true, true, true, false, TypeNoticeForPlayer.None);
            pl.ComponentObjectOfMap.Visible = true;
            Location.Lairs.Add(pl);
        }

        internal void DoDefense()
        {
            AssertNotHidden();
            AssertNotDestroyed();
            Debug.Assert(ComponentObjectOfMap.TypeFlag == TypeFlag.Defense);
            Debug.Assert(ComponentObjectOfMap.ListHeroesForFlag.Count > 0);

            // Раздаем награду
            HandOutGoldHeroes();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);

            Destroyed = true;
        }

        internal void MonsterIsDead(Creature m)
        {
            Debug.Assert(m != null);
            Debug.Assert(m.BattleParticipant == this);
            Debug.Assert(Monsters.IndexOf(m) != -1);

            m.SetIsDead(FormMain.Descriptors.ReasonOfDeathInBattle);
            CombatHeroes.Remove(m);
            Monsters.Remove(m);

            if (Lobby.Layer.PlayerObjectIsSelected(m))
                Lobby.Layer.SelectPlayerObject(null);
        }

        // Раздаем деньги за флаг героям
        private void HandOutGoldHeroes()
        {
/*            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            if (listAttackedHero.Count > 0)
            {
                Debug.Assert(SpendedGoldForSetFlag != null);

                // Определяем, по сколько денег достается каждому герою
                int goldPerHero = SpendedGoldForSetFlag.ValueGold() / listAttackedHero.Count;
                int delta = SpendedGoldForSetFlag.ValueGold() - goldPerHero * listAttackedHero.Count;
                Debug.Assert(goldPerHero * listAttackedHero.Count + delta == SpendedGoldForSetFlag.ValueGold());

                foreach (Hero h in listAttackedHero)
                    h.AddGold(goldPerHero);

                // Остаток отдаем первому герою
                if (delta > 0)
                    listAttackedHero[0].AddGold(delta);
            }*/
        }

        internal string ListMonstersForHint()
        {
            if (ComponentObjectOfMap.Visible)
            {
                if (Monsters.Count == 0)
                    return "Нет существ";

                string list = "";
                int pos = 1;
                foreach (Creature m in Monsters)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{pos}. {m.TypeCreature.Name} ({m.Level})";
                    pos++;
                }

                return list;
            }
            else
                return "Пока место не разведано, существа в нем неизвестны";
        }

        internal void PrepareHintForBuildOrUpgrade(PanelHint panelHint, int requiredLevel)
        {
            if (requiredLevel > Descriptor.MaxLevel)
                return;// Убрать это
            Debug.Assert(requiredLevel > 0);
            Debug.Assert(requiredLevel <= Descriptor.MaxLevel);

            panelHint.AddStep2Entity(this);
            panelHint.AddStep4Level(requiredLevel == 1 ? "Уровень 1" : $"Улучшить строение ({requiredLevel} ур.)");
            panelHint.AddStep5Description(requiredLevel == 1 ? Descriptor.Description : "");
            panelHint.AddStep6Income(IncomeForLevel(requiredLevel));
            panelHint.AddStep8Greatness(GreatnesAddForLevel(requiredLevel), GreatnesPerDayForLevel(requiredLevel));
            panelHint.AddStep9PlusBuilders(BuildersPerDayForLevel(requiredLevel));
            if (Descriptor.Levels[requiredLevel].DescriptorVisit != null)
            {
                panelHint.AddStep9Interest(Descriptor.Levels[requiredLevel].DescriptorVisit.Interest, false);
                panelHint.AddStep9ListNeeds(Descriptor.Levels[requiredLevel].DescriptorVisit.ListNeeds, false);
            }
            panelHint.AddStep12Creating(Player, Descriptor.Levels[requiredLevel].GetCreating().CalcConstructionPoints(Player), DayBuildingForLevel(requiredLevel),
                Descriptor.Levels[requiredLevel].GetCreating().CostResources, GetTextRequirements(requiredLevel));
            //panelHint.AddStep12Gold(Player.BaseResources, Descriptor.Levels[requiredLevel].GetCreating().CostResources);
            //panelHint.AddStep13Builders(Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player), Player.RestConstructionPoints >= Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player));
        }

        internal void PrepareHintForInhabitantCreatures(PanelHint panelHint)
        {
            if (Heroes.Count > 0)
            {

                string list = "";
                int pos = 1;
                foreach (Creature h in Heroes)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{pos}. {h.GetNameHero()} ({h.Level})";
                    pos++;
                }

                panelHint.AddStep2Header(Descriptor.IsOurConstruction ? "Жители" : "Существа");
                panelHint.AddStep5Description(list);
            }
            else
                panelHint.AddSimpleHint("Обитателей нет");
        }

        internal override int GetImageIndex()
        {
            AssertNotDestroyed();

            if (NextLocation != null)
                return NextLocation.GetImageIndex();
            else if ((Player.Lobby.CurrentPlayer is null) || (Player == Player.Lobby.CurrentPlayer))
                return ComponentObjectOfMap.Visible ? Descriptor.ImageIndex : FormMain.IMAGE_INDEX_UNKNOWN;
            else
                return FormMain.Config.Gui48_Battle;
        }

        internal override int GetCellImageIndex()
        {
            return CellMenuBuildNewConstruction is null ? GetImageIndex() : CellMenuBuildNewConstruction.GetImageIndex();
        }

        internal override int GetImageIndex24() => State == StateConstruction.NeedRepair ? FormMain.GUI_24_BROKEN_HOUSE : State == StateConstruction.Repair ? FormMain.GUI_24_HAMMER : -1;

        internal override string GetText() => CellMenuBuildNewConstruction is null ? "" : CellMenuBuildNewConstruction.GetText();

        internal override bool GetNormalImage() => (CurrentDurability == MaxDurability) && ((Level > 0) || (Descriptor.MaxLevel == 0));

        internal override string GetLevel()
        {
            AssertNotDestroyed();

            return !ComponentObjectOfMap.Visible ? "" : Level == 0 ? "" : (Level == 1) && (Descriptor.MaxLevel == 1) ? "" : Level < Descriptor.MaxLevel ? $"{Level}/{Descriptor.MaxLevel}" : Level.ToString();
        }

        internal override void Click(VCCell pe)
        {
            base.Click(pe);
            Lobby.Layer.SelectPlayerObject(this, -1, true);
        }

        internal override TypePlayer GetTypePlayer()
        {
            return TypePlayer.Lair;
        }

        internal override Player GetPlayer()
        {
            return Player;
        }

        internal override int GetImageIndexAvatar()
        {
            return Descriptor.ImageIndex;
        }

        internal List<ConstructionProduct> GetProducts(DescriptorCreature dc)
        {
            List<ConstructionProduct> list = new List<ConstructionProduct>();

            foreach (ConstructionProduct cp in ListEntities)
            {
                if (cp.IsAvailableForCreature(dc))
                {
                    list.Add(cp);
                }
            }

            return list;
        }

        internal Ability PurchaseAbility(Creature creature, ConstructionAbility ca)
        {
            Debug.Assert(Abilities.IndexOf(ca) >= 0);

            Ability a = new Ability(creature, ca.DescriptorAbility);
            return a;
        }

        private void AddEntity(EntityForConstruction entity)
        {
            foreach (EntityForConstruction i in ListEntities)
            {
                Debug.Assert(i.Descriptor.ID != entity.Descriptor.ID);
            }

            ListEntities.Add(entity);
        }

        internal void AddVisit(ConstructionVisit cv)
        {
            AddEntity(cv);
            Visits.Add(cv);
        }

        internal void AddExtension(ConstructionExtension extension)
        {
            AddEntity(extension);

            // Прибавляем ее удовлетворение потребностей к текущим
            Extensions.Add(extension);

            foreach ((DescriptorNeed, int) need in extension.Descriptor.ListNeeds)
            {
                ChangeNeed(need.Item1, need.Item2);
            }

            if (CurrentVisit != null)
                UpdateInterestMainVisit();
        }

        internal void AddImprovement(ConstructionImprovement improvement)
        {
            AddEntity(improvement);

            // Прибавляем ее удовлетворение потребностей к текущим
            Improvements.Add(improvement);
        }

        internal void AddResource(ConstructionResource resource)
        {
            AddEntity(resource);
            Resources.Add(resource);
        }

        internal void AddAbility(ConstructionAbility ca)
        {
            AddEntity(ca);
            Abilities.Add(ca);
        }

        internal void AddSpell(ConstructionSpell cs)
        {
            AddEntity(cs);
            Spells.Add(cs);
            Player.ConstructionSpells.Add(cs);
        }

        internal void AddMassEvent(ConstructionEvent ce)
        {
            AddEntity(ce);

            Debug.Assert(CurrentMassEvent is null);
            Debug.Assert(CurrentTournament is null);

            Visits.Add(ce);
        }

        internal void AddTournament(ConstructionTournament ct)
        {
            AddEntity(ct);

            Debug.Assert(CurrentMassEvent is null);
            Debug.Assert(CurrentTournament is null);

            Visits.Add(ct);
            CurrentTournament = ct;
        }

        internal void AddService(ConstructionService cs)
        {
            AddEntity(cs);
            Services.Add(cs);
        }

        internal void AddProduct(ConstructionProduct cp)
        {
            AddEntity(cp);

            if ((cp.DescriptorItem != null) || (cp.DescriptorGroupItem != null))
            {
                Goods.Add(cp);
            }
        }

        internal void RemoveProduct(DescriptorSmallEntity de)
        {
            EntityForConstruction productFromRemove = null;

            foreach (EntityForConstruction cp in ListEntities)
            {
                if (cp.Descriptor.ID == de.ID)
                {
                    productFromRemove = cp;
                    break;
                }
            }

            Debug.Assert(productFromRemove != null);

            if (CurrentVisit == productFromRemove)
                CurrentVisit = null;
            if (CurrentMassEvent == productFromRemove)
                CurrentMassEvent = null;
            if (CurrentTournament == productFromRemove)
                CurrentTournament = null;

            RemoveEntity(productFromRemove);
        }

        internal void RemoveEntity(EntityForConstruction entity)
        {
            Debug.Assert(entity != null);

            if (!ListEntities.Remove(entity))
                Debug.Fail($"Не смог удалить сущность {entity.Descriptor.ID} из сооружения {Descriptor.ID}");

            if (entity is ConstructionExtension ce)
            {
                if (!Extensions.Remove(ce))
                    Debug.Fail($"Не смог удалить доп. сооружение {entity.Descriptor.ID} из сооружения {Descriptor.ID}");
            }
            else if (entity is ConstructionResource cr)
            {
                if (!Resources.Remove(cr))
                    Debug.Fail($"Не смог удалить ресурс {entity.Descriptor.ID} из сооружения {Descriptor.ID}");
            }
            else if (entity is ConstructionEvent cev)
            {
                Debug.Assert(CurrentMassEvent != null);
                Debug.Assert(CurrentMassEvent == cev);
                CurrentMassEvent = null;
            }
            else if (entity is ConstructionTournament ct)
            {
                Debug.Assert(CurrentTournament != null);
                Debug.Assert(CurrentTournament == ct);
                CurrentTournament = null;
            }
            else if (entity is ConstructionAbility ca)
            {
                if (!Abilities.Remove(ca))
                    Debug.Fail($"Не смог удалить умение {entity.Descriptor.ID} из сооружения {Descriptor.ID}");
            }
            else if (entity is ConstructionSpell csp)
            {
                if (!Spells.Remove(csp))
                    Debug.Fail($"Не смог удалить заклинание {entity.Descriptor.ID} из сооружения {Descriptor.ID}");

                if (!Player.ConstructionSpells.Remove(csp))
                    Debug.Fail($"Не смог удалить заклинание {entity.Descriptor.ID} у игрока");
            }
            else if (entity is ConstructionService cs)
            {
                if (!Services.Remove(cs))
                    Debug.Fail($"Не смог удалить услугу {entity.Descriptor.ID} из сооружения {Descriptor.ID}");
            }
            else if (entity is ConstructionProduct cp)
            {
                Goods.Remove(cp);
            }
            else if (entity is ConstructionVisit cv)
            {
                Visits.Remove(cv);
            }
            else
                throw new Exception($"Неизвестная сущность {entity.Descriptor.ID}.");
        }

        internal void UpdateInterestMainVisit()
        {
            CurrentVisit.Interest = CurrentVisit.DescriptorConstructionVisit.Interest;

            foreach (ConstructionExtension cp in Extensions)
                CurrentVisit.Interest += cp.Descriptor.ModifyInterest;
        }

        private void ChangeNeed(DescriptorNeed need, int value)
        {
            SatisfactionNeeds[need.Index] += value;
        }

        internal bool GoodsExists(DescriptorItem item)
        {
            foreach (CellMenuConstruction cm in Actions)
            {
                if (cm is CellMenuConstructionResearch cmr)
                    if (cmr.Entity.ID == item.ID)
                        return true;
            }

            return false;
        }

        internal bool GoodsAvailabled(DescriptorProduct item)
        {
            foreach (ConstructionProduct cp in Goods)
            {
                if (cp.Descriptor.ID == item.ID)
                    return true;
            }

            return false;
        }

        internal bool ExtensionAvailabled(DescriptorConstructionExtension extension)
        {
            foreach (ConstructionExtension cp in Extensions)
            {
                if (cp.Descriptor.ID == extension.ID)
                    return true;
            }

            return false;
        }

        internal string HintDescriptionInterest()
        {
            Debug.Assert(Level > 0);

            if (GetInterest() == 0)// Возможно, это ошибка. Сооружение дает плюс, перк дает минус, в итоге ноль
                return "";

            string text = "Сооружение: " + Utils.DecIntegerBy10(Descriptor.Levels[Level].DescriptorVisit.Interest, false);

            foreach (ConstructionExtension cp in Extensions)
            {
                if (cp.Descriptor.ModifyInterest > 0)
                    text += Environment.NewLine + cp.Descriptor.Name + ": " + Utils.DecIntegerBy10(cp.Descriptor.ModifyInterest, true);
            }

            return text;
        }

        internal void AddEntityToQueueProcessing(CellMenuConstruction cell)
        {
            /*QueueExecuting.Add(cell);
            return;

            cell.DaysLeft = cell.InstantExecute() ? 1 : cell.Descriptor.CreatedEntity.GetCreating().DaysProcessing;
            if (cell.DaysLeft > 0)
                cell.DaysLeft--;

            if ((cell.DaysLeft == 0) || cell.InstantExecute())
            {
                //SpendForBuild(cell);
                cell.Execute();
            }
            else
            {
                //SpendForBuild(cell);
                QueueExecuting.Add(cell);
                //Player.AddEntityToQueueBuilding()
                cell.ExecutingAction.InQueue = true;

                if (cell is CellMenuConstructionBuild cm)
                {
                    CellMenuBuildNewConstruction = cm;
                }
            }*/
        }

        internal void RemoveEntityFromQueueProcessing(CellMenuConstruction cell, bool removeFromList)
        {
            /*
            Debug.Assert(QueueExecuting.IndexOf(cell) != -1);
            //Debug.Assert((cell.DaysLeft == 0) || (cell.DaysProcessed == 0));
            Debug.Assert(cell.ExecutingAction.InQueue);
            Debug.Assert(cell.PurchaseValue != null);

            cell.ExecutingAction.InQueue = false;
            Player.ReturnResource(cell.PurchaseValue);
            //Player.UnuseFreeBuilders(usedBuilders);

            if (removeFromList)
                QueueExecuting.Remove(cell);

            for (int i = 0; i < QueueExecuting.Count; i++)
            {
                //QueueExecuting[i].PosInQueue = i + 1;
            }

            if (CellMenuBuildNewConstruction != null)
            {
                Debug.Assert(CellMenuBuildNewConstruction == cell);

                CellMenuBuildNewConstruction = null;
            }*/
        }

        private void UpdateSelectedColor()
        {
            if (PlayerIsOwner)
                SelectedColor = Color.White;
            else if (PlayerCanOwn)
                SelectedColor = Color.LimeGreen;
            else if (!IsEnemy)
                SelectedColor = Color.Yellow;
            else
                SelectedColor = Color.Red;
        }

        internal void ChangeGold(int gold)
        {
            Gold += gold;

            Debug.Assert(Gold >= 0);
        }

        internal int CalcTax(int gold)
        {
            Debug.Assert(gold > 0);
            Debug.Assert(Level > 0);

            return gold * Descriptor.Levels[Level].Tax / 100;
        }

        internal void TuneLinks()
        {
            if (IDPathToLocation.Length > 0)
            {
                foreach (Location l in Player.Locations)
                {
                    if (IDPathToLocation == l.Settings.ID)
                    {
                        NextLocation = l;
                        break;
                    }
                }

                Debug.Assert(NextLocation != null);
            }
        }

        internal override Color GetSelectedColor() => SelectedColor;

        internal override void PlaySoundSelect()
        {
            if (ComponentObjectOfMap.Visible)
                base.PlaySoundSelect();
            else
                Program.formMain.PlayPushButton();
        }

        // Настройка сооружения при создании
        private void TuneByCreate()
        {
            foreach (DescriptorCellMenu d in Descriptor.CellsMenu)
                Actions.Add(CellMenuConstruction.Create(this, d));

            if (Descriptor.Monsters.Count > 0)// Убрать эту проверку после настройки всех логов
                CreateMonsters();

            IncomeBaseResources = new ConstructionListBaseResources(this);

            DayLevelConstructed = new int[Descriptor.Levels.Length + 1];
            for (int i = 1; i < DayLevelConstructed.Length; i++)
                DayLevelConstructed[i] = -1;

            UpdatePointsResearch();

            Player.AddConstruction(this);
        }

        private void UpdatePointsResearch()
        {
            ResearchPoints = FormMain.Config.DefaultResearchPoints;
            RestResearchPoints = ResearchPoints;
        }

        internal void UpdateDaysConstruction()
        {
            AssertNotDestroyed();

            if (ActionRepair != null)
            {
                if (CurrentDurability == MaxDurability)
                {
                    if (!Actions.Remove(ActionRepair))
                        EntityDoException("Не смог убрать кнопку окончания ремонта");

                    ActionRepair = null;
                }
                else
                    ActionRepair.DaysForRepair = Player.CalcDaysForEndConstruction(CurrentDurability, MaxDurability);
            }

            foreach (CellMenuConstruction cm in Actions)
            {
                if (cm is CellMenuConstructionLevelUp cml)
                {
                    Debug.Assert(cml.Descriptor.Number > Level);// Не должно быть действия на постройку уже построенного уровня

                    // Учитываем, что следующий уровень может быть построен
                    cml.DaysForConstructed = Player.CalcDaysForEndConstruction(cml.Descriptor.Number == 1 ? 0 : Descriptor.Levels[cml.Descriptor.Number - 1].Durability, cml.Descriptor.Durability);
                }
            }

            TuneCellMenuBuildOrUpgrade();// Если кнорка ремонта была удалена, надо обновить действия
        }

        // Подготовка строительства сооружения
        // Вызывается у городских сооружений сразу
        internal void PrepareBuilding()
        {
            MaxDurability = Descriptor.Levels[Level + 1].Durability;
            UpdateState();
        }

        internal void StartBuilding()
        {
            if (Level > 0)
            {
                PrepareBuilding();
            }

            Player.AddToQueueBuilding(ActionBuildOrLevelUp);
            UpdateState();
        }

        internal void StartRepair()
        {
            Player.AddToQueueBuilding(ActionRepair);
            UpdateState();
        }

        internal void CancelRepair()
        {
            Player.RemoveFromQueueBuilding(ActionRepair, false);
            Player.RebuildQueueBuilding();
            UpdateState();
        }

        internal void CancelBuilding()
        {
            if (Level > 0)
                MaxDurability = Descriptor.Levels[Level].Durability;

            Player.RemoveFromQueueBuilding(ActionBuildOrLevelUp, false);
            Player.RebuildQueueBuilding();
            UpdateState();
        }

        internal void TuneConstructAfterCreate()
        {
            UpdateCurrentIncomeResources();
            TuneCellMenuBuildOrUpgrade();
            UpdateSelectedColor();
            UpdateState();
        }

        internal void UpdateState()
        {
            if (Destroyed)
                State = StateConstruction.Destroyed;
            else if ((Level == 1) && (MaxDurability == 0))
                State = StateConstruction.None;// Если сооружение построено, и у него нет прочности, это элемент ландшафта. У него нет состояния.
            else if (Level == 0)
            {
                Assert(!InRepair);

                if (ActionBuildOrLevelUp.ExecutingAction.InQueue)
                {
                    if (CurrentDurability == 0)
                        State = StateConstruction.PreparedBuild;// Стройка подготовлена, еще не начата
                    else
                    {
                        if (ActionBuildOrLevelUp.ExecutingAction.CurrentPoints > 0)
                            State = StateConstruction.Build;// Стройка идет
                        else
                            State = StateConstruction.InQueueBuild;// В очереди на строительство
                    }
                }
                else
                    State = StateConstruction.NotBuild;// Сооружение не построено
            }
            else if (InRepair)
                State = StateConstruction.Repair;// Идет ремонт
            else if (CurrentDurability == MaxDurability)
                State = StateConstruction.Work;// Прочность равна дефолтной, сооружение работает
            else if (CurrentDurability < MaxDurability)
            {
                State = StateConstruction.NeedRepair;// Сооружение повреждено, требуется ремонт
                //CellMenuRepair.PurchaseValue = CompCostRepair(Math.Min(Player.RestConstructionPoints, restCP, c.MaxDurability - c.CurrentDurability))            
            }
            else
                DoException("Неопределенное состояние сооружения");
        }

        internal void DoDamage(int damage)
        {
            Assert(damage >= 0);
            Assert(damage < CurrentDurability);
            Assert((State == StateConstruction.Build) || (State == StateConstruction.Repair)
                || (State == StateConstruction.NeedRepair) || (State == StateConstruction.Work));

            if (damage > 0)
            {
                CurrentDurability -= damage;

                if (ActionRepair is null)
                {
                    ActionRepair = new CellMenuConstructionRepair(this, new DescriptorCellMenu(new Point(0, 0)));
                    //CellMenuRepair.PurchaseValue = new ListBaseResources(MaxDurability - CurrentDurability);

                    Actions.Add(ActionRepair);
                }

                Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.ConstructionDamaged, damage);
                UpdateState();
                TuneCellMenuBuildOrUpgrade();
            }
        }

        internal ListBaseResources CompCostRepair(int durability)
        {
            return new ListBaseResources(durability);
        }

        internal (string, Color) GetDataState()
        {
            switch (State)
            {
                case StateConstruction.None:
                    return ("", Color.White);
                case StateConstruction.Work:
                    return ("Работает", Color.LightGreen);
                case StateConstruction.NotBuild:
                    return ("Не построено", Color.Gray);
                case StateConstruction.PreparedBuild:
                    return ("Подготовлено к строительству", Color.SkyBlue);
                case StateConstruction.Build:
                    return ("Строится", Color.SkyBlue);
                case StateConstruction.InQueueBuild:
                    return ("В очереди на строительство", Color.White);
                case StateConstruction.NeedRepair:
                    return ("Требуется ремонт", Color.Red);
                case StateConstruction.Repair:
                    return ("Ремонтируется", Color.Yellow);
                case StateConstruction.Destroyed:
                    throw new Exception("Сооружение уничтожено");
                default:
                    throw new Exception("Неизвестное состояние");
            }
        }

        internal void ClearQueueExecuting()
        {
            foreach (CellMenuConstruction cmc in QueueExecuting)
                RemoveCellMenuFromQueue(cmc, false, true);

            QueueExecuting.Clear();
            QueueBlocked = false;

            // После очистки очереди, количество оставшихся очков исследования должно равняться дефолтному
            Assert(RestResearchPoints == ResearchPoints);
        }

        internal void AddCellMenuToQueue(CellMenuConstruction cmc)
        {
            AssertNotDestroyed();
            Assert(cmc.Construction == this);
            Assert(QueueExecuting.IndexOf(cmc) == -1);
            Assert(!cmc.ExecutingAction.InQueue);

            cmc.ExecutingAction.InQueue = true;
            cmc.ExecutingAction.RestDaysExecuting = 0;

            QueueExecuting.Add(cmc);
            if (!Actions.Remove(cmc))
                EntityDoException($"Не удалось удалить {cmc} из списка действий.");
            Program.formMain.layerGame.UpdateMenu();

            // Если очередь заблокирована, ставим невозможность выполнения и выходм
            if (QueueBlocked)
                return;

            int expenseCP = 0;

            // Правильная реализация
            if (cmc.ExecutingAction.IsConstructionPoints)
            {
                // Если строительство еще не начинали, но очки строительства еще есть, списываем деньги
                if ((cmc.ExecutingAction.AppliedPoints == 0) && (Player.RestConstructionPoints > 0))
                {
                    cmc.UpdatePurchase();// Это может быть какой-нибудь ремонт, который зависит от остатка денег и очков. Пересчитываем расход перед списанием

                    if (Player.RestConstructionPoints > 0)
                    {
                        if (Player.CheckRequiredResources(cmc.PurchaseValue))
                        {
                            Player.SpendResource(cmc.PurchaseValue);
                        }
                        else
                        {
                            QueueBlocked = true;
                            return;
                        }
                    }
                }

                // Ресурсы списаны, можно ставить в очередь
                if (Player.RestConstructionPoints > 0)
                {
                    expenseCP = Math.Min(Player.RestConstructionPoints, cmc.ExecutingAction.NeedPoints);
                }

                if (expenseCP > 0)
                {
                    cmc.ExecutingAction.CurrentPoints = expenseCP;
                    Player.RestConstructionPoints -= expenseCP;
                }

                Player.UsedConstructionPoints += cmc.ExecutingAction.NeedPoints;
                cmc.ExecutingAction.RestDaysExecuting = CalcDaysForExecuting(Player.UsedConstructionPoints, Player.ConstructionPoints);

                // Если ресурсы еще не тратили, пробуем потратить. Возможно, их не хватит
                /*if (Level == 0)
                {

                    expenseCP = Math.Min(restCP, cmc.ExecutingAction.NeedPoints);
                    Debug.Assert(expenseCP > 0);
                }
                else
                {
                    Assert(InRepair);

                    // В случае ремонта, мы тратим столько очков строительства, на сколько у нас хватает денег
                    // Причем деньги тратятся только на текущий ход (вполне может быть, что сооружение будет снова подломано, поэтому чинить надо будет больше)
                    // Поэтому сейчас просто возвращаем все ресурсы, и заново просчитываем
                    if (cmc.ExecutingAction.AppliedPoints == 0)
                        Player.ReturnResource(cmc.PurchaseValue);

                    // Пока что считаем количество требуемого золота по соотношению 1 к 1
                    expenseCP = Math.Min(Gold, Math.Min(restCP, cmc.ExecutingAction.NeedPoints));
                    cmc.UpdatePurchase();
                    Player.SpendResource(cmc.PurchaseValue);
                }

                // Если ресурсы были потрачены, то тратим очки строительства
                if (cmc.ExecutingAction.AppliedPoints == 0)
                {
                    usedCP += cmc.ExecutingAction.NeedPoints;

                    cmc.ExecutingAction.CurrentPoints = expenseCP;

                    restCP -= expenseCP;
                }
            }
            else
            {
                // Очки строительства закончились
                // Если были потрачены ресурсы, возвращаем их
                if (cmc.ExecutingAction.AppliedPoints == 0)
                    Player.ReturnResource(cmc.PurchaseValue);

                usedCP += cmc.ExecutingAction.NeedPoints;

            }*/
            }
            else
            {

            }

            //cmc.ExecutingAction.CurrentPoints = 0;
            //cmc.ExecutingAction.RestDaysExecuting = CalcDaysForExecuting(usedCP, Player.ConstructionPoints) + Player.ShiftDayForConstruct;

            //Player.RestConstructionPoints = restCP;

            //cmc.ExecutingAction.InQueue = true;
        }

        internal int CalcDaysForExecuting(int applyPoints, int freePoints)
        {
            int d = applyPoints / freePoints + (applyPoints % freePoints == 0 ? 0 : 1);
            Assert(d > 0);
            return d;
        }

        internal void RemoveCellMenuFromQueue(CellMenuConstruction cmc, bool removeFromList, bool forCancel)
        {
            Assert(cmc.Construction == this);
            Assert(cmc.ExecutingAction.InQueue);
            Assert(QueueExecuting.IndexOf(cmc) != -1);
            if (forCancel)
            {
                Assert(cmc.ExecutingAction.AppliedPoints == 0);
            }

            if (cmc is CellMenuConstructionLevelUp)
            {
                //Assert( || InRepair);
                Assert(MaxDurability > 0);

                if (forCancel)
                {
                    // Если сооружение еще не начинали строить, только возвращаем ресурсы
                    if (State == StateConstruction.PreparedBuild)
                    {
                        //InConstructing = false;
                    }
                    else if (State == StateConstruction.Repair)
                    {
                        InRepair = false;
                    }
                }
            }

            // Освобождаем потраченные ресурсы, если выполнение действия не началось
            if (forCancel)
            {
                Assert(cmc.ExecutingAction.AppliedPoints == 0);
                if (cmc.ExecutingAction.CurrentPoints > 0)
                    Player.ReturnResource(cmc.PurchaseValue);
                if (cmc.ExecutingAction.IsConstructionPoints && (cmc.ExecutingAction.CurrentPoints > 0))
                    Player.RestConstructionPoints += cmc.ExecutingAction.CurrentPoints;
            }

            if (removeFromList)
            {
                if (!QueueExecuting.Remove(cmc))
                    DoException($"{IDEntity}: не удалось удалить {IDEntity} из очереди строительства");

                Player.DeleteFromQueueBuilding(cmc);
            }

            if (forCancel)
            {
                Assert(Actions.IndexOf(cmc) == -1);
                Actions.Add(cmc);
                Program.formMain.layerGame.UpdateMenu();
            }

            cmc.ExecutingAction.InQueue = false;
            cmc.ExecutingAction.CurrentPoints = 0;
        }

        internal void CalcPurchasesInActions()
        {
            AssertNotDestroyed();

            foreach (CellMenuConstruction cmc in Actions)
                if (cmc.ExecutingAction != null)
                {
                    if (cmc.ExecutingAction.AppliedPoints == 0)
                    {
                        Assert(!cmc.ExecutingAction.InQueue);
                        Assert(cmc.ExecutingAction.CurrentPoints == 0);
                        cmc.UpdatePurchase();
                    }
                }
                else
                    cmc.UpdatePurchase();
        }

        internal void CalcDaysExecutingInActions()
        {
            foreach (CellMenuConstruction cmc in Actions)
                cmc.UpdateDaysExecuted();
        }
    }
}
