using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle.Source.Lobby
{
    internal sealed class ComponentSelling
    {
        internal ComponentSelling(DescriptorComponentSelling descriptor)
        {
            Descriptor = descriptor;
        }

        internal DescriptorComponentSelling Descriptor { get; }
        internal int RestQuantity { get; set; }// Осталось количество товара/услуг
        internal int CountLife { get; set; }// Счетчик дней жизни товара/услуги
        internal bool Enabled { get; set; }// Товар/услуга разрешены к продаже
        internal int TotalSellQuantity { get; set; }// Всего количество продаж
        internal int TotalSellGold { get; set; }// Суммма продаж (без налога)
        internal int TotalDuty { get; set; }// Всего начислено налога с продаж

        internal void Reset()
        {
            RestQuantity = Descriptor.Quantity;
        }
    }
}