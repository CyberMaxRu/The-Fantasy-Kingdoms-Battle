using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс описателя ячейки меню
    internal abstract class DescriptorCellMenu : Descriptor
    {
        private string nameEntityForCreate;

        public DescriptorCellMenu(DescriptorActiveEntity activeEntity, XmlNode n) : this(activeEntity, GetPoint(n, "Pos"))
        {
        }

        public DescriptorCellMenu(DescriptorActiveEntity activeEntity, Point coord) : base()
        {
            ActiveEntity = activeEntity;

            Coord = coord;

            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        internal DescriptorActiveEntity ActiveEntity { get; }
        internal DescriptorEntityForActiveEntity EntityForCreate { get; private set; }
        internal Point Coord { get; }// Координаты ячейки в меню

        internal override void TuneLinks()
        {
            base.TuneLinks();

            EntityForCreate = ActiveEntity. nameEntityForCreate
        }
    }
}
