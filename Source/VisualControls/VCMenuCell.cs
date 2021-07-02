using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - ячейка меню
    internal sealed class VCMenuCell : VCImage
    {
        private PlayerResearch research;

        public VCMenuCell(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY, bitmapList, -1)
        {
            UseFilter = true;
        }

        internal override void DoClick()
        {
            base.DoClick();

            Debug.Assert(research != null);
            if (research.CheckRequirements())
            {
                Program.formMain.PlayPushButton();
                research.DoResearch();
            }
        }

        internal override bool PrepareHint()
        {
            // После клика на ячейке меню, надо перепоказать подсказку
            // Если на ячейке исследования больше нет, то сообщаем, что подсказки нет
            if (research != null)
            {
                Program.formMain.formHint.AddStep1Header(research.Research.Entity.Name, "", research.Research.Entity.Description);
                Program.formMain.formHint.AddStep3Requirement(research.GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(research.Cost(), research.Cost() <= research.Construction.Player.Gold);

                return true;
            }

            return false;
        }

        internal override void Draw(Graphics g)
        {
            if (research != null)
            {
                Cost = research.Cost().ToString();
                ImageIndex = research.Research.Entity.ImageIndex;
                ImageIsEnabled = research.CheckRequirements();

                // Накладываем фильтр
                //if (!research.CheckRequirements())
                //    ImageFilter = ImageFilter.Disabled;
            }
            else
            {
                ImageIndex = -1;
                ImageFilter = ImageFilter.None;
            }

            base.Draw(g);
        }

        internal PlayerResearch Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                    Cost = research.Cost().ToString();
            }
        }
    }
}
