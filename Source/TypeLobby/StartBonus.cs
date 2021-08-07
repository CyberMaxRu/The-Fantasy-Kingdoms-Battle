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
            Builders = XmlUtils.GetInteger(n.SelectSingleNode("Builders"));
            ScoutPlace = XmlUtils.GetInteger(n.SelectSingleNode("ScoutPlace"));
            HolyPlace = XmlUtils.GetInteger(n.SelectSingleNode("HolyPlace"));
            TradePlace = XmlUtils.GetInteger(n.SelectSingleNode("TradePlace"));
            Points = XmlUtils.GetInteger(n.SelectSingleNode("Points"));
            MaxQuantity = XmlUtils.GetInteger(n.SelectSingleNode("MaxQuantity"));
            CurrentQuantity = 0;

            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 10000);
            Debug.Assert(Greatness >= 0);
            Debug.Assert(Greatness <= 10000);
            Debug.Assert(Builders >= 0);
            Debug.Assert(Builders <= 10);
            Debug.Assert(ScoutPlace >= 0);
            Debug.Assert(ScoutPlace <= 10);
            Debug.Assert(HolyPlace >= 0);
            Debug.Assert(HolyPlace <= 2);
            Debug.Assert(TradePlace >= 0);
            Debug.Assert(TradePlace <= 3);
            Debug.Assert(Points > 0);
            Debug.Assert(Points <= 10);
            Debug.Assert(MaxQuantity >= -1);
            Debug.Assert(MaxQuantity <= 10);
        }

        internal int Gold { get; private set; }
        internal int Greatness { get; private set; }
        internal int Builders { get; private set; }
        internal int ScoutPlace { get; private set; }
        internal int HolyPlace { get; private set; }
        internal int TradePlace { get; private set; }
        internal int Points { get; private set; }
        internal int MaxQuantity { get; private set; }
        internal int CurrentQuantity { get; private set; }

        internal void AddBonus(StartBonus sb)
        {
            Gold += sb.Gold;
            Greatness += sb.Greatness;
            Builders += sb.Builders;
            ScoutPlace += sb.ScoutPlace;
            HolyPlace += sb.HolyPlace;
            TradePlace += sb.TradePlace;
            Points += sb.Points;
            sb.CurrentQuantity++;
        }

        internal void ClearQuantity()
        {
            CurrentQuantity = 0;
        }

        public override bool Equals(object obj)
        {
            StartBonus otherStartBonus = obj as StartBonus;

            return (Gold == otherStartBonus.Gold) 
                && (Greatness == otherStartBonus.Greatness)
                && (ScoutPlace == otherStartBonus.ScoutPlace)
                && (HolyPlace == otherStartBonus.HolyPlace)
                && (TradePlace == otherStartBonus.TradePlace)
                && (Builders == otherStartBonus.Builders);
        }
    }
}
