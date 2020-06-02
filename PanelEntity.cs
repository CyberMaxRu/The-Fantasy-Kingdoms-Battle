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
    // Класс панели для рисования иконки объекта, поддерживающего интерфейс ячейки
    internal sealed class PanelEntity : Label
    {
        private Point pointIcon;
        private Point pointQuantity;
        private PlayerItem playerItem;
        private Entity entity;
        private string quantity;

        public PanelEntity()
        {
            Size = Program.formMain.bmpBorderForIcon.Size;
            BackColor = Color.Transparent;
            ForeColor = Program.formMain.ColorLevel;
            Font = Program.formMain.fontLevel;
            TextAlign = ContentAlignment.TopRight;

            pointIcon = new Point(3, 2);
            pointQuantity = new Point(2, Height - 20);
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
            if (entity != null)
            {
                e.Graphics.DrawImageUnscaled(Program.formMain.ilItems.Images[entity.ImageIndex], pointIcon);
                e.Graphics.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }
            else
                e.Graphics.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

            if ((playerItem != null) && (playerItem.Quantity > 1))
            {
                quantity = playerItem.Quantity.ToString();
                pointQuantity.X = Width - (quantity.Length * 12) - 6;
                e.Graphics.DrawString(quantity, Program.formMain.fontQuantity, Program.formMain.brushQuantity, pointQuantity);
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
    }
}
