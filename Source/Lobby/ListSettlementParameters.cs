using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal class ListSettlementParameters : List<int>
    {
        public ListSettlementParameters() : base(FormMain.Descriptors.SettlementParameters.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.SettlementParameters.Count; i++)
                Add(i);// 0
        }

        public ListSettlementParameters(XmlNode n) : base(FormMain.Descriptors.SettlementParameters.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.SettlementParameters.Count; i++)
                Add(0);

            if (n != null)
            {
                DescriptorSettlementParameter sp;
                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    sp = FormMain.Descriptors.FindSettlementParameter(n.ChildNodes[i].Name);
                    Debug.Assert(this[sp.Index] == 0);
                    //Debug.Assert(value >= 0);

                    this[sp.Index] = value;
                }
            }
        }
    }
}
