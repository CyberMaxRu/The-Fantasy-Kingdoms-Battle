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
            CanOwn = XmlUtils.GetBooleanNotNull(n, "CanOwn");
            IsEnemy = XmlUtils.GetBooleanNotNull(n, "IsEnemy");

            Debug.Assert(!(Own && !Visible));

            if (IsEnemy)
            {
                Debug.Assert(!Own);
                Debug.Assert(!CanOwn);
            }
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
        internal bool CanOwn { get; }
        internal bool IsEnemy { get; }
    }
}
