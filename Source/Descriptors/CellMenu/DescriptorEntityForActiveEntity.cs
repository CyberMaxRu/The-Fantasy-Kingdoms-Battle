using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal class DescriptorEntityForActiveEntity : DescriptorSmallEntity
    {
        public DescriptorEntityForActiveEntity(XmlNode n) : base(n)
        {
            DaysProcessing = GetInteger(n, "DaysProcessing");
            Cost = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(this, n.SelectSingleNode("Requirements"));
        }

        internal int DaysProcessing { get; }// Количество дней для создания
        internal ListBaseResources Cost { get; }// Стоимость
        internal ListDescriptorRequirements Requirements { get; }// Список требований для создания
    }
}
