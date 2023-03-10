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
    internal sealed class ListBaseResources : List<int>
    #pragma warning restore CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    {
        public ListBaseResources() : base(FormMain.Descriptors.BaseResources.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.BaseResources.Count; i++)  
                Add(0);
        }

        public ListBaseResources(XmlNode n) : base(FormMain.Descriptors.BaseResources.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.BaseResources.Count; i++)
                Add(0);

            if (n != null)
            {
                DescriptorBaseResource res;
                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    res = FormMain.Descriptors.FindBaseResource(n.ChildNodes[i].Name);
                    Debug.Assert(this[res.Number] == 0);
                    Debug.Assert(value >= 0);

                    this[res.Number] = value;
                }
            }
        }

        public ListBaseResources(ListBaseResources qbr) : base(FormMain.Descriptors.BaseResources.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.BaseResources.Count; i++)
                Add(0);

            for (int i = 0; i < Count; i++)
            {
                this[i] = qbr[i];
            }    
        }

        public ListBaseResources(int gold) : base(FormMain.Descriptors.BaseResources.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.BaseResources.Count; i++)
                Add(0);

            this[0] = gold;
        }

        internal void AddResources(ListBaseResources qbr)
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

        internal int Gold { get => this[0]; set { this[0] = value; } }

        internal bool ResourcesEnough(ListBaseResources listOther)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] < listOther[i])
                    return false;
            }

            return true;
        }

        internal void ToZero()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] = 0;
            }
        }

        public override bool Equals(object obj)
        {
            ListBaseResources qbr = obj as ListBaseResources;

            for (int i = 0; i < Count; i++)
            {
                if (this[i] != qbr[i])
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            throw new Exception("Нельзя список преобразовать в строку.");
        }
    }
}
