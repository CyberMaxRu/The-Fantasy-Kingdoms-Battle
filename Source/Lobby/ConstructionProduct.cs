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
        }

        internal DescriptorProduct Product { get; }
        internal DescriptorAbility DescriptorAbility { get; }
        internal DescriptorItem DescriptorItem { get; }
        internal DescriptorGroupItems DescriptorGroupItem { get; }
        internal DescriptorConstructionEvent DescriptorConstructionEvent { get; }
        internal DescriptorConstructionVisit DescriptorConstructionVisit { get; }
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
            return Product.Cost;
        }

        internal override void PrepareHint()
        {
            if (DescriptorConstructionEvent != null)
            {
                Program.formMain.formHint.AddStep2Header(DescriptorConstructionEvent.NameGoods, GetImageIndex());
                Program.formMain.formHint.AddStep3Type("Мероприятие");
                Program.formMain.formHint.AddStep4Level(Duration > 0 ? $"Осталось дней: {Counter}" : "");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionEvent.ListNeeds, true);
            }
            else
            {
                Program.formMain.formHint.AddStep2Header(Descriptor.Name, GetImageIndex());
                if (DescriptorAbility != null)
                    Program.formMain.formHint.AddStep3Type(DescriptorAbility.TypeAbility.Name);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep3Type("Посещение");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                Program.formMain.formHint.AddStep10CostGold(GetCostGold());
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, false);
                if (DescriptorItem != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorItem.ListNeeds, false);
                if (DescriptorConstructionEvent != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, false);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionVisit.ListNeeds, false);
            }
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
