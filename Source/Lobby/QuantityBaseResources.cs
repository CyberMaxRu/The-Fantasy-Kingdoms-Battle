using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class QuantityBaseResources
    {
        private int[] quantity;

        public QuantityBaseResources(XmlNode n)
        {
            CountResources = FormMain.Config.BaseResources.Count;
            quantity = new int[CountResources];

            if (n != null)
            {
                DescriptorBaseResource res;
                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    res = FormMain.Config.FindBaseResource(n.ChildNodes[i].Name);
                    Debug.Assert(quantity[res.Number] == 0);
                    Debug.Assert(value >= 0);

                    quantity[res.Number] = value;
                }
            }
        }

        public QuantityBaseResources(QuantityBaseResources qbs)
        {
            CountResources = FormMain.Config.BaseResources.Count;
            quantity = new int[CountResources];

            for (int i = 0; i < CountResources; i++)
            {
                quantity[i] = qbs.quantity[i];
            }    
        }

        internal int CountResources { get; }

        internal int Quantity(DescriptorBaseResource res)
        {
            return quantity[res.Number];
        }

        internal void ChangeQuantity(DescriptorBaseResource res, int value)
        {
            quantity[res.Number] += value;

            Debug.Assert(quantity[res.Number] >= 0);
        }
    }
}
