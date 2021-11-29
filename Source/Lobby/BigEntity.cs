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
        public BigEntity(Lobby lobby) : base()
        {
            Lobby = lobby;
        }

        internal Lobby Lobby { get; }  
        internal List<CellMenuConstruction> Researches { get; } = new List<CellMenuConstruction>();
        internal EntityProperties Properties { get; set; }// Характеристики

        internal abstract void ShowInfo(int selectPage = -1);
        internal abstract void HideInfo();

        internal abstract void MakeMenu(VCMenuCell[,] menu);

        protected void FillResearches(VCMenuCell[,] menu)
        {
            foreach (CellMenuConstruction pr in Researches)
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
