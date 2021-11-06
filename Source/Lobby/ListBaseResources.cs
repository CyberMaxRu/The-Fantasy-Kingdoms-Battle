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
    internal sealed class ListBaseResources : List<BaseResource>
    #pragma warning restore CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    {
        public ListBaseResources() : base(FormMain.Config.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Config.BaseResources)
                Add(new BaseResource(br));
        }

        public ListBaseResources(XmlNode n) : base(FormMain.Config.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Config.BaseResources)
                Add(new BaseResource(br));

            if (n != null)
            {
                DescriptorBaseResource res;
                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    res = FormMain.Config.FindBaseResource(n.ChildNodes[i].Name);
                    Debug.Assert(this[res.Number].Quantity == 0);
                    Debug.Assert(value >= 0);

                    this[res.Number].Quantity = value;
                }
            }
        }

        public ListBaseResources(ListBaseResources qbr) : base(FormMain.Config.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Config.BaseResources)
                Add(new BaseResource(br));

            for (int i = 0; i < Count; i++)
            {
                this[i].Quantity = qbr[i].Quantity;
            }    
        }

        public ListBaseResources(int gold) : base(FormMain.Config.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Config.BaseResources)
                Add(new BaseResource(br));

            this[FormMain.Config.Gold.Number].Quantity = gold;
        }

        internal void AddResources(ListBaseResources qbr)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].Quantity += qbr[i].Quantity;
            }
        }

        internal bool ExistsResources()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Quantity != 0)
                    return true;
            }

            return false;
        }

        internal int ValueGold()
        {
            return this[FormMain.Config.Gold.Number].Quantity;
        }

        internal bool ResourcesEnough(ListBaseResources listOther)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Quantity < listOther[i].Quantity)
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ListBaseResources qbr = obj as ListBaseResources;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Quantity != qbr[i].Quantity)
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
