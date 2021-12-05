using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionProduct : EntityForConstruction
    {
        public ConstructionProduct(Construction construction, DescriptorProduct product) : base(construction, product)
        {
            Product = product;

            if (product.SmallEntity is DescriptorItem di)
                DescriptorItem = di;
            else if (product.SmallEntity is DescriptorGroupItems dgi)
                DescriptorGroupItem = dgi;
            else
                throw new Exception($"Неизвестная сущность: {product.SmallEntity.ID}.");
        }

        internal DescriptorProduct Product { get; }
        internal DescriptorItem DescriptorItem { get; }
        internal DescriptorGroupItems DescriptorGroupItem { get; }
        internal int QuantityPerDay { get; }// Количество товара в сооружении
        internal int Duration { get; private set; }// Длительность нахождения товара в сооружении
        internal int Cost { get; }// Стоимость товара
        internal int Interest { get; set; }// Интерес героев к сущности// !!! Удалить!!! брать из интереса товара

    
        internal int Quantity { get; set; }
        internal int Counter { get; set; }// Счетчик дней товара в сооружении
        internal bool Enabled { get; set; } = true;// Товар доступен для продажи// !!! Делать через 0 В количестве?!!!

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override bool GetNormalImage() => Enabled;

        internal override string GetText()
        {
            if (Duration > 0)
                return Counter.ToString();
            //if (DescriptorAbility != null)
            //    return DescriptorAbility.TypeAbility.ShortName;

//            if (DescriptorItem != null)
                //return DescriptorItem.CategoryItem;
            return base.GetText();
        }

        internal int GetCostGold()
        {
            return Product.Selling.Gold;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            panelHint.AddStep5Description(Product.Description);
            panelHint.AddStep10CostGold(GetCostGold());
            if (DescriptorItem != null)
                panelHint.AddStep9ListNeeds(DescriptorItem.ListNeeds, false);
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
