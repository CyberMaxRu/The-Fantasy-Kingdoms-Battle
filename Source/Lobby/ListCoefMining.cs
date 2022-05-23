using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ListCoefMining : List<int>
    {
        public ListCoefMining() : base(FormMain.Descriptors.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Descriptors.BaseResources)
                Add(0);
        }

        public ListCoefMining(XmlNode n) : base(FormMain.Descriptors.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Descriptors.BaseResources)
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
    }
}
