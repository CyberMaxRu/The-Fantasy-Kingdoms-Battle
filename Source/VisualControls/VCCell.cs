using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - ячейка
    internal class VCCell : VCImage48
    {
        public VCCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
            HighlightUnderMouse = true;
            ShiftImageX = 2;
            ShiftImageY = 0;

            // Ставим размеры после изменения ShiftImageX и ShiftImageY, так так там меняется размер ячейки
            Width = Program.formMain.bmpBorderForIcon.Width;
            Height = Program.formMain.bmpBorderForIcon.Height;

            DrawState = true;
        }

        internal bool DrawState { get; set; }
        internal string HintForEmpty { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            Debug.Assert(Width == Program.formMain.bmpBorderForIcon.Width);
            Debug.Assert(Height == Program.formMain.bmpBorderForIcon.Height);
        }

        internal override bool PrepareHint()
        {
            if (Entity != null)
            {
                Entity.PrepareHint();
                return true;
            }

            return true;
        }

        internal override void DoClick()
        {
            base.DoClick();

            if (AllowClick())
                Entity?.Click(this);
        }

        internal void ShowCell(Entity c)
        {

            //if (!(cell is null) && (cell is PlayerHero ph) && (cell != c))
            //    ph.Selected = false;

            Entity = c;

            if (Entity is null)
                Hint = HintForEmpty;
        }

        protected override bool Selected()
        {
            return ManualSelected || ((Entity != null) && (Entity != null) && Program.formMain.PlayerObjectIsSelected(Entity));
        }

        protected override bool PlaySelectSound()
        {
            return (Entity != null) && base.PlaySelectSound();
        }

        internal override void Draw(Graphics g)
        {
            if (Visible && !ManualDraw)
            {
                if (Entity != null)
                {
                    ImageIndex = Entity.GetImageIndex();
                    ImageIsEnabled = Entity.GetNormalImage();                        
                    Quantity = Entity.GetQuantity();
                    Level = Entity.GetLevel();
                    Cost = Entity.GetCost();

                    Debug.Assert(BitmapList.Size == 48);
                }
                else
                    ImageIndex = -1;
            }

            //if (!(cell is null) && (cell is PlayerHero ph))
            //    ManualSelected = !MouseOver && ph.Selected;

            base.Draw(g);

            if (Visible)
            {
                if (Entity != null)
                {
                    g.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, Left, Top);

                    Entity.CustomDraw(g, Left, Top, DrawState);
                }
                else
                    g.DrawImageUnscaled(Program.formMain.bmpEmptyEntity, Left, Top);
            }
        }

        /*
        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            if (!(cell is null) && (cell is PlayerHero ph))
            {
                ph.Selected = true;
            }
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            if (!(cell is null) && (cell is PlayerHero ph))
            {
                ph.Selected = false;
            }
        }
        */
    }
}