using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class TypeLobbyLairSettings
    {
        public TypeLobbyLairSettings(XmlNode n)
        {
            NameTypeLair = n.SelectSingleNode("ID").InnerText;
            MinQuantity = XmlUtils.GetInteger(n.SelectSingleNode("MinQuantity"));
            MaxQuantity = XmlUtils.GetInteger(n.SelectSingleNode("MaxQuantity"));

            Debug.Assert(MinQuantity >= 0);
            Debug.Assert(MaxQuantity < 20);
            Debug.Assert(MinQuantity <= MaxQuantity);
        }

        public TypeLobbyLairSettings(string ID, int quantity)
        {
            NameTypeLair = ID;
            MinQuantity = quantity;
            MaxQuantity = quantity;

            Debug.Assert(MinQuantity >= 0);
            Debug.Assert(MaxQuantity < 20);
            Debug.Assert(MinQuantity <= MaxQuantity);
        }

        internal void TuneDeferredLinks()
        {
            TypeLair = FormMain.Config.FindTypeConstruction(NameTypeLair);
            NameTypeLair = null;
        }

        internal string NameTypeLair { get; private set; }
        internal TypeConstruction TypeLair { get; private set; }
        internal int MinQuantity { get; }
        internal int MaxQuantity { get; }
    }
}
