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
    internal sealed class DescriptorConstructionVisit : DescriptorEntityForConstruction
    {
        public DescriptorConstructionVisit(DescriptorConstructionLevel level, XmlNode n) : base(level.ForConstruction, n)
        {
            Debug.Assert(Interest >= 0);
            Debug.Assert(Interest <= 100);

            ImageIndex = level.ForConstruction.ImageIndex;
            ConstructionLevel = level;

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

        internal DescriptorConstructionLevel ConstructionLevel { get; }
        internal int Interest { get; }// Интерес для посещения сооружения
        internal ListNeeds ListNeeds { get; }

        internal override int GetImageIndex(XmlNode n)
        {
            return 0;
        }

        internal override string GetTypeEntity()
        {
            return "Посещение";
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            ListNeeds.TuneDeferredLinks();
            Debug.Assert(ListNeeds.Count > 0);
        }
    }
}