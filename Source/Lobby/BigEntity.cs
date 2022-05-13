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
        public BigEntity(DescriptorEntity descriptor, Lobby lobby) : base()
        {
            Descriptor = descriptor;
            Lobby = lobby;
        }

        internal DescriptorEntity Descriptor { get; }
        internal Lobby Lobby { get; }  
        internal List<CellMenuConstruction> Researches { get; } = new List<CellMenuConstruction>();

        internal Perk MainPerk { get; set; }// Основной перк существа 
        internal List<Perk> Perks { get; } = new List<Perk>();// Перки
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
                else if (((CellMenuConstruction)menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research).Construction == pr.Construction)
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research = pr;
            }
        }

        internal virtual void PerksChanged()
        {
            CalcProperties();
        }

        protected void CalcProperties()
        {
            if (Properties != null)
                for (int i = 0; i < Properties.Count; i++)
                    if (Properties[i] != null)
                        CalcProperty(Properties[i]);
        }

        protected void CalcProperty(CreatureProperty cp)
        {
            cp.ListSource.Clear();
            cp.Value = 0;
            int value;

            foreach (Perk p in Perks)
            {
                value = p.ListProperty[cp.Property.Index];
                if (value != 0)
                {
                    cp.ListSource.Add(p);
                    cp.Value += value;
                }
            }

            if (cp.Value > FormMain.Config.MaxValueProperty)
                cp.Value = FormMain.Config.MaxValueProperty;
            else if (cp.Value < -FormMain.Config.MaxValueProperty)
                cp.Value = -FormMain.Config.MaxValueProperty;
        }

        internal virtual void Initialize()
        {

            // 
            PerksChanged();
        }
    }
}
