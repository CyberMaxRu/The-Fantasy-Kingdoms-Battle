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
    internal abstract class CellMenuConstruction : CellOfMenu
    {
        public CellMenuConstruction(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Construction = c;
        }

        internal Construction Construction { get; }

        internal override bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Construction.TypeConstruction.IsInternalConstruction)
                if (ConstructionMustMeConstructed())
                    if (Construction.Level == 0)
                        return false;

            // Потом проверяем наличие золота
            if (!Construction.Player.CheckRequiredResources(GetCost()))
                return false;

            if (Descriptor.CreatedEntity != null)
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.GetCreating().Requirements);

            return true;
        }

        protected void RemoveSelf()
        {
            Debug.Assert(Construction.Researches.IndexOf(this) != -1);
            Construction.Researches.Remove(this);
            Construction.Player.Lobby.Layer.UpdateMenu();
        }

        protected virtual bool ConstructionMustMeConstructed() => true;

        internal override List<TextRequirement> GetTextRequirements()
        {
            return Construction.GetResearchTextRequirements(this);
        }

        internal static CellMenuConstruction Create(Construction c, DescriptorCellMenu d)
        {
            if (d.CreatedEntity != null)
            {
                if (d.CreatedEntity is DescriptorProduct)
                    return new CellMenuConstructionResearch(c, d);
                if (d.CreatedEntity is DescriptorConstructionLevel)
                    return new CellMenuConstructionLevelUp(c, d);
                if (d.CreatedEntity is DescriptorConstructionEvent)
                    return new CellMenuConstructionEvent(c, d);
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
                    return new CellMenuConstructionHireCreature(c, d);

                throw new Exception($"Неизвестный тип сущности: {d.CreatedEntity.ID}.");
            }
            else
            {
                return new CellMenuConstructionAction(c, d);
            }
        }

        internal override void Click()
        {
            if (PosInQueue == 0)
            {
                if (CheckRequirements())
                {
                    if (!(this is CellMenuConstructionHireCreature))
                        if (!(this is CellMenuConstructionSpell))
                            Debug.Assert(Descriptor.CreatedEntity.GetCreating().DaysProcessing > 0);

                    Program.formMain.PlayPushButton();

                    DaysLeft = InstantExecute() ? 1 : Descriptor.CreatedEntity.GetCreating().DaysProcessing;
                    if (DaysLeft > 0)
                        DaysLeft--;

                    if ((DaysLeft == 0) || InstantExecute())
                        Execute();
                    else
                        Construction.AddEntityToQueueProcessing(this);
                }
            }
            else
            {
                if (DaysProcessed == 0)
                {
                    Program.formMain.PlayPushButton();
                    Construction.RemoveEntityFromQueueProcessing(this);
                }
            }

        }
    }

    internal sealed class CellMenuConstructionResearch : CellMenuConstruction
    {
        public CellMenuConstructionResearch(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.GetCreating().CostResources.ValueGold() > 0, $"У {d.CreatedEntity.ID} не указана цена.");

            Entity = d.CreatedEntity as DescriptorProduct;
        }

        internal DescriptorProduct Entity { get; }
        internal override void PrepareHint(PanelHint panelHint)
        {
            //string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            panelHint.AddStep2Header(Entity.Name, GetImageIndex());
            panelHint.AddStep3Type(Entity.SmallEntity != null ? Entity.SmallEntity.GetTypeEntity() : Entity.SmallEntity.GetTypeEntity());
            //panelHint.AddStep4Level(level);
            panelHint.AddStep5Description(Entity.SmallEntity.Description);
            //PanelHint.AddStep6Income(Descriptor.Income);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "и" : "";

        internal override void Execute()
        {
            RemoveSelf();

            if (Entity.SmallEntity is DescriptorAbility da)
            {
                ConstructionAbility ca = new ConstructionAbility(Construction, Entity, da);
                Construction.AddAbility(ca);
                Construction.Player.AddNoticeForPlayer(ca, TypeNoticeForPlayer.Research);
            }
            else if (Entity.SmallEntity is DescriptorConstructionSpell ds)
            {
                ConstructionSpell cs = new ConstructionSpell(Construction, Entity, ds);
                Construction.AddSpell(cs);
                Construction.Player.AddNoticeForPlayer(cs, TypeNoticeForPlayer.Research);
            }
            else
            {
                ConstructionProduct cp = new ConstructionProduct(Construction, Entity);
                Construction.AddProduct(cp);
                Construction.Player.AddNoticeForPlayer(cp, TypeNoticeForPlayer.Research);
            }

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal sealed class CellMenuConstructionService : CellMenuConstruction
    {
        public CellMenuConstructionService(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Debug.Assert(d.CreatedEntity.GetCreating().CostResources.ValueGold() > 0, $"У {d.CreatedEntity.ID} не указана цена.");

            Entity = d.CreatedEntity as DescriptorConstructionService;
            Debug.Assert(Entity != null);
        }

        internal DescriptorConstructionService Entity { get; }
        internal override void PrepareHint(PanelHint panelHint)
        {
            //string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            panelHint.AddStep2Header(Entity.Name, GetImageIndex());
            panelHint.AddStep3Type(Entity.GetTypeEntity());
            //panelHint.AddStep4Level(level);
            panelHint.AddStep5Description(Entity.Description);
            //panelHint.AddStep6Income(Descriptor.Income);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "и" : "";

        internal override void Execute()
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

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal sealed class CellMenuConstructionBuild : CellMenuConstruction
    {
        public CellMenuConstructionBuild(Construction c, DescriptorCellMenu d) : base(c, d)
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
                return Construction.Player.CanBuildTypeConstruction(TypeConstruction);
            }
        }

        internal override void Execute()
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

            Construction pc = new Construction(Construction.Player, TypeConstruction, 1, Construction.X, Construction.Y, Construction.Location, TypeNoticeForPlayer.Build);
            Construction.Location.Lairs[pc.Y, pc.X] = pc;
            Program.formMain.layerGame.SelectPlayerObject(pc);
            //}

            if (Construction.Player.GetTypePlayer() == TypePlayer.Human)
                Program.formMain.layerGame.UpdateNeighborhoods();

            Program.formMain.SetNeedRedrawFrame();
            Program.formMain.PlayConstructionComplete();
        }

        internal override ListBaseResources GetCost()
        {
            return TypeConstruction.Levels[1].GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "1" : "";

        internal override int GetImageIndex()
        {
            return TypeConstruction.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            Construction.Player.PrepareHintForBuildTypeConstruction(panelHint, TypeConstruction);
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyBuilding;
    }

    internal sealed class CellMenuConstructionLevelUp : CellMenuConstruction
    {
        public CellMenuConstructionLevelUp(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Descriptor = d.CreatedEntity as DescriptorConstructionLevel;
        }

        internal override void Execute()
        {
            Construction.Build(true);
        }

        internal new DescriptorConstructionLevel Descriptor { get; }

        protected override bool ConstructionMustMeConstructed() => false;

        internal override bool CheckRequirements()
        {
            return Construction.CheckLevelRequirements(Descriptor.Number);
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.GetCreating().CostResources;
        }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override string GetLevel()
        {
            return Descriptor.Number == 1 ? "" : Descriptor.Number.ToString();
        }

        internal override Color GetColorText()
        {
            return GetImageIsEnabled() ? DaysLeft == 0 ? Color.LimeGreen : FormMain.Config.CommonCost : Color.Gray;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            Construction.PrepareHintForBuildOrUpgrade(panelHint, Descriptor.Number);
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyBuilding;
    }

    internal sealed class CellMenuConstructionHireCreature : CellMenuConstruction
    {
        public CellMenuConstructionHireCreature(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Creature = d.CreatedEntity as DescriptorCreature;
        }

        internal DescriptorCreature Creature { get; private set; }

        internal override void Execute()
        {
            Hero h = Construction.HireHero(Creature, GetCost());

            if (Descriptor.CreatedEntity.GetCreating().DaysProcessing > 0)
                Construction.Player.AddNoticeForPlayer(h, TypeNoticeForPlayer.HireHero);
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }
        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "н" : "";

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
            panelHint.AddStep2Header(Creature.Name, Creature.ImageIndex);
            panelHint.AddStep3Type("Найм");
            panelHint.AddStep5Description(Creature.Description);
            panelHint.AddStep75Salary(Creature.Salary);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyHire;
    }

    internal sealed class CellMenuConstructionEvent : CellMenuConstruction
    {
        private ConstructionEvent cp;
        public CellMenuConstructionEvent(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            ConstructionEvent = d.CreatedEntity as DescriptorConstructionEvent;
        }

        internal DescriptorConstructionEvent ConstructionEvent { get; }
        internal int Cooldown { get; private set; }

        internal override void Execute()
        {
            Debug.Assert(Construction.Researches.IndexOf(this) != -1);
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

        internal override List<TextRequirement> GetTextRequirements()
        {
            Debug.Assert(!((cp != null) && (Cooldown > 0)));

            List<TextRequirement> list = base.GetTextRequirements();
            if (Construction.Level > 1)
                list.Add(new TextRequirement((cp is null) && (Cooldown == 0) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null), (cp is null) && (Cooldown == 0) && (Construction.CurrentVisit?.DescriptorConstructionVisit != null)
                    ? "Событие можно проводить" : Construction.CurrentVisit?.DescriptorConstructionVisit == null ? "В сооружении уже идет другое событие" : cp != null ? $"Событие будет идти еще {cp.Counter} дн." : $"Осталось подождать дней: {Cooldown}"));

            return list;
        }

        internal override string GetText()
        {
            return (cp is null) && (Cooldown == 0) ? GetCost().ValueGold().ToString() : cp != null ? "идёт" : Cooldown.ToString() + " дн.";
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "м" : "";

        internal override int GetImageIndex()
        {
            return ConstructionEvent.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(ConstructionEvent.Name, ConstructionEvent.ImageIndex);
            panelHint.AddStep3Type("Мероприятие");
            panelHint.AddStep4Level($"Длительность: {ConstructionEvent.Duration} дн."
                + Environment.NewLine + $"Перерыв: {ConstructionEvent.Cooldown} дн.");
            panelHint.AddStep5Description(ConstructionEvent.Description);
            panelHint.AddStep9Interest(ConstructionEvent.Interest, false);
            panelHint.AddStep9ListNeeds(ConstructionEvent.ListNeeds, false);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override void PrepareTurn()
        {
            base.PrepareTurn();

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

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }
    
    internal sealed class CellMenuConstructionExtension : CellMenuConstruction
    {
        public CellMenuConstructionExtension(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Entity = d.CreatedEntity as DescriptorConstructionExtension;
        }

        internal DescriptorConstructionExtension Entity { get; }

        internal override void Execute()
        {
            RemoveSelf();

            ConstructionExtension ce = new ConstructionExtension(Construction, Entity);
            Construction.AddExtension(ce);

            Program.formMain.SetNeedRedrawFrame();

            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Extension);
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "д" : "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            panelHint.AddStep3Type("Доп. сооружение");
            panelHint.AddStep5Description(Entity.Description);
            //panelHint.AddStep6Income(Descriptor.Income);
            panelHint.AddStep9Interest(Entity.ModifyInterest, true);
            panelHint.AddStep9ListNeeds(Entity.ListNeeds, true);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal sealed class CellMenuConstructionImprovement : CellMenuConstruction
    {
        public CellMenuConstructionImprovement(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Entity = d.CreatedEntity as DescriptorConstructionImprovement;
        }

        internal DescriptorConstructionImprovement Entity { get; }

        internal override void Execute()
        {
            RemoveSelf();

            ConstructionImprovement ce = new ConstructionImprovement(Construction, Entity);
            Construction.AddImprovement(ce);

            Program.formMain.SetNeedRedrawFrame();

            Construction.Player.AddNoticeForPlayer(ce, TypeNoticeForPlayer.Improvement);
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "у" : "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            panelHint.AddStep3Type("Улучшение");
            panelHint.AddStep5Description(Entity.Description);
            //CreatedEntity.Creating.panelHint.AddStep6Income(Descriptor.Income);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal sealed class CellMenuConstructionTournament : CellMenuConstruction
    {
        public CellMenuConstructionTournament(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            //Entity = Config.FindItem(d.NameEntity);
            Entity = Descriptors.FindProduct(d.IDCreatedEntity);
        }

        internal DescriptorProduct Entity { get; }

        internal override void Execute()
        {
            ConstructionProduct cp = new ConstructionProduct(Construction, Entity);
            Construction.AddProduct(cp);

            Program.formMain.SetNeedRedrawFrame();

            Construction.Player.AddNoticeForPlayer(cp, TypeNoticeForPlayer.TournamentBegin);
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "т" : "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            panelHint.AddStep3Type("Турнир");
            panelHint.AddStep5Description(Entity.Description);
            //panelHint.AddStep6Income(Descriptor.Income);
            panelHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.GetCreating().DaysProcessing);
            panelHint.AddStep11Requirement(GetTextRequirements());
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal enum TypeExtra { Builder, LevelUp, Research };

    internal sealed class CellMenuConstructionExtra : CellMenuConstruction
    {
        public CellMenuConstructionExtra(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            TypeExtra = (TypeExtra)Enum.Parse(typeof(TypeExtra), d.IDCreatedEntity);
        }

        internal int Counter { get; set; }
        internal TypeExtra TypeExtra { get; }

        internal override bool CheckRequirements()
        {
            return (Counter == 0) && base.CheckRequirements();
        }

        internal override List<TextRequirement> GetTextRequirements()
        {
            List<TextRequirement> list = base.GetTextRequirements();
            list.Add(new TextRequirement(Counter == 0, Counter == 0 ? "Покупка доступна" : "Дней до новой покупки: " + Counter.ToString()));
            return list;
        }

        internal override void Execute()
        {
            Debug.Assert(CheckRequirements());

            Construction.Player.SpendResource(GetCost());

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

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.GetCreating().CostResources;
        }

        internal override string GetText()
        {
            return Counter == 0 ? GetCost().ValueGold().ToString() : Counter.ToString() + " д.";
        }

        internal override string GetLevel() => "+1";

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
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override void PrepareTurn()
        {
            base.PrepareTurn();

            if (Counter > 0)
                Counter--;
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal sealed class CellMenuConstructionAction : CellMenuConstruction
    {
        public CellMenuConstructionAction(Construction c, DescriptorCellMenu d) : base(c, d)
        {
        }

        internal override string GetLevel() => "";

        internal override int GetImageIndex()
        {
            return 1;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
        }

        internal override ListBaseResources GetCost()
        {
            return new ListBaseResources();
        }

        internal override void Execute()
        {
            
        }

        internal override bool InstantExecute()
        {
            return false;
        }
    }

    internal sealed class CellMenuConstructionSpell : CellMenuConstruction
    {
        public CellMenuConstructionSpell(Construction forConstruction, ConstructionSpell spell) : base(forConstruction, new DescriptorCellMenu(forConstruction.TypeConstruction, spell.DescriptorSpell.Coord))
        {
            ForConstruction = forConstruction;
            Spell = spell;
            Entity = spell.DescriptorSpell;
        }

        internal Construction ForConstruction { get; }
        internal ConstructionSpell Spell { get; }
        internal DescriptorConstructionSpell Entity { get; }

        internal override string GetLevel() => "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            panelHint.AddStep3Type("Заклинание");
            panelHint.AddStep4Level($"Осталось: {Spell.Selling.RestQuantity}");
            panelHint.AddStep5Description(Entity.Description);
            panelHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override ListBaseResources GetCost()
        {
            return new ListBaseResources(Entity.Selling.Gold);
        }

        internal override void Execute()
        {
            switch (Entity.Action)
            {
                case ActionOfSpell.Scout:
                    Assert(ForConstruction.Hidden);
                    ForConstruction.Unhide(false);
                    Spell.Selling.Use();

                    Assert(ForConstruction.MenuSpells.IndexOf(this) != -1);
                    ForConstruction.MenuSpells.Remove(this);
                    break;
                default:
                    DoException($"Неизвестное действие: {Entity.Action}");
                    break;
            }

            Construction.Player.Lobby.Layer.UpdateMenu();
        }

        internal override bool CheckRequirements() => (Spell.Selling.RestQuantity > 0) && (base.CheckRequirements());

        internal override bool InstantExecute() => true;

        internal override void PrepareTurn()
        {
            base.PrepareTurn();

            Spell.Selling.Reset();
        }
    }
}