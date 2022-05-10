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
            Visible = XmlUtils.GetBooleanNotNull(n, "Visible");
            Own = XmlUtils.GetBooleanNotNull(n, "Own");

            Debug.Assert(!(Own && !Visible));
        }

        internal void TuneDeferredLinks()
        {
            TypeLair = FormMain.Descriptors.FindConstruction(NameTypeLair);
            NameTypeLair = null;
        }

        internal string NameTypeLair { get; private set; }
        internal DescriptorConstruction TypeLair { get; private set; }
        internal bool Visible { get; }
        internal bool Own { get; }
    }
}
