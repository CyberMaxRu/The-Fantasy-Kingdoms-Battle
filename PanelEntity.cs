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
    internal sealed class PanelEntity : Control
    {
        private readonly ImageList imageListItems;
        private Point pointIcon;
        private Point pointQuantity;
        private PlayerItem playerItem;
        private string quantity;

        public PanelEntity(Control parent, ImageList ilItems, int numberCell)
        {
            Debug.Assert(numberCell >= 0);

            imageListItems = ilItems;
            NumberCell = numberCell;

            Parent = parent;
            Size = Program.formMain.bmpBorderForIcon.Size;

            pointIcon = new Point(3, 1);
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
            if (playerItem != null)
                Program.formMain.formHint.ShowHint(new Point(10 + Parent.Left + Left, Parent.Top + Top + Height),
                    playerItem.Item.Name,
                    "",
                    playerItem.Item.Description,
                    null,
                    0,
                    false, 0,
                    0, false, playerItem);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (playerItem != null)
            {
                e.Graphics.DrawImageUnscaled(imageListItems.Images[playerItem.Item.ImageIndex], pointIcon);
                e.Graphics.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }

            if ((playerItem != null) && (playerItem.Quantity > 1))
            {
                quantity = playerItem.Quantity.ToString();
                pointQuantity.X = Width - (quantity.Length * 12) - 6;
                e.Graphics.DrawString(quantity, Program.formMain.fontQuantity, Program.formMain.brushQuantity, pointQuantity);
            }
        }

        internal int NumberCell { get; }
        internal void ShowItem(PlayerItem pi)
        {
            playerItem = pi;
            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }
    }
}
