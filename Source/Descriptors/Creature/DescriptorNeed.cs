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

            ShowForConstruction = GetBooleanNotNull(n, "ShowForConstruction");
        }

        internal int Index { get; }
        internal bool ShowForConstruction { get; }// Показывать в панели информации о сооружении
    }
}
