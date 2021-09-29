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
        public Item(BigEntity owner, DescriptorItem i, int quantity, Hero signer = null)
        {
            Debug.Assert(i != null);
            Debug.Assert(quantity > 0);

            Owner = owner;
            Signer = signer;
            Descriptor = i;
            Quantity = quantity;
        }

        internal BigEntity Owner { get; private set; }//Владелец предмета (у кого он сейчас находится)
        internal Hero Signer { get; }// Подписант
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
            Program.formMain.formHint.AddStep2Header(Descriptor.Name);
            Program.formMain.formHint.AddStep5Description(Descriptor.Description);
            Program.formMain.formHint.AddStep14PlayerItem(this);
            Program.formMain.formHint.AddStep17Signer(Signer);
            Program.formMain.formHint.AddStep19Descriptors(Modifiers);
            Program.formMain.formHint.AddStep20Perks(Descriptor.Perks);
        }

        internal void AddModificator(DescriptorItem descriptor)
        {
            Debug.Assert(descriptor.CategoryItem == CategoryItem.Enchant);
            Debug.Assert((Descriptor.CategoryItem == CategoryItem.Weapon) || (Descriptor.CategoryItem == CategoryItem.Armour));

            Modifiers.Add(descriptor);
        }
    }
}
