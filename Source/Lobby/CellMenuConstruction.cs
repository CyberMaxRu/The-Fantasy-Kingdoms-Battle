﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

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

            return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.Creating.Requirements);
        }

        protected void RemoveSelf()
        {
            Debug.Assert(Construction.Researches.IndexOf(this) != -1);
            Construction.Researches.Remove(this);
        }

        protected virtual bool ConstructionMustMeConstructed() => true;

        internal override List<TextRequirement> GetTextRequirements()
        {
            return Construction.GetResearchTextRequirements(this);
        }

        internal static CellMenuConstruction Create(Construction c, DescriptorCellMenu d)
        {
            switch (d.Action)
            {
                case "Research":
                    return new CellMenuConstructionResearch(c, d);
                case "Build":
                    return new CellMenuConstructionBuild(c, d);
                case "HireCreature":
                    return new CellMenuConstructionHireCreature(c, d);
                case "Event":
                    return new CellMenuConstructionEvent(c, d);
                case "LevelUp":
                    return new CellMenuConstructionLevelUp(c, d);
                case "Extension":
                    return new CellMenuConstructionExtension(c, d);
                case "Improvement":
                    return new CellMenuConstructionImprovement(c, d);
                case "Tournament":
                    return new CellMenuConstructionTournament(c, d);
                case "Extra":
                    return new CellMenuConstructionExtra(c, d);
                //case TypeCellMenuForConstruction.Action:
                //    break;
                default:
                    throw new Exception($"Неизвестный тип ячейки: {d.Action}");
            }
        }

        internal override void Click()
        {
            if (PosInQueue == 0)
            {
                if (CheckRequirements())
                {
                    if (!(this is CellMenuConstructionHireCreature))
                        Debug.Assert(Descriptor.CreatedEntity.Creating.DaysProcessing > 0);

                    Program.formMain.PlayPushButton();

                    if (InstantExecute() || (Descriptor.CreatedEntity.Creating.DaysProcessing == 0))
                        Execute();
                    else
                    {
                        Construction.AddEntityToQueueProcessing(this);
                        DaysLeft = Descriptor.CreatedEntity.Creating.DaysProcessing;
                    }
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
            Debug.Assert(d.CreatedEntity.Creating.CostResources.ValueGold() > 0, $"У {d.IDCreatedEntity} не указана цена.");

            Entity = Descriptors.FindProduct(d.IDCreatedEntity);
        }

        internal DescriptorProduct Entity { get; }
        internal override void PrepareHint()
        {
            //string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            Program.formMain.formHint.AddStep2Header(Entity.Name, GetImageIndex());
            Program.formMain.formHint.AddStep3Type(Entity.DescriptorEntity != null ? Entity.DescriptorEntity.GetTypeEntity() : Entity.EntityForConstruction.GetTypeEntity());
            //Program.formMain.formHint.AddStep4Level(level);
            Program.formMain.formHint.AddStep5Description(Entity.Construction.Description);
            //Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.Creating.DaysProcessing);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.Creating.CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "и" : "";

        internal override void Execute()
        {
            RemoveSelf();

            ConstructionProduct cp = new ConstructionProduct(Construction, Entity);
            Construction.AddProduct(cp);
            Construction.Player.AddNoticeForPlayer(cp, TypeNoticeForPlayer.Research);

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
            Debug.Assert(d.CreatedEntity.Creating.CostResources.ValueGold() == 0, $"У {d.IDCreatedEntity} цена должна быть 0 (указана {d.CreatedEntity.Creating.CostResources.ValueGold()}).");

            TypeConstruction = Descriptors.FindConstruction(d.IDCreatedEntity);
            if (TypeConstruction.Category == CategoryConstruction.Temple)
                ConstructionForBuild = c.Player.GetPlayerConstruction(TypeConstruction);
            else if (TypeConstruction.Category == CategoryConstruction.External)
            {
            }
            else
                throw new Exception("Неизвестная категория сооружения: " + TypeConstruction.ID);
        }

        private DescriptorConstruction TypeConstruction { get; set; }// Описатель строимого сооружения
        private Construction ConstructionForBuild { get; }// Строимое у игрока сооружение

        internal override bool CheckRequirements()
        {
            if (!base.CheckRequirements())
                return false;

            if (TypeConstruction is null)
                return Construction.Player.CheckRequirements(Descriptor.CreatedEntity.Creating.Requirements);
            else
            {
                if (ConstructionForBuild != null)
                    return ConstructionForBuild.CheckRequirements();
                else
                    return Construction.Player.CanBuildTypeConstruction(TypeConstruction);
            }
        }

        internal override void Execute()
        {
            if (ConstructionForBuild != null)
            {
                Debug.Assert(ConstructionForBuild.Level == 0);
                ConstructionForBuild.Build(true);
                ConstructionForBuild.X = Construction.X;
                ConstructionForBuild.Y = Construction.Y;
                ConstructionForBuild.Location = Construction.Location;
                ConstructionForBuild.Location.Lairs[ConstructionForBuild.Y, ConstructionForBuild.X] = ConstructionForBuild;
            }
            else
            {
                Construction pc = new Construction(Construction.Player, Construction.TypeConstruction, 1, Construction.X, Construction.Y, Construction.Location, TypeNoticeForPlayer.Build);
                Construction.Location.Lairs[pc.Y, pc.X] = pc;
                Program.formMain.SelectPlayerObject(pc);
            }

            if (Construction.Player.GetTypePlayer() == TypePlayer.Human)
                Program.formMain.UpdateNeighborhoods();

            Program.formMain.SetNeedRedrawFrame();
            Program.formMain.PlayConstructionComplete();
        }

        internal override ListBaseResources GetCost()
        {
            if (ConstructionForBuild != null)
                return ConstructionForBuild.CostBuyOrUpgrade();
            else
                return TypeConstruction.Levels[1].Creating.CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "1" : "";

        internal override int GetImageIndex()
        {
            if (ConstructionForBuild != null)
                return ConstructionForBuild.TypeConstruction.ImageIndex;
            else
                return TypeConstruction.ImageIndex;
        }

        internal override void PrepareHint()
        {
            ConstructionForBuild.PrepareHintForBuildOrUpgrade(Construction.Level + 1);
            //else
            //    Player.PrepareHintForBuildTypeConstruction(Research.Construction);
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
            return (Construction.Level + 1 == Descriptor.Number) && Construction.CheckRequirements();
        }

        internal override ListBaseResources GetCost()
        {
            return Construction.CostBuyOrUpgrade();
        }

        internal override int GetImageIndex()
        {
            return Descriptor.Number == 1 ? Config.Gui48_Build : Config.Gui48_LevelUp;
        }

        internal override string GetLevel()
        {
            return Descriptor.Number.ToString();
        }

        internal override Color GetColorText()
        {
            return GetImageIsEnabled() ? DaysLeft == 0 ? Color.LimeGreen : FormMain.Config.CommonCost : Color.Gray;
        }

        internal override void PrepareHint()
        {
            Construction.PrepareHintForBuildOrUpgrade(Descriptor.Number);
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyBuilding;
    }

    internal sealed class CellMenuConstructionHireCreature : CellMenuConstruction
    {
        public CellMenuConstructionHireCreature(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Creature = Descriptors.FindCreature(d.IDCreatedEntity);
        }

        internal DescriptorCreature Creature { get; private set; }

        internal override void Execute()
        {
            Hero h = Construction.HireHero(Creature, GetCost());

            if (Descriptor.CreatedEntity.Creating.DaysProcessing > 0)
                Construction.Player.AddNoticeForPlayer(h, TypeNoticeForPlayer.HireHero);
        }

        internal override ListBaseResources GetCost()
        {
            return Descriptor.CreatedEntity.Creating.CostResources;
        }
        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "н" : "";

        internal override int GetImageIndex()
        {
            return Creature.ImageIndex;
        }

        internal override void PrepareHint()
        {
            /*Program.formMain.formHint.AddStep2Header(TypeConstruction.TrainedHero.Name);
            Program.formMain.formHint.AddStep5Description(TypeConstruction.TrainedHero.Description);
            if ((TypeConstruction.TrainedHero != null) && (TypeConstruction.TrainedHero.Cost > 0))
                Program.formMain.formHint.AddStep11Requirement(GetTextRequirementsHire());
            Program.formMain.formHint.AddStep12Gold(TypeConstruction.TrainedHero.Cost, Player.Gold >= TypeConstruction.TrainedHero.Cost);
            */
            Program.formMain.formHint.AddStep2Header(Creature.Name, Creature.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Найм");
            Program.formMain.formHint.AddStep5Description(Creature.Description);
            Program.formMain.formHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.Creating.DaysProcessing);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyHire;
    }

    internal sealed class CellMenuConstructionEvent : CellMenuConstruction
    {
        private ConstructionProduct cp;
        public CellMenuConstructionEvent(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Entity = Descriptors.FindEntity(d.IDCreatedEntity) as DescriptorProduct;
            Debug.Assert(Entity.EntityForConstruction is DescriptorConstructionEvent);
            ConstructionEvent = Entity.EntityForConstruction as DescriptorConstructionEvent;
        }

        internal DescriptorConstructionEvent ConstructionEvent { get; }
        internal DescriptorProduct Entity { get; }
        internal int Cooldown { get; private set; }

        internal override void Execute()
        {
            Debug.Assert(Construction.Researches.IndexOf(this) != -1);
            Debug.Assert(cp is null);

            cp = new ConstructionProduct(Construction, Entity);
            Construction.AddProduct(cp);

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
            return Descriptor.CreatedEntity.Creating.CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "м" : "";

        internal override int GetImageIndex()
        {
            return ConstructionEvent.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(ConstructionEvent.Name, ConstructionEvent.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Мероприятие");
            Program.formMain.formHint.AddStep4Level($"Длительность: {ConstructionEvent.Duration} дн."
                + Environment.NewLine + $"Перерыв: {ConstructionEvent.Cooldown} дн.");
            Program.formMain.formHint.AddStep5Description(ConstructionEvent.Description);
            Program.formMain.formHint.AddStep9Interest(ConstructionEvent.Interest, false);
            Program.formMain.formHint.AddStep9ListNeeds(ConstructionEvent.ListNeeds, false);
            Program.formMain.formHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.Creating.DaysProcessing);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
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
            Entity = (d.ForEntity as DescriptorConstruction).FindExtension(d.IDCreatedEntity, true);
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
            return Descriptor.CreatedEntity.Creating.CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "д" : "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Доп. сооружение");
            Program.formMain.formHint.AddStep5Description(Entity.Description);
            //Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep9Interest(Entity.ModifyInterest, true);
            Program.formMain.formHint.AddStep9ListNeeds(Entity.ListNeeds, true);
            Program.formMain.formHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.Creating.DaysProcessing);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }

    internal sealed class CellMenuConstructionImprovement : CellMenuConstruction
    {
        public CellMenuConstructionImprovement(Construction c, DescriptorCellMenu d) : base(c, d)
        {
            Entity = (d.ForEntity as DescriptorConstruction).FindConstructionImprovement(d.IDCreatedEntity, true);
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
            return Descriptor.CreatedEntity.Creating.CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "у" : "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Улучшение");
            Program.formMain.formHint.AddStep5Description(Entity.Description);
            //CreatedEntity.Creating.Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.Creating.DaysProcessing);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
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
            return Descriptor.CreatedEntity.Creating.CostResources;
        }

        internal override string GetLevel() => Program.formMain.Settings.ShowTypeCellMenu ? "т" : "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Турнир");
            Program.formMain.formHint.AddStep5Description(Entity.Description);
            //Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep10DaysBuilding(PosInQueue == 1 ? DaysProcessed : -1, Descriptor.CreatedEntity.Creating.DaysProcessing);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
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
            return Descriptor.CreatedEntity.Creating.CostResources;
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

        internal override void PrepareHint()
        {
            switch (TypeExtra)
            {
                case TypeExtra.Builder:
                    Program.formMain.formHint.AddStep2Header("+1 Строитель");
                    Program.formMain.formHint.AddStep5Description("Добавляет 1 строителя на текущий ход" + Environment.NewLine + "Пауза: " + Descriptor.DaysCooldown.ToString() + " дн.");
                    break;
                case TypeExtra.LevelUp:
                    Program.formMain.formHint.AddStep2Header("+1 улучшение сооружение");
                    Program.formMain.formHint.AddStep5Description("Добавляет 1 внеплановое улучшение на текущий ход" + Environment.NewLine + "Пауза: " + Descriptor.DaysCooldown.ToString() + " дн.");
                    break;
                case TypeExtra.Research:
                    Program.formMain.formHint.AddStep2Header("+1 исследование");
                    Program.formMain.formHint.AddStep5Description("Добавляет 1 внеплановое исследование на текущий ход" + Environment.NewLine + "Пауза: " + Descriptor.DaysCooldown.ToString() + " дн.");
                    break;
                default:
                    throw new Exception($"Неизвестный тип бонуса: {TypeExtra}.");
            }

            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(Construction.Player.BaseResources, GetCost());
        }

        internal override void PrepareTurn()
        {
            base.PrepareTurn();

            if (Counter > 0)
                Counter--;
        }

        internal override bool InstantExecute() => Construction.Player.CheatingInstantlyResearch;
    }
}