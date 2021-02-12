using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Интерфейс для работы с ячейкой
    internal interface ICell
    {
        VCCell Panel { get; set; }
        BitmapList BitmapList();
        int ImageIndex();
        bool NormalImage();
        int Value();
        void PrepareHint();
        void Click(VCCell pe);
    }

    // Визуальный контрол - ячейка
    internal sealed class VCCell : VCImage
    {
        private ICell cell;

        public VCCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.ilItems, -1)
        {
            Width = Program.formMain.bmpBorderForIcon.Width;
            Height = Program.formMain.bmpBorderForIcon.Height;

            HighlightUnderMouse = true;
            ShiftImageX = 3;
            ShiftImageY = 2;
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

            if (cell != null)
            {
                //Debug.Assert(cell.Panel == this);
                cell?.Click(this);
            }
        }

        internal void ShowCell(ICell c)
        {
            if (cell != null)
                cell.Panel = null;

            cell = c;
            if (cell != null)
                cell.Panel = this;
        }

        internal override void Draw(Graphics g)
        {
            if (cell != null)
            {
                BitmapList = cell.BitmapList();
                ImageIndex = cell.ImageIndex();
                //ImageState = cell.NormalImage() ? ImageState.Normal : ImageState.Disabled;
                Quantity = cell.Value();

                Debug.Assert(BitmapList.Size == 48);
            }
            else
                ImageIndex = -1;

            base.Draw(g);

            if (cell != null)
            {
                if (Program.formMain.SelectedPanelEntity == this)
                    Program.formMain.ilMenuCellFilters.DrawImage(g, 2, ImageState.Normal, Left, Top);

                g.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, Left, Top);
            }
            else
                g.DrawImageUnscaled(Program.formMain.bmpEmptyEntity, Left, Top);
        }
    }
}