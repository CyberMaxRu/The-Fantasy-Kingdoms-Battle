using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Увеличение параметров при получении нового уровня
    internal sealed class ConfigNextLevelHero
    {
        public ConfigNextLevelHero(XmlNode n)
        {
            StatPoints = Convert.ToInt32(n.SelectSingleNode("StatPoints").InnerText);
            Health = Convert.ToInt32(n.SelectSingleNode("Health").InnerText);
            Mana = Convert.ToInt32(n.SelectSingleNode("Mana").InnerText);
            ResistMagic = Convert.ToInt32(n.SelectSingleNode("ResistMagic").InnerText);
            WeightStrength = Convert.ToInt32(n.SelectSingleNode("WeightStrength").InnerText);
            WeightDexterity = Convert.ToInt32(n.SelectSingleNode("WeightDexterity").InnerText);
            WeightMagic = Convert.ToInt32(n.SelectSingleNode("WeightMagic").InnerText);
            WeightVitality = Convert.ToInt32(n.SelectSingleNode("WeightVitality").InnerText);

            Debug.Assert(StatPoints > 0);
            Debug.Assert(StatPoints <= 20);
            Debug.Assert(WeightStrength + WeightDexterity + WeightMagic + WeightVitality == 100);
        }

        internal int StatPoints { get; }
        internal int Health { get; }
        internal int Mana { get; }
        internal int ResistMagic { get; }

        // Вес основных параметров при получении нового уровня
        internal int WeightStrength { get; }
        internal int WeightDexterity { get; }
        internal int WeightMagic { get; }
        internal int WeightVitality { get; }
    }

    // Класс основных параметров существа
    internal sealed class MainParameters
    {
        public MainParameters(XmlNode n)
        {
            Strength = Convert.ToInt32(n.SelectSingleNode("Strength").InnerText);
            Dexterity = Convert.ToInt32(n.SelectSingleNode("Dexterity").InnerText);
            Magic = Convert.ToInt32(n.SelectSingleNode("Magic").InnerText);
            Vitality = Convert.ToInt32(n.SelectSingleNode("Vitality").InnerText);
            TimeAttack = Convert.ToInt32(n.SelectSingleNode("TimeAttack").InnerText);
            CoefHealth = Convert.ToInt32(n.SelectSingleNode("CoefHealth").InnerText);
            CoefMana = Convert.ToInt32(n.SelectSingleNode("CoefMana").InnerText);
        }

        public MainParameters(MainParameters mp)
        {
            Strength = mp.Strength;
            Dexterity = mp.Dexterity;
            Magic = mp.Magic;
            Vitality = mp.Vitality;
            TimeAttack = mp.TimeAttack;
            CoefHealth = mp.CoefHealth;
            CoefMana = mp.CoefMana;

            Health = Vitality * CoefHealth;
            Mana = Magic * CoefMana;
        }

        internal int Strength { get; set; }// Сила
        internal int Dexterity { get; set; }// Ловкость
        internal int Magic { get; set; }// Магия
        internal int Vitality { get; set; }// Живучесть
        internal int TimeAttack { get; set; }
        internal int CoefHealth { get; }
        internal int CoefMana { get; }
        internal int Stamina { get; set; }
        internal int Health { get; set; }
        internal int Mana { get; set; }
        internal int AttackMelee { get; set; }// Умение рукопашной атаки
        internal int AttackMissile { get; set; }
        internal int AttackMagic { get; set; }
        internal int DefenseMelee { get; set; }
        internal int DefenseMissile { get; set; }
        internal int DefenseMagic { get; set; }
    }
}
