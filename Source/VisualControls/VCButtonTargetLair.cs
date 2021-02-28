using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCButtonTargetLair : VCButton
    {
        public VCButtonTargetLair(VisualControl parent) : base(parent, 0, 0, Program.formMain.imListObjectsCell, -1)
        {

        }

        internal PlayerLair Lair { get; set; }

        internal override void DoClick()
        {
            base.DoClick();

            Program.formMain.ActivatePageLairs();
            Program.formMain.SelectLair(Lair.TypeLair.Panel);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Lair.TypeLair.Name, "", "");

            return true;
        }

        internal override void Draw(Graphics g)
        {
            if (Lair.Hidden)
            {
                BitmapList = Program.formMain.ilGui;
                ImageIndex = FormMain.GUI_FLAG_SCOUT;
            }
            else
            {
                BitmapList = Program.formMain.imListObjectsCell;
                ImageIndex = Lair.ImageIndexLair();
            }
            Level = (int)Lair.PriorityFlag + 1;
            Cost = Lair.listAttackedHero.Count();
            ShowCostZero = true;

            base.Draw(g);
        }
    }
}
