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
        private readonly Label lblQuantity;
        private readonly ImageList imageListItems;

        public PanelItem(Control parent, ImageList ilItems, int numberSlot)
        {
            Debug.Assert(numberSlot >= 0);

            imageListItems = ilItems;
            NumberSlot = numberSlot;

            Parent = parent;
            BorderStyle = BorderStyle.FixedSingle;
            Width = imageListItems.ImageSize.Width + 2;
            Height = imageListItems.ImageSize.Height + 2;

            lblQuantity = new Label()
            {
                Parent = this,
                AutoSize = false,
                Left = 2,
                Top = Height - 20,
                Height = 16,
                Width = Width - 4,
                BackColor = Color.Transparent,
                ForeColor = Color.FromKnownColor(KnownColor.White),
                TextAlign = ContentAlignment.MiddleRight
            };
            lblQuantity.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        }

        internal int NumberSlot { get; }
        internal void ShowItem(PlayerItem pi)
        {
            if (pi != null)
            {
                Debug.Assert(pi.Quantity > 0);

                Image = imageListItems.Images[pi.Item.ImageIndex];

                if (pi.Quantity > 1)
                {
                    lblQuantity.Show();
                    lblQuantity.Text = pi.Quantity.ToString();
                }
                else
                    lblQuantity.Hide();
            }
            else
            {
                Image = null;
                lblQuantity.Hide();
            }
        }
    }
}
