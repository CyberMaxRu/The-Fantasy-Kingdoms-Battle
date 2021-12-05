using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class ConstructionEvent : ConstructionVisit
    {
        public ConstructionEvent(Construction construction, DescriptorConstructionEvent ce) : base(construction, ce)
        {
            DescriptorConstructionEvent = ce;
        }

        internal DescriptorConstructionEvent DescriptorConstructionEvent { get; }
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
            return 0;// DescriptorConstructionEvent.co.;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(DescriptorConstructionEvent.NameGoods, GetImageIndex());
            panelHint.AddStep3Type("Мероприятие");
            panelHint.AddStep4Level(Duration > 0 ? $"Осталось дней: {Counter}" : "");
            panelHint.AddStep5Description(Descriptor.Description);
            panelHint.AddStep9ListNeeds(DescriptorConstructionEvent.ListNeeds, true);
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
