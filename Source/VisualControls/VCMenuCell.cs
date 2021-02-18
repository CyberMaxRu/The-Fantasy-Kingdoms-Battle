using System;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - ячейка меню
    internal sealed class VCMenuCell : VCImage
    {
        private PlayerResearch research;
        private bool mouseOver;
        private bool mouseClicked;

        public VCMenuCell(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY, bitmapList, -1)
        {
        }

        internal override void DoClick()
        {
            base.DoClick();

            if (research.CheckRequirements())
            {
                research.DoResearch();
            }
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(research.Research.Entity.Name, "", research.Research.Entity.Description);
            Program.formMain.formHint.AddStep3Requirement(research.GetTextRequirements());
            Program.formMain.formHint.AddStep4Gold(research.Cost(), research.Cost() <= research.Building.Player.Gold);

            return true;
        }

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            mouseOver = true;
            if (!leftButtonDown)
                mouseClicked = false;
            Program.formMain.ShowFrame();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            mouseOver = false;
            Program.formMain.ShowFrame();
        }

        internal override void MouseDown()
        {
            base.MouseDown();

            mouseClicked = true;
            Program.formMain.ShowFrame();
        }

        internal override void MouseUp()
        {
            base.MouseUp();

            if (mouseClicked != false)
            {
                mouseClicked = false;
                Program.formMain.ShowFrame();
            }
        }

        internal override void Draw(Graphics g)
        {
            if (research != null)
            {
                Cost = research.Cost();
                ImageIndex = research.Research.Entity.ImageIndex;
            }
            else
                ImageIndex = -1;

            if (research != null)
            {
                // Накладываем фильтр
                if (!research.CheckRequirements())
                    ImageFilter = ImageFilter.Disabled;
                else if (mouseClicked && mouseOver)
                    ImageFilter = ImageFilter.Press;
                else if (mouseOver)
                    ImageFilter = ImageFilter.Select;
                else
                    ImageFilter = ImageFilter.Active;
            }
            else
                ImageFilter = ImageFilter.None;

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
                    Cost = research.Cost();
            }
        }
    }
}
