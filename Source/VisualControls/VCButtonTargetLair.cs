using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCButtonTargetLair : VCCell
    {
        public VCButtonTargetLair(VisualControl parent) : base(parent, 0, 0)
        {
            BitmapList = Program.formMain.imListObjectsCell;
        }

        internal PlayerLair Lair { get => Cell as PlayerLair; }

        internal override void DoClick()
        {
            base.DoClick();

            Program.formMain.ActivatePageLairs();
            if (Lair != null)
                Program.formMain.SelectPlayerObject(Lair);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddHeader(Lair != null ? Lair.NameLair() : "Флаг не назначен");

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
                Cost = null;
            }
            else
            {
                ImageIsEnabled = true;

                if (Lair.Hidden)
                {
                    BitmapList = Program.formMain.ilGui;
                    ImageIndex = FormMain.IMAGE_INDEX_UNKNOWN;
                }
                else
                {
                    BitmapList = Program.formMain.imListObjectsCell;
                    ImageIndex = Lair.ImageIndexLair();
                }
                Level = (int)Lair.PriorityFlag + 1;
                Cost = Lair.listAttackedHero.Count().ToString();
            }

            base.Draw(g);
        }

        protected override bool Selected()
        {
            return (Lair != null) && Program.formMain.PlayerObjectIsSelected(Lair);
        }
    }
}
