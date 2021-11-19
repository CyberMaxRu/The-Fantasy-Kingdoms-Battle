using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ListNeeds : List<(DescriptorNeed, int)>
    {
        private List<(string, int)> listLinks;

        public ListNeeds(XmlNode n) : base()
        {
            if (n != null)
            {
                listLinks = new List<(string, int)>();

                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    int value = Convert.ToInt32(n.ChildNodes[i].InnerText);
                    string nameNeed = n.ChildNodes[i].Name;

                    // Проверяем на повтор
                    foreach ((string, int) ll in listLinks)
                    {
                        Debug.Assert(ll.Item1 != nameNeed);
                    }

                    if (value != 0)
                    {
                        listLinks.Add((nameNeed, value));
                    }
                }
            }
        }

        internal void TuneDeferredLinks()
        {
            if (listLinks != null)
            {
                Capacity = listLinks.Count;

                foreach ((string, int) need in listLinks)
                {
                    NameNeedCreature nameNeed = (NameNeedCreature)Enum.Parse(typeof(NameNeedCreature), need.Item1);
                    Add((FormMain.Descriptors.FindNeedCreature(nameNeed), need.Item2));
                }

                listLinks = null;
            }
        }
    }
}
