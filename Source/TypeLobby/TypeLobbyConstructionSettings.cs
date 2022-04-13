using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    sealed internal class TypeLobbyConstructionSettings : Descriptor
    {
        private string nameConstruction;

        public TypeLobbyConstructionSettings(XmlNode n) : base()
        {
            nameConstruction = GetStringNotNull(n, "ID");
            Quantity = GetIntegerNotNull(n, "Quantity");
        }

        internal DescriptorConstruction Construction { get; private set; }
        internal int Quantity { get; set; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Construction = Descriptors.FindConstruction(nameConstruction);
            nameConstruction = "";
        }
    }
}
