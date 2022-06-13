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
        public TypeLobbyLairSettings(XmlNode n, TypeLobbyLocationSettings ls)
        {
            NameTypeLair = n.SelectSingleNode("ID").InnerText;
            Visible = XmlUtils.GetBooleanNotNull(n, "Visible");
            Own = XmlUtils.GetBooleanNotNull(n, "Own");
            CanOwn = XmlUtils.GetBooleanNotNull(n, "CanOwn");
            IsEnemy = XmlUtils.GetBooleanNotNull(n, "IsEnemy");
            MinPercentScout = XmlUtils.GetPercent(n, "MinPercentScout");
            MaxPercentScout = XmlUtils.GetPercent(n, "MaxPercentScout");
            PathToLocation = XmlUtils.GetString(n, "PathToLocation");

            XmlNode nr = n.SelectSingleNode("Resources");
            if (nr != null)
                Resources = new ListBaseResources(nr);

            Debug.Assert(!(Own && !Visible));

            if (IsEnemy)
            {
                Debug.Assert(!Own);
                Debug.Assert(!CanOwn);
            }

            if (Visible)
            {
                Debug.Assert(MinPercentScout == 0);
                Debug.Assert(MaxPercentScout == 0);
            }
            else
            {
                Debug.Assert(MinPercentScout >= 0);
                Debug.Assert(MaxPercentScout <= 1_000);
                Debug.Assert(MinPercentScout <= MaxPercentScout);
                Debug.Assert(MinPercentScout >= ls.PercentScoutedArea);
            }
        }

        internal void TuneDeferredLinks()
        {
            DescriptorConstruction = FormMain.Descriptors.FindConstruction(NameTypeLair);
            NameTypeLair = "";

            if (DescriptorConstruction.Category != CategoryConstruction.Path)
            {
                Debug.Assert(PathToLocation.Length == 0);
            }
            else
            {
                Debug.Assert(PathToLocation.Length > 0);
            }
        }

        internal string NameTypeLair { get; private set; }
        internal DescriptorConstruction DescriptorConstruction { get; private set; }
        internal bool Visible { get; }
        internal bool Own { get; }
        internal bool CanOwn { get; }
        internal bool IsEnemy { get; }
        internal int MinPercentScout { get; }
        internal int MaxPercentScout { get; }
        internal string PathToLocation { get; }//
        internal ListBaseResources Resources { get; }// Ресурсы в сооружении
    }
}
