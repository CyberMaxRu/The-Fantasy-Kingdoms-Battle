using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс мероприятий в сооружении
    internal sealed class DescriptorConstructionEvent : DescriptorSmallEntity
    {
        public DescriptorConstructionEvent(XmlNode n) : base(n)
        {
            Duration = XmlUtils.GetIntegerNotNull(n, "Duration");
            Cooldown = XmlUtils.GetIntegerNotNull(n, "Cooldown");
            Interest = XmlUtils.GetIntegerNotNull(n, "Interest");
            NameGoods = XmlUtils.GetStringNotNull(n, "NameGoods");

            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            Debug.Assert(Duration >= 1);
            Debug.Assert(Duration < 10);
            Debug.Assert(Cooldown >= 1);
            Debug.Assert(Cooldown <= 100);
            Debug.Assert(Interest >= 1);
            Debug.Assert(Interest <= 100);

            foreach (DescriptorConstructionEvent ce in Config.ConstructionsEvents)
            {
                Debug.Assert(ce.ID != ID);
                Debug.Assert(ce.Name != Name);
                Debug.Assert(ce.Description != Description);
                Debug.Assert(ce.ImageIndex != ImageIndex);
            }
        }

        internal int Duration { get; }// Длительность (в днях)
        internal int Cooldown { get; }// Пауза до возможности снова использовать (в днях)
        internal int Interest { get; }// Интерес к событию
        internal string NameGoods { get; }// Наименование мероприятия-товара
        internal ListNeeds ListNeeds { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            ListNeeds.TuneDeferredLinks();
            Debug.Assert(ListNeeds.Count > 0);
        }
    }
}
