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
        internal abstract int GetCost();
        internal abstract int GetImageIndex();
        internal virtual bool CheckRequirements() => true;
        internal virtual List<TextRequirement> GetTextRequirements() => new List<TextRequirement>();
        internal virtual void PrepareHint() { }
        internal abstract void Execute();
    }

    internal abstract class ConstructionCellMenu : CellMenu
    {
        public ConstructionCellMenu(Construction c, DescriptorCellMenuForConstruction d) : base(d)
        {
            Construction = c;
        }

        internal Construction Construction { get; }

        internal override bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Construction.TypeConstruction.IsInternalConstruction)
                if (Construction.Level == 0)
                    return false;

            // Потом проверяем наличие золота
            return Construction.Player.Gold >= GetCost();
        }

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
        }

        internal new DescriptorCellMenuForConstruction Descriptor { get; }
        internal DescriptorSmallEntity Entity { get; }
        internal override void PrepareHint()
        {
            string level = Entity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
            Program.formMain.formHint.AddStep1Header(Entity.Name, level, Entity.Description);
            Program.formMain.formHint.AddStep2Income(Descriptor.Income);
            Program.formMain.formHint.AddStep3Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep4Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override bool CheckRequirements()
        {
            return base.CheckRequirements() && Construction.CanResearch();
        }

        internal override void Execute()
        {
            Debug.Assert(CheckRequirements());
            Debug.Assert(Construction.Researches.IndexOf(this) != -1);

            Construction.Researches.Remove(this);

            Construction.Player.SpendGold(GetCost());

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
        internal override int GetImageIndex()
        {
            if (ConstructionForBuild != null)
                return ConstructionForBuild.TypeConstruction.ImageIndex;
            else
                return TypeConstruction.ImageIndex;
        }

        internal override void PrepareHint()
        {
            ConstructionForBuild.PrepareHintForBuildOrUpgrade();
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

        internal override int GetImageIndex()
        {
            return Creature.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Creature.Name, "", Creature.Description);
            Program.formMain.formHint.AddStep3Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep4Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }
    }

    internal sealed class CellMenuConstructionEvent : ConstructionCellMenu
    {
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

            Construction.Player.SpendGold(GetCost());
            Construction.AddProduct(new ConstructionProduct(ConstructionEvent));

            Cooldown = ConstructionEvent.Cooldown;
        }

        internal override bool CheckRequirements()
        {
            return (Cooldown == 0) && base.CheckRequirements();
        }

        internal override List<TextRequirement> GetTextRequirements()
        {
            List<TextRequirement> list = base.GetTextRequirements();
            list.Add(new TextRequirement(Cooldown == 0, Cooldown == 0 ? "Событие можно проводить" : $"Осталось подождать дней: {Cooldown}"));
            return list;
        }

        internal override int GetCost()
        {
            return Descriptor.Cost;
        }

        internal override int GetImageIndex()
        {
            return ConstructionEvent.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(ConstructionEvent.Name, $"Длительность: {ConstructionEvent.Duration} дн.", ConstructionEvent.Description);
            Program.formMain.formHint.AddStep3Requirement(GetTextRequirements());
            Program.formMain.formHint.AddStep4Gold(GetCost(), GetCost() <= Construction.Player.Gold);
        }
    }
}
