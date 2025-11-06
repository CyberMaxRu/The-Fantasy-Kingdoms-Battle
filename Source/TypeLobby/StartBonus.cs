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
            Builders = XmlUtils.GetInteger(n, "Builders");
            Scouting = XmlUtils.GetInteger(n, "Scouting");
            PeasantHouse = XmlUtils.GetInteger(n, "PeasantHouse");
            HolyPlace = XmlUtils.GetInteger(n, "HolyPlace");
            Points = XmlUtils.GetInteger(n, "Points");
            MaxQuantity = XmlUtils.GetInteger(n, "MaxQuantity");
            if (MaxQuantity == 0)
                MaxQuantity = 10;
            CurrentQuantity = 0;

                Debug.Assert(Gold >= 0);
                Debug.Assert(Gold <= 10_000);
            
            Debug.Assert(Builders >= 0);
            Debug.Assert(Builders <= 10);
            Debug.Assert(Scouting >= 0);
            Debug.Assert(Scouting <= 10);
            Debug.Assert(PeasantHouse >= 0);
            Debug.Assert(PeasantHouse <= 5);
            Debug.Assert(HolyPlace >= 0);
            Debug.Assert(HolyPlace <= 2);
            Debug.Assert(Points > 0);
            Debug.Assert(Points <= 10);
            Debug.Assert(MaxQuantity >= 1);
            Debug.Assert(MaxQuantity <= 10);
            Debug.Assert((Builders > 0) || (Scouting > 0) || (PeasantHouse > 0) || (HolyPlace > 0) || (Gold > 0));
        }

        internal int Builders { get; private set; }
        internal int Scouting { get; private set; }
        internal int PeasantHouse { get; private set; }
        internal int HolyPlace { get; private set; }
        internal int Gold { get; set; }
        internal int Points { get; private set; }
        internal int MaxQuantity { get; private set; }
        internal int CurrentQuantity { get; private set; }

        internal void AddBonus(StartBonus sb)
        {
            Gold += sb.Gold;
            Builders += sb.Builders;
            Scouting += sb.Scouting;
            PeasantHouse += sb.PeasantHouse;
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
                + (Builders != 0 ? 1 : 0)
                + (Scouting != 0 ? 1 : 0)
                + (PeasantHouse != 0 ? 1 : 0)
                + (HolyPlace != 0 ? 1 : 0);

            return q;
        }

        public override bool Equals(object obj)
        {
            StartBonus otherStartBonus = obj as StartBonus;

            return (Gold == otherStartBonus.Gold)
                && (Scouting == otherStartBonus.Scouting)
                && (PeasantHouse == otherStartBonus.PeasantHouse)
                && (HolyPlace == otherStartBonus.HolyPlace)
                && (Builders == otherStartBonus.Builders);
        }
    }
}
