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
        int Level();
        int Quantity();
        void PrepareHint();
    }

    // Класс панели для рисования иконки объекта, поддерживающего интерфейс ячейки
    internal sealed class PanelEntity : Label
    {
        private Point pointIcon;
        private RectangleF rectQuantity;
        private StringFormat strFormatQuantity = new StringFormat();
        private PlayerItem playerItem;
        private Entity entity;
        private string quantity;
        private ICell cell;

        public PanelEntity()
        {
            Size = Program.formMain.bmpBorderForIcon.Size;
            BackColor = Color.Transparent;
            ForeColor = Program.formMain.ColorLevel;
            Font = Program.formMain.fontLevel;
            TextAlign = ContentAlignment.TopRight;

            pointIcon = new Point(3, 2);
            rectQuantity = new RectangleF(4, Height - 24, Width - 8, 24);
            strFormatQuantity.Alignment = StringAlignment.Far;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (entity != null)
                Program.formMain.formHint.ShowHint(this,
                    entity.Name,
                    "",
                    entity.Description,
                    null,
                    0,
                    false, 0,
                    0, false, playerItem);
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
            else if (entity != null)
            {
                e.Graphics.DrawImageUnscaled(Program.formMain.ilItems.Images[entity.ImageIndex], pointIcon);
                e.Graphics.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }
            else
                e.Graphics.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

            if ((cell != null) && (cell.Quantity() > 0))
            {
                quantity = cell.Quantity().ToString();
                e.Graphics.DrawString(quantity, Program.formMain.fontQuantity, Program.formMain.brushQuantity, rectQuantity, strFormatQuantity);
            }

            base.OnPaint(e);
        }

        internal int NumberCell { get; }
        internal void ShowPlayerItem(PlayerItem pi)
        {
            playerItem = pi;
            entity = pi?.Item;
            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }

        internal void ShowEntity(Entity e)
        {
            playerItem = null;
            entity = e;

            //Text = e != null ? "1" : "";

            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }

        internal void ShowCell(ICell c)
        {
            cell = c;
            Text = c != null ? c.Level().ToString() : "";
            Invalidate();
        }
    }
}