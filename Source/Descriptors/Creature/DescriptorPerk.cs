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
            Loyalty = XmlUtils.GetInteger(n, "Loyalty");

            Debug.Assert(Loyalty >= 0);
            Debug.Assert(Loyalty <= 100);

            foreach (DescriptorPerk dp in Config.Perks)
            {
                Debug.Assert(dp.ID != ID);
                Debug.Assert(dp.Name != Name);
            }
        }

        internal int Loyalty { get; }// Дает уровень лояльности
    }
}
