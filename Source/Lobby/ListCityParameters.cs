using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal class ListCityParameters : List<int>
    {
        public ListCityParameters() : base(FormMain.Descriptors.CityParameters.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.CityParameters.Count; i++)
                Add(0);
        }

        public ListCityParameters(XmlNode n) : base(FormMain.Descriptors.CityParameters.Count)
        {
            for (int i = 0; i < FormMain.Descriptors.CityParameters.Count; i++)
                Add(0);

            if (n != null)
            {
                DescriptorCityParameter sp;
                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    sp = FormMain.Descriptors.FindCityParameter(n.ChildNodes[i].Name);
                    Debug.Assert(this[sp.Index] == 0);
                    //Debug.Assert(value >= 0);

                    this[sp.Index] = value;
                }
            }
        }

        public ListCityParameters(ListCityParameters list) : base(FormMain.Descriptors.CityParameters.Count)
        {
            for (int i = 0; i < list.Count; i++)
                Add(list[i]);

            Utils.Assert(Count == Capacity);
        }

        internal void Zeroing()
        {
            for (int i = 0; i < Count; i++)
                this[i] = 0;
        }

        internal void AddParameters(ListCityParameters list)
        {
            for (int i = 0; i < Count; i++)
                this[i] += list[i];
        }
    }
}
