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
    internal sealed class PanelEntity : VisualControl
    {
        private ICell cell;
        private bool lastEmptyCell;
        private int lastImageIndex;
        private bool lastNormal;
        private bool lastSelected;
        private bool lastWithBack;
        private Bitmap bmpImage;

        public PanelEntity(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            //ForeColor = FormMain.Config.CommonQuantity;
            //BackColor = Color.Transparent;
            //TextAlign = ContentAlignment.BottomRight;
            //Padding = new Padding(0, 0, 0, 3);
            //Font = FormMain.Config.FontQuantity;

            Width = Program.formMain.bmpBorderForIcon.Width;
            Height = Program.formMain.bmpBorderForIcon.Height;

            bmpImage = new Bitmap(Width, Height);

        }

        internal override bool PrepareHint()
        {
            if (cell != null)
            {
                cell.PrepareHint();
                return true;
            }

            return false;
        }

        protected void OnMouseLeave(EventArgs e)
        {
            //base.OnMouseLeave(e);

            Program.formMain.formHint.HideHint();
        }

        protected void OnPaint(PaintEventArgs e)
        {
            if (!lastWithBack || (lastEmptyCell != (cell == null)) || ((cell != null) && ((lastImageIndex != cell.ImageIndex()) || (lastNormal != cell.NormalImage()) || (lastSelected != (Program.formMain.SelectedPanelEntity == this)))))
                DrawCell();

            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.DrawImage(bmpImage, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);

            //base.OnPaint(e);
        }

        protected void Dispose(bool disposing)
        {
            //base.Dispose(disposing);

            if (cell != null)
                cell.Panel = null;
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

            //Text = (cell != null) && (cell.Value() > 0) ? c.Value().ToString() : "";

            DrawCell();
            //Invalidate();
        }

        private void DrawCell()
        {
            Graphics g = Graphics.FromImage(bmpImage);

            // Перед отрисовкой надо очистить картинку, чтобы новая не накладывалась на старую
            // Берем картинку с главной формы            
            lastWithBack = Program.formMain.bmpBackground != null;
            //if (lastWithBack)
                //g.DrawImage(Program.formMain.bmpBackground, ClientRectangle, ClientRectangle, GraphicsUnit.Pixel);
//                g.DrawImage(Program.formMain.bmpBackground, ClientRectangle, ClientRectangle, GraphicsUnit.Pixel);
            //else
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
        internal override void Draw(Graphics g)
        {
            //DrawCell();
            g.DrawImageUnscaled(bmpImage, Left, Top);
            //DrawToBitmap(b, new Rectangle(Left + panelAvatar.Left, Top + panelAvatar.Top, panelAvatar.Width, panelAvatar.Height)
        }
    }
}