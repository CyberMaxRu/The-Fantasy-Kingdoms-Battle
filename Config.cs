using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    internal sealed class Config
    {
        internal const int GRID_SIZE = 8;
        internal const int UNIT_IN_ROW = 5;
        internal const int ROWS_IN_SQUAD = 4;
        internal const int MAX_STEP_IN_BATTLE_SQUADS = 500;
        internal static int RESERVED_HEROES = 5;

        internal const string HERO_PEASANT = "Peasant";

        public Config(string pathResources, FormMain fm)
        {
            FormMain.Config = this;
            PathResources = pathResources;

            // 
            MaxLevelSkill = 3;

            //
            XmlDocument xmlDoc;

            // Загрузка конфигурации зданий
            xmlDoc = CreateXmlDocument("Config\\Buildings.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Buildings/Building"))
            {
                Buildings.Add(new Building(n));
            }
            
            foreach (Building b in Buildings)
                foreach (Level l in b.Levels)
                    if (l != null)
                        foreach (Requirement r in l.Requirements)
                            r.FindBuilding();

            // Загрузка типов предметов
            xmlDoc = CreateXmlDocument("Config\\TypeItems.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeItems/TypeItem"))
            {
                TypeItems.Add(new TypeItem(n));
            }

            // Загрузка предметов
            xmlDoc = CreateXmlDocument("Config\\Items.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Items/Item"))
            {
                Items.Add(new Item(n));
            }

            // Загрузка конфигурации героев
            xmlDoc = CreateXmlDocument("Config\\Heroes.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Heroes/Hero"))
            {
                Heroes.Add(new Hero(n));
            }

            // Загрузка фракций
            xmlDoc = CreateXmlDocument("Config\\Fractions.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Fractions/Fraction"))
            {
                Fractions.Add(new Fraction(n, fm));
            }

            // Загрузка классов юнитов
            xmlDoc = CreateXmlDocument("Config\\ClassesUnits.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/ClassesUnits/ClassUnit"))
            {
                ClassesUnits.Add(new ClassUnit(n));
            }

            // Загрузка навыков
            xmlDoc = CreateXmlDocument("Config\\Skills.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Skills/Skill"))
            {
                Skills.Add(new Skill(n));
            }

            // Вспомогательные методы
            XmlDocument CreateXmlDocument(string pathToXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathResources + pathToXml);
                return doc;
            }

            // Создаем конфигурацию поля боя для использования игроками
            Battlefield = new Battlefield(pathResources + "\\Config\\Battlefield.xml");
        }

        internal string PathResources { get; }
        internal List<Building> Buildings { get; } = new List<Building>();
        internal List<Hero> Heroes { get; } = new List<Hero>();
        internal List<Fraction> Fractions { get; } = new List<Fraction>();
        internal List<ClassUnit> ClassesUnits { get; } = new List<ClassUnit>();
        internal List<TypeItem> TypeItems { get; } = new List<TypeItem>();
        internal List<Item> Items { get; } = new List<Item>();
        internal List<Skill> Skills { get; } = new List<Skill>();
        internal int MaxLevelSkill { get; }
        internal Battlefield Battlefield { get; }

        internal Fraction FindFraction(string ID)
        {
            foreach (Fraction f in Fractions)
            {
                if (f.ID == ID)
                {
                    return f;
                }
            }

            throw new Exception("Фракция " + ID + " не найдена.");
        }

        internal ClassUnit FindClassUnit(string ID)
        {
            foreach (ClassUnit cu in ClassesUnits)
            {
                if (cu.ID == ID)
                {
                    return cu;
                }
            }

            throw new Exception("Класс юнитов " + ID + " не найден.");
        }

        internal Skill FindSkill(string ID)
        {
            foreach (Skill s in Skills)
            {
                if (s.ID == ID)
                {
                    return s;
                }
            }

            throw new Exception("Навык " + ID + " не найден.");
        }

        internal Building FindBuilding(string ID)
        {
            foreach (Building b in Buildings)
            {
                if (b.ID == ID)
                {
                    return b;
                }
            }

            throw new Exception("Здание " + ID + " не найдено.");
        }

        internal Hero FindHero(string ID)
        {
            foreach (Hero h in Heroes)
            {
                if (h.ID == ID)
                    return h;
            }

            throw new Exception("Герой " + ID + " не найден.");
        }

        internal TypeItem FindTypeItem(string ID)
        {
            foreach (TypeItem ti in TypeItems)
            {
                if (ti.ID == ID)
                    return ti;
            }

            throw new Exception("Тип предмета " + ID + " не найден.");
        }

        internal Item FindItem(string ID)
        {
            foreach (Item i in Items)
            {
                if (i.ID == ID)
                    return i;
            }

            throw new Exception("Предмет " + ID + " не найден.");
        }
    }
}
