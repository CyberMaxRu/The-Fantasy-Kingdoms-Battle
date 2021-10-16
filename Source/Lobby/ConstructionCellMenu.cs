using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{

    internal abstract class CellMenu
    {
        public CellMenu(DescriptorCellMenu d)
        {
            Descriptor = d;
        }

        internal static Config Config { get; set; }
        internal DescriptorCellMenu Descriptor { get; }
        internal virtual string GetText() => GetCost().ToString();
        internal abstract int GetCost();
        internal abstract int GetImageIndex();
        internal virtual string GetLevel() => "";
        internal virtual bool CheckRequirements() => true;
        internal virtual List<TextRequirement> GetTextRequirements() => new List<TextRequirement>();
        internal virtual void PrepareHint() { }
        internal abstract void Execute();
        internal virtual void PrepareTurn() { }
    }

    internal abstract class ConstructionCellMenu : CellMenu
    {
        public ConstructionCellMenu(Construction c, DescriptorCellMenuForConstruction d) : base(d)
        {
            Construction = c;
            Descriptor = d;
        }

        internal Construction Construction { get; }
        internal new DescriptorCellMenuForConstruction Descriptor { get; }

        internal override bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Construction.TypeConstruction.IsInternalConstruction)
                if (ConstructionMustMeConstructed())
                    if (Construction.Level == 0)
                        return false;

            // Потом проверяем наличие золота
            if (!Construction.Player.CheckRequireGold(GetCost()))
                return false;

            return Construction.Player.CheckRequirements(Descriptor.Requirements);
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

        internal static ConstructionCellMenu Create(Construction c, DescriptorCellMenuForConstruction d)
        {
            switch (d.Type)
            {
                case TypeCellMenuForConstruction.Research:
                    return new CellMenuConstructionResearch(c, d);
                case TypeCellMenuForConstruction.Build:
                    return new CellMenuConstructionBuild(c, d);
                case TypeCellMenuForConstruction.HireCreature:
                    return new CellMenuConstructionHireCreature(c, d);
                case TypeCellMenuForConstruction.Event:
                    return new CellMenuConstructionEvent(c, d);
                case TypeCellMenuForConstruction.LevelUp:
                    return new CellMenuConstructionLevelUp(c, d as DescriptorCellMenuForConstructionLevel);
                case TypeCellMenuForConstruction.Extension:
                    return new CellMenuConstructionExtension(c, d);
                case TypeCellMenuForConstruction.Tournament:
                    return new CellMenuConstructionTournament(c, d);
                //case TypeCellMenuForConstruction.Action:
                //    break;
                default:
                    throw new Exception($"Неизвестный тип ячейки: {d.Type}");
            }
        }
    }

    internal sealed class CellMenuConstructionResearch : ConstructionCellMenu
    {
        public CellMenuConstructionResearch(Construction c, DescriptorCellMenuForConstruction d) : base(c, d)
        {
            Debug.Assert(d.Cost > 0, $"У {d.NameEntity} не указана цена.");

            Descriptor = d;

            Entity = Config.FindAbility(d.NameEntity, false);
            if (Entity is null)
                Entity = Config.FindItem(d.NameEntity, false);
            if (Entity is null)
                Entity = Config.FindGroupItem(d.NameEntity, false);

            Debug.Assert(Entity != null, $"Для {c.TypeConstruction.ID} не найдена сущность {d.NameEntity}.");
        }

        internal new DescriptorCellMenuForConstruction Descriptor { get; }
        internal DescriptorSmallEntity Entity { get; }
        internal override void PrepareHint()
        {
            string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            Program.formMain.formHint.AddStep2Header(Entity.Name, GetImageIndex());
            Program.formMain.formHint.AddStep3Type(NameType());
            Program.formMain.formHint.AddStep4Level(level);
            Program.formMain.formHint.AddStep5Description(Entity.Description);
            Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(GetCost(), GetCost() <= Construction.Player.Gold);

            string NameType()
            {
                if (Entity is DescriptorAbility)
                    return "Умение";
                if (Entity is DescriptorItem)
                    return "Товар";
                if (Entity is DescriptorGroupItems)
                    return "Группа товаров";
                throw new Exception("Тип исследования не определен.");
            }
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override string GetLevel() => "и";

        internal override bool CheckRequirements()
        {
            return Construction.CanResearch() && base.CheckRequirements();
        }

        internal override void Execute()
        {
            Debug.Assert(CheckRequirements());

            Construction.Player.SpendGold(GetCost());
            RemoveSelf();

            Debug.Assert(Construction.ResearchesAvailabled > 0);

            Construction.ResearchesAvailabled--;

            if (Entity is DescriptorItem di)
                Construction.AddProduct(new ConstructionProduct(di));
            else if (Entity is DescriptorAbility da)
                Construction.AddProduct(new ConstructionProduct(da));
            else if (Entity is DescriptorGroupItems dgi)
                Construction.AddProduct(new ConstructionProduct(dgi));
            else
                throw new Exception("неизвестный тип");

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }
    }

    internal sealed class CellMenuConstructionBuild : ConstructionCellMenu
    {
        public CellMenuConstructionBuild(Construction c, DescriptorCellMenuForConstruction d) : base(c, d)
        {
            Debug.Assert(d.Cost == 0, $"У {d.NameEntity} цена должна быть 0 (указана {d.Cost}).");

            TypeConstruction = Config.FindConstruction(d.NameEntity);
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
                return Construction.Player.CheckRequirements(Descriptor.Requirements);
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
                ConstructionForBuild.Build();
                ConstructionForBuild.X = Construction.X;
                ConstructionForBuild.Y = Construction.Y;
                ConstructionForBuild.Layer = Construction.Layer;
                Construction.Player.Lairs[ConstructionForBuild.Layer, ConstructionForBuild.Y, ConstructionForBuild.X] = ConstructionForBuild;
            }
            else
            {
                Construction pc = new Construction(Construction.Player, Construction.TypeConstruction, 1, Construction.X, Construction.Y, Construction.Layer);
                Construction.Player.Lairs[pc.Layer, pc.Y, pc.X] = pc;
                Program.formMain.SelectPlayerObject(pc);
            }

            if (Construction.Player.GetTypePlayer() == TypePlayer.Human)
                Program.formMain.UpdateNeighborhood();

            Program.formMain.SetNeedRedrawFrame();
            Program.formMain.PlayConstructionComplete();
        }

        internal override int GetCost()
        {
            if (ConstructionForBuild != null)
                return ConstructionForBuild.CostBuyOrUpgrade();
            else
                return TypeConstruction.Levels[1].Cost;
        }

        internal override string GetLevel() => "п";

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
    }

    internal sealed class CellMenuConstructionHireCreature : ConstructionCellMenu
    {
        public CellMenuConstructionHireCreature(Construction c, DescriptorCellMenuForConstruction d) : base(c, d)
        {
            Creature = Config.FindCreature(d.NameEntity);
        }

        internal DescriptorCreature Creature { get; private set; }

        internal override void Execute()
        {
            Construction.HireHero(Creature);
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }
        internal override string GetLevel() => "н";

        internal override int GetImageIndex()
        {
            return Creature.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Creature.Name, Creature.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Найм");
            Program.formMain.formHint.AddStep5Description(Creature.Description);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }
    }

    internal sealed class CellMenuConstructionEvent : ConstructionCellMenu
    {
        private ConstructionProduct cp;
        public CellMenuConstructionEvent(Construction c, DescriptorCellMenuForConstruction d) : base(c, d)
        {
            ConstructionEvent = Config.FindConstructionEvent(d.NameEntity);
        }

        internal DescriptorConstructionEvent ConstructionEvent { get; }
        internal int Cooldown { get; private set; }

        internal override void Execute()
        {
            Debug.Assert(CheckRequirements());
            Debug.Assert(Construction.Researches.IndexOf(this) != -1);
            Debug.Assert(cp is null);

            Construction.Player.SpendGold(GetCost());
            cp = new ConstructionProduct(ConstructionEvent);
            Construction.AddProduct(cp);

            //Cooldown = ConstructionEvent.Cooldown;
        }

        internal override bool CheckRequirements()
        {
            return (cp is null) && (Cooldown == 0) && base.CheckRequirements();
        }

        internal override List<TextRequirement> GetTextRequirements()
        {
            Debug.Assert(!((cp != null) && (Cooldown > 0)));

            List<TextRequirement> list = base.GetTextRequirements();
            list.Add(new TextRequirement((cp is null) && (Cooldown == 0), (cp is null) && (Cooldown == 0) ? "Событие можно проводить" :                
                cp != null ? $"Событие будет идти еще {cp.Counter} дн." : $"Осталось подождать дней: {Cooldown}"));

            return list;
        }

        internal override string GetText()
        {
            return (cp is null) && (Cooldown == 0) ? GetCost().ToString() : 
                cp != null ? "идёт" : Cooldown.ToString() + " дн.";
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override string GetLevel() => "с";

        internal override int GetImageIndex()
        {
            return ConstructionEvent.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(ConstructionEvent.Name, ConstructionEvent.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Событие");
            Program.formMain.formHint.AddStep4Level($"Длительность: {ConstructionEvent.Duration} дн."
                + Environment.NewLine + $"Перерыв: {ConstructionEvent.Cooldown} дн.");
            Program.formMain.formHint.AddStep5Description(ConstructionEvent.Description);
            Program.formMain.formHint.AddStep9ListNeeds(ConstructionEvent.ListNeeds);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }

        internal override void PrepareTurn()
        {
            base.PrepareTurn();

            if (cp?.Counter == 0)
            {
                cp = null;
                Cooldown = ConstructionEvent.Cooldown;
            }
            else if (Cooldown > 0)
            {
                Cooldown--;
            }
        }
    }

    internal sealed class CellMenuConstructionLevelUp : ConstructionCellMenu
    {
        public CellMenuConstructionLevelUp(Construction c, DescriptorCellMenuForConstructionLevel d) : base(c, d)
        {
            Descriptor = d;
        }

        internal override void Execute()
        {
            Construction.Build();
        }

        internal new DescriptorCellMenuForConstructionLevel Descriptor { get; }

        protected override bool ConstructionMustMeConstructed() => false;

        internal override bool CheckRequirements()
        {
            return (Construction.Level + 1 == Descriptor.Number) && Construction.CheckRequirements();
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override int GetImageIndex()
        {
            return Descriptor.Number == 1 ? Config.Gui48_Build : Config.Gui48_LevelUp;
        }

        internal override string GetLevel()
        {
            return Descriptor.Number == 1 ? "" : Descriptor.Number.ToString();
        }

        internal override void PrepareHint()
        {
            Construction.PrepareHintForBuildOrUpgrade(Descriptor.Number);
        }
    }

    internal sealed class CellMenuConstructionExtension : ConstructionCellMenu
    {
        public CellMenuConstructionExtension(Construction c, DescriptorCellMenuForConstruction d) : base(c, d)
        {
            Entity = Config.FindConstructionExtension(d.NameEntity);
        }

        internal DescriptorConstructionExtension Entity { get; }

        internal override void Execute()
        {
            Debug.Assert(CheckRequirements());

            Construction.Player.SpendGold(GetCost());
            RemoveSelf();

            Construction.AddProduct(new ConstructionProduct(Entity));

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override string GetLevel() => "д";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Доп. сооружение");
            Program.formMain.formHint.AddStep5Description(Entity.Description);
            Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep9Interest(Entity.Interest);
            Program.formMain.formHint.AddStep9ListNeeds(Entity.ListNeeds);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }
    }

    internal sealed class CellMenuConstructionTournament : ConstructionCellMenu
    {
        public CellMenuConstructionTournament(Construction c, DescriptorCellMenuForConstruction d) : base(c, d)
        {
            Entity = Config.FindItem(d.NameEntity);
        }

        internal DescriptorSmallEntity Entity { get; }

        internal override void Execute()
        {
            Debug.Assert(CheckRequirements());

            Construction.Player.SpendGold(GetCost());
            RemoveSelf();

            Construction.AddProduct(new ConstructionProduct(Entity as DescriptorItem));

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override string GetLevel() => "т";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Entity.Name, Entity.ImageIndex);
            Program.formMain.formHint.AddStep3Type("Турнир");
            Program.formMain.formHint.AddStep5Description(Entity.Description);
            Program.formMain.formHint.AddStep6Income(Descriptor.Income);
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep12Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }
    }
}