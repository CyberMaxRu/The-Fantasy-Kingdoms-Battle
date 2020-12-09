using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fantasy_King_s_Battle
{
    // Интерфейс для работы с ячейкой
    internal interface ICell
    {
        PanelEntity Panel { get; set; }
        ImageList ImageList();
        int ImageIndex();
        bool NormalImage();
        int Value();        
        void PrepareHint();
        void Click(PanelEntity pe);
    }

    // Класс панели для рисования иконки объекта, поддерживающего интерфейс ячейки
    internal sealed class PanelEntity : Label
    {
        private ICell cell;
        private bool lastEmptyCell;
        private int lastImageIndex;
        private bool lastNormal;
        private bool lastSelected;
        private Bitmap bmpImage;

        public PanelEntity()
        {
            Size = Program.formMain.bmpBorderForIcon.Size;
            ForeColor = FormMain.Config.CommonQuantity;
            BackColor = Color.Transparent;
            TextAlign = ContentAlignment.BottomRight;
            Padding = new Padding(0, 0, 0, 3);
            Font = FormMain.Config.FontQuantity;

            ShowHint = true;

            bmpImage = new Bitmap(Size.Width, Size.Height);
        }

        internal bool ShowHint { get; set; }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if ((cell != null) && ShowHint)
            {
                Program.formMain.formHint.Clear();
                cell.PrepareHint();
                Program.formMain.formHint.ShowHint(this);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Program.formMain.formHint.HideHint();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            //if ((lastEmptyCell != (cell == null)) || ((cell != null) && ((lastImageIndex != cell.ImageIndex()) || (lastNormal != cell.NormalImage()) || (lastSelected != (Program.formMain.SelectedPanelEntity == this)))))
                DrawCell();

            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.DrawImageUnscaled(bmpImage, e.ClipRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (cell != null)
                cell.Panel = null;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

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

            Text = (cell != null) && (cell.Value() > 0) ? c.Value().ToString() : "";

            DrawCell();
            Invalidate();
        }

        private void DrawCell()
        {
            Graphics g = Graphics.FromImage(bmpImage);

            // Перед отрисовкой надо очистить картинку, чтобы новая не накладывалась на старую
            g.Clear(Color.Transparent);
            
            if (cell != null)
            {
                //Debug.Assert(cell.Panel == this);

                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(cell.ImageList(), cell.ImageIndex(), cell.NormalImage()), FormMain.Config.ShiftForBorder);

                if (Program.formMain.SelectedPanelEntity == this)
                {
                    g.DrawImage(Program.formMain.ilMenuCellFilters.Images[2], new Rectangle(0, 0, Width - 1, Height - 1));
                    g.DrawImage(Program.formMain.ilMenuCellFilters.Images[2], new Rectangle(0, 0, Width - 1, Height - 1));
                    g.DrawImage(Program.formMain.ilMenuCellFilters.Images[2], new Rectangle(0, 0, Width - 1, Height - 1));
                    g.DrawImage(Program.formMain.ilMenuCellFilters.Images[2], new Rectangle(0, 0, Width - 1, Height - 1));
                }

                g.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }
            else
                g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

            g.Dispose();

            lastEmptyCell = cell == null;
            if (!lastEmptyCell)
            {
                lastImageIndex = cell.ImageIndex();
                lastNormal = cell.NormalImage();
                lastSelected = Program.formMain.SelectedPanelEntity == this;
            }
        }
    }
}