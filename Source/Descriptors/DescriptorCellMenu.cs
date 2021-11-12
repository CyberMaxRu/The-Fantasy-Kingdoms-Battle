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
        public DescriptorCellMenu(DescriptorEntity forEntity, XmlNode n) : base()
        {
            ForEntity = forEntity;

            Coord = GetPoint(n, "Pos");
            Cost = new ListBaseResources(n.SelectSingleNode("Cost"));
            DaysProcessing = GetInteger(n, "DaysProcessing");
            Requirements = new ListDescriptorRequirements(this, n.SelectSingleNode("Requirements"));
            
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        public DescriptorCellMenu(DescriptorEntity forEntity, Point coord, XmlNode n) : base()
        {
            ForEntity = forEntity;

            Coord = coord;
            Cost = new ListBaseResources(n.SelectSingleNode("Cost"));
            DaysProcessing = GetInteger(n, "DaysProcessing");
            Requirements = new ListDescriptorRequirements(this, n.SelectSingleNode("Requirements"));

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        internal DescriptorEntity ForEntity { get; }
        internal Point Coord { get; }// Координаты ячейки
        internal ListBaseResources Cost { get; }// Стоимость
        internal int DaysProcessing { get; }// Количество дней для реализации действия
        internal ListDescriptorRequirements Requirements { get; }// Список требований

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }
    }
}
