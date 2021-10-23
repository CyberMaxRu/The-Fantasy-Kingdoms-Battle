using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class TypeLobbyLocationElement
    {
        public TypeLobbyLocationElement(XmlNode n)
        {
            NameElement = n.SelectSingleNode("ID").InnerText;
            MinQuantity = XmlUtils.GetInteger(n, "MinQuantity");
            MaxQuantity = XmlUtils.GetInteger(n, "MaxQuantity");

            Debug.Assert(MinQuantity >= 0);
            Debug.Assert(MaxQuantity < 20);
            Debug.Assert(MinQuantity <= MaxQuantity);
        }

        public TypeLobbyLocationElement(string ID, int quantity)
        {
            NameElement = ID;
            MinQuantity = quantity;
            MaxQuantity = quantity;

            Debug.Assert(MinQuantity >= 0);
            Debug.Assert(MaxQuantity < 20);
            Debug.Assert(MinQuantity <= MaxQuantity);
        }

        internal void TuneDeferredLinks()
        {
            ElementLandscape = FormMain.Config.FindElementLandscape(NameElement);
            NameElement = null;
        }

        internal string NameElement { get; private set; }
        internal DescriptorElementLandscape ElementLandscape { get; private set; }
        internal int MinQuantity { get; }
        internal int MaxQuantity { get; }
    }
}

