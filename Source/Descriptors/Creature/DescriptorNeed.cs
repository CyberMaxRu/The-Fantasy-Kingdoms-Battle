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
    internal enum NameNeedCreature { Food, Rest, Entertainment, Money };

    // Класс потребности 
    internal sealed class DescriptorNeed : DescriptorEntity
    {
        public DescriptorNeed(XmlNode n) : base(n)
        {
            Index = Descriptors.NeedsCreature.Count;

            NameNeed = (NameNeedCreature)Enum.Parse(typeof(NameNeedCreature), ID);
            ReasonOfDeath = (ReasonOfDeath)Enum.Parse(typeof(ReasonOfDeath), GetStringNotNull(n, "ReasonOfDeath"));
        }

        internal int Index { get; }
        internal NameNeedCreature NameNeed { get; }
        internal ReasonOfDeath ReasonOfDeath { get; }// Причина смерти при неудовлетворении потребности
    }
}
