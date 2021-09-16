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
        public Item(BigEntity owner, DescriptorItem i, int quantity)
        {
            Debug.Assert(i != null);
            Debug.Assert(quantity > 0);

            Owner = owner;
            Descriptor = i;
            Quantity = quantity;
        }

        internal BigEntity Owner { get; private set; }//Владелец предмета (у кого он сейчас находится)
        internal DescriptorItem Descriptor { get; }
        internal int Quantity { get; set; }// Количество предметов
        internal List<DescriptorItem> Modifiers { get; } = new List<DescriptorItem>();// Модификаторы (зачарование, яды) в предмете (оружие, доспехи)

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override int GetQuantity()
        {
            return Quantity == 1 ? 0 : Quantity;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Descriptor.Name, "", Descriptor.Description);
            Program.formMain.formHint.AddStep6PlayerItem(this);
            Program.formMain.formHint.AddStep9Descriptors(Modifiers);
        }

        internal void AddModificator(DescriptorItem descriptor)
        {
            Debug.Assert(descriptor.CategoryItem == CategoryItem.Enchant);
            Debug.Assert((Descriptor.CategoryItem == CategoryItem.Weapon) || (Descriptor.CategoryItem == CategoryItem.Armour));

            Modifiers.Add(descriptor);
        }
    }
}
