using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Security.Policy;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class ActionInConstruction : ActionForEntity
    {
        public ActionInConstruction(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Construction = c;

            if (Descriptor.CreatedEntity != null)
            {
                Creating = Descriptor.CreatedEntity.ComponentCreating;

                if (Creating != null)
                    if (Creating.Time > 0)

                ProgressExecuting = new ComponentProgressExecuting(Creating.Time, Creating.Builders, Construction.Player.GetMilliTicksForAction());
            }
        }

        internal Construction Construction { get; }
        internal DescriptorComponentCreating Creating { get; }
        internal ComponentProgressExecuting ProgressExecuting { get; private protected set; }

        protected virtual void BeforeAddToQueue() { }
        internal override string GetText() => (ProgressExecuting != null) && ProgressExecuting.InQueue ? "" : PurchaseValue != null ? PurchaseValue.Gold.ToString() : "";

        internal override bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Construction.Descriptor.IsInternalConstruction)
                if (ConstructionMustMeConstructed())
                    if (Construction.Level == 0)
                        return false;

            // Проверяем, не заполнена ли очередь
            if ((ProgressExecuting != null) && !ProgressExecuting.InQueue)
                if (Construction.QueueExecuting.Count >= Config.MaxLengthQueue)
                    return false;

            // Потом проверяем наличие требуемых ресурсов
            if (PurchaseValue != null)
                if (!Construction.Player.BaseResources.ResourcesEnough(PurchaseValue))
                    return false;

            if (Descriptor.CreatedEntity != null)
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.ComponentCreating.Requirements);

            return true;
        }

        internal sealed override string GetLevel()
        {
            return Program.formMain.Settings.ShowTypeCellMenu ? GetTextForLevel() : "";
        }

        protected override int GetTimeExecuting()
        {
            if (Program.formMain.Settings.ShowTimeForExecuting && (ProgressExecuting != null))
                return ProgressExecuting.RestTimeExecuting;
            
            return base.GetTimeExecuting();
        }

        protected virtual string GetTextForLevel() => "";
        protected abstract void Execute();

        protected void RemoveSelf(bool withDestroy)
        {
            Debug.Assert(Construction.Actions.IndexOf(this) != -1);
            if (withDestroy)
            {
                Destroyed = true;
                Construction.Actions.Remove(this);
            }
            Construction.Player.Lobby.Layer.UpdateMenu();
        }

        protected virtual bool ConstructionMustMeConstructed() => true;

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);

            if (Construction.Descriptor.IsInternalConstruction && ConstructionMustMeConstructed())
            {
                // Если нет требований, то по умолчанию остается только одно - сооружение должно быть построено
                // Если есть, то не надо писать, что сооружение не построено - оно будет прописано в условии
                if (Construction.Level == 0)
                {
                    if ((Descriptor.CreatedEntity.ComponentCreating.Requirements != null) && (Descriptor.CreatedEntity.ComponentCreating.Requirements.RequirementOurConstruction is null))
                        list.Add((false, "Построить сооружение"));
                }

                Construction.Player.TextRequirements(Descriptor.CreatedEntity.ComponentCreating.Requirements, list, Construction);
            }

            // Проверяем, не заполнена ли очередь
            if ((ProgressExecuting != null) && !ProgressExecuting.InQueue)
                if (Construction.QueueExecuting.Count >= Config.MaxLengthQueue)
                    list.Add((false, "Очередь заполнена"));
        }

        internal static ActionInConstruction Create(Construction c, DescriptorActionForEntity d)
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

        internal override StateRestTime GetStateRestTime()
        {
            if ((ProgressExecuting.State == StateProgress.Inactive) || (ProgressExecuting.State == StateProgress.Active))
                return StateRestTime.Active;

            return StateRestTime.Pause;
        }

        internal override void Click()
        {
            if (ProgressExecuting != null)
            {
                if (!ProgressExecuting.InQueue)
                {
                    if (CheckRequirements())
                    {
                        Construction.Player.SpendResource(PurchaseValue);
                        Program.formMain.PlayPushButton();
                        BeforeAddToQueue();
                        Construction.Player.AddActionToQueue(ActionForAddToQueue());
                        Construction.Player.Lobby.Layer.UpdateMenu();
                    }
                }
                else
                {
                    if (ProgressExecuting.PassedMilliTicks == 0)
                    {
                        Program.formMain.PlayPushButton();
                        Construction.Player.RemoveFromQueueExecuting(this, false);
                    }
                }
            }
        }

        protected virtual ActionInConstruction ActionForAddToQueue() => this;

        internal virtual void StartProgress() { }// Вызывается перед началом выполнения действия

        internal virtual void DoTick()
        {
            if ((ProgressExecuting != null) && ProgressExecuting.State == StateProgress.Active)
            {
                if (ProgressExecuting.PassedMilliTicks == 0)
                    StartProgress();

                ProgressExecuting.CalcTick(Construction.Player.GetMilliTicksForAction());

                // Если прогресс завершен, выполняем действие
                if (ProgressExecuting.RestMilliTicks == 0)
                {
                    Execute();
                }
            }
        }

        internal void UpdateTime()
        {
            ProgressExecuting?.UpdateRestTimeExecuting(Construction.Player.GetMilliTicksForAction());
        }
    }

    internal sealed class CellMenuConstructionResearch : ActionInConstruction
    {
        public CellMenuConstructionResearch(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.ComponentCreating.CostResources.Gold > 0, $"У {d.CreatedEntity.ID} не указана цена.");

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
            panelHint.AddStep12CostExecuting("Исследовать", PurchaseValue, ProgressExecuting.RestTimeExecuting, 0, GetTextRequirements());
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Research);
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

            RemoveSelf(true);
            Construction.Player.RemoveFromQueueExecuting(this, true);
        }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }
    }

    internal sealed class CellMenuConstructionService : ActionInConstruction
    {
        public CellMenuConstructionService(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.ComponentCreating.CostResources.Gold > 0, $"У {d.CreatedEntity.ID} не указана цена.");

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
            panelHint.AddStep12CostExecuting("Исследовать", PurchaseValue, ProgressExecuting.RestTimeExecuting, 0, GetTextRequirements());
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Research);
        }

        protected override string GetTextForLevel() => "и";

        protected override void Execute()
        {
            RemoveSelf(true);

            ConstructionService cs = new ConstructionService(Construction, Entity);
            Construction.AddService(cs);
            Construction.Player.AddNoticeForPlayer(cs, TypeNoticeForPlayer.Research);
        }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }
    }

    internal sealed class CellMenuConstructionBuild : ActionInConstruction
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
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.ComponentCreating.Requirements);
            else
            {
                // Сначала проверяем наличие ресурсов
                if (!Construction.Player.BaseResources.ResourcesEnough(TypeConstruction.Levels[1].ComponentCreating.CostResources))
                    return false;

                // Проверяем требования к зданиям
                return Construction.Player.CheckRequirements(TypeConstruction.Levels[1].ComponentCreating.Requirements);
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
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(TypeConstruction.Levels[1].ComponentCreating.CostResources, PurchaseValue, TypeCreating.Building);
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
            //panelHint.AddStep10DaysBuilding(-1, );
            panelHint.AddStep12CostExecuting("Построить", TypeConstruction.Levels[1].ComponentCreating.CostResources, TypeConstruction.Levels[1].ComponentCreating.Time, TypeConstruction.Levels[1].ComponentCreating.Builders, Construction.Player.GetTextRequirementsBuildTypeConstruction(TypeConstruction));
        }
    }

    internal sealed class CellMenuConstructionLevelUp : ActionInConstruction
    {
        private int milliTicksForOneDurability;// Сколько миллитиков необходимо для увеличения прочности на 1 единицу
        private int elapsedMilliTicks;// Сколько миллитиков прошло с последнего увеличения прочности

        public CellMenuConstructionLevelUp(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Descriptor = d.CreatedEntity as DescriptorConstructionLevel;

            milliTicksForOneDurability = Descriptor.ComponentCreating.Time * FormMain.Config.TicksInSecond * 1000 / Descriptor.IncreaseDurability;
        }

        internal new DescriptorConstructionLevel Descriptor { get; }

        // Реализация
        internal override bool CheckRequirements()
        {
            // При постройке храма из меню Святой земли, сюда прилетает 2 уровень
            if (Construction.Descriptor.MaxLevel < Descriptor.Number)
                return false;

            // Сначала проверяем наличие золота
            if (!Construction.Player.BaseResources.ResourcesEnough(PurchaseValue))
                return false;

            // Проверяем наличие очков строительства
            if (Construction.Player.MaxBuilders < Descriptor.ComponentCreating.Builders)
                return false;

            // Проверяем, что нет события или турнира
            if (Construction.CurrentMassEvent != null)
                return false;
            if (Construction.CurrentTournament != null)
                return false;

            // Проверяем требования к зданиям
            return Construction.Player.CheckRequirements(Descriptor.ComponentCreating.Requirements);

        }
        internal override int GetImageIndex() => Descriptor.ImageIndex;
        internal override bool GetImageIsEnabled() => ProgressExecuting.InQueue && (Construction.Level + 1 == Descriptor.Number) || base.GetImageIsEnabled();
        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Building);
        }

        protected override bool ConstructionMustMeConstructed() => false;
        protected override string GetTextForLevel() => Descriptor.Number == 1 ? "" : Descriptor.Number.ToString();

        internal override Color GetColorText()
        {
            if (GetImageIsEnabled())
            {
                if (Construction.Level + 1 == Descriptor.Number)
                    return FormMain.Config.CommonCost;
                else
                    return Color.LimeGreen;
            }
            else
                return Color.Gray;
        }

        internal override void StartProgress()
        {
            /*if (Construction.Level > 0)
            {
                Assert(Construction.CurrentDurability == Construction.MaxDurability);
            }*/

            base.StartProgress();
        }

        protected override void Execute()
        {
            Assert(Construction.CurrentDurability == Construction.MaxDurability);
            Construction.Build(true, false);
            Construction.Player.RemoveFromQueueExecuting(this, true);
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            if (Descriptor.Number > Construction.Descriptor.MaxLevel)
                return;// Убрать это

            //panelHint.AddStep2Entity(Construction);
            string nameCurrent = Descriptor.Number > 1 ? Construction.GetNameForLevel(Descriptor.Number - 1) : Construction.GetNameForLevel(Descriptor.Number);
            panelHint.ShowEntity(nameCurrent, Construction.GetTypeEntity(), Construction.GetImageIndex(), Construction.ProperName());
            //panelHint.AddStep4Level(Descriptor.Number == 1 ? "Уровень 1" : $"Улучшить строение ({Descriptor.Number} ур.)");
            panelHint.AddStep5Description(Descriptor.Number == 1 ? Descriptor.ActiveEntity.Description : "");
            panelHint.AddStep55Durability(Construction.DurabilityForLevel(Descriptor.Number));
            panelHint.AddStep6Income(Construction.IncomeForLevel(Descriptor.Number));
            panelHint.AddStep8Greatness(Construction.GreatnesAddForLevel(Descriptor.Number), Construction.GreatnesPerDayForLevel(Descriptor.Number));
            panelHint.AddStep9CityParameters(Descriptor.ChangeCityParametersPerTurn);
            panelHint.AddStep9PlusBuilders(Descriptor.AddConstructionPoints);
            if (Descriptor.DescriptorVisit != null)
            {
                panelHint.AddStep9Interest(Descriptor.DescriptorVisit.Interest, false);
                panelHint.AddStep9ListNeeds(Descriptor.DescriptorVisit.ListNeeds, false);
            }
            string nameNextLevel = Descriptor.NewName ? $"Улучшить до {Descriptor.Name} ({Descriptor.Number} ур.)" : $"Улучшить до {Descriptor.Number} ур.";
            string nameExecuting = "";
            if (!ProgressExecuting.InQueue)
                nameExecuting = Descriptor.Number == 1 ? "Построить" : nameNextLevel;
            else
            {
                if (ProgressExecuting.State == StateProgress.Active)
                    nameExecuting = (Descriptor.Number == 1 ? "Строится" : "Улучшается") + (ProgressExecuting.UsedBuilders > 0 ? $" ({ProgressExecuting.UsedBuilders} строит.)" : "");
                else if (ProgressExecuting.State == StateProgress.WaitBuilders)
                    nameExecuting = "Ожидание строителей" + $" ({ProgressExecuting.NeedBuilders} строит.)";
                else if (ProgressExecuting.State == StateProgress.WaitInQueue)
                    nameExecuting = "Ожидание очереди";
                else
                    DoException($"Неизвестное состояние {ProgressExecuting.State}");
            }

            if (ProgressExecuting.InQueue)
                panelHint.AddStep12CostExecuting(nameExecuting, null);
            else
                panelHint.AddStep12CostExecuting(nameExecuting, Descriptor.ComponentCreating.CostResources, ProgressExecuting.RestTimeExecuting, Descriptor.ComponentCreating.Builders, GetTextRequirements());
            //panelHint.AddStep12Gold(Player.BaseResources, Descriptor.Levels[requiredLevel].GetCreating().CostResources);
            //panelHint.AddStep13Builders(Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player), Player.RestConstructionPoints >= Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player));
        }

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);

            Construction.Player.TextRequirements(Descriptor.ComponentCreating.Requirements, list, Construction);

            if (Construction.CurrentMassEvent != null)
                list.Add((false, "В сооружении идет мероприятие"));

            if (Construction.CurrentTournament != null)
                list.Add((false, "В сооружении идет турнир"));
        }


        internal override void DoTick()
        {
            if ((ProgressExecuting != null) && (ProgressExecuting.State == StateProgress.Active))
            {
                Assert(ProgressExecuting.InQueue);

                if (ProgressExecuting.PassedMilliTicks == 0)
                {
                    Construction.InLevelUp = true;
                    Construction.UpdateMaxDurability();
                }

                elapsedMilliTicks += Construction.Player.GetMilliTicksForAction();
                if (elapsedMilliTicks >= milliTicksForOneDurability)
                {
                    int incDurability = elapsedMilliTicks / milliTicksForOneDurability;
                    Construction.CurrentDurability += incDurability;
                    elapsedMilliTicks -= milliTicksForOneDurability * incDurability;
                }
            }

            base.DoTick();
        }

        protected override void BeforeAddToQueue()
        {
            base.BeforeAddToQueue();

            if (Descriptor.Number == 1)
            {
                Assert(Construction.QueueExecuting.Count == 0);// Постройка - всегда первая
                Assert(Construction.CurrentDurability == 0);
            }
            else
            {
                Assert(Construction.MaxDurability > 0);
            }

            Assert((Construction.State == StateConstruction.Work) || (Construction.State == StateConstruction.NotBuild) || (Construction.State == StateConstruction.InQueueBuild)
              || (Construction.State == StateConstruction.NeedRepair));

            if (Construction.State == StateConstruction.NeedRepair)
            {
                Assert(Construction.TurnLevelConstructed[Construction.Level] != -1);
            }
            else
            {
                Assert(Construction.TurnLevelConstructed[Construction.Level + 1] == -1);
            }

            if (Construction.State == StateConstruction.NeedRepair)
                Construction.InRepair = true;
        }    
    }

    internal sealed class CellMenuConstructionRepair : ActionInConstruction
    {
        private ListBaseResources cost = new ListBaseResources();
        int elapsedMilliTicks;// Сколько миллитиков прошло с последнего увеличения прочности

        public CellMenuConstructionRepair(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            ProgressExecuting = new ComponentProgressExecuting(c.MaxDurability - c.CurrentDurability, 1, Construction.Player.GetMilliTicksForAction());
        }

        internal override void DoTick()
        {
            Assert(Construction.CurrentDurability < Construction.MaxDurability);

            // Пересчитывать параметры ремонта надо каждый тик, т.к. между прибавлениями единиц прочности могут нанести повреждение, а также может поменяться скорость ремонта
            // Сколько миллитиков необходимо для увеличения прочности на 1 единицу. Для простоты берем прочность и время стройки первого уровня
            // !!! Бонусы и штрафы не учитываются !!!
            int milliTicksForOneDurability = Construction.Descriptor.Levels[1].ComponentCreating.Time * FormMain.Config.TicksInSecond * 1000 / Construction.Descriptor.Levels[1].Durability;
            int milliTicksForRepair = (Construction.MaxDurability - Construction.CurrentDurability) * milliTicksForOneDurability;
            int seconds = (int)Math.Truncate(milliTicksForRepair * 1.000 / FormMain.Config.TicksInSecond / 1000 + 0.99);
            ProgressExecuting.RefreshProgress(seconds, Construction.Player.GetMilliTicksForAction());

            if (ProgressExecuting.State == StateProgress.Active)
            {
                Assert(ProgressExecuting.InQueue);

                elapsedMilliTicks += Construction.Player.GetMilliTicksForAction();
                if (elapsedMilliTicks >= milliTicksForOneDurability)
                {
                    int incDurability = elapsedMilliTicks / milliTicksForOneDurability;
                    Construction.CurrentDurability += incDurability;
                    elapsedMilliTicks -= milliTicksForOneDurability * incDurability;

                    if (Construction.CurrentDurability == Construction.MaxDurability)
                    {
                        elapsedMilliTicks = 0;
                        ProgressExecuting.RefreshProgress(0, Construction.Player.GetMilliTicksForAction());
                    }
                }
            }
            else
                elapsedMilliTicks = 0;// Если перестали ремонтировать, обнуляем остатки миллитиков

            base.DoTick();
        }

        protected override void Execute()
        {
            Assert(Construction.CurrentDurability == Construction.MaxDurability);

            Construction.Player.RemoveFromQueueExecuting(this, true);
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

        internal override void UpdatePurchase()
        {
            //Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, cost, TypeCreating.Research);
            //PurchaseValue = cost;

            //int expenseCP = Math.Min(Construction.Player.Gold, Math.Min(Construction.Player.RestConstructionPoints, Construction.MaxDurability.Value - Construction.CurrentDurability.Value));
            //PurchaseValue = Construction.CompCostRepair(expenseCP);
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

        protected override void BeforeAddToQueue()
        {
            Assert(Construction.CurrentDurability < Construction.MaxDurability);

            base.BeforeAddToQueue();

            Construction.InRepair = true;

            /*if (Construction.State == StateConstruction.NeedRepair)
                Construction.StartRepair();
            else if (Construction.State == StateConstruction.Repair)
                Construction.CancelRepair();
            else
                DoException($"Неправильное состояние: {Construction.State}");*/
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            //Construction.PrepareHintForBuildOrUpgrade(panelHint, Descriptor.Number);
        }
    }

    internal sealed class CellMenuConstructionRecruitCreature : ActionInConstruction
    {
        public CellMenuConstructionRecruitCreature(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Creature = d.CreatedEntity as DescriptorCreature;
        }

        internal DescriptorCreature Creature { get; private set; }

        protected override void Execute()
        {
            DoException("Действие не может быть выполнено");
        }

        internal override bool CheckRequirements()
        {
            return base.CheckRequirements() && Construction.AllowHire();
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Hire);
        }
        
        protected override string GetTextForLevel() => "р";

        internal override int GetImageIndex()
        {
            return Creature.ImageIndex;
        }

        protected override ActionInConstruction ActionForAddToQueue()
        {
            CellMenuConstructionCreatingCreature cc = new CellMenuConstructionCreatingCreature(Construction, Descriptor);
            Construction.Actions.Add(cc);
            Construction.CreaturesInQueue.Add(cc);
            return cc;
        }

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);

            if (Construction.MaxCreaturesInConstruction())
                list.Add((false, Construction.Descriptor.GetTextConstructionIsFull()));

            if (Construction.MaxHeroesAtPlayer())
                list.Add((false, "Достигнуто максимальное количество героев"));
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
            panelHint.AddStep12CostExecuting("Рекрутировать", PurchaseValue, 0, 0, GetTextRequirements());
        }
    }

    internal sealed class CellMenuConstructionCreatingCreature : ActionInConstruction
    {
        public CellMenuConstructionCreatingCreature(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Creature = d.CreatedEntity as DescriptorCreature;
        }

        internal DescriptorCreature Creature { get; private set; }

        protected override void Execute()
        {
            Assert(Construction.CreaturesInQueue.IndexOf(this) != -1);
            Construction.CreaturesInQueue.Remove(this);
            Creature h = Construction.HireHero(Creature, null);// Обучение уже оплачено
            Construction.Player.RemoveFromQueueExecuting(this, true);
            Construction.Player.AddNoticeForPlayer(h, TypeNoticeForPlayer.HireHero);
        }

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
            //panelHint.AddStep75Salary(Creature.CostOfHiring);
            //panelHint.AddStep10DaysBuilding(InQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep12CostExecuting("Обучение", null);
        }
    }

    internal sealed class CellMenuConstructionMassEvent : ActionInConstruction
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

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);

            Debug.Assert(!((cp != null) && (Cooldown > 0)));

            if (Construction.Level > 1)
                list.Add(((cp is null) && (Cooldown == 0) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null), (cp is null) && (Cooldown == 0) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null)
                    ? "Событие можно проводить" : Construction.CurrentVisit?.DescriptorConstructionVisit == null ? "В сооружении уже идет другое событие" : cp != null ? $"Событие будет идти еще {cp.Counter} дн." : $"Осталось подождать дней: {Cooldown}"));
        }

        internal override string GetText()
        {
            return (cp is null) && (Cooldown == 0) ? PurchaseValue.Gold.ToString() : cp != null ? "идёт" : Cooldown.ToString() + " дн.";
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.MassEvent);
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
            panelHint.AddStep12CostExecuting("Подготовить мероприятие", PurchaseValue, ProgressExecuting.RestTimeExecuting, 0, GetTextRequirements());
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
    
    internal sealed class CellMenuConstructionExtension : ActionInConstruction
    {
        public CellMenuConstructionExtension(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Entity = d.CreatedEntity as DescriptorConstructionExtension;
        }

        internal DescriptorConstructionExtension Entity { get; }

        protected override void Execute()
        {
            RemoveSelf(true);

            ConstructionExtension ce = new ConstructionExtension(Construction, Entity);
            Construction.AddExtension(ce);

            Construction.Player.RemoveFromQueueExecuting(this, true);
            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Extension);
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Building);
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
            panelHint.AddStep9CityParameters(Entity.ChangeCityParametersPerTurn);
            panelHint.AddStep9Interest(Entity.ModifyInterest, true);
            panelHint.AddStep9ListNeeds(Entity.ListNeeds, true);
            //panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep12CostExecuting("Построить", PurchaseValue, ProgressExecuting.RestTimeExecuting, 0, GetTextRequirements());
        }
    }

    internal sealed class CellMenuConstructionImprovement : ActionInConstruction
    {
        public CellMenuConstructionImprovement(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Entity = d.CreatedEntity as DescriptorConstructionImprovement;
        }

        internal DescriptorConstructionImprovement Entity { get; }

        protected override void Execute()
        {
            RemoveSelf(true);

            ConstructionImprovement ce = new ConstructionImprovement(Construction, Entity);
            Construction.AddImprovement(ce);
            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Improvement);
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Research);
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
            panelHint.AddStep12CostExecuting("Улучшение", PurchaseValue, ProgressExecuting.RestTimeExecuting, 0, GetTextRequirements());
        }
    }

    internal sealed class CellMenuConstructionTournament : ActionInConstruction
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

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);

            if (Construction.Level > 1)
                list.Add(((ct is null) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null), (ct is null) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null)
                    ? "Турнир можно проводить" : Construction.CurrentVisit?.DescriptorConstructionVisit == null ? "В сооружении уже идет другое событие" : $"Осталось подождать дней: {1}"));
        }

        internal override string GetText()
        {
            return ct is null ? PurchaseValue.Gold.ToString() : "идёт";
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Tournament);
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
            panelHint.AddStep12CostExecuting("Подготовить турнир", PurchaseValue, ProgressExecuting.RestTimeExecuting, 0, GetTextRequirements());
        }
    }

    internal enum TypeExtra { Builder, LevelUp, Research };

    internal sealed class CellMenuConstructionExtra : ActionInConstruction
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

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);
            list.Add((Counter == 0, Counter == 0 ? "Покупка доступна" : "Дней до новой покупки: " + Counter.ToString()));
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
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Extra);
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

            panelHint.AddStep12CostExecuting("Добавить бонус", PurchaseValue, 0, 0, GetTextRequirements());
        }

        internal override void PrepareNewDay()
        {
            base.PrepareNewDay();

            if (Counter > 0)
                Counter--;
        }
    }

    internal sealed class CellMenuConstructionAction : ActionInConstruction
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

    internal sealed class CellMenuConstructionSpell : ActionInConstruction
    {
        public CellMenuConstructionSpell(Construction forConstruction, ConstructionSpell spell) : base(forConstruction, new DescriptorActionForEntity(spell.DescriptorSpell.Coord))
        {
            ForConstruction = forConstruction;
            Spell = spell;
            Entity = spell.DescriptorSpell;

            PurchaseValue.Gold = Entity.Selling.Gold;                
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
            Location.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.CostResources, PurchaseValue, TypeCreating.Hire);
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