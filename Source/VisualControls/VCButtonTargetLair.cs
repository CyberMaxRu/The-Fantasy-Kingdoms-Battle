using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCButtonTargetLair : VCIconButton
    {
        public VCButtonTargetLair(VisualControl parent) : base(parent, 0, 0, Program.formMain.imListObjectsCell, -1)
        {

        }

        internal PlayerLair Lair { get; set; }

        internal override void DoClick()
        {
            base.DoClick();

            Program.formMain.ActivatePageLairs();
            Program.formMain.SelectPlayerObject(Lair);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddHeader(Lair != null ? Lair.TypeLair.Name : "Флаг не назначен");

            return true;
        }

        internal override void Draw(Graphics g)
        {
            if (Lair == null)
            {
                BitmapList = Program.formMain.imListObjectsCell;
                ImageIndex = FormMain.IMAGE_INDEX_NONE;
                ImageIsEnabled = false;
                Level = 0;
                Cost = 0;
                ShowCostZero = false;
            }
            else
            {
                ImageIsEnabled = true;

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
            }

            base.Draw(g);
        }
    }
}
