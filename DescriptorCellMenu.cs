using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Deployment.Internal;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Класс исследования
    internal abstract class DescriptorCellMenu : Descriptor
    {
        public DescriptorCellMenu(XmlNode n) : base()
        {
            Coord = new Point(XmlUtils.GetIntegerNotNull(n, "PosX") - 1, XmlUtils.GetIntegerNotNull(n, "PosY") - 1);
            Layer = XmlUtils.GetIntegerNotNull(n, "Layer") - 1;
            NameEntity = XmlUtils.GetStringNotNull(n, "Entity");
            DefaultCost = XmlUtils.GetInteger(n, "Cost");
            XmlUtils.LoadRequirements(Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
            Debug.Assert(Layer >= 0);
            Debug.Assert(Layer <= 4);
            Debug.Assert(NameEntity.Length > 0);
        }

        internal Point Coord { get; }// Координаты ячейки
        internal int Layer { get; }// Визуальный слой ячейки
        private List<Requirement> Requirements { get; } = new List<Requirement>();

        protected int DefaultCost { get; }// Стоимость
        protected string NameEntity { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (Requirement r in Requirements)
                r.TuneDeferredLinks();

        }

        internal abstract int GetImageIndex();
        internal abstract int Cost(Player p);
        internal bool Enabled(Player p)
        {
            return true;
        }
    }

    internal abstract class DescriptorCellMenuConstruction : DescriptorCellMenu
    {
        public DescriptorCellMenuConstruction(XmlNode n) : base(n)
        {
        }

        internal DescriptorConstruction Construction { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Construction = Config.FindConstruction(NameEntity);
        }

        internal override int GetImageIndex() => Construction.ImageIndex;
        internal override int Cost(Player p)
        {
            return DefaultCost;
        }
    }

    internal abstract class DescriptorCellMenuConstructionResearch : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionResearch(XmlNode n) : base(n)
        {
            Debug.Assert(DefaultCost > 0, $"У {NameEntity} не указана цена.");
        }
    }

    internal sealed class DescriptorCellMenuConstructionResearchAbility : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionResearchAbility(XmlNode n) : base(n)
        {
        }

        internal DescriptorAbility Ability { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Ability = Config.FindAbility(NameEntity, false);
        }
    }

    internal sealed class DescriptorCellMenuConstructionResearchItem : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionResearchItem(XmlNode n) : base(n)
        {
        }

        internal DescriptorItem Item { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Item = Config.FindItem(NameEntity, false);
        }
    }

    internal sealed class DescriptorCellMenuConstructionResearchGroupItem : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionResearchGroupItem(XmlNode n) : base(n)
        {
        }

        internal DescriptorGroupItems GroupItems { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            GroupItems = Config.FindGroupItem(NameEntity, false);
        }
    }

    internal sealed class DescriptorCellMenuConstructionBuild : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionBuild(XmlNode n) : base(n)
        {
            Debug.Assert(DefaultCost == 0, $"У {NameEntity} цена должна быть 0 (указана {DefaultCost}).");
        }

        internal DescriptorConstruction TypeConstruction { get; set; }// Строимое сооружение
    }

    internal sealed class DescriptorCellMenuConstructionHireHero : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionHireHero(XmlNode n) : base(n)
        {

        }

        internal DescriptorCreature Creature { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Creature = Config.FindCreature(NameEntity);
        }
    }

    internal sealed class DescriptorCellMenuConstructionEvent : DescriptorCellMenuConstruction
    {
        public DescriptorCellMenuConstructionEvent(XmlNode n) : base(n)
        {

        }
    }
}
