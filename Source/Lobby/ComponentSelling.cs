using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle.Source.Lobby
{
    internal sealed class ComponentSelling
    {
        internal ComponentSelling(DescriptorSelling descriptor)
        {
            Descriptor = descriptor;
        }

        internal DescriptorSelling Descriptor { get; }
        internal int RestQuantity { get; set; }
        internal int CountLife { get; set; }
        internal bool Enabled { get; set; }
        internal int TotalSellQuantity { get; set; }// Количество продаж
        internal int TotalSellGold { get; set; }// Суммма продаж (без налога)
        internal int TotalDuty { get; set; }// Налог с продаж

        internal void Reset()
        {
            RestQuantity = Descriptor.Quantity;
        }
    }
}
