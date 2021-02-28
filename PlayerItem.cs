using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
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
        VCCell ICell.Panel { get; set; }
        BitmapList ICell.BitmapList() => Program.formMain.ilItems;
        bool ICell.NormalImage() => true;
        int ICell.ImageIndex() => Item.ImageIndex;
        int ICell.Value() => !OwnerIsPlayer ? 0 : Quantity;

        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Item.Name, "", Item.Description);
            Program.formMain.formHint.AddStep6PlayerItem(this);
        }

        void ICell.Click(VCCell pe)
        {

        }

        void ICell.CustomDraw(Graphics g) { }
    }
}
