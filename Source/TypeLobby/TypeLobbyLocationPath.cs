using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс пути до локации
    sealed internal class TypeLobbyLocationPath : DescriptorWithID
    {
        private string idToLocation;
        public TypeLobbyLocationPath(XmlNode n, TypeLobbyLocationSettings ls) : base(n)
        {
            Location = ls;

            idToLocation = XmlUtils.GetStringNotNull(n, "Location");
            Visible = XmlUtils.GetBooleanNotNull(n, "Visible");
            MinPercentScout = XmlUtils.GetInteger(n, "MinPercentScout");
            MaxPercentScout = XmlUtils.GetInteger(n, "MaxPercentScout");

            if (Visible)
            {
                Debug.Assert(MinPercentScout == 0);
                Debug.Assert(MaxPercentScout == 0);
            }
            else
            {
                Debug.Assert(MinPercentScout >= 0);
                Debug.Assert(MaxPercentScout <= 100);
                Debug.Assert(MinPercentScout <= MaxPercentScout);
                Debug.Assert(MinPercentScout >= ls.PercentScoutedArea);
            }

            Debug.Assert(ls.ID != idToLocation);
        }

        internal TypeLobbyLocationSettings Location { get; private set; }
        internal TypeLobbyLocationSettings PathToLocation { get; private set; }
        internal bool Visible { get; }
        internal int MinPercentScout { get; }
        internal int MaxPercentScout { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            foreach (TypeLobbyLocationSettings ls in Location.TypeLobby.Locations)
            {
                if (ls.ID == idToLocation)
                {
                    Debug.Assert(!ls.VisibleByDefault);
                    PathToLocation = ls;
                    idToLocation = "";

                    break;
                }
            }

            Debug.Assert(PathToLocation != null);
        }
    }
}
