using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс типа (конфигурации) лобби
    internal sealed class TypeLobby
    {
        public TypeLobby(XmlNode n)
        {
            Name = XmlUtils.GetString(n.SelectSingleNode("Name"));
            QuantityPlayers = XmlUtils.GetInteger(n.SelectSingleNode("QuantityPlayers"));
            DurabilityCastle = XmlUtils.GetInteger(n.SelectSingleNode("DurabilityCastle"));
            Gold = XmlUtils.GetInteger(n.SelectSingleNode("Gold"));
            MaxHeroes = XmlUtils.GetInteger(n.SelectSingleNode("MaxHeroes"));
            StartPointConstructionGuild = XmlUtils.GetInteger(n.SelectSingleNode("StartPointConstructionGuild"));
            StartPointConstructionEconomic = XmlUtils.GetInteger(n.SelectSingleNode("StartPointConstructionEconomic"));
            PointConstructionGuildPerDay = XmlUtils.GetInteger(n.SelectSingleNode("PointConstructionGuildPerDay"));
            PointConstructionEconomicPerDay = XmlUtils.GetInteger(n.SelectSingleNode("PointConstructionEconomicPerDay"));
            DayStartTournament = XmlUtils.GetInteger(n.SelectSingleNode("DayStartTournament"));

            Debug.Assert(Name.Length > 0);
            Debug.Assert(QuantityPlayers >= 2);
            Debug.Assert(QuantityPlayers >= 8);
            Debug.Assert(QuantityPlayers % 2 == 0);
            Debug.Assert(DurabilityCastle > 0);
            Debug.Assert(DurabilityCastle <= 1000);
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 100000);
            Debug.Assert(MaxHeroes >= 1);
            Debug.Assert(MaxHeroes <= 100);// Здесь проверять через максим. число героев на поле боя
            Debug.Assert(StartPointConstructionGuild >= 1);
            Debug.Assert(StartPointConstructionGuild <= 10);
            Debug.Assert(StartPointConstructionEconomic >= 1);
            Debug.Assert(StartPointConstructionEconomic <= 10);
            Debug.Assert(PointConstructionGuildPerDay >= 1);
            Debug.Assert(PointConstructionGuildPerDay <= 10);
            Debug.Assert(PointConstructionEconomicPerDay >= 1);
            Debug.Assert(PointConstructionEconomicPerDay <= 10);
            Debug.Assert(DayStartTournament >= 2);
            Debug.Assert(DayStartTournament <= 50);

            foreach (TypeLobby t in FormMain.Config.TypeLobbies)
            {
                if (Name == t.Name)
                    throw new Exception("Лобби с наименованием [" + Name + "] уже существует.");
            }

            // Загружаем настройки логов
            XmlNode nodeLairSettings = n.SelectSingleNode("LairSettings");
            Debug.Assert(nodeLairSettings != null);

            int layers = Convert.ToInt32(nodeLairSettings.Attributes["Layers"].Value);
            Debug.Assert(layers >= 1);
            Debug.Assert(layers <= FormMain.MAX_LAIR_LAYERS);

            LairSettings = new TypeLobbyLairSettings[layers];
            TypeLobbyLairSettings ls;

            foreach (XmlNode l in nodeLairSettings.SelectNodes("Layer"))
            {
                ls = new TypeLobbyLairSettings(l);

                Debug.Assert(ls.Number >= 0);
                Debug.Assert(ls.Number <= FormMain.MAX_LAIR_LAYERS - 1);
                Debug.Assert(LairSettings[ls.Number] == null);

                LairSettings[ls.Number] = ls;
            }

            // Проверяем, что указаны все слои
            for (int i = 0; i < LairSettings.Length; i++)
                Debug.Assert(LairSettings[i] != null);
        }

        internal string Name { get; }
        internal int QuantityPlayers { get; }
        internal int DurabilityCastle { get; }
        internal int Gold { get; }
        internal int MaxHeroes { get; }
        internal int DayStartTournament { get; }
        internal int StartPointConstructionGuild { get; }
        internal int StartPointConstructionEconomic { get; }
        internal int PointConstructionGuildPerDay { get; }
        internal int PointConstructionEconomicPerDay { get; }
        internal TypeLobbyLairSettings[] LairSettings { get; }
    }
}
