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
        public StartBonus()
        {
        }

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

        internal int Gold { get; private set; }
        internal int Greatness { get; private set; }
        internal int PointConstructionGuild { get; private set; }
        internal int Points { get; private set; }

        internal void AddBonus(StartBonus sb)
        {
            Gold += sb.Gold;
            Greatness += sb.Greatness;
            PointConstructionGuild += sb.PointConstructionGuild;
            Points += sb.Points;
        }

        public override bool Equals(object obj)
        {
            StartBonus otherStartBonus = obj as StartBonus;

            return (Gold == otherStartBonus.Gold) 
                && (Greatness == otherStartBonus.Greatness) 
                && (PointConstructionGuild == otherStartBonus.PointConstructionGuild);
        }
    }
}
