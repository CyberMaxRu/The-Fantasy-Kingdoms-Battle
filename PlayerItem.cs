using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс предмета у игрока (находящегося на герое или на складе)
    internal sealed class PlayerItem : ICell
    {
        public PlayerItem(Item i, int quantity)
        {
            Debug.Assert(i != null);
            Debug.Assert(quantity > 0);

            Item = i;
            Quantity = quantity;
        }

        internal Item Item { get; }
        internal int Quantity { get; set; }// Количество предметов

        // Реализация интерфейса
        ImageList ICell.ImageList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => Item.ImageIndex;
        int ICell.Level() => 0;
        int ICell.Quantity() => Quantity;
        void ICell.PrepareHint()
        {

        }
    }
}
