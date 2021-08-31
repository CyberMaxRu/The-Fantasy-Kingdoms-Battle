using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс типы награды
    internal sealed class TypeReward
    {
        public TypeReward(XmlNode n)
        {
            Gold = XmlUtils.GetInteger(n, "Gold");
            Greatness = XmlUtils.GetInteger(n, "Greatness");

            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 50_000);
            Debug.Assert(Greatness >= 0);
            Debug.Assert(Greatness <= 10_000);
        }

        internal int Gold { get; }
        internal int Greatness { get; }
    }
}
