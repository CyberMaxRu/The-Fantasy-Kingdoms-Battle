﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorConstructionService : DescriptorEntityForActiveEntity
    {
        public DescriptorConstructionService(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
            Debug.Assert(Interest >= 0);
            Debug.Assert(Interest <= 100);

            Interest = GetInteger(n, "Interest");
            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            foreach (DescriptorConstructionVisitSimple cv in Descriptors.ConstructionsVisits)
            {
                Debug.Assert(cv.ID != ID);
                Debug.Assert(cv.Name != Name);
                //Debug.Assert(cv.Description != Description);
                //Debug.Assert(cv.ImageIndex != ImageIndex);
            }
        }

        internal int Interest { get; }// Интерес для посещения сооружения
        internal ListNeeds ListNeeds { get; }

        internal override string GetTypeEntity()
        {
            return "Услуга";
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            ListNeeds.TuneDeferredLinks();

            //Debug.Assert(ListNeeds.Count > 0);
        }
    }
}
