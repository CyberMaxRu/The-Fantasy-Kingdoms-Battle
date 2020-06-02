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
        public PlayerItem(Item i, int quantity, bool ownerIsPlayer)
        {
            Debug.Assert(i != null);
            Debug.Assert(quantity > 0);

            Item = i;
            Quantity = quantity;
            OwnerIsPlayer = ownerIsPlayer;
        }

        internal Item Item { get; }
        internal int Quantity { get; set; }// Количество предметов
        internal bool OwnerIsPlayer { get; set; }

        // Реализация интерфейса
        ImageList ICell.ImageList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => Item.ImageIndex;
        int ICell.Value() => !OwnerIsPlayer && Item.TypeItem.Required ? 0 : Quantity;
        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Item.Name, "", Item.Description);
            Program.formMain.formHint.AddStep6PlayerItem(this);
        }
    }
}
