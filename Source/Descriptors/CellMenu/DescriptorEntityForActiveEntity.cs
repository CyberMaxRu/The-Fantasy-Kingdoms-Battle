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
    internal abstract class DescriptorEntityForActiveEntity : DescriptorSmallEntity
    {
        public DescriptorEntityForActiveEntity(DescriptorActiveEntity activeEntity, XmlNode n) : base(n)
        {
            ActiveEntity = activeEntity;

            XmlNode ns = n.SelectSingleNode("Selling");
            if (ns != null)
                Selling = new DescriptorComponentSelling(this, ns);

            ActiveEntity.AddEntity(this);
        }

        public DescriptorEntityForActiveEntity(DescriptorActiveEntity activeEntity, string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {
            ActiveEntity = activeEntity;

            ActiveEntity.AddEntity(this);
        }

        internal DescriptorActiveEntity ActiveEntity { get; }
        internal DescriptorComponentSelling Selling { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Selling?.TuneLinks();
        }
    }
}