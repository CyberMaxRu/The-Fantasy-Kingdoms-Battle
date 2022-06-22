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
            BaseResources = new ListBaseResources();
        }

        public StartBonus(XmlNode n)
        {
            BaseResources = new ListBaseResources(n.SelectSingleNode("Resources"));
            Builders = XmlUtils.GetInteger(n, "Builders");
            Scouting = XmlUtils.GetInteger(n, "Scouting");
            PeasantHouse = XmlUtils.GetInteger(n, "PeasantHouse");
            HolyPlace = XmlUtils.GetInteger(n, "HolyPlace");
            Points = XmlUtils.GetInteger(n, "Points");
            MaxQuantity = XmlUtils.GetInteger(n, "MaxQuantity");
            if (MaxQuantity == 0)
                MaxQuantity = 10;
            CurrentQuantity = 0;

            for (int i = 0; i < BaseResources.Count; i++)
            {
                Debug.Assert(BaseResources[i] >= 0);
                Debug.Assert(BaseResources[i] <= 10000);
            }

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
            Debug.Assert((Builders > 0) || (Scouting > 0) || (PeasantHouse > 0) || (HolyPlace > 0) || BaseResources.ExistsResources());
        }

        internal int Builders { get; private set; }
        internal int Scouting { get; private set; }
        internal int PeasantHouse { get; private set; }
        internal int HolyPlace { get; private set; }
        internal ListBaseResources BaseResources { get; }
        internal int Points { get; private set; }
        internal int MaxQuantity { get; private set; }
        internal int CurrentQuantity { get; private set; }

        internal void AddBonus(StartBonus sb)
        {
            BaseResources.AddResources(sb.BaseResources);
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
            int q = (Builders != 0 ? 1 : 0)
                + (Scouting != 0 ? 1 : 0)
                + (PeasantHouse != 0 ? 1 : 0)
                + (HolyPlace != 0 ? 1 : 0);

            for (int i = 0; i < BaseResources.Count; i++)
            {
                if (BaseResources[i] != 0)
                    q++;
            }

            return q;
        }

        public override bool Equals(object obj)
        {
            StartBonus otherStartBonus = obj as StartBonus;

            return BaseResources.Equals(otherStartBonus.BaseResources)
                && (Scouting == otherStartBonus.Scouting)
                && (PeasantHouse == otherStartBonus.PeasantHouse)
                && (HolyPlace == otherStartBonus.HolyPlace)
                && (Builders == otherStartBonus.Builders);
        }
    }
}
