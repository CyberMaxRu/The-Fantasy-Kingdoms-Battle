using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal class DescriptorEntityForActiveEntity : DescriptorSmallEntity
    {
        public DescriptorEntityForActiveEntity(XmlNode n) : base(n)
        {
            XmlNode ns = n.SelectSingleNode("Selling");
            if (ns != null)
                Selling = new DescriptorComponentSelling(this, ns);
        }

        public DescriptorEntityForActiveEntity(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {
        }

        internal DescriptorComponentSelling Selling { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Selling?.TuneLinks();
        }
    }
}