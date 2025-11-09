using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.Utils;

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
            MaxHeroesForBattle = XmlUtils.GetInteger(n, "MaxHeroesForBattle");
            DayStartBattleBetweenPlayers = XmlUtils.GetInteger(n, "DayStartBattleBetweenPlayers");
            DaysBeforeNextBattleBetweenPlayers = XmlUtils.GetInteger(n, "DaysBeforeNextBattleBetweenPlayers");
            PointStartBonus = XmlUtils.GetInteger(n, "PointStartBonus");
            VariantPersistentBonus = XmlUtils.GetInteger(n, "VariantPersistentBonus");
            VariantStartBonus = XmlUtils.GetInteger(n, "VariantStartBonus");
            VariantsUpSimpleHero = XmlUtils.GetInteger(n, "VariantsUpSimpleHero");
            VariantsUpTempleHero = XmlUtils.GetInteger(n, "VariantsUpTempleHero");
            MaxLoses = XmlUtils.GetInteger(n, "MaxLoses");

            Debug.Assert(Name.Length > 0);
            Debug.Assert(QuantityPlayers >= 2);
            Debug.Assert(QuantityPlayers >= 8);
            //Debug.Assert(QuantityPlayers <= FormMain.Descriptors.ComputerPlayers.Count); NLE
            Debug.Assert(QuantityPlayers % 2 == 0);
            Debug.Assert(MaxHeroes >= 1);
            Debug.Assert(MaxHeroes <= 100);// Здесь проверять через максим. число героев на поле боя
            Debug.Assert(MaxHeroesForBattle >= 1);
            Debug.Assert(MaxHeroesForBattle <= 25);
            Debug.Assert(DayStartBattleBetweenPlayers >= 1);
            Debug.Assert(DayStartBattleBetweenPlayers <= 50);
            Debug.Assert(DaysBeforeNextBattleBetweenPlayers >= 0);
            Debug.Assert(DaysBeforeNextBattleBetweenPlayers <= 10);
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
            Debug.Assert(MaxLoses >= 1);
            Debug.Assert(MaxLoses <= 5);

            Debug.Assert(Gold >= 0);
            Debug.Assert(MaxGold >= 1_000);
            Debug.Assert(MaxGold <= 1_000_000);
            Debug.Assert(Gold <= MaxGold);
        }

        internal string Name { get; }
        internal int QuantityPlayers { get; }
        internal int Gold { get; }
        internal int MaxGold { get; }
        internal int MaxHeroes { get; }
        internal int MaxHeroesForBattle { get; }
        internal int DayStartBattleBetweenPlayers { get; }
        internal int DaysBeforeNextBattleBetweenPlayers { get; }
        internal int PointStartBonus { get; }
        internal int VariantPersistentBonus { get; }
        internal int VariantStartBonus { get; }
        internal int VariantsUpSimpleHero { get; }
        internal int VariantsUpTempleHero { get; }
        internal int MaxLoses { get; }
        internal void TuneDeferredLinks()
        {
        }
    }
}
