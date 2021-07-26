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
            Gold = XmlUtils.GetInteger(n.SelectSingleNode("Gold"));
            MaxHeroes = XmlUtils.GetInteger(n.SelectSingleNode("MaxHeroes"));
            StartQuantityFlags = XmlUtils.GetInteger(n.SelectSingleNode("StartQuantityFlags"));
            MaxQuantityFlags = XmlUtils.GetInteger(n.SelectSingleNode("MaxQuantityFlags"));
            MaxHeroesForScoutFlag = XmlUtils.GetInteger(n.SelectSingleNode("MaxHeroesForScoutFlag"));
            StartBuilders = XmlUtils.GetInteger(n.SelectSingleNode("StartBuilders"));
            StartPointConstructionTemple = XmlUtils.GetInteger(n.SelectSingleNode("StartPointConstructionTemple"));
            StartPointConstructionTradePost = XmlUtils.GetInteger(n.SelectSingleNode("StartPointConstructionTradePost"));
            PointStartBonus = XmlUtils.GetInteger(n.SelectSingleNode("PointStartBonus"));
            VariantStartBonus = XmlUtils.GetInteger(n.SelectSingleNode("VariantStartBonus"));
            StartScoutedLairs = XmlUtils.GetInteger(n.SelectSingleNode("StartScoutedLairs"));
            MaxLoses = XmlUtils.GetInteger(n.SelectSingleNode("MaxLoses"));
            DayStartTournament = XmlUtils.GetInteger(n.SelectSingleNode("DayStartTournament"));
            LairsLayers = XmlUtils.GetInteger(n.SelectSingleNode("LairsLayers"));
            LairsWidth = XmlUtils.GetInteger(n.SelectSingleNode("LairsWidth"));
            LairsHeight = XmlUtils.GetInteger(n.SelectSingleNode("LairsHeight"));

            Debug.Assert(Name.Length > 0);
            Debug.Assert(QuantityPlayers >= 2);
            Debug.Assert(QuantityPlayers >= 8);
            Debug.Assert(QuantityPlayers <= FormMain.Config.ComputerPlayers.Count);
            Debug.Assert(QuantityPlayers % 2 == 0);
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 100000);
            Debug.Assert(MaxHeroes >= 1);
            Debug.Assert(MaxHeroes <= 100);// Здесь проверять через максим. число героев на поле боя
            Debug.Assert(StartQuantityFlags >= 1);
            Debug.Assert(StartQuantityFlags <= 10);
            Debug.Assert(MaxQuantityFlags >= 1);
            Debug.Assert(MaxQuantityFlags <= 10);
            Debug.Assert(StartQuantityFlags <= MaxQuantityFlags);
            Debug.Assert(MaxHeroesForScoutFlag >= 1);
            Debug.Assert(MaxHeroesForScoutFlag <= 25);
            Debug.Assert(StartBuilders >= 0);
            Debug.Assert(StartBuilders <= 10);
            Debug.Assert(StartPointConstructionTemple >= 0);
            Debug.Assert(StartPointConstructionTemple <= 5);
            Debug.Assert(StartPointConstructionTradePost>= 0);
            Debug.Assert(StartPointConstructionTradePost <= 5);
            Debug.Assert(PointStartBonus >= 0);
            Debug.Assert(PointStartBonus <= 20);
            Debug.Assert(VariantStartBonus >= 0);
            Debug.Assert(VariantStartBonus <= 3);
            Debug.Assert(((PointStartBonus > 0) && (VariantStartBonus > 0)) || ((PointStartBonus == 0) && (VariantStartBonus == 0)));
            Debug.Assert(StartScoutedLairs >= 0);
            Debug.Assert(StartScoutedLairs <= 12);
            Debug.Assert(MaxLoses >= 1);
            Debug.Assert(MaxLoses <= 5);
            Debug.Assert(LairsLayers >= 1);
            Debug.Assert(LairsLayers <= 5);
            Debug.Assert(LairsWidth >= 2);
            Debug.Assert(LairsWidth <= 5);
            Debug.Assert(LairsHeight >= 1);
            Debug.Assert(LairsHeight <= 4);
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

            LayerSettings = new TypeLobbyLayerSettings[layers];
            TypeLobbyLayerSettings ls;

            foreach (XmlNode l in nodeLairSettings.SelectNodes("Layer"))
            {
                ls = new TypeLobbyLayerSettings(l, LairsWidth * LairsHeight);

                Debug.Assert(ls.Number >= 0);
                Debug.Assert(ls.Number <= FormMain.MAX_LAIR_LAYERS - 1);
                Debug.Assert(LayerSettings[ls.Number] == null);

                LayerSettings[ls.Number] = ls;
            }

            // Проверяем, что указаны все слои
            for (int i = 0; i < LayerSettings.Length; i++)
                Debug.Assert(LayerSettings[i] != null);

            // Проверяем, что количество у слоев указано корректно

            // Загружаем настройку коэффициентов для флагов разведки и атаки
            CoefFlagScout = LoadCoef(n.SelectSingleNode("CoefficientFlags/Scout"));
            CoefFlagAttack = LoadCoef(n.SelectSingleNode("CoefficientFlags/Attack"));
            CoefFlagDefense = LoadCoef(n.SelectSingleNode("CoefficientFlags/Defense"));

            int[] LoadCoef(XmlNode node)
            {
                Debug.Assert(node != null);

                PriorityExecution pe;
                int val;

                int[] array = new int[(int)PriorityExecution.Exclusive + 1];

                foreach (XmlNode np in node.SelectNodes("Priority"))
                {
                    pe = (PriorityExecution)Enum.Parse(typeof(PriorityExecution), XmlUtils.GetStringNotNull(np.SelectSingleNode("ID")));
                    val = XmlUtils.GetInteger(np.SelectSingleNode("Value"));

                    Debug.Assert(val > 0);
                    Debug.Assert(val <= 1_000);
                    Debug.Assert(array[(int)pe] == 0);

                    array[(int)pe] = val;
                }

                // Проверяем, что указаны все коэффициенты
                for (int i = 0; i < array.Length; i++)
                {
                    Debug.Assert(array[i] != 0);

                    if (i > 0)
                        Debug.Assert(array[i] > array[i - 1]);
                }

                return array;
            }
        }

        internal string Name { get; }
        internal int QuantityPlayers { get; }
        internal int Gold { get; }
        internal int MaxHeroes { get; }
        internal int StartQuantityFlags { get; }
        internal int MaxQuantityFlags { get; }
        internal int MaxHeroesForScoutFlag { get; }
        internal int DayStartTournament { get; }
        internal int StartBuilders { get; }
        internal int StartPointConstructionTemple { get; }
        internal int StartPointConstructionTradePost { get; }
        internal int PointStartBonus { get; }
        internal int VariantStartBonus { get; }
        internal int StartScoutedLairs { get; }
        internal int MaxLoses { get; }
        internal int LairsLayers{ get; }
        internal int LairsWidth { get; }
        internal int LairsHeight { get; }
        internal TypeLobbyLayerSettings[] LayerSettings { get; }
        internal int[] CoefFlagScout { get; }
        internal int[] CoefFlagAttack { get; }
        internal int[] CoefFlagDefense { get; }

        internal void TuneDeferredLinks()
        {
            foreach (TypeLobbyLayerSettings ls in LayerSettings)
            {
                ls.TuneDeferredLinks();
            }
        }
    }
}
