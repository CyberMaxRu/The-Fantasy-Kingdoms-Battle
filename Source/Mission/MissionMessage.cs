using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class MissionMessage
    {
        public MissionMessage(XmlNode n)
        {
            Turn = GetIntegerNotNull(n, "Turn");
            From = GetStringNotNull(n, "From");
            Text = GetStringNotNull(n, "Text");
        }

        internal int Turn { get; }
        internal string From { get; }
        internal string Text { get; }
    }
}
