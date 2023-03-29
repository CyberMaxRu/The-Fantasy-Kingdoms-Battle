using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс типа традиции
    internal sealed class DescriptorTypeTradition : DescriptorVisual
    {
        public DescriptorTypeTradition(XmlNode n) : base(n)
        {
            Index = FormMain.Descriptors.TypeTraditions.Count;
        }

        internal int Index { get; }
        protected override int ShiftImageIndex() => Config.ImageIndexFirstItems;
    }
}
