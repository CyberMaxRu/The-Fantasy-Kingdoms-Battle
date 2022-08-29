using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class CellMenuConstruction : ActionForEntity
    {
        public CellMenuConstruction(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Construction = c;

            if (Descriptor.CreatedEntity != null)
            {
                Creating = Descriptor.CreatedEntity.GetCreating();

                if (Creating != null)
                    if ((Creating.ConstructionPoints > 0) || (Creating.ResearchPoints > 0))
                        ExecutingAction = new ComponentExecutingAction(Creating);
            }
        }

        internal Construction Construction { get; }
        internal DescriptorComponentCreating Creating { get; }
        internal ComponentExecutingAction ExecutingAction { get; private protected set; }

        internal virtual void InQueueChanged()
        {

        }

        internal override string GetText() => (ExecutingAction != null) && ExecutingAction.InQueue ? "" : PurchaseValue != null ? PurchaseValue.Gold.ToString() : "";

        internal override bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Construction.Descriptor.IsInternalConstruction)
                if (ConstructionMustMeConstructed())
                    if (Construction.Level == 0)
                        return false;

            if ((ExecutingAction != null) && !ExecutingAction.InQueue)
                if (Construction.QueueExecuting.Count >= Config.MaxLengthQueue)
                    return false;

            // Потом проверяем наличие требуемых ресурсов
            if (PurchaseValue != null)
                if (!Construction.Player.CheckRequiredResources(PurchaseValue))
                    return false;

            if (Descriptor.CreatedEntity != null)
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.GetCreating().Requirements);

            return true;
        }

        internal sealed override string GetLevel()
        {
            return Program.formMain.Settings.ShowTypeCellMenu ? GetTextForLevel() : "";
        }

        protected override int GetDaysExecuting()
        {
            if (!Program.formMain.Settings.ShowQuantityDaysForExecuting)
                return -1;

            if (ExecutingAction is null)
                return -1;

            //Assert(ExecutingAction.RestDaysExecuting > 0);
            return ExecutingAction.RestDaysExecuting;
        }

        protected virtual string GetTextForLevel() => "";
        protected abstract void Execute();

        protected void RemoveSelf()
        {
            Debug.Assert(Construction.Actions.IndexOf(this) != -1);
            Construction.Actions.Remove(this);
            Construction.Player.Lobby.Layer.UpdateMenu();
        }

        protected virtual bool ConstructionMustMeConstructed() => true;

        protected override void UpdateTextRequirements(List<TextRequirement> list)
        {
            base.UpdateTextRequirements(list);

            if (Construction.Descriptor.IsInternalConstruction && ConstructionMustMeConstructed())
            {
                // Если нет требований, то по умолчанию остается только одно - сооружение должно быть построено
                // Если есть, то не надо писать, что сооружение не построено - иначе не видно, какие там требования
                if (Construction.Level == 0)
                    list.Add(new TextRequirement(false, "Сооружение не построено"));

                Construction.Player.TextRequirements(Descriptor.CreatedEntity.GetCreating().Requirements, list);
            }
        }

        internal static CellMenuConstruction Create(Construction c, DescriptorActionForEntity d)
        {
            if (d.CreatedEntity != null)
            {
                if (d.CreatedEntity is DescriptorProduct)
                    return new CellMenuConstructionResearch(c, d);
                if (d.CreatedEntity is DescriptorConstructionLevel)
                    return new CellMenuConstructionLevelUp(c, d);
                if (d.CreatedEntity is DescriptorConstructionMassEvent)
                    return new CellMenuConstructionMassEvent(c, d);
                if (d.CreatedEntity is DescriptorConstructionExtension)
                    return new CellMenuConstructionExtension(c, d);
                if (d.CreatedEntity is DescriptorConstructionImprovement)
                    return new CellMenuConstructionImprovement(c, d);
                if (d.CreatedEntity is DescriptorConstructionService)
                    return new CellMenuConstructionService(c, d);
                if (d.CreatedEntity is DescriptorConstructionTournament)
                    return new CellMenuConstructionTournament(c, d);
                if (d.CreatedEntity is DescriptorConstruction)
                    return new CellMenuConstructionBuild(c, d);
                if (d.CreatedEntity is DescriptorCreature)
                    return new CellMenuConstructionRecruitCreature(c, d);

                throw new Exception($"Неизвестный тип сущности: {d.CreatedEntity.ID}.");
            }
            else
            {
                return new CellMenuConstructionAction(c, d);
            }
        }

        internal override void Click()
        {
            if (ExecutingAction != null)
            {
                if (!ExecutingAction.InQueue)
                {
                    if (CheckRequirements())
                    {
                        Program.formMain.PlayPushButton();

                        Construction.Player.AddToQueueBuilding(this);
                        //Construction.AddEntityToQueueProcessing(this);
                    }
                }
                else
                {
                    //if (DaysProcessed == 0)
                    {
                        Program.formMain.PlayPushButton();
                        Construction.Player.RemoveFromQueueBuilding(this, false);
                        //Construction.RemoveEntityFromQueueProcessing(this);
                    }
                }
            }
        }

        internal void AddToQueue()
        {
            Construction.AddCellMenuToQueue(this);
        }

        internal void RemoveFromQueue(bool forCancel)
        {
            Construction.RemoveCellMenuFromQueue(this, true, forCancel);
            // Если не было отмены, значит, идет процесс отработки прогресса и строительство завершено.
            // Перестраивать очередь не нужно
            if (forCancel)
                Construction.Player.RebuildQueueBuilding();
        }

        internal void UpdateDaysExecuted()
        {
            if (ExecutingAction != null)
            {

                if (ExecutingAction.InQueue)
                {
                    /*if (ExecutingAction.IsConstructionPoints)
                        ExecutingAction.RestDaysExecuting = Construction.CalcDaysForExecuting(ExecutingAction.NeedPoints, Construction.Player.ConstructionPoints);
                    else
                        ExecutingAction.RestDaysExecuting = Construction.CalcDaysForExecuting(ExecutingAction.NeedPoints, Construction.ResearchPoints);*/
                }
                else
                {
                    if (ExecutingAction.IsConstructionPoints)
                        ExecutingAction.RestDaysExecuting = Construction.CalcDaysForExecuting(ExecutingAction.Points, Construction.Player.ConstructionPoints, true);
                    else
                        ExecutingAction.RestDaysExecuting = Construction.CalcDaysForExecuting(ExecutingAction.Points, Construction.ResearchPoints, false);
                }

                //Assert(ExecutingAction.RestDaysExecuting > 0);
            }
        }
        internal virtual void StartExecute() { }// Вызывается перед началом выполнения действия

        internal virtual void DoProgressExecutingAction()
        {
            if (ExecutingAction.CurrentPoints > 0)
            {
                if (ExecutingAction.AppliedPoints == 0)
                    StartExecute();

                ExecutingAction.AppliedPoints += ExecutingAction.CurrentPoints;
                ExecutingAction.CurrentPoints = 0;
            }

            // Если прогресс завершен, выполняем действие
            Assert(ExecutingAction.NeedPoints >= 0);
            if (ExecutingAction.NeedPoints == 0)
            {
                Execute();
            }
        }
    }

    internal sealed class ComponentExecutingAction
    {
        public ComponentExecutingAction(DescriptorComponentCreating ddc)
        {
            if (ddc.ConstructionPoints > 0)
            {
                Points = ddc.ConstructionPoints;
                IsConstructionPoints = true;
            }
            else if (ddc.ResearchPoints > 0)
            {
                Points = ddc.ResearchPoints;
                IsConstructionPoints = false;
            }
            else
                DoException("Нет очков для действия");
        }

        public ComponentExecutingAction(int constructionPoints)
        {
            Assert(constructionPoints > 0);

            Points = constructionPoints;
            IsConstructionPoints = true;
        }

        internal bool IsConstructionPoints { get; }// Используются очки строительства иначе очки действий
        internal bool InQueue { get; set; }// Действие в очереди
        internal int Points { get; }// Сколько очков нужно для выполнения действия
        internal int AppliedPoints { get; set; }// Сколько очков было применено
        internal int CurrentPoints { get; set; }// Сколько очков будет примено на текущем ходу
        internal int NeedPoints { get => Points - AppliedPoints; }// Сколько очков осталось
        internal int RestDaysExecuting { get; set; }// Сколько дней осталось до конца выполнения действия
    }


    internal sealed class CellMenuConstructionResearch : CellMenuConstruction
    {
        public CellMenuConstructionResearch(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.GetCreating().CostResources.Gold > 0, $"У {d.CreatedEntity.ID} не указана цена.");

            Entity = d.CreatedEntity as DescriptorProduct;
        }

        internal DescriptorProduct Entity { get; }
        internal override void PrepareHint(PanelHint panelHint)
        {
            //string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            panelHint.AddStep2Descriptor(Entity);
            //panelHint.AddStep4Level(level);
            panelHint.AddStep5Description(Entity.SmallEntity.Description);
            //PanelHint.AddStep6Income(Descriptor.Income);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Исследовать", PurchaseValue);
        }


        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "и";

        protected override void Execute()
        {
            if (Entity.SmallEntity is DescriptorAbility da)
            {
                ConstructionAbility ca = new ConstructionAbility(Construction, Entity, da);
                Construction.AddAbility(ca);
                Construction.Player.AddNoticeForPlayer(ca, TypeNoticeForPlayer.Research);
            }
            else if (Entity.SmallEntity is DescriptorConstructionSpell ds)
            {
                ConstructionSpell cs;
                switch (ds.TypeEntity)
                {
                    case TypeEntity.Location:
                        cs = new ConstructionSpell(Construction, Entity, ds);
                        Construction.AddSpell(cs);
                        Construction.Player.AddNoticeForPlayer(cs, TypeNoticeForPlayer.Research);
                        break;
                    case TypeEntity.Construction:
                        cs = new ConstructionSpell(Construction, Entity, ds);
                        Construction.AddSpell(cs);
                        Construction.Player.AddNoticeForPlayer(cs, TypeNoticeForPlayer.Research);
                        break;
                    default:
                        DoException("Неизвестный тип сущности");
                        break;
                }
            }
            else
            {
                ConstructionProduct cp = new ConstructionProduct(Construction, Entity);
                Construction.AddProduct(cp);
                Construction.Player.AddNoticeForPlayer(cp, TypeNoticeForPlayer.Research);
            }

            RemoveSelf();
            Construction.Player.RemoveFromQueueBuilding(this, true);
            Program.formMain.SetNeedRedrawFrame();
        }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }
    }

    internal sealed class CellMenuConstructionService : CellMenuConstruction
    {
        public CellMenuConstructionService(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.GetCreating().CostResources.Gold > 0, $"У {d.CreatedEntity.ID} не указана цена.");

            Entity = d.CreatedEntity as DescriptorConstructionService;
            Debug.Assert(Entity != null);
        }

        internal DescriptorConstructionService Entity { get; }
        internal override void PrepareHint(PanelHint panelHint)
        {
            //string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            panelHint.AddStep2Descriptor(Entity);
            //panelHint.AddStep4Level(level);
            panelHint.AddStep5Description(Entity.Description);
            //panelHint.AddStep6Income(Descriptor.Income);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Исследовать", PurchaseValue);
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "и";

        protected override void Execute()
        {
            RemoveSelf();

            ConstructionService cs = new ConstructionService(Construction, Entity);
            Construction.AddService(cs);
            Construction.Player.AddNoticeForPlayer(cs, TypeNoticeForPlayer.Research);

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }
    }

    internal sealed class CellMenuConstructionBuild : CellMenuConstruction
    {
        public CellMenuConstructionBuild(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            TypeConstruction = d.CreatedEntity as DescriptorConstruction;

            /*if (TypeConstruction.Category == CategoryConstruction.Temple)
                ConstructionForBuild = c.Player.GetPlayerConstruction(TypeConstruction);
            else if (TypeConstruction.Category == CategoryConstruction.External)
            {
            }
            else
                throw new Exception("Неизвестная категория сооружения: " + TypeConstruction.ID);*/
        }

        private DescriptorConstruction TypeConstruction { get; set; }// Описатель строимого сооружения

        internal override bool CheckRequirements()
        {
            if (!base.CheckRequirements())
                return false;

            if (TypeConstruction is null)
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.GetCreating().Requirements);
            else
            {
                // Сначала проверяем наличие ресурсов
                if (!Construction.Player.BaseResources.ResourcesEnough(TypeConstruction.Levels[1].GetCreating().CostResources))
                    return false;

                // Проверяем требования к зданиям
                return Construction.Player.CheckRequirements(TypeConstruction.Levels[1].GetCreating().Requirements);
            }
        }

        protected override void Execute()
        {
            /*if (ConstructionForBuild != null)
            {
                Debug.Assert(ConstructionForBuild.Level == 0);
                ConstructionForBuild.Build(true);
                ConstructionForBuild.X = Construction.X;
                ConstructionForBuild.Y = Construction.Y;
                ConstructionForBuild.Location = Construction.Location;
                ConstructionForBuild.Location.Lairs[ConstructionForBuild.Y, ConstructionForBuild.X] = ConstructionForBuild;
            }
            else
            {*/

            Construction pc = new Construction(Construction.Player, TypeConstruction, 1, Construction.X, Construction.Y, Construction.Location, true, true, true, false, TypeNoticeForPlayer.Build, Construction.InitialQuantityBaseResources);
            Construction.Location.Lairs[Construction.Location.Lairs.IndexOf(Construction)] = pc;
            Construction.Destroy();
            if (!Construction.Player.Lobby.InPrepareTurn)
                Program.formMain.layerGame.SelectPlayerObject(pc);
            //}

            if (Construction.Player.GetTypePlayer() == TypePlayer.Human)
                Program.formMain.layerGame.UpdateNeighborhoods();

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = TypeConstruction.Levels[1].GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "с";

        internal override int GetImageIndex()
        {
            return TypeConstruction.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(TypeConstruction);
            //panelHint.AddStep4Level("Уровень 1");
            //panelHint.AddStep6Income(type.Levels[1].Income);
            panelHint.AddStep8Greatness(TypeConstruction.Levels[1].GreatnessByConstruction, TypeConstruction.Levels[1].GreatnessPerDay);
            panelHint.AddStep9PlusBuilders(TypeConstruction.Levels[1].AddConstructionPoints);
            //panelHint.AddStep10DaysBuilding(-1, TypeConstruction.Levels[1].GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(Construction.Player.GetTextRequirementsBuildTypeConstruction(TypeConstruction));
            panelHint.AddStep12CostExecuting("Построить", TypeConstruction.Levels[1].GetCreating().CostResources);
            panelHint.AddStep13Builders(TypeConstruction.Levels[1].GetCreating().CalcConstructionPoints(Construction.Player));
        }
    }

    internal sealed class CellMenuConstructionLevelUp : CellMenuConstruction
    {
        public CellMenuConstructionLevelUp(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Descriptor = d.CreatedEntity as DescriptorConstructionLevel;
        }

        internal new DescriptorConstructionLevel Descriptor { get; }

        // Реализация
        internal override bool CheckRequirements()
        {
            // При постройке храма из меню Святой земли, сюда прилетает 2 уровень
            if (Construction.Descriptor.MaxLevel < Descriptor.Number)
                return false;

            // Сначала проверяем наличие золота
            if (!Construction.Player.CheckRequiredResources(Descriptor.GetCreating().CostResources))
                return false;

            // Проверяем наличие очков строительства
            //if (!Player.CheckRequireBuilders(Descriptor.Levels[level].GetCreating().ConstructionPoints(Player)))
            //    return false;

            // Проверяем, что нет события или турнира
            if (Construction.CurrentMassEvent != null)
                return false;
            if (Construction.CurrentTournament != null)
                return false;

            // Проверяем требования к зданиям
            return Construction.Player.CheckRequirements(Descriptor.GetCreating().Requirements);

        }
        internal override int GetImageIndex() => Descriptor.ImageIndex;
        internal override bool GetImageIsEnabled() => ExecutingAction.InQueue && (Construction.Level + 1 == Descriptor.Number) || base.GetImageIsEnabled();
        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.GetCreating().CostResources;
        }

        protected override bool ConstructionMustMeConstructed() => false;
        protected override string GetTextForLevel() => Descriptor.Number == 1 ? "" : Descriptor.Number.ToString();

        internal override Color GetColorText()
        {
            if (GetImageIsEnabled())
            {
                if ((Construction.Level + 1 == Descriptor.Number))
                    return FormMain.Config.CommonCost;
                else
                    return Color.LimeGreen;
            }
            else
                return Color.Gray;
        }

        internal override void StartExecute()
        {
            if (Construction.Level > 0)
            {
                Assert(Construction.CurrentDurability == Construction.MaxDurability);
            }

            base.StartExecute();
        }

        protected override void Execute()
        {
            Construction.Build(true);
            Construction.Player.RemoveFromQueueBuilding(this, true);
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            if (Descriptor.Number > Construction.Descriptor.MaxLevel)
                return;// Убрать это

            panelHint.AddStep2Entity(Construction);
            panelHint.AddStep4Level(Descriptor.Number == 1 ? "Уровень 1" : $"Улучшить строение ({Descriptor.Number} ур.)");
            panelHint.AddStep5Description(Descriptor.Number == 1 ? Descriptor.Description : "");
            panelHint.AddStep6Income(Construction.IncomeForLevel(Descriptor.Number));
            panelHint.AddStep8Greatness(Construction.GreatnesAddForLevel(Descriptor.Number), Construction.GreatnesPerDayForLevel(Descriptor.Number));
            panelHint.AddStep9PlusBuilders(Descriptor.AddConstructionPoints);
            if (Descriptor.DescriptorVisit != null)
            {
                panelHint.AddStep9Interest(Descriptor.DescriptorVisit.Interest, false);
                panelHint.AddStep9ListNeeds(Descriptor.DescriptorVisit.ListNeeds, false);
            }
            panelHint.AddStep12CostExecuting(Descriptor.Number == 1 ? "Построить" : $"Улучшить до {Descriptor.Number} ур.", Descriptor.GetCreating().CostResources, Descriptor.GetCreating().CalcConstructionPoints(Construction.Player), ExecutingAction.RestDaysExecuting, GetTextRequirements());
            //panelHint.AddStep12Gold(Player.BaseResources, Descriptor.Levels[requiredLevel].GetCreating().CostResources);
            //panelHint.AddStep13Builders(Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player), Player.RestConstructionPoints >= Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player));
        }

        protected override void UpdateTextRequirements(List<TextRequirement> list)
        {
            base.UpdateTextRequirements(list);

            Construction.Player.TextRequirements(Descriptor.GetCreating().Requirements, list);

            if (Construction.CurrentMassEvent != null)
                list.Add(new TextRequirement(false, "В сооружении идет мероприятие"));

            if (Construction.CurrentTournament != null)
                list.Add(new TextRequirement(false, "В сооружении идет турнир"));
        }


        internal override void DoProgressExecutingAction()
        {
            //Assert(Construction.MaxDurability > 0);

            if (Descriptor.Number == 1)
            {
                Construction.CurrentDurability += ExecutingAction.CurrentPoints;
            }

            base.DoProgressExecutingAction();
        }

        internal override void InQueueChanged()
        {
            base.InQueueChanged();

            if (Descriptor.Number == 1)
                Construction.UpdateMaxDurability();
        }
    }

    internal sealed class CellMenuConstructionRepair : CellMenuConstruction
    {
        public CellMenuConstructionRepair(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            ExecutingAction = new ComponentExecutingAction(c.MaxDurability - c.CurrentDurability);
        }

        internal int DaysForRepair { get; set; }// Дней на завершение ремонта

        protected override int GetDaysExecuting()
        {
            if (ExecutingAction != null)
            {
                //Assert(ExecutingAction.RestDaysExecuting > 0);

                return ExecutingAction.RestDaysExecuting;
                /*if (Construction.DaysConstructLeft == 0)
                    return "";
                else
                    return Construction.DaysConstructLeft.ToString();*/
            }
            else
                return DaysForRepair;
        }

        protected override void Execute()
        {
            Construction.Player.RemoveFromQueueBuilding(this, true);
            Construction.InRepair = false;
            Construction.Player.AddNoticeForPlayer(Construction, TypeNoticeForPlayer.ConstructionRepaired);
            Construction.UpdateState();
        }

        protected override bool ConstructionMustMeConstructed() => true;

        internal override bool CheckRequirements() => true;

        internal override bool GetImageIsEnabled()
        {
            return base.GetImageIsEnabled();
        }

        internal override string GetText()
        {
            if (Construction.State == StateConstruction.Repair)
                return "Отм.";
            else
                return base.GetText();
        }

        internal override void UpdatePurchase()
        {
            int expenseCP = Math.Min(Construction.Player.Gold, Math.Min(Construction.Player.RestConstructionPoints, Construction.MaxDurability - Construction.CurrentDurability));
            PurchaseValue = Construction.CompCostRepair(expenseCP);
            // Если цены ремонта нет, значит, оно не в очереди. Пытаемся подсчитать, сколько это будет стоить
            /*if (PurchaseValue is null)
            {
                int expenseCP = Math.Min(Construction.Player.Gold, Math.Min(Construction.Player.RestConstructionPoints, Construction.MaxDurability - Construction.CurrentDurability));
                PurchaseValue = Construction.CompCostRepair(expenseCP);

                return PurchaseValue;
            }
            else
            {
                return PurchaseValue;
            }*/
        }

        internal override int GetImageIndex() => Config.Gui48_Build;
        
        protected override string GetTextForLevel() => "";

        internal override Color GetColorText()
        {
            if (GetImageIsEnabled())
            {
                if (Construction.InRepair)
                    return FormMain.Config.CommonCost;
                else
                    return Color.LimeGreen;
            }
            else
                return Color.Gray;
        }

        internal override void Click()
        {
            if (Construction.State == StateConstruction.NeedRepair)
                Construction.StartRepair();
            else if (Construction.State == StateConstruction.Repair)
                Construction.CancelRepair();
            else
                DoException($"Неправильное состояние: {Construction.State}");
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            //Construction.PrepareHintForBuildOrUpgrade(panelHint, Descriptor.Number);
        }
    }

    internal sealed class CellMenuConstructionRecruitCreature : CellMenuConstruction
    {
        public CellMenuConstructionRecruitCreature(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Creature = d.CreatedEntity as DescriptorCreature;
        }

        internal DescriptorCreature Creature { get; private set; }

        protected override void Execute()
        {
            Creature h = Construction.HireHero(Creature, PurchaseValue);

            Construction.Player.AddNoticeForPlayer(h, TypeNoticeForPlayer.HireHero);
        }

        internal override bool CheckRequirements()
        {
            return base.CheckRequirements() && Construction.AllowHire();
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }
        
        protected override string GetTextForLevel() => "р";

        internal override int GetImageIndex()
        {
            return Creature.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            /*panelHint.AddStep2Header(TypeConstruction.TrainedHero.Name);
            panelHint.AddStep5Description(TypeConstruction.TrainedHero.Description);
            if ((TypeConstruction.TrainedHero != null) && (TypeConstruction.TrainedHero.Cost > 0))
                panelHint.AddStep11Requirement(GetTextRequirementsHire());
            panelHint.AddStep12Gold(TypeConstruction.TrainedHero.Cost, Player.Gold >= TypeConstruction.TrainedHero.Cost);
            */
            panelHint.AddStep2Descriptor(Creature);
            panelHint.AddStep5Description(Creature.Description);
            panelHint.AddStep75Salary(Creature.CostOfHiring);
            //panelHint.AddStep10DaysBuilding(InQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(Construction.GetTextRequirementsHire());
            panelHint.AddStep12CostExecuting("Рекрутировать", PurchaseValue);
        }
    }

    internal sealed class CellMenuConstructionMassEvent : CellMenuConstruction
    {
        private ConstructionEvent cp;

        public CellMenuConstructionMassEvent(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            ConstructionEvent = d.CreatedEntity as DescriptorConstructionMassEvent;
            Debug.Assert(ConstructionEvent != null);
        }

        internal DescriptorConstructionMassEvent ConstructionEvent { get; }
        internal int Cooldown { get; private set; }

        protected override void Execute()
        {
            Debug.Assert(Construction.Actions.IndexOf(this) != -1);
            Debug.Assert(cp is null);

            cp = new ConstructionEvent(Construction, ConstructionEvent);
            Construction.AddMassEvent(cp);

            Construction.Player.AddNoticeForPlayer(cp, TypeNoticeForPlayer.MassEventBegin);
            //Cooldown = ConstructionEvent.Cooldown;
        }

        internal override bool CheckRequirements()
        {
            return (cp is null) && (Cooldown == 0) && base.CheckRequirements() && (Construction.CurrentVisit.DescriptorConstructionVisit != null);
        }

        protected override void UpdateTextRequirements(List<TextRequirement> list)
        {
            base.UpdateTextRequirements(list);

            Debug.Assert(!((cp != null) && (Cooldown > 0)));

            if (Construction.Level > 1)
                list.Add(new TextRequirement((cp is null) && (Cooldown == 0) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null), (cp is null) && (Cooldown == 0) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null)
                    ? "Событие можно проводить" : Construction.CurrentVisit?.DescriptorConstructionVisit == null ? "В сооружении уже идет другое событие" : cp != null ? $"Событие будет идти еще {cp.Counter} дн." : $"Осталось подождать дней: {Cooldown}"));
        }

        internal override string GetText()
        {
            return (cp is null) && (Cooldown == 0) ? PurchaseValue.Gold.ToString() : cp != null ? "идёт" : Cooldown.ToString() + " дн.";
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "м";

        internal override int GetImageIndex()
        {
            return ConstructionEvent.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(ConstructionEvent);
            panelHint.AddStep4Level($"Длительность: {ConstructionEvent.Duration} дн."
                + Environment.NewLine + $"Перерыв: {ConstructionEvent.Cooldown} дн.");
            panelHint.AddStep5Description(ConstructionEvent.Description);
            panelHint.AddStep9Interest(ConstructionEvent.Interest, false);
            panelHint.AddStep9ListNeeds(ConstructionEvent.ListNeeds, false);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Подготовить мероприятие", PurchaseValue);
        }

        internal override void PrepareNewDay()
        {
            base.PrepareNewDay();

            if (cp?.Counter == 0)
            {
                Construction.Player.AddNoticeForPlayer(cp, TypeNoticeForPlayer.MassEventEnd);

                cp = null;
                Cooldown = ConstructionEvent.Cooldown;
            }
            else if (Cooldown > 0)
            {
                Cooldown--;
            }
        }
    }
    
    internal sealed class CellMenuConstructionExtension : CellMenuConstruction
    {
        public CellMenuConstructionExtension(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Entity = d.CreatedEntity as DescriptorConstructionExtension;
        }

        internal DescriptorConstructionExtension Entity { get; }

        protected override void Execute()
        {
            RemoveSelf();

            ConstructionExtension ce = new ConstructionExtension(Construction, Entity);
            Construction.AddExtension(ce);

            Program.formMain.SetNeedRedrawFrame();

            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Extension);
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "д";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(Entity);
            panelHint.AddStep5Description(Entity.Description);
            //panelHint.AddStep6Income(Descriptor.Income);
            panelHint.AddStep9Interest(Entity.ModifyInterest, true);
            panelHint.AddStep9ListNeeds(Entity.ListNeeds, true);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Построить", PurchaseValue);
            panelHint.AddStep13Builders(Entity.Durability);
        }
    }

    internal sealed class CellMenuConstructionImprovement : CellMenuConstruction
    {
        public CellMenuConstructionImprovement(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Entity = d.CreatedEntity as DescriptorConstructionImprovement;
        }

        internal DescriptorConstructionImprovement Entity { get; }

        protected override void Execute()
        {
            RemoveSelf();

            ConstructionImprovement ce = new ConstructionImprovement(Construction, Entity);
            Construction.AddImprovement(ce);

            Program.formMain.SetNeedRedrawFrame();

            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Improvement);
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "у";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(Entity);
            panelHint.AddStep5Description(Entity.Description);
            //CreatedEntity.Creating.panelHint.AddStep6Income(Descriptor.Income);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Построить улучшение", PurchaseValue);
        }
    }

    internal sealed class CellMenuConstructionTournament : CellMenuConstruction
    {
        private ConstructionTournament ct;

        public CellMenuConstructionTournament(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            ConstructionTournament = d.CreatedEntity as DescriptorConstructionTournament;
            Debug.Assert(ConstructionTournament != null);
        }

        internal DescriptorConstructionTournament ConstructionTournament { get; }

        protected override void Execute()
        {
            Debug.Assert(Construction.Actions.IndexOf(this) != -1);
            Debug.Assert(ct is null);

            ct = new ConstructionTournament(Construction, ConstructionTournament);
            Construction.AddTournament(ct);

            Construction.Player.AddNoticeForPlayer(ct, TypeNoticeForPlayer.TournamentBegin);
            //Cooldown = ConstructionEvent.Cooldown;
        }

        internal override bool CheckRequirements()
        {
            return (ct is null) && base.CheckRequirements() && (Construction.CurrentVisit.DescriptorConstructionVisit != null);
        }

        protected override void UpdateTextRequirements(List<TextRequirement> list)
        {
            base.UpdateTextRequirements(list);

            if (Construction.Level > 1)
                list.Add(new TextRequirement((ct is null) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null), (ct is null) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null)
                    ? "Турнир можно проводить" : Construction.CurrentVisit?.DescriptorConstructionVisit == null ? "В сооружении уже идет другое событие" : $"Осталось подождать дней: {1}"));
        }

        internal override string GetText()
        {
            return ct is null ? PurchaseValue.Gold.ToString() : "идёт";
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        protected override string GetTextForLevel() => "т";

        internal override int GetImageIndex()
        {
            return ConstructionTournament.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(ConstructionTournament);
            panelHint.AddStep4Level(Environment.NewLine + $"Перерыв: {ConstructionTournament.Cooldown} дн.");
            panelHint.AddStep5Description(ConstructionTournament.Description);
            panelHint.AddStep9Interest(ConstructionTournament.Interest, false);
            panelHint.AddStep9ListNeeds(ConstructionTournament.ListNeeds, false);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Подготовить турнир", PurchaseValue);
        }
    }

    internal enum TypeExtra { Builder, LevelUp, Research };

    internal sealed class CellMenuConstructionExtra : CellMenuConstruction
    {
        public CellMenuConstructionExtra(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            TypeExtra = (TypeExtra)Enum.Parse(typeof(TypeExtra), d.IDCreatedEntity);
        }

        internal int Counter { get; set; }
        internal TypeExtra TypeExtra { get; }

        internal override bool CheckRequirements()
        {
            return (Counter == 0) && base.CheckRequirements();
        }

        protected override void UpdateTextRequirements(List<TextRequirement> list)
        {
            base.UpdateTextRequirements(list);
            list.Add(new TextRequirement(Counter == 0, Counter == 0 ? "Покупка доступна" : "Дней до новой покупки: " + Counter.ToString()));
        }

        protected override void Execute()
        {
            Debug.Assert(CheckRequirements());

            Construction.Player.SpendResource(PurchaseValue);

            switch (TypeExtra)
            {
                case TypeExtra.Builder:
                    Construction.Player.AddFreeBuilder();
                    break;
                case TypeExtra.LevelUp:
                    Construction.Player.AddExtraLevelUp();
                    break;
                case TypeExtra.Research:
                    Construction.Player.AddExtraResearch();
                    break;
                default:
                    throw new Exception($"Неизвестный тип бонуса: {TypeExtra}.");
            }

            Counter = Descriptor.DaysCooldown;

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetText()
        {
            return Counter == 0 ? PurchaseValue.Gold.ToString() : Counter.ToString() + " д.";
        }

        protected override string GetTextForLevel() => "+1";

        internal override int GetImageIndex()
        {
            switch (TypeExtra)
            {
                case TypeExtra.Builder:
                    return Config.Gui48_Build;
                case TypeExtra.LevelUp:
                    return Config.Gui48_LevelUp;
                case TypeExtra.Research:
                    return Config.Gui48_Goods;
                default:
                    throw new Exception($"Неизвестный тип бонуса: {TypeExtra}.");
            }
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            switch (TypeExtra)
            {
                case TypeExtra.Builder:
                    panelHint.AddStep2Header("+1 Строитель");
                    panelHint.AddStep5Description("Добавляет 1 строителя на текущий ход" + Environment.NewLine + "Пауза: " + Descriptor.DaysCooldown.ToString() + " дн.");
                    break;
                case TypeExtra.LevelUp:
                    panelHint.AddStep2Header("+1 улучшение сооружение");
                    panelHint.AddStep5Description("Добавляет 1 внеплановое улучшение на текущий ход" + Environment.NewLine + "Пауза: " + Descriptor.DaysCooldown.ToString() + " дн.");
                    break;
                case TypeExtra.Research:
                    panelHint.AddStep2Header("+1 исследование");
                    panelHint.AddStep5Description("Добавляет 1 внеплановое исследование на текущий ход" + Environment.NewLine + "Пауза: " + Descriptor.DaysCooldown.ToString() + " дн.");
                    break;
                default:
                    throw new Exception($"Неизвестный тип бонуса: {TypeExtra}.");
            }

            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12CostExecuting("Добавить бонус", PurchaseValue);
        }

        internal override void PrepareNewDay()
        {
            base.PrepareNewDay();

            if (Counter > 0)
                Counter--;
        }
    }

    internal sealed class CellMenuConstructionAction : CellMenuConstruction
    {
        public CellMenuConstructionAction(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
        }

        internal override int GetImageIndex()
        {
            return 1;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
        }

        protected override void Execute()
        {
            
        }
    }

    internal sealed class CellMenuConstructionSpell : CellMenuConstruction
    {
        public CellMenuConstructionSpell(Construction forConstruction, ConstructionSpell spell) : base(forConstruction, new DescriptorActionForEntity(spell.DescriptorSpell.Coord))
        {
            ForConstruction = forConstruction;
            Spell = spell;
            Entity = spell.DescriptorSpell;
        }

        internal Construction ForConstruction { get; }
        internal ConstructionSpell Spell { get; }
        internal DescriptorConstructionSpell Entity { get; }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(Entity);
            panelHint.AddStep4Level($"Осталось: {Spell.Selling.RestQuantity}");
            panelHint.AddStep5Description(Entity.Description);
            panelHint.AddStep12CostExecuting("Применить заклинание", PurchaseValue);
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = new ListBaseResources(Entity.Selling.Gold);
        }

        protected override void Execute()
        {
            switch (Entity.Action)
            {
                default:
                    DoException($"Неизвестное действие: {Entity.Action}");
                    break;
            }

            Construction.Player.Lobby.Layer.UpdateMenu();
        }

        internal override bool CheckRequirements() => (Spell.Selling.RestQuantity > 0) && (base.CheckRequirements());

        internal override void PrepareNewDay()
        {
            base.PrepareNewDay();

            Spell.Selling.Reset();
        }
    }

    sealed internal class CellMenuConstructionRecruitToGuild : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuConstructionRecruitToGuild(Location l, DescriptorActionForEntity d) : base(l, d)
        {
        }

        internal override string GetLevel() => "\u2026";// Троеточие
        internal override string GetText() => Location.ComponentObjectOfMap.ListHeroesForFlag.Count > 0 ? Location.ComponentObjectOfMap.ListHeroesForFlag.Count.ToString() : "";

        internal override void Click()
        {
            Location.StateMenu = 1;
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 184;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddSimpleHint("Информация о задании разведки");
        }
    }
}