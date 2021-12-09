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

            DrawState = true;
        }

        internal bool DrawState { get; set; }
        internal string HintForEmpty { get; set; }

        internal override bool PrepareHint()
        {
            if (Entity != null)
            {
                Entity.PrepareHint(PanelHint);
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

        protected override void SetEntity(Entity po)
        {
            base.SetEntity(po);

            //if (!(cell is null) && (cell is PlayerHero ph) && (cell != c))
            //    ph.Selected = false;

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
            ShowBorder = Entity != null;

            if (Visible && !ManualDraw)
            {
                if (Entity != null)
                {
                    ImageIndex = Entity.GetImageIndex();
                    ImageIsEnabled = Entity.GetNormalImage();                        
                    Quantity = Entity.GetQuantity();
                    Level = Entity.GetLevel();
                    LowText = Entity.GetText();

                    Debug.Assert(BitmapList.Size.Width == 48);
                    Debug.Assert(BitmapList.Size.Height == 48);
                }
                else
                    ImageIndex = -1;
            }

            //if (!(cell is null) && (cell is PlayerHero ph))
            //    ManualSelected = !MouseOver && ph.Selected;

            base.Draw(g);

        }

        internal override void PaintBorder(Graphics g)
        {
            g.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, Left - 2, Top - 1);

            Entity?.CustomDraw(g, Left, Top, DrawState);
        }

        internal override void PaintForeground(Graphics g)
        {
            base.PaintForeground(g);

            if (Visible)
            {
                if (Entity != null)
                {
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