using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс изменения параметров существа
    internal sealed class CreatureModifyParameters
    {
        public CreatureModifyParameters(XmlNode n)
        {
            XmlNodeList nodes = n.ChildNodes;
            foreach (XmlNode l in nodes)
            {
                if (l.Name == "MeleeAttackPercent")
                {
                    Assert(MeleeAttackPercent == 0, $"Повторяется параметр {l.Name}");
                    MeleeAttackPercent = Convert.ToInt32(l.InnerText);
                }
                else if (l.Name == "RangeAttackPercent")
                {
                    Assert(RangeAttackPercent == 0, $"Повторяется параметр {l.Name}");
                    RangeAttackPercent = Convert.ToInt32(l.InnerText);
                }
                else
                    DoException($"Не опознан тип модификации {l.Name}");
            }

            if (MeleeAttackPercent != 0)
                TextHint += $"+{MeleeAttackPercent}% к урону ближнего боя{Environment.NewLine}";
            if (RangeAttackPercent != 0)
                TextHint += $"+{RangeAttackPercent}% к урону дальнего боя{Environment.NewLine}";
        }

        internal int MeleeAttackPercent { get; }
        internal int RangeAttackPercent { get; }
        internal string TextHint { get; }
    }
}
