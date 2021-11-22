using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс турниров в сооружении
    internal sealed class DescriptorConstructionTournament : DescriptorConstructionVisit
    {
        public DescriptorConstructionTournament(DescriptorConstruction descriptor, XmlNode n) : base(descriptor, n)
        {
            Cooldown = XmlUtils.GetIntegerNotNull(n, "Cooldown");
            Interest = XmlUtils.GetIntegerNotNull(n, "Interest");

            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            Debug.Assert(Cooldown >= 1);
            Debug.Assert(Cooldown <= 100);
            Debug.Assert(Interest >= 1);
            Debug.Assert(Interest <= 100);

            foreach (DescriptorConstructionTournament ct in Construction.Tournaments)
            {
                Debug.Assert(ct.ID != ID);
                Debug.Assert(ct.Name != Name);
                Debug.Assert(ct.ImageIndex != ImageIndex);
            }
        }

        internal int Cooldown { get; }// Пауза до возможности снова использовать (в днях)
        internal int Interest { get; }// Интерес к событию
        internal ListNeeds ListNeeds { get; }

        internal override string GetTypeEntity()
        {
            return "Турнир";
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            ListNeeds.TuneDeferredLinks();
            Debug.Assert(ListNeeds.Count > 0);
        }
    }
}