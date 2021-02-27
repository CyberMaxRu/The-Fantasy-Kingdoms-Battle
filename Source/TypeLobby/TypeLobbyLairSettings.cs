using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настроек типа лобби
    internal sealed class TypeLobbyLairSettings
    {
        public TypeLobbyLairSettings(XmlNode n)
        {
            Number = XmlUtils.GetInteger(n.SelectSingleNode("Number")) - 1;
            CostScout = XmlUtils.GetInteger(n.SelectSingleNode("CostScout"));

            Debug.Assert(CostScout > 0);
            Debug.Assert(CostScout <= 10_000);
        }

        internal int Number { get; }// Номер слоя
        internal int CostScout { get; }// Стоимость разведки
    }
}
