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
    // Класс панели для рисования иконки сущности
    internal sealed class PanelEntity : PictureBox
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
            DoubleBuffered = true;

            pointIcon = new Point(3, 2);
            pointQuantity = new Point(2, Height - 20);

            MouseEnter += PanelItem_MouseEnter;
            MouseLeave += PanelItem_MouseLeave;
        }

        private void PanelItem_MouseLeave(object sender, EventArgs e)
        {
            Program.formMain.formHint.HideHint();
        }

        private void PanelItem_MouseEnter(object sender, EventArgs e)
        {
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

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
            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }
    }
}
