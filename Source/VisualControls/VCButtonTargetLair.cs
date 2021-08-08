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
            ManualDraw = true;
        }

        internal PlayerConstruction Lair { get => Cell as PlayerConstruction; }

        internal override void DoClick()
        {
            base.DoClick();

            if ((Lair != null) && (Lair.TypeFlag != TypeFlag.Battle))
            {
                Program.formMain.ActivatePageLairs();
                Program.formMain.SelectPlayerObject(Lair);
            }
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
                if (Lair.TypeFlag != TypeFlag.Battle)
                    Program.formMain.formHint.AddStep1Header(Lair.NameLair(), "", Lair.ListHeroesForHint());
                else
                    Program.formMain.formHint.AddStep1Header("Битва против игрока", "", Lair.ListHeroesForHint());
            }

            return true;
        }

        /*protected override int GetLevel()
        {
            return Level = (int)Lair.PriorityFlag + 1;
        }*/

        internal override void Draw(Graphics g)
        {
            if (Lair is null)
            {
                BitmapList = Program.formMain.imListObjectsCell;
                ImageIndex = FormMain.IMAGE_INDEX_NONE;
                ImageIsEnabled = false;
                Level = 0;
                Quantity = 0;
                Cost = null;
            }
            else
            {
                ImageIsEnabled = true;
                Quantity = Lair.listAttackedHero.Count();

                if (Lair.TypeFlag != TypeFlag.Battle)
                {
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
                else
                {
                    BitmapList = Program.formMain.ilGui;
                    ImageIndex = FormMain.GUI_BATTLE_2;
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
