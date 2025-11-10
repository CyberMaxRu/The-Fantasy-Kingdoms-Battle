using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
//using System.Runtime.InteropServices.WindowsRuntime;

namespace Fantasy_Kingdoms_Battle
{
    // Класс сооружения у игрока
    internal sealed class Construction : BigEntity
    {
        private List<ActionInConstruction> tempListActions = new List<ActionInConstruction>();

        // Конструктор для городских сооружений, которые создаются в начале миссии
        public Construction(Player p, DescriptorConstruction dc) : base(dc, p.Lobby, p)
        {
            //Assert(dc.IsInternalConstruction);

            Descriptor = dc;

            TuneByCreate();
            TuneConstructAfterCreate();
        }

        // Конструктор для сооружений, которые создаются в процессе игры
        public Construction(Player p, DescriptorConstruction dc, int level, int x, int y, TypeNoticeForPlayer typeNotice, int initQ = 0) : base(dc, p.Lobby, p)
        {
            Assert(level <= 1);

            Descriptor = dc;
            X = x;
            Y = y;

            TuneByCreate();

            if (level == 1)
                Build(false, true);

            if (typeNotice != TypeNoticeForPlayer.None)
                Player.AddNoticeForPlayer(this, typeNotice);

            TuneConstructAfterCreate();
        }

        internal new DescriptorConstruction Descriptor { get; }// Описатель сооружения
        internal int Level { get; private set; }

        //
        internal int Gold { get; set; }
        internal List<Creature> Heroes { get; } = new List<Creature>();

        // Свойства для панели сооружений
        internal int X { get; set; }// Позиция по X в панели сооружений
        internal int Y { get; set; }// Позиция по Y в панели сооружений
        internal Color SelectedColor { get; private set; }// Цвет рамки при выделении

        // Small-сущности в сооружении
        internal List<EntityForConstruction> ListEntities { get; } = new List<EntityForConstruction>();// Все сущности в сооружении
        internal ConstructionVisitSimple CurrentVisit { get; private set; }// Текущее активное посещение сооружения
        internal ConstructionEvent CurrentMassEvent { get; set; }// Текущее мероприятие
        internal List<ConstructionVisit> Visits { get; } = new List<ConstructionVisit>();//
        internal List<ConstructionExtension> Extensions { get; } = new List<ConstructionExtension>();// Дополнения
        internal List<ConstructionImprovement> Improvements { get; } = new List<ConstructionImprovement>();// Улучшения
        internal int IncomeBaseResources { get; set; }// Поступление базовых ресурсов
        internal List<ConstructionService> Services { get; } = new List<ConstructionService>();// Услуги, доступные в строении
        internal List<ConstructionProduct> Goods { get; } = new List<ConstructionProduct>();// Товары, доступные в строении
        internal List<ConstructionAbility> Abilities { get; } = new List<ConstructionAbility>();// Умения, доступные в строении
        internal List<ConstructionSpell> Spells { get; } = new List<ConstructionSpell>();// Заклинания, доступные в строении

        // Действия
        internal ActionInConstruction ActionMain { get; private set; }// Основное действие, которое отображается в панели сооружения
        private CellMenuConstructionLevelUp ActionBuildOrLevelUp { get; set; }// Действие для постройки/улучшения сооружения
        internal CellMenuConstructionBuild CellMenuBuildNewConstruction { get; set; }// Ячейка меню, которая строит новое сооружение на этом месте

        //
        internal List<Creature> Recruits { get; } = new List<Creature>();// Рекруты, готовые к найму
        internal List<CellMenuConstructionCreatingCreature> CreaturesInQueue = new List<CellMenuConstructionCreatingCreature>();// Существа в очереди выполнения

        internal int[] SatisfactionNeeds { get; private set; }// Удовлетворяемые потребности
        internal List<CellMenuConstructionSpell> MenuSpells { get; } = new List<CellMenuConstructionSpell>();
        // 
        internal int IncomeResources { get; set; } = 0;// Собрано ресурсов (для зачета для игрока в текущем тике)

        internal override string GetIDEntity(DescriptorEntity descriptor)
        {
            return descriptor.ID;
        }

        // Методы, связанные с повышением уровня
        internal void LevelUp(int ToLevel)
        {
            Assert(Level < ToLevel);
            Assert(ToLevel <= Descriptor.MaxLevel);

            while (Level < ToLevel)
            {
                Build(false, true);
            }
        }

        private void TuneActionLevelUp()
        {
            ActionBuildOrLevelUp = null;

            if (Descriptor.Levels.Length > 2)
            {
                // Сооружение не построено, ищем действие для постройки
                List<ActionInConstruction> listForDelete = new List<ActionInConstruction>();

                foreach (ActionInConstruction cm in Actions)
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
                foreach (ActionInConstruction cmd in listForDelete)
                    cmd.Destroyed = true;

                if (ActionBuildOrLevelUp != null)
                    ActionMain = ActionBuildOrLevelUp;
                else
                    ActionMain = null;
            }
        }

        private void UpdateCurrentIncomeResources()
        {
            /*
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

                    if (Descriptor.Levels[Level].IncomeResources != 0)
                    {
                        ProvideBaseResources = true;
                        int q = 0;

                        q += Descriptor.Levels[Level].IncomeResources;

                        Debug.Assert(q > 0);
                    }
                }
            }
            */
        }

        internal void Build(bool needNotice, bool instant)
        {
            if (!Lobby.InPrepareTurn && (Lobby.CurrentPlayer?.GetTypePlayer() == TypePlayer.Human))
                Program.formMain.PlayConstructionComplete();

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

            Level++;

            if (Level == 1)
            {
                ValidateHeroes();
                //PrepareTurn();
            }

            CreateProducts();

                // Убираем операцию постройки из меню
                ActionInConstruction cmBuild = null;
                foreach (ActionInConstruction cm in Actions)
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

            TuneActionLevelUp();
            UpdateCurrentIncomeResources();
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
            Debug.Assert(Descriptor != null);

            ValidateResearches();
            FillResearches(menu);
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

        internal int Income()
        {
            return (Level > 0) ? IncomeBaseResources : 0;
        }

        internal int IncomeForLevel(int level)
        {
            return Descriptor.Levels[level].IncomeResources != 0 ? Descriptor.Levels[level].IncomeResources : 0;
        }

        internal int GreatnesAddForLevel(int level)
        {
            return Descriptor.Levels[level].GreatnessByConstruction;
        }

        internal int GreatnesPerDayForLevel(int level)
        {
            return Descriptor.Levels[level].GreatnessPerDay;
        }

        internal int IncomeNextLevel()
        {
            return Level < Descriptor.MaxLevel ? IncomeForLevel(Level + 1) : 0;
        }

        internal int GreatnessPerDay()
        {
            return Level > 0 ? Descriptor.Levels[Level].GreatnessPerDay : 0;
        }

        internal int GreatnessAddNextLevel()
        {
            return Level < Descriptor.MaxLevel ? GreatnesAddForLevel(Level + 1) : 0;
        }

        internal int GreatnessPerDayNextLevel()
        {
            return Level < Descriptor.MaxLevel ? GreatnesPerDayForLevel(Level + 1) : 0;
        }

        internal int MaxHeroes()
        {
            return Level > 0 ? Descriptor.Levels[Level].MaxInhabitant : 0;
        }

        internal bool MaxCreaturesInConstruction()
        {
            return Level > 0 ? Heroes.Count + CreaturesInQueue.Count == MaxHeroes() : false;
        }

        internal bool MaxHeroesAtPlayer()
        {
            return Player.CombatHeroes.Count == Player.Lobby.TypeLobby.MaxHeroes;
        }

        internal bool AllowHire()
        {
            if (Level == 0)
                return false;

            if (MaxCreaturesInConstruction())
                return false;

            if (MaxHeroesAtPlayer())
                return false;

            return true;
        }

        internal Creature HireHero(DescriptorCreature th, int cost)
        {
            Debug.Assert(!MaxCreaturesInConstruction());
            Debug.Assert(!MaxHeroesAtPlayer());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= TypeConstruction.TrainedHero.Cost);

            Creature h = new Creature(this, th, Player, Player, 1);

            if (cost != 0)
                Player.SpendResource(cost);

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

        internal override void PrepareHint(PanelHint panelHint)
        {
            if (Player == Player.Lobby.CurrentPlayer)
            {

                panelHint.AddStep2Entity(this);
                if (!((Level == 1) && (Descriptor.MaxLevel == 1)))
                    panelHint.AddStep4Level(Level > 0 ? "Уровень " + Level.ToString(): "");
                panelHint.AddStep5Description(Descriptor.Description + ((Level > 0) && (Heroes.Count > 0) ? Environment.NewLine + Environment.NewLine
                    + (Heroes.Count > 0 ? "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : "") : ""));
                panelHint.AddStep6Income(Income());
                panelHint.AddStep9Interest(GetInterest(), false);
                panelHint.AddStep9ListNeeds(SatisfactionNeeds);
            }
        }

        internal override void HideInfo()
        {
            base.HideInfo();

            //Debug.Assert(!Destroyed);// Assert не нужен - если сооружение уничтожено, его надо скрыть

            Lobby.Layer.panelConstructionInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Lobby.Layer.panelConstructionInfo.Visible = true;
            Lobby.Layer.panelConstructionInfo.Entity = this;
            if (selectPage >= 0)
                Lobby.Layer.panelConstructionInfo.SelectPage(selectPage);
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

                foreach (ActionInConstruction cm in Actions)
                {
                    cm.PrepareNewDay();
                }

                foreach (CellMenuConstructionSpell cm in MenuSpells)
                {
                    cm.PrepareNewDay();
                }
            }

            foreach (ActionInConstruction cmc in Actions)
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

        internal string GetNameForLevel(int level)
        {
            if (Descriptor.Levels[level].NewName)
                return Descriptor.Levels[level].Name;

            return Descriptor.Name;
        }

        internal override string GetName()
        {
            AssertNotDestroyed();

            if ((Level > 0) && Descriptor.Levels[Level].NewName)
                return Descriptor.Levels[Level].Name;

            return Descriptor.Name;
        }

        internal override string GetTypeEntity() => Descriptor.TypeConstruction.Name;

        internal ListTextRequirement GetRequirements()
        {
            AssertNotDestroyed();

            ListTextRequirement list = new ListTextRequirement();

            return list;
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

                panelHint.AddStep2Header("Жители");
                panelHint.AddStep5Description(list);
            }
            else
                panelHint.AddSimpleHint("Обитателей нет");
        }

        internal override int GetImageIndex()
        {
            AssertNotDestroyed();

            if ((Player.Lobby.CurrentPlayer is null) || (Player == Player.Lobby.CurrentPlayer))
                return Descriptor.ImageIndex;
            else
                return FormMain.Config.Gui48_Battle;
        }

        internal override int GetCellImageIndex()
        {
            return CellMenuBuildNewConstruction is null ? GetImageIndex() : CellMenuBuildNewConstruction.GetImageIndex();
        }

        internal override int GetImageIndex24() => -1;

        internal override string GetText() => CellMenuBuildNewConstruction is null ? "" : CellMenuBuildNewConstruction.GetText();

        internal override bool GetNormalImage() => (Level > 0) || (Descriptor.MaxLevel == 0);

        internal override string GetLevel()
        {
            AssertNotDestroyed();

            return Level == 0 ? "" : (Level == 1) && (Descriptor.MaxLevel == 1) ? "" : Level < Descriptor.MaxLevel ? $"{Level}/{Descriptor.MaxLevel}" : Level.ToString();
        }

        internal override void Click(VCCell pe)
        {
            base.Click(pe);
            Lobby.Layer.SelectPlayerObject(this, -1, true);
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

        internal void AddAbility(ConstructionAbility ca)
        {
            AddEntity(ca);
            Abilities.Add(ca);
        }

        internal void AddSpell(ConstructionSpell cs)
        {
            AddEntity(cs);
            Spells.Add(cs);
            //Player.ConstructionSpells.Add(cs);
        }

        internal void AddMassEvent(ConstructionEvent ce)
        {
            AddEntity(ce);

            Debug.Assert(CurrentMassEvent is null);

            Visits.Add(ce);
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
            else if (entity is ConstructionEvent cev)
            {
                Debug.Assert(CurrentMassEvent != null);
                Debug.Assert(CurrentMassEvent == cev);
                CurrentMassEvent = null;
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

                //if (!Player.ConstructionSpells.Remove(csp))
                //    Debug.Fail($"Не смог удалить заклинание {entity.Descriptor.ID} у игрока");
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
            foreach (ActionInConstruction cm in Actions)
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

        internal void AddEntityToQueueProcessing(ActionInConstruction cell)
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

        internal void RemoveEntityFromQueueProcessing(ActionInConstruction cell, bool removeFromList)
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
            SelectedColor = Color.White;
        }

        internal void ChangeGold(int gold)
        {
            Gold += gold;

            Debug.Assert(Gold >= 0);
        }

        internal override Color GetSelectedColor() => SelectedColor;

        internal override void PlaySoundSelect()
        {
            base.PlaySoundSelect();
        }

        // Настройка сооружения при создании
        private void TuneByCreate()
        {
            foreach (DescriptorActionForEntity d in Descriptor.CellsMenu)
                Actions.Add(ActionInConstruction.Create(this, d));

            IncomeBaseResources = 0;

            Player.AddConstruction(this);
        }

        // Подготовка строительства сооружения
        // Вызывается у городских сооружений сразу
        
        internal void TuneConstructAfterCreate()
        {
            UpdateCurrentIncomeResources();
            TuneActionLevelUp();
            UpdateSelectedColor();
        }

        internal void CalcPurchasesInActions()
        {
            AssertNotDestroyed();

            foreach (ActionInConstruction cmc in Actions)
                cmc.UpdatePurchase();
        }

        internal void CalcDaysExecutingInActions()
        {
            //foreach (CellMenuConstruction cmc in Actions)
            //    cmc.UpdateTimeExecuted();
        }

        // Новые методы для реал-таймового режима
        internal void DoTick(bool startNewDay)
        {
            if (startNewDay)
            {
                if (Level > 0)
                    if (Descriptor.Levels[Level].IncomeResources != null)
                    {
                        // Подготавливаем сбор ресурсов за ход
                        Assert(IncomeResources == 0);
                        IncomeResources += Descriptor.Levels[Level].IncomeResources;
                    }
            }

            tempListActions.Clear();
            tempListActions.AddRange(Actions);

            foreach (ActionInConstruction cmc in tempListActions)
            {
            }
        }

        internal void UpdateAfterTick()
        {
            ValidateActions();
            CalcPurchasesInActions();

            foreach (ActionInConstruction cm in Actions)
            {
                if (cm is CellMenuConstructionLevelUp cml)
                {
                    Debug.Assert(cml.Descriptor.Number > Level);// Не должно быть действия на постройку уже построенного уровня
                }
            }

            TuneActionLevelUp();// Если кнопка ремонта была удалена, надо обновить действия
        }

        internal void ValidateActions()
        {
            for (int i = 0; i < Actions.Count;)
            {
                if (Actions[i].Destroyed)
                    Actions.RemoveAt(i);
                else
                    i++;
            }
        }

        internal int CalcTimeForExecuting(Integer1000 progress, int max, int fullTime, TypeCreating typeCreating)
        {
            Assert(progress.Value < max * 1000);

            // Сначала вычисляем, сколько очков прогресса выполняется за 1 секунду
            double perSecond = max / fullTime;
            int time = (int)((max - progress.AsInteger) / perSecond);

            // Если значение получилось меньше 1, то ставим в 1, чтобы продолжить показывать оставшуюся 1 секунду
            if (time == 0)
                time = 1;

            return time;
            /*
            if (isConstructionPoints && Player.CheatingInstantlyBuilding)
                return 0;
            if (!isConstructionPoints && Player.CheatingInstantlyResearch)
                return 0;

            int d = applyPoints / freePoints + (applyPoints % freePoints == 0 ? 0 : 1);
            Assert(d > 0);
            return d;
            */
        }

        private int ConstructionPointPerTick()
        {
            //int cpPerTick = 1000 * FormMain.Config.ConstructionPointsPerHour / FormMain.Config.TicksInHour;
            return 0;// cpPerTick;
        }
    }
}
