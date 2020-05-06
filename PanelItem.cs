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
    // Класс ячейки с предметами
    internal sealed class PanelItem : PictureBox
    {
        private readonly ImageList imageListItems;
        private Point pointQuantity;
        private PlayerItem playerItem;
        private string quantity;

        public PanelItem(Control parent, ImageList ilItems, int numberCell)
        {
            Debug.Assert(numberCell >= 0);

            imageListItems = ilItems;
            NumberCell = numberCell;

            Parent = parent;
            BorderStyle = BorderStyle.FixedSingle;
            Width = imageListItems.ImageSize.Width + 2;
            Height = imageListItems.ImageSize.Height + 2;

            Paint += PanelItem_Paint;

            pointQuantity = new Point(2, Height - 20);
        }

        private void PanelItem_Paint(object sender, PaintEventArgs e)
        {
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
            Image = pi != null ? imageListItems.Images[pi.Item.ImageIndex] : null;
        }
    }
}
