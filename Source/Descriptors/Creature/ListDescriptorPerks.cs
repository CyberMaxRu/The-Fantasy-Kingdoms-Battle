using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ListDescriptorPerks : List<DescriptorPerk>
    {
        private List<string> namePerks;

        public ListDescriptorPerks(XmlNode n)
        {
            if (n != null)
            {
                namePerks = new List<string>();

                foreach (XmlNode l in n.SelectNodes("Perk"))
                {
                    string namePerk = l.InnerText;
                    Debug.Assert(namePerk.Length > 0);

                    // Проверяем, что такой перк не повторяется
                    foreach (string p2 in namePerks)
                    {
                        Debug.Assert(p2 != namePerk);
                    }

                    namePerks.Add(namePerk);
                }
            }
        }

        internal void TuneDeferredLinks()
        {
            if (namePerks != null)
            {
                Capacity = namePerks.Count;

                foreach (string perk in namePerks)
                    Add(FormMain.Descriptors.FindPerk(perk));

                namePerks = null;
            }
        }
    }
}
