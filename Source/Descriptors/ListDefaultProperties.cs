using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ListDefaultProperties: List<DescriptorCreatureProperty>
    {
        public ListDefaultProperties(XmlNode n)
        {
            foreach (XmlNode nnl in n.SelectNodes("Property"))
            {
                string idProperty = GetStringNotNull(nnl, "ID");
                DescriptorCreatureProperty dcp = new DescriptorCreatureProperty(FormMain.Descriptors.FindPropertyCreature(idProperty), nnl);

                foreach (DescriptorCreatureProperty dcp2 in this)
                {
                    Assert(dcp2.Descriptor.ID != dcp.Descriptor.ID);
                }

                Add(dcp);
            }
        }
    }
}