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
    internal sealed class DescriptorConstructionExtension : DescriptorEntityForConstruction
    {
        public DescriptorConstructionExtension(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
            Debug.Assert(ModifyInterest >= 0);
            Debug.Assert(ModifyInterest <= 100);

            ModifyInterest = GetInteger(n, "Interest");
            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            foreach (DescriptorConstructionVisit cv in Config.ConstructionsVisits)
            {
                Debug.Assert(cv.ID != ID);
                Debug.Assert(cv.Name != Name);
                //Debug.Assert(cv.Description != Description);
                //Debug.Assert(cv.ImageIndex != ImageIndex);
            }
        }

        internal int ModifyInterest { get; }// Изменение интереса к сооружению
        internal ListNeeds ListNeeds { get; }// Изменение удовлетворения потребностей героев

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            ListNeeds.TuneDeferredLinks();
            //Debug.Assert(ListNeeds.Count > 0);
        }

        internal override void AfterTune()
        {
            base.AfterTune();

            if (UseForResearch.Count > 0)
            {
                Description += Environment.NewLine + "- Необходимо для:";

                foreach (DescriptorCellMenu cm in UseForResearch)
                {
                    if (cm is DescriptorCellMenuForConstructionLevel cmcl)
                        Description += Environment.NewLine + "    - { " + cmcl.ForEntity.Name + " (" + cmcl.Number.ToString() + " ур.)}";
                    else if (cm is DescriptorCellMenuForConstruction cmc)
                        Description += Environment.NewLine + "    - {" + cmc.Entity.Name + "}" + (cmc.ForEntity.ID != Descriptor.ID ? " ({" + cm.ForEntity.Name + "})" : "");
                }
            }
        }
    }
}
