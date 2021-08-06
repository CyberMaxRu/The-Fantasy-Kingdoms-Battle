using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
            if ((Lair is null) || (Lair.listAttackedHero.Count == 0))
            { 
                Program.formMain.formHint.AddHeader(Lair != null ? Lair.NameLair() : "Флаг не назначен");
            }
            else
            {
                Debug.Assert(!(Lair is null));
                Program.formMain.formHint.AddStep1Header(Lair.NameLair(), "", Lair.ListHeroesForHint());
            }
            
            return true;
        }

        /*protected override int GetLevel()
        {
            return Level = (int)Lair.PriorityFlag + 1;
        }*/

        protected override int GetQuantity()
        {
            return Lair != null ? Lair.listAttackedHero.Count() : 0;
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
            }

            base.Draw(g);
        }

        protected override bool Selected()
        {
            return (Lair != null) && Program.formMain.PlayerObjectIsSelected(Lair);
        }
    }
}
