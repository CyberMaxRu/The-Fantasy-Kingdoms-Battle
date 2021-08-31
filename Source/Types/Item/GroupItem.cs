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
    internal sealed class GroupItem : TypeEntity
    {
        public GroupItem(XmlNode n) : base(n)
        {
            ShortName = XmlUtils.GetStringNotNull(n, "ShortName");

            // Проверяем, что таких ID, Name и ImageIndex нет
            foreach (GroupItem gi in FormMain.Config.GroupItem)
            {
                Debug.Assert(gi.ID != ID);
                Debug.Assert(gi.Name != Name);
                Debug.Assert(gi.ImageIndex != ImageIndex);
                //Debug.Assert(gi.Description != Description);
            }
        }

        internal string ShortName { get; }//
        internal List<TypeItem> Items { get; } = new List<TypeItem>();
        internal List<TypeCreature> UsedByTypeCreature { get; } = new List<TypeCreature>();

        internal override void TuneDeferredLinks()
        {
            Description += (Description.Length > 0 ? Environment.NewLine : "") + "- Используется:";

            foreach (TypeCreature tc in UsedByTypeCreature)
            {
                Description += Environment.NewLine + "  - " + tc.Name;
            }
        }

        /*protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => ShortName;*/
    }
}