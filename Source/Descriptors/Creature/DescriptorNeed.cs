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
    // Класс потребности 
    internal sealed class DescriptorNeed : DescriptorVisual
    {
        public DescriptorNeed(XmlNode n) : base(n)
        {
            Index = Descriptors.NeedsCreature.Count;

            ReasonOfDeath = Descriptors.FindReasonOfDeath(GetStringNotNull(n, "ReasonOfDeath"));
            ShowForConstruction = GetBooleanNotNull(n, "ShowForConstruction");
        }

        internal int Index { get; }
        internal DescriptorReasonOfDeath ReasonOfDeath { get; }// Причина смерти при неудовлетворении потребности
        internal bool ShowForConstruction { get; }// Показывать в панели информации о сооружении
    }
}
