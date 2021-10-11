using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    class DescriptorConstructionVisit : DescriptorSmallEntity
    {
        public DescriptorConstructionVisit(XmlNode n) : base(n)
        {
            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            foreach (DescriptorConstructionVisit cv in Config.ConstructionsVisits)
            {
                Debug.Assert(cv.ID != ID);
                Debug.Assert(cv.Name != Name);
                //Debug.Assert(cv.Description != Description);
                //Debug.Assert(cv.ImageIndex != ImageIndex);
            }
        }

        internal ListNeeds ListNeeds { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            ListNeeds.TuneDeferredLinks();
            Debug.Assert(ListNeeds.Count > 0);
        }
    }
}