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
            }
        }

        internal Construction Construction { get; }
        internal DescriptorComponentCreating Creating { get; }

        internal override string GetText() => PurchaseValue.ToString();

        internal override bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (ConstructionMustMeConstructed())
                if (Construction.Level == 0)
                    return false;

            // Потом проверяем наличие требуемых ресурсов
            if (Construction.Player.Gold < PurchaseValue)
                return false;

            if (Descriptor.CreatedEntity != null)
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.ComponentCreating.Requirements);

            return true;
        }

        internal sealed override string GetLevel()
        {
            return Program.formMain.Settings.ShowTypeCellMenu ? GetTextForLevel() : "";
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

            if (ConstructionMustMeConstructed())
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
            if (CheckRequirements())
            {
                Construction.Player.SpendResource(PurchaseValue);
                Program.formMain.PlayPressButton();
                Execute();
                Construction.Player.Lobby.Layer.UpdateMenu();
            }
            else
                Program.formMain.PlayPressButton();
        }

        protected virtual ActionInConstruction ActionForAddToQueue() => this;

        internal virtual void StartProgress() { }// Вызывается перед началом выполнения действия
    }

    internal sealed class CellMenuConstructionResearch : ActionInConstruction
    {
        public CellMenuConstructionResearch(Construction c, DescriptorActionForEntity d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.ComponentCreating.Cost > 0, $"У {d.CreatedEntity.ID} не указана цена.");

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
            panelHint.AddStep12CostExecuting("Исследовать", PurchaseValue, GetTextRequirements());
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.Cost, PurchaseValue, TypeCreating.Research);
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
            Debug.Assert(d.CreatedEntity.ComponentCreating.Cost > 0, $"У {d.CreatedEntity.ID} не указана цена.");

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
            panelHint.AddStep12CostExecuting("Исследовать", PurchaseValue, GetTextRequirements());
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.Cost, PurchaseValue, TypeCreating.Research);
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
                if (Construction.Player.Gold < TypeConstruction.Levels[1].ComponentCreating.Cost)
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

            Construction pc = new Construction(Construction.Player, TypeConstruction, 1, Construction.X, Construction.Y, TypeNoticeForPlayer.Build);
            if (!Construction.Player.Lobby.InPrepareTurn)
                Program.formMain.layerGame.SelectPlayerObject(pc);
            //}
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(TypeConstruction.Levels[1].ComponentCreating.Cost, PurchaseValue, TypeCreating.Building);
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
            //panelHint.AddStep10DaysBuilding(-1, );
            panelHint.AddStep12CostExecuting("Построить", TypeConstruction.Levels[1].ComponentCreating.Cost, Construction.Player.GetTextRequirementsBuildTypeConstruction(TypeConstruction));
        }
    }

    internal sealed class CellMenuConstructionLevelUp : ActionInConstruction
    {
        private int elapsedMilliTicks;// Сколько миллитиков прошло с последнего увеличения прочности

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
            if (Construction.Player.Gold < PurchaseValue)
                return false;

            // Проверяем, что нет события
            if (Construction.CurrentMassEvent != null)
                return false;

            // Проверяем требования к зданиям
            return Construction.Player.CheckRequirements(Descriptor.ComponentCreating.Requirements);

        }
        internal override int GetImageIndex() => Descriptor.ImageIndex;
        internal override bool GetImageIsEnabled() => (Construction.Level + 1 == Descriptor.Number) || base.GetImageIsEnabled();
        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.ComponentCreating.Cost, PurchaseValue, TypeCreating.Building);
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
            Construction.Build(true, false);
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
            panelHint.AddStep6Income(Construction.IncomeForLevel(Descriptor.Number));
            if (Descriptor.DescriptorVisit != null)
            {
                panelHint.AddStep9Interest(Descriptor.DescriptorVisit.Interest, false);
                panelHint.AddStep9ListNeeds(Descriptor.DescriptorVisit.ListNeeds, false);
            }
            string nameNextLevel = Descriptor.NewName ? $"Улучшить до {Descriptor.Name} ({Descriptor.Number} ур.)" : $"Улучшить до {Descriptor.Number} ур.";
            string nameExecuting = "";
            nameExecuting = Descriptor.Number == 1 ? "Построить" : nameNextLevel;

            panelHint.AddStep12CostExecuting(nameExecuting, Descriptor.ComponentCreating.Cost, GetTextRequirements());
            //panelHint.AddStep12Gold(Player.BaseResources, Descriptor.Levels[requiredLevel].GetCreating().CostResources);
            //panelHint.AddStep13Builders(Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player), Player.RestConstructionPoints >= Descriptor.Levels[requiredLevel].GetCreating().ConstructionPoints(Player));
        }

        protected override void UpdateTextRequirements(ListTextRequirement list)
        {
            base.UpdateTextRequirements(list);

            Construction.Player.TextRequirements(Descriptor.ComponentCreating.Requirements, list, Construction);

            if (Construction.CurrentMassEvent != null)
                list.Add((false, "В сооружении идет мероприятие"));
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
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.Cost, PurchaseValue, TypeCreating.Hire);
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
            panelHint.AddStep12CostExecuting("Рекрутировать", PurchaseValue, GetTextRequirements());
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
            Creature h = Construction.HireHero(Creature, 0);// Обучение уже оплачено
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
            panelHint.AddStep12CostExecuting("Обучение", 0);
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
            return (cp is null) && (Cooldown == 0) ? PurchaseValue.ToString() : cp != null ? "идёт" : Cooldown.ToString() + " дн.";
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.Cost, PurchaseValue, TypeCreating.MassEvent);
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
            panelHint.AddStep12CostExecuting("Подготовить мероприятие", PurchaseValue, GetTextRequirements());
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

            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Extension);
        }

        internal override void UpdatePurchase()
        {
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.Cost, PurchaseValue, TypeCreating.Building);
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
            panelHint.AddStep12CostExecuting("Построить", PurchaseValue, GetTextRequirements());
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
            Construction.Player.CompPurchase(Descriptor.CreatedEntity.ComponentCreating.Cost, PurchaseValue, TypeCreating.Research);
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
            panelHint.AddStep12CostExecuting("Улучшение", PurchaseValue, GetTextRequirements());
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

            PurchaseValue = Entity.Selling.Gold;                
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
}