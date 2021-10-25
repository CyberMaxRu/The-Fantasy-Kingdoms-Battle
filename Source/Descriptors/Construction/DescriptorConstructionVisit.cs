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
    class DescriptorConstructionVisit : DescriptorSmallEntity
    {
        public DescriptorConstructionVisit(DescriptorConstruction construction, XmlNode n) : base(n)
        {
            Debug.Assert(Interest >= 0);
            Debug.Assert(Interest <= 100);

            Construction = construction;

            Interest = GetInteger(n, "Interest");
            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            foreach (DescriptorConstructionVisit cv in Config.ConstructionsVisits)
            {
                Debug.Assert(cv.ID != ID);
                Debug.Assert(cv.Name != Name);
                //Debug.Assert(cv.Description != Description);
                //Debug.Assert(cv.ImageIndex != ImageIndex);
            }
        }

        internal DescriptorConstruction Construction { get; }
        internal int Interest { get; }// Интерес для посещения сооружения
        internal ListNeeds ListNeeds { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            ListNeeds.TuneDeferredLinks();
            Debug.Assert(ListNeeds.Count > 0);
        }
    }
}