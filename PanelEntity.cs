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
        private readonly ImageList imageListItems;
        private Point pointIcon;
        private Point pointQuantity;
        private PlayerItem playerItem;
        private Item item;
        private Ability ability;
        private string quantity;

        public PanelEntity(Control parent, ImageList ilItems, int numberCell)
        {
            Debug.Assert(numberCell >= 0);

            imageListItems = ilItems;
            NumberCell = numberCell;

            Parent = parent;
            Size = Program.formMain.bmpBorderForIcon.Size;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

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
            if (item != null)
                Program.formMain.formHint.ShowHint(this,
                    item.Name,
                    "",
                    item.Description,
                    null,
                    0,
                    false, 0,
                    0, false, playerItem);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (item != null)
            {
                e.Graphics.DrawImageUnscaled(imageListItems.Images[item.ImageIndex], pointIcon);
                e.Graphics.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }
            if (ability != null)
            {
                e.Graphics.DrawImageUnscaled(imageListItems.Images[ability.ImageIndex], pointIcon);
                e.Graphics.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            }
            else
                e.Graphics.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 1, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

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
            item = pi?.Item;
            ability = null;
            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }

        internal void ShowItem(Item i)
        {
            playerItem = null;
            item = i;
            ability = null;
            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }

        private void ShowAbility(Ability a)
        {
            playerItem = null;
            item = null;
            ability = a;
            Invalidate();
            //Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }

        internal void ShowEntity(Entity i)
        {
            if (i is null)
            {
                playerItem = null;
                item = null;
                ability = null;
                Invalidate();
            }
            else if (i is Item item1)
                ShowItem(item1);
            else if (i is Ability ability1)
                ShowAbility(ability1);
            else
                throw new Exception("не поддерживается.");
        }
    }
}
