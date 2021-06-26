using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс стартовых бонусов
    internal sealed class StartBonus
    {
        public StartBonus(XmlNode n)
        {
            Gold = XmlUtils.GetInteger(n.SelectSingleNode("Gold"));
            Greatness = XmlUtils.GetInteger(n.SelectSingleNode("Greatness"));
            PointConstructionGuild = XmlUtils.GetInteger(n.SelectSingleNode("PointConstructionGuild"));
            Points = XmlUtils.GetInteger(n.SelectSingleNode("Points"));

            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 10000);
            Debug.Assert(Greatness >= 0);
            Debug.Assert(Greatness <= 10000);
            Debug.Assert(PointConstructionGuild >= 0);
            Debug.Assert(PointConstructionGuild <= 10);
            Debug.Assert(Points > 0);
            Debug.Assert(Points <= 10);
        }

        internal int Gold { get; }
        internal int Greatness { get; }
        internal int PointConstructionGuild { get; }
        internal int Points { get; }
    }
}
