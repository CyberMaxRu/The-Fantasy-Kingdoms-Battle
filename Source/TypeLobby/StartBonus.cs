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
#pragma warning disable CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    internal sealed class StartBonus
#pragma warning restore CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    {
        public StartBonus()
        {
            Gold = 0;
        }

        public StartBonus(XmlNode n)
        {
            Gold = XmlUtils.GetInteger(n, "Gold");
            HolyPlace = XmlUtils.GetInteger(n, "HolyPlace");
            Points = XmlUtils.GetInteger(n, "Points");
            MaxQuantity = XmlUtils.GetInteger(n, "MaxQuantity");
            if (MaxQuantity == 0)
                MaxQuantity = 10;
            CurrentQuantity = 0;

                Debug.Assert(Gold >= 0);
                Debug.Assert(Gold <= 10_000);
            
            Debug.Assert(HolyPlace >= 0);
            Debug.Assert(HolyPlace <= 2);
            Debug.Assert(Points > 0);
            Debug.Assert(Points <= 10);
            Debug.Assert(MaxQuantity >= 1);
            Debug.Assert(MaxQuantity <= 10);
            Debug.Assert((HolyPlace > 0) || (Gold > 0));
        }

        internal int HolyPlace { get; private set; }
        internal int Gold { get; set; }
        internal int Points { get; private set; }
        internal int MaxQuantity { get; private set; }
        internal int CurrentQuantity { get; private set; }

        internal void AddBonus(StartBonus sb)
        {
            Gold += sb.Gold;
            HolyPlace += sb.HolyPlace;
            Points += sb.Points;
            sb.CurrentQuantity++;
        }

        internal void ClearQuantity()
        {
            CurrentQuantity = 0;
        }

        internal int QuantityElements()
        {
            int q = (Gold != 0 ? 1 : 0)
                + (HolyPlace != 0 ? 1 : 0);

            return q;
        }

        public override bool Equals(object obj)
        {
            StartBonus otherStartBonus = obj as StartBonus;

            return (Gold == otherStartBonus.Gold)
                && (HolyPlace == otherStartBonus.HolyPlace);
        }
    }
}
