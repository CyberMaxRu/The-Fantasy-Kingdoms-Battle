using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal class DescriptorTradition : DescriptorWithID
    {
        public DescriptorTradition(XmlNode n) : base(n)
        {
            TypeTradition = Descriptors.FindTypeTradition(XmlUtils.GetStringNotNull(n, "TypeTradition"));
        }

        internal DescriptorTypeTradition TypeTradition { get; }
    }
}
