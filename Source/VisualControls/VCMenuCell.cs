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

                Program.formMain.UpdateMenu();
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
            base.Draw(g);

            // Рисуем иконку
            if (research != null)
            {
                BitmapList.DrawImage(g, research.Research.Entity.ImageIndex, ImageState.Normal, Left, Top);

                // Накладываем фильтр
                if (!research.CheckRequirements())
                    g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(3, ImageState.Normal), Left, Top);
                else if (mouseClicked && mouseOver)
                    g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(2, ImageState.Normal), Left, Top);
                else if (mouseOver)
                    g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(1, ImageState.Normal), Left, Top);
                else
                    g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(0, ImageState.Normal), Left, Top);
            }
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
