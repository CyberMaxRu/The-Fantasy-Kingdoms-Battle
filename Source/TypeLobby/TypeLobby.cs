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
            BaseResources = new ListBaseResources(n.SelectSingleNode("BaseResources"));
            MaxBaseResources = new ListBaseResources(n.SelectSingleNode("MaxBaseResources"));
            MaxHeroes = XmlUtils.GetInteger(n, "MaxHeroes");
            StartQuantityFlags = XmlUtils.GetInteger(n, "StartQuantityFlags");
            MaxQuantityFlags = XmlUtils.GetInteger(n, "MaxQuantityFlags");
            MaxHeroesForScoutFlag = XmlUtils.GetInteger(n, "MaxHeroesForScoutFlag");
            MaxHeroesForBattle = XmlUtils.GetInteger(n, "MaxHeroesForBattle");
            DayStartBattleBetweenPlayers = XmlUtils.GetInteger(n, "DayStartBattleBetweenPlayers");
            DaysBeforeNextBattleBetweenPlayers = XmlUtils.GetInteger(n, "DaysBeforeNextBattleBetweenPlayers");
            StartBuilders = XmlUtils.GetInteger(n, "StartBuilders");
            PointStartBonus = XmlUtils.GetInteger(n, "PointStartBonus");
            VariantPersistentBonus = XmlUtils.GetInteger(n, "VariantPersistentBonus");
            VariantStartBonus = XmlUtils.GetInteger(n, "VariantStartBonus");
            VariantsUpSimpleHero = XmlUtils.GetInteger(n, "VariantsUpSimpleHero");
            VariantsUpTempleHero = XmlUtils.GetInteger(n, "VariantsUpTempleHero");
            StartScoutedLairs = XmlUtils.GetInteger(n, "StartScoutedLairs");
            MaxLoses = XmlUtils.GetInteger(n, "MaxLoses");
            MapWidth = XmlUtils.GetInteger(n, "MapWidth");
            MapHeight = XmlUtils.GetInteger(n, "MapHeight");
            LairsWidth = XmlUtils.GetInteger(n, "LairsWidth");
            LairsHeight = XmlUtils.GetInteger(n, "LairsHeight");
            CostMove = XmlUtils.GetIntegerNotNull(n, "Capital/CostMove");

            Debug.Assert(Name.Length > 0);
            Debug.Assert(QuantityPlayers >= 2);
            Debug.Assert(QuantityPlayers >= 8);
            Debug.Assert(QuantityPlayers <= FormMain.Descriptors.ComputerPlayers.Count);
            Debug.Assert(QuantityPlayers % 2 == 0);
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
            Debug.Assert(VariantPersistentBonus >= 2);
            Debug.Assert(VariantPersistentBonus <= 4);
            Debug.Assert(VariantStartBonus >= 2);
            Debug.Assert(VariantStartBonus <= 4);
            Debug.Assert(((PointStartBonus > 0) && (VariantStartBonus > 0)) || ((PointStartBonus == 0) && (VariantStartBonus == 0)));
            Debug.Assert(VariantsUpSimpleHero >= 1);
            Debug.Assert(VariantsUpSimpleHero <= FormMain.Config.MaxHeroForSelectBonus);
            Debug.Assert(VariantsUpTempleHero >= 1);
            Debug.Assert(VariantsUpTempleHero <= FormMain.Config.MaxHeroForSelectBonus);
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

            for (int i = 0; i < BaseResources.Count; i++)
            {
                Debug.Assert(BaseResources[i].Quantity >= 0);
                Debug.Assert(MaxBaseResources[i].Quantity >= 1_000);
                Debug.Assert(MaxBaseResources[i].Quantity <= 1_000_000);
                Debug.Assert(BaseResources[i].Quantity <= MaxBaseResources[i].Quantity);
            }

            foreach (TypeLobby t in FormMain.Descriptors.TypeLobbies)
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
            string nameLocationCapital = XmlUtils.GetStringNotNull(n, "Capital/Location");

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
        internal ListBaseResources BaseResources { get; }
        internal ListBaseResources MaxBaseResources { get; }
        internal int MaxHeroes { get; }
        internal int StartQuantityFlags { get; }
        internal int MaxQuantityFlags { get; }
        internal int MaxHeroesForScoutFlag { get; }
        internal int MaxHeroesForBattle { get; }
        internal int DayStartBattleBetweenPlayers { get; }
        internal int DaysBeforeNextBattleBetweenPlayers { get; }
        internal int StartBuilders { get; }
        internal int PointStartBonus { get; }
        internal int VariantPersistentBonus { get; }
        internal int VariantStartBonus { get; }
        internal int VariantsUpSimpleHero { get; }
        internal int VariantsUpTempleHero { get; }
        internal int StartScoutedLairs { get; }
        internal int MaxLoses { get; }
        internal int MapWidth{ get; }
        internal int MapHeight { get; }
        internal int LairsWidth { get; }
        internal int LairsHeight { get; }
        internal TypeLobbyLocationSettings LocationCapital { get; }// Локация столицы
        internal int CostMove { get; }// Стоимость перемещения между объектами
        internal TypeLobbyLocationSettings[,] Locations { get; }
        internal int[] CoefFlagScout { get; }
        internal int[] CoefFlagAttack { get; }
        internal int[] CoefFlagDefense { get; }

        internal void TuneDeferredLinks()
        {
            foreach (TypeLobbyLocationSettings ls in Locations)
            {
                ls.TuneLinks();
            }
        }
    }
}
