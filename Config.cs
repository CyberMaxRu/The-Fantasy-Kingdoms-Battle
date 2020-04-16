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
        public Config(string pathResources)
        {
            FormMain.Config = this;
            PathResources = pathResources;

            //
            XmlDocument xmlDoc;

            // Загрузка конфигурации ресурсов
            xmlDoc = CreateXmlDocument("Config\\Resources.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Resources/Resource"))
            {
                Resources.Add(new Resource(n));
            }

            // Загрузка конфигурации зданий
            xmlDoc = CreateXmlDocument("Config\\Buildings.xml");
            Building b;
            foreach (XmlNode n in xmlDoc.SelectNodes("/Buildings/Building"))
            {
                b = new Building(n);

                switch (b.PlaceBuilding)
                {
                    case PlaceBuilding.External:
                        ExternalBuildings.Add(b);
                        break;
                    case PlaceBuilding.Internal:
                        InternalBuildings.Add(b);
                        break;
                    default:
                        new Exception("Неизвестный тип здания");
                        break;
                }
            }

            // Загрузка фракций
            xmlDoc = CreateXmlDocument("Config\\Fractions.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Fractions/Fraction"))
            {
                Fractions.Add(new Fraction(n));
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

            // Вспомогательные методы
            XmlDocument CreateXmlDocument(string pathToXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathResources + pathToXml);
                return doc;
            }
        }

        internal string PathResources { get; }
        internal List<Resource> Resources { get; } = new List<Resource>();
        internal List<Building> ExternalBuildings { get; } = new List<Building>();
        internal List<Building> InternalBuildings { get; } = new List<Building>();
        internal List<Fraction> Fractions { get; } = new List<Fraction>();
        internal List<ClassUnit> ClassesUnits { get; } = new List<ClassUnit>();
        internal List<TypeUnit> TypeUnits { get; } = new List<TypeUnit>();

        internal Resource FindResource(string ID)
        {
            foreach (Resource r in Resources)
            {
                if (r.ID == ID)
                {
                    return r;
                }
            }

            throw new Exception("Ресурс " + ID + " не найден.");
        }

        internal Building FindExternalBuilding(string ID)
        {
            foreach (Building b in ExternalBuildings)
            {
                if (b.ID == ID)
                {
                    return b;
                }
            }

            throw new Exception("Здание " + ID + " не найдено.");
        }

        internal Building FindInternalBuilding(string ID)
        {
            foreach (Building b in InternalBuildings)
            {
                if (b.ID == ID)
                {
                    return b;
                }
            }

            throw new Exception("Здание " + ID + " не найдено.");
        }

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
    }
}
