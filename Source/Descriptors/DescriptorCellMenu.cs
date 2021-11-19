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
        public DescriptorCellMenu(DescriptorActiveEntity activeEntity, XmlNode n) : this(activeEntity, GetPoint(n, "Pos"))
        {
        }

        public DescriptorCellMenu(DescriptorActiveEntity forEntity, Point coord) : base()
        {
            ForEntity = forEntity;

            Coord = coord;

            Debug.Assert(Coord.X <= Descriptors.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Descriptors.PlateHeight - 1);
        }

        internal DescriptorActiveEntity ForEntity { get; }
        internal Point Coord { get; }// Координаты ячейки в меню
    }
}