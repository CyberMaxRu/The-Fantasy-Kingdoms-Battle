using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;

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
            Stamina = Convert.ToInt32(n.SelectSingleNode("Stamina").InnerText);
            ResistMagic = Convert.ToInt32(n.SelectSingleNode("ResistMagic").InnerText);
            WeightStrength = Convert.ToInt32(n.SelectSingleNode("WeightStrength").InnerText);
            WeightDexterity = Convert.ToInt32(n.SelectSingleNode("WeightDexterity").InnerText);
            WeightMagic = Convert.ToInt32(n.SelectSingleNode("WeightMagic").InnerText);
            WeightVitality = Convert.ToInt32(n.SelectSingleNode("WeightVitality").InnerText);

            Debug.Assert(StatPoints > 0);
            Debug.Assert(StatPoints <= Config.MAX_STAT_POINT_PER_LEVEL);
            Debug.Assert(WeightStrength + WeightDexterity + WeightMagic + WeightVitality == 100);
        }

        internal int StatPoints { get; }
        internal int Health { get; }
        internal int Mana { get; }
        internal int Stamina { get; }
        internal int ResistMagic { get; }

        // Вес основных параметров при получении нового уровня
        internal int WeightStrength { get; }
        internal int WeightDexterity { get; }
        internal int WeightMagic { get; }
        internal int WeightVitality { get; }
    }

    // Класс основных параметров существа
    internal sealed class HeroParameters
    {
        public HeroParameters(XmlNode n)
        {
            Strength = Convert.ToInt32(n.SelectSingleNode("Strength").InnerText);
            Dexterity = Convert.ToInt32(n.SelectSingleNode("Dexterity").InnerText);
            Magic = Convert.ToInt32(n.SelectSingleNode("Magic").InnerText);
            Vitality = Convert.ToInt32(n.SelectSingleNode("Vitality").InnerText);

            CoefHealth = Convert.ToInt32(n.SelectSingleNode("CoefHealth").InnerText);
            CoefMana = Convert.ToInt32(n.SelectSingleNode("CoefMana").InnerText);
            CoefStamina = Convert.ToInt32(n.SelectSingleNode("CoefStamina").InnerText);

            Health = Vitality * CoefHealth;
            Mana = Magic * CoefMana;
            Stamina = Vitality * CoefStamina;
        }

        public HeroParameters(HeroParameters mp)
        {
            GetFromParams(mp);
        }

        internal int Strength { get; set; }// Сила
        internal int Dexterity { get; set; }// Ловкость
        internal int Magic { get; set; }// Магия
        internal int Vitality { get; set; }// Живучесть
        internal int TimeAttack { get; set; }// Время между атаками (в тиках)
        internal int CoefHealth { get; set; }
        internal int CoefMana { get; set; }
        internal int CoefStamina { get; set; }
        internal int Health { get; set; }
        internal int CurrentHealth { get; set; }
        internal int Mana { get; set; }
        internal int CurrentMana { get; set; }
        internal int Stamina { get; set; }
        internal int CurrentStamina { get; set; }

        internal int MinMeleeDamage { get; set; }
        internal int MaxMeleeDamage { get; set; }
        internal int MinMissileDamage { get; set; }
        internal int MaxMissileDamage { get; set; }
        internal int MagicDamage { get; set; }
        internal int DefenseMelee { get; set; }
        internal int DefenseMissile { get; set; }
        internal int DefenseMagic { get; set; }
        internal int StepsInTumbstone { get; set; }// Сколько шагов боя герой уже в состоянии могилы
        internal int StepsInResurrection { get; set; }// Сколько шагов боя герой уже воскрешается

        internal void GetFromParams(HeroParameters hp)
        {

            Strength = hp.Strength;
            Dexterity = hp.Dexterity;
            Magic = hp.Magic;
            Vitality = hp.Vitality;
            CoefHealth = hp.CoefHealth;
            CoefMana = hp.CoefMana;
            CoefStamina = hp.CoefStamina;
            Health = hp.Health;
            Mana = hp.Mana;
            Stamina = hp.Stamina;
    }

    // Пересчитать параметры
    internal void RecalcParameter()
        {

        }
    }
}
