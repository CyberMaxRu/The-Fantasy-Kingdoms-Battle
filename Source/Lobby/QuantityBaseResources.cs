using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    #pragma warning disable CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    internal sealed class QuantityBaseResources : List<int>
    #pragma warning restore CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    {
        public QuantityBaseResources() : base(FormMain.Config.BaseResources.Count)
        {
            for (int i = 0; i < Capacity; i++)
                Add(0);
        }


        public QuantityBaseResources(XmlNode n) : base(FormMain.Config.BaseResources.Count)
        {
            for (int i = 0; i < Capacity; i++)
                Add(0);

            if (n != null)
            {
                DescriptorBaseResource res;
                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    res = FormMain.Config.FindBaseResource(n.ChildNodes[i].Name);
                    Debug.Assert(this[res.Number] == 0);
                    Debug.Assert(value >= 0);

                    this[res.Number] = value;
                }
            }
        }

        public QuantityBaseResources(QuantityBaseResources qbr) : base(FormMain.Config.BaseResources.Count)
        {
            for (int i = 0; i < Capacity; i++)
                Add(0);

            for (int i = 0; i < Count; i++)
            {
                this[i] = qbr[i];
            }    
        }


        internal int Quantity(DescriptorBaseResource res)
        {
            return this[res.Number];
        }

        internal void ChangeQuantity(DescriptorBaseResource res, int value)
        {
            this[res.Number] += value;

            Debug.Assert(this[res.Number] >= 0);
        }

        internal void AddResources(QuantityBaseResources qbr)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] += qbr[i];
            }
        }

        internal bool ExistsResources()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] != 0)
                    return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            QuantityBaseResources qbr = obj as QuantityBaseResources;

            for (int i = 0; i < Count; i++)
            {
                if (this[i] != qbr[i])
                    return false;
            }

            return true;
        }
    }
}
