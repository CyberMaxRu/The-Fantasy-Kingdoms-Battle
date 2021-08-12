using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Интерфейс для работы с ячейкой
    internal interface ICell
    {
        BitmapList BitmapList();
        int ImageIndex();
        bool NormalImage();
        int Level();
        int Quantity();
        string Cost();
        void PrepareHint();
        void Click(VCCell pe);
        void CustomDraw(Graphics g, int x, int y, bool drawState);
    }

    // Визуальный контрол - ячейка
    internal class VCCell : VCImage
    {
        private ICell cell;

        public VCCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.ilItems, -1)
        {
            HighlightUnderMouse = true;
            ShiftImageX = 2;
            ShiftImageY = 0;

            // Ставим размеры после изменения ShiftImageX и ShiftImageY, так так там меняется размер ячейки
            Width = Program.formMain.bmpBorderForIcon.Width;
            Height = Program.formMain.bmpBorderForIcon.Height;

            DrawState = true;
        }

        internal ICell Cell { get => cell; }
        internal bool DrawState { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            Debug.Assert(Width == Program.formMain.bmpBorderForIcon.Width);
            Debug.Assert(Height == Program.formMain.bmpBorderForIcon.Height);
        }

        internal override bool PrepareHint()
        {
            if (cell != null)
            {
                cell.PrepareHint();
                return true;
            }

            return true;
        }

        internal override void DoClick()
        {
            base.DoClick();

            if (AllowClick())
                cell?.Click(this);
        }

        internal void ShowCell(ICell c)
        {
            if (c is PlayerObject po)
                PlayerObject = po;

            //if (!(cell is null) && (cell is PlayerHero ph) && (cell != c))
            //    ph.Selected = false;

            cell = c;
        }

        protected override bool Selected()
        {
            return ManualSelected || ((PlayerObject != null) && (cell != null) && Program.formMain.PlayerObjectIsSelected(PlayerObject));
        }

        protected override bool PlaySelectSound()
        {
            return (cell != null) && base.PlaySelectSound();
        }

        protected virtual int GetLevel()
        {
            return cell.Level(); 
        }

        protected virtual int GetQuantity()
        {
            return cell.Quantity();
        }

        internal override void Draw(Graphics g)
        {
            if (Visible && !ManualDraw)
            {
                if (cell != null)
                {
                    BitmapList = cell.BitmapList();
                    ImageIndex = cell.ImageIndex();
                    ImageIsEnabled = cell.NormalImage();                        
                    Quantity = GetQuantity();
                    Level = GetLevel();
                    Cost = cell.Cost();

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
                if (cell != null)
                {
                    g.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, Left, Top);

                    cell.CustomDraw(g, Left, Top, DrawState);
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