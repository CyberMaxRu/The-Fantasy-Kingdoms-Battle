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
            Name = XmlUtils.GetString(n, "Name");
            QuantityPlayers = XmlUtils.GetInteger(n, "QuantityPlayers");
            Gold = XmlUtils.GetInteger(n, "Gold");
            MaxGold = XmlUtils.GetInteger(n, "MaxGold");
            MaxHeroes = XmlUtils.GetInteger(n, "MaxHeroes");
            StartQuantityFlags = XmlUtils.GetInteger(n, "StartQuantityFlags");
            MaxQuantityFlags = XmlUtils.GetInteger(n, "MaxQuantityFlags");
            MaxHeroesForScoutFlag = XmlUtils.GetInteger(n, "MaxHeroesForScoutFlag");
            MaxHeroesForBattle = XmlUtils.GetInteger(n, "MaxHeroesForBattle");
            DayStartBattleBetweenPlayers = XmlUtils.GetInteger(n, "DayStartBattleBetweenPlayers");
            DaysBeforeNextBattleBetweenPlayers = XmlUtils.GetInteger(n, "DaysBeforeNextBattleBetweenPlayers");
            StartBuilders = XmlUtils.GetInteger(n, "StartBuilders");
            PointStartBonus = XmlUtils.GetInteger(n, "PointStartBonus");
            VariantStartBonus = XmlUtils.GetInteger(n, "VariantStartBonus");
            StartScoutedLairs = XmlUtils.GetInteger(n, "StartScoutedLairs");
            MaxLoses = XmlUtils.GetInteger(n, "MaxLoses");
            MapWidth = XmlUtils.GetInteger(n, "MapWidth");
            MapHeight = XmlUtils.GetInteger(n, "MapHeight");
            LairsWidth = XmlUtils.GetInteger(n, "LairsWidth");
            LairsHeight = XmlUtils.GetInteger(n, "LairsHeight");

            Debug.Assert(Name.Length > 0);
            Debug.Assert(QuantityPlayers >= 2);
            Debug.Assert(QuantityPlayers >= 8);
            Debug.Assert(QuantityPlayers <= FormMain.Config.ComputerPlayers.Count);
            Debug.Assert(QuantityPlayers % 2 == 0);
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 100000);
            Debug.Assert(MaxGold >= 100_00);
            Debug.Assert(MaxGold <= 1_000_000);
            Debug.Assert(Gold <= MaxGold);
            Debug.Assert(MaxHeroes >= 1);
            Debug.Assert(MaxHeroes <= 100);// Здесь проверять через максим. число героев на поле боя
            Debug.Assert(StartQuantityFlags >= 1);
            Debug.Assert(StartQuantityFlags <= 10);
            Debug.Assert(MaxQuantityFlags >= 1);
            Debug.Assert(MaxQuantityFlags <= 10);
            Debug.Assert(StartQuantityFlags <= MaxQuantityFlags);
            Debug.Assert(MaxHeroesForScoutFlag >= 1);
            Debug.Assert(MaxHeroesForScoutFlag <= 25);
            Debug.Assert(MaxHeroesForBattle >= 1);
            Debug.Assert(MaxHeroesForBattle <= 25);
            Debug.Assert(DayStartBattleBetweenPlayers >= 1);
            Debug.Assert(DayStartBattleBetweenPlayers <= 50);
            Debug.Assert(DaysBeforeNextBattleBetweenPlayers >= 0);
            Debug.Assert(DaysBeforeNextBattleBetweenPlayers <= 10);
            Debug.Assert(StartBuilders >= 0);
            Debug.Assert(StartBuilders <= 10);
            Debug.Assert(PointStartBonus >= 0);
            Debug.Assert(PointStartBonus <= 20);
            Debug.Assert(VariantStartBonus >= 0);
            Debug.Assert(VariantStartBonus <= 3);
            Debug.Assert(((PointStartBonus > 0) && (VariantStartBonus > 0)) || ((PointStartBonus == 0) && (VariantStartBonus == 0)));
            Debug.Assert(StartScoutedLairs >= 0);
            Debug.Assert(StartScoutedLairs <= 12);
            Debug.Assert(MaxLoses >= 1);
            Debug.Assert(MaxLoses <= 5);
            Debug.Assert(MapWidth >= 3);
            Debug.Assert(MapWidth <= 5);
            Debug.Assert(MapHeight >= 3);
            Debug.Assert(MapHeight <= 5);
            Debug.Assert(LairsWidth >= 2);
            Debug.Assert(LairsWidth <= 5);
            Debug.Assert(LairsHeight >= 1);
            Debug.Assert(LairsHeight <= 4);

            foreach (TypeLobby t in FormMain.Config.TypeLobbies)
            {
                if (Name == t.Name)
                    throw new Exception("Лобби с наименованием [" + Name + "] уже существует.");
            }

            // Загружаем настройки логов
            XmlNode nodeLairSettings = n.SelectSingleNode("Locations");
            Debug.Assert(nodeLairSettings != null);

            Locations = new TypeLobbyLocationSettings[MapHeight, MapWidth];
            TypeLobbyLocationSettings ls;

            foreach (XmlNode l in nodeLairSettings.SelectNodes("Location"))
            {
                ls = new TypeLobbyLocationSettings(this, l, LairsWidth * LairsHeight);

                foreach (TypeLobbyLocationSettings ls2 in Locations)
                {
                    if (ls2 != null)
                    {
                        Debug.Assert(ls2.ID != ls.ID);
                        Debug.Assert(ls2.Name != ls.Name);
                    }
                }

                Debug.Assert(Locations[ls.Coord.Y, ls.Coord.X] == null);
                Locations[ls.Coord.Y, ls.Coord.X] = ls;
            }

            // Проверяем, что указаны все локации
            string nameLocationCapital = XmlUtils.GetStringNotNull(n, "LocationCapital");

            for (int y = 0; y < MapHeight; y++)
                for (int x = 0; x < MapWidth; x++)
                {
                    Debug.Assert(Locations[y, x] != null);

                    if (Locations[y, x].ID == nameLocationCapital)
                    {
                        Debug.Assert(LocationCapital is null);
                        LocationCapital = Locations[y, x];
                    }
                }

            Debug.Assert(LocationCapital != null);

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
                    pe = (PriorityExecution)Enum.Parse(typeof(PriorityExecution), XmlUtils.GetStringNotNull(np, "ID"));
                    val = XmlUtils.GetInteger(np, "Value");

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
        internal int MaxGold { get; }
        internal int MaxHeroes { get; }
        internal int StartQuantityFlags { get; }
        internal int MaxQuantityFlags { get; }
        internal int MaxHeroesForScoutFlag { get; }
        internal int MaxHeroesForBattle { get; }
        internal int DayStartBattleBetweenPlayers { get; }
        internal int DaysBeforeNextBattleBetweenPlayers { get; }
        internal int StartBuilders { get; }
        internal int PointStartBonus { get; }
        internal int VariantStartBonus { get; }
        internal int StartScoutedLairs { get; }
        internal int MaxLoses { get; }
        internal int MapWidth{ get; }
        internal int MapHeight { get; }
        internal int LairsWidth { get; }
        internal int LairsHeight { get; }
        internal TypeLobbyLocationSettings LocationCapital { get; }// Локация столицы
        internal TypeLobbyLocationSettings[,] Locations { get; }
        internal int[] CoefFlagScout { get; }
        internal int[] CoefFlagAttack { get; }
        internal int[] CoefFlagDefense { get; }

        internal void TuneDeferredLinks()
        {
            foreach (TypeLobbyLocationSettings ls in Locations)
            {
                ls.TuneDeferredLinks();
            }
        }
    }
}
