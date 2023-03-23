using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - ячейка
    internal class VCCell : VCImage48
    {
        private VCImage img24;

        public VCCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
            HighlightUnderMouse = true;

            DrawState = true;
        }

        internal bool DrawState { get; set; }
        internal int ImageIndex24 { get; set; } = -1;
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
            return ManualSelected || ((Entity != null) && (Entity != null) && Program.formMain.layerGame.PlayerObjectIsSelected(Entity));
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
                    ImageIndex = Entity.GetCellImageIndex();
                    ImageIsEnabled = Entity.GetNormalImage();
                    ImageIndex24 = Entity.GetImageIndex24();
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

            if (Visible && !ManualDraw)
            {
                if (ImageIndex24 != -1)
                {
                    if (img24 is null)
                    {
                        img24 = new VCImage(this, (Width - Program.formMain.BmpListGui24.Size.Width) / 2, (Height - Program.formMain.BmpListGui24.Size.Height) / 2, Program.formMain.BmpListGui24, ImageIndex24);
                        img24.IsActiveControl = false;
                        ArrangeControl(img24);
                    }

                    img24.ImageIndex = ImageIndex24;
                    img24.Draw(g);
                }
            }
        }

        internal override void PaintBorder(Graphics g)
        {
            DrawImage(g, Program.formMain.bmpBorderForIcon, Left - 2, Top - 1);

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
                    DrawImage(g, Program.formMain.bmpEmptyEntity, Left, Top);
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