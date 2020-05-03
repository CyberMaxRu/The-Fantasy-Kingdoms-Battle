using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс ячейки с предметами
    internal sealed class PanelItem : PictureBox
    {
        private readonly Label lblQuantity;
        private readonly ImageList imageListItems;

        public PanelItem(Control parent, int left, int top, ImageList ilItems, int numberSlot)
        {
            imageListItems = ilItems;
            NumberSlot = numberSlot;

            Parent = parent;
            BorderStyle = BorderStyle.FixedSingle;
            Left = left;
            Top = top;
            Width = imageListItems.ImageSize.Width + 2;
            Height = imageListItems.ImageSize.Height + 2;

            SendToBack();
        }

        internal int NumberSlot { get; }
        internal void ShowItem(PlayerItem pi)
        {
            if (pi != null)
                Image = imageListItems.Images[pi.Item.ImageIndex];
            else
                Image = null;
        }
    }
}
