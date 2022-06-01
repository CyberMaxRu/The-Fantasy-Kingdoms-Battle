using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorProperty : DescriptorVisual
    {
        string nameAverageFromCreatures;

        public DescriptorProperty(XmlNode n) : base(n)
        {
            Index = Descriptors.PropertiesCreature.Count;

            nameAverageFromCreatures = GetStringFromXmlNode(n, "AverageFromCreatures", false);
        }

        internal int Index { get; }
        internal DescriptorProperty AverageFromCreatures { get; private set; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (nameAverageFromCreatures?.Length > 0)
            {
                AverageFromCreatures = Descriptors.FindPropertyCreature(nameAverageFromCreatures);
                nameAverageFromCreatures = "";
            }
        }
    }
}