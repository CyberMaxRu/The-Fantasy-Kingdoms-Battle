using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Типы перков существ
    internal sealed class DescriptorPerk : DescriptorSmallEntity
    {
        public DescriptorPerk(XmlNode n) : base(n)
        {
            XmlAttribute attrIcon = n.SelectSingleNode("ImageIndex").Attributes["Size"];
            if ((attrIcon != null) && (attrIcon.Value == "128"))
                ImageIndex = XmlUtils.GetIntegerNotNull(n, "ImageIndex") - 1;

            Honor = XmlUtils.GetInteger(n, "Honor");
            Enthusiasm = XmlUtils.GetInteger(n, "Enthusiasm");
            Morale = XmlUtils.GetInteger(n, "Morale");
            Luck = XmlUtils.GetInteger(n, "Luck");

            //Debug.Assert(Honor <= 0);
            //Debug.Assert(Honor > 0);
            Debug.Assert(Enthusiasm >= 0);
            //Debug.Assert(Luck >= 0);

            foreach (DescriptorPerk dp in Config.Perks)
            {
                Debug.Assert(dp.ID != ID);
                Debug.Assert(dp.Name != Name);
            }
        }

        internal int Honor { get; }
        internal int Enthusiasm { get; }
        internal int Morale { get; }
        internal int Luck { get; }
    }
}
