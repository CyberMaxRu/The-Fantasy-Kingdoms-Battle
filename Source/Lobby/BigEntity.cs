using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class BigEntity : Entity
    {
        public BigEntity() : base()
        {

        }

        internal List<ConstructionCellMenu> Researches { get; } = new List<ConstructionCellMenu>();

        internal abstract void ShowInfo();
        internal abstract void HideInfo();

        internal abstract void MakeMenu(VCMenuCell[,] menu);

        protected void FillResearches(VCMenuCell[,] menu)
        {
            foreach (ConstructionCellMenu pr in Researches)
            {
                if (!menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Used)
                {
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research = pr;
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Used = true;
                }
                else if (menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research.Construction == pr.Construction)
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research = pr;
            }
        }
    }
}
