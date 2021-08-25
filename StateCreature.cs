using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Состояния существа
    internal enum NameStateCreature { Nothing, DoScoutFlag, DoAttackFlag, DoDefenseFlag, BattleWithPlayer, InHome, Therapy, King, Advisor, Captain, Treasurer };

    // Класс описания состояния существ
    internal sealed class StateCreature
    {
        public StateCreature(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n, "ID");
            Name = XmlUtils.GetStringNotNull(n, "Name");
            Description = XmlUtils.GetDescription(n, "Description");
            ImageIndex = XmlUtils.GetInteger(n, "ImageIndex");

            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);
            Debug.Assert(ImageIndex >= -1);
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
    }
}
