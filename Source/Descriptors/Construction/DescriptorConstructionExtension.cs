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
    internal sealed class DescriptorConstructionExtension : DescriptorEntityForActiveEntity
    {
        public DescriptorConstructionExtension(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
            Debug.Assert(ModifyInterest >= 0);
            Debug.Assert(ModifyInterest <= 100);

            Durability = GetIntegerNotNull(n, "Durability", ID, 1, 1_000_000);
            ModifyInterest = GetInteger(n, "Interest");
            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));
        }

        internal int Durability { get; }// Прочность улучшения
        internal int ModifyInterest { get; }// Изменение интереса к сооружению
        internal ListNeeds ListNeeds { get; }// Изменение удовлетворения потребностей героев

        internal override void TuneLinks()
        {
            base.TuneLinks();

            ListNeeds.TuneDeferredLinks();
            //Debug.Assert(ListNeeds.Count > 0);
        }

        internal override void AfterTuneLinks()
        {
            base.AfterTuneLinks();

            if (UseForResearch.Count > 0)
            {
                Description += Environment.NewLine + "- Необходимо для:";

                foreach (DescriptorSmallEntity cm in UseForResearch)
                {
                    if (cm is DescriptorConstructionLevel cmcl)
                        Description += Environment.NewLine + "    - { " + cmcl.ActiveEntity.Name + " (" + cmcl.Number.ToString() + " ур.)}";
                    //else if (cm is DescriptorCellMenuForConstruction cmc)
                    //    Description += Environment.NewLine + "    - {" + cmc.Entity.Name + "}" + (cmc.ForEntity.ID != Construction.ID ? " ({" + cm.ForEntity.Name + "})" : "");
                }
            }
        }

        internal override string GetTypeEntity()
        {
            return "Доп. сооружение";
        }
    }
}
