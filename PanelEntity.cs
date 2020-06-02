using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Интерфейс для работы с ячейкой
    internal interface ICell
    {
        ImageList ImageList();
        int ImageIndex();
        int Value();        
        void PrepareHint();
    }

    // Класс панели для рисования иконки объекта, поддерживающего интерфейс ячейки
    internal sealed class PanelEntity : Label
    {
        private Point pointIcon;
        private ICell cell;

        public PanelEntity()
        {
            Size = Program.formMain.bmpBorderForIcon.Size;
            BackColor = Color.Transparent;
            ForeColor = Program.formMain.ColorQuantity;
            Font = Program.formMain.fontQuantity;
            TextAlign = ContentAlignment.BottomRight;
            Padding = new Padding(0, 0, 0, 3);
            
            pointIcon = new Point(3, 2);

            ShowHint = true;
        }

        internal bool ShowHint { get; set; }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if ((cell != null) && (ShowHint == true))
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

        protected override void OnPaint(PaintEventArgs e)
        {
            if (cell != null)
            {
                e.Graphics.DrawImageUnscaled(cell.ImageList().Images[cell.ImageIndex()], pointIcon);
                e.Graphics.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }
            else
                e.Graphics.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

            base.OnPaint(e);
        }

        internal int NumberCell { get; }

        internal void ShowCell(ICell c)
        {
            cell = c;

            Text = (cell != null) && (cell.Value() > 0) ? c.Value().ToString() : "";

            Invalidate();
        }
    }
}