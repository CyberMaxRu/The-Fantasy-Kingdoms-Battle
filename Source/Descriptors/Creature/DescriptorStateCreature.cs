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
    internal sealed class DescriptorStateCreature : DescriptorEntity
    {
        public DescriptorStateCreature(XmlNode n) : base(n)
        {
            foreach (DescriptorStateCreature sc in Config.StatesCreature)
            {
                Debug.Assert(sc.ID != ID);
                Debug.Assert(sc.Name != Name);
                Debug.Assert(sc.Description != Description);
                Debug.Assert(sc.ImageIndex != ImageIndex);
            }
        }
    }
}
