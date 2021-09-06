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
    internal sealed class Item : SmallEntity
    {
        public Item(DescriptorItem i, int quantity, bool ownerIsPlayer)
        {
            Debug.Assert(i != null);
            Debug.Assert(quantity > 0);

            Descriptor = i;
            Quantity = quantity;
            OwnerIsPlayer = ownerIsPlayer;
        }

        internal DescriptorItem Descriptor { get; }
        internal int Quantity { get; set; }// Количество предметов
        internal bool OwnerIsPlayer { get; set; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex; 
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override int GetLevel()
        {
            return 0;
        }

        internal override int GetQuantity()
        {
            return !OwnerIsPlayer ? 0 : Quantity == 1 ? 0 : Quantity; 
        }

        internal override string GetCost()
        {
            return null;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Descriptor.Name, "", Descriptor.Description);
            Program.formMain.formHint.AddStep6PlayerItem(this);
        }
    }
}
