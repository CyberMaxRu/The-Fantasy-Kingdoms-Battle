using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Группа предметов
    internal sealed class DescriptorGroupItem : DescriptorSmallEntity
    {
        public DescriptorGroupItem(XmlNode n) : base(n)
        {
            ShortName = XmlUtils.GetStringNotNull(n, "ShortName");

            // Проверяем, что таких ID, Name и ImageIndex нет
            foreach (DescriptorGroupItem gi in Config.GroupItem)
            {
                Debug.Assert(gi.ID != ID);
                Debug.Assert(gi.Name != Name);
                Debug.Assert(gi.ImageIndex != ImageIndex);
                //Debug.Assert(gi.Description != Description);
            }
        }

        internal string ShortName { get; }//
        internal List<DescriptorItem> Items { get; } = new List<DescriptorItem>();
        internal List<DescriptorCreature> UsedByTypeCreature { get; } = new List<DescriptorCreature>();

        internal override void TuneDeferredLinks()
        {
            Description += (Description.Length > 0 ? Environment.NewLine : "") + "- Используется:";

            foreach (DescriptorCreature tc in UsedByTypeCreature)
            {
                Description += Environment.NewLine + "  - " + tc.Name;
            }
        }

        /*protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => ShortName;*/
    }
}