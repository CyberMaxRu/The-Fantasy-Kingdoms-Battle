using System;
using System.Collections.Generic;
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

        public Config(string pathResources, FormMain fm)
        {
            FormMain.Config = this;
            PathResources = pathResources;

            // 
            MaxLevelSkill = 3;

            //
            XmlDocument xmlDoc;

            // Загрузка конфигурации гильдий
            xmlDoc = CreateXmlDocument("Config\\Guilds.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Guilds/Guild"))
            {
                Guilds.Add(new Guild(n));
            }

            // Загрузка конфигурации зданий
            xmlDoc = CreateXmlDocument("Config\\Buildings.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Buildings/Building"))
            {
                Buildings.Add(new Building(n));
            }

            // Загрузка конфигурации храмов
            xmlDoc = CreateXmlDocument("Config\\Temples.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/Temples/Temple"))
            {
                Temples.Add(new Temple(n));
            }

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

            // Загрузка типов юнитов
            xmlDoc = CreateXmlDocument("Config\\TypeUnits.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeUnits/TypeUnit"))
            {
                TypeUnits.Add(new TypeUnit(n));
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
        }

        internal string PathResources { get; }
        internal List<Guild> Guilds { get; } = new List<Guild>();
        internal List<Building> Buildings { get; } = new List<Building>();
        internal List<Temple> Temples { get; } = new List<Temple>();
        internal List<Hero> Heroes { get; } = new List<Hero>();
        internal List<Fraction> Fractions { get; } = new List<Fraction>();
        internal List<ClassUnit> ClassesUnits { get; } = new List<ClassUnit>();
        internal List<TypeUnit> TypeUnits { get; } = new List<TypeUnit>();
        internal List<TypeItem> TypeItems { get; } = new List<TypeItem>();
        internal List<Item> Items { get; } = new List<Item>();
        internal List<Skill> Skills { get; } = new List<Skill>();
        internal int MaxLevelSkill { get; }

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

        internal TypeUnit FindTypeUnit(string ID)
        {
            foreach (TypeUnit tu in TypeUnits)
            {
                if (tu.ID == ID)
                {
                    return tu;
                }
            }

            throw new Exception("Тип юнитов " + ID + " не найден.");
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

        internal Guild FindGuild(string ID)
        {
            foreach (Guild g in Guilds)
            {
                if (g.ID == ID)
                {
                    return g;
                }
            }

            throw new Exception("Гильдия " + ID + " не найдена.");
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

        internal Temple FindTemple(string ID)
        {            
            foreach (Temple t in Temples)
            {
                if (t.ID == ID)
                    return t;
            }

            throw new Exception("Храм " + ID + " не найден.");
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
