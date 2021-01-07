using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Тип объекта карты - базовый класс для всех зданий, построек и логов
    internal abstract class TypeMapObject : TypeObject
    {
        public TypeMapObject(XmlNode n) : base(n)
        {
            DefaultLevel = XmlUtils.GetInteger(n.SelectSingleNode("DefaultLevel"));
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);
            Line = XmlUtils.GetInteger(n.SelectSingleNode("Line"));

            Debug.Assert(DefaultLevel >= 0);
            Debug.Assert(DefaultLevel <= 5);
            Debug.Assert(MaxLevel > 0);
            Debug.Assert(MaxLevel <= 5);
            Debug.Assert(DefaultLevel <= MaxLevel);
            Debug.Assert(Line >= 1);
            Debug.Assert(Line <= 3);
        }

        internal int DefaultLevel { get; }
        internal int MaxLevel { get; }
        internal int Line { get; }

    }
}
