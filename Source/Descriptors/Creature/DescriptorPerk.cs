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
            Honor = XmlUtils.GetInteger(n, "Honor");
            Enthusiasm = XmlUtils.GetInteger(n, "Enthusiasm");
            Morale = XmlUtils.GetInteger(n, "Morale");
            Luck = XmlUtils.GetInteger(n, "Luck");

            //Debug.Assert(Honor <= 0);
            //Debug.Assert(Honor > 0);
            Debug.Assert(Enthusiasm >= 0);
            //Debug.Assert(Luck >= 0);

            Debug.Assert(Loyalty >= 0);
            Debug.Assert(Loyalty <= 100);

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
        internal int Loyalty { get; }// Дает уровень лояльности
    }
}
