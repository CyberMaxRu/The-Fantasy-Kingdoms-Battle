using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal enum CategoryBuilding { Guild, Castle, Temple, Tower }
    internal enum TypeIncome { None, Persistent, PerHeroes }

    // Класс здания
    internal sealed class Building : TypeKingdomConstruction
    {
        public Building(XmlNode n) : base(n)
        {
            CategoryBuilding = (CategoryBuilding)Enum.Parse(typeof(CategoryBuilding), n.SelectSingleNode("CategoryBuilding").InnerText);
            TypeIncome = (TypeIncome)Enum.Parse(typeof(TypeIncome), n.SelectSingleNode("TypeIncome").InnerText);
            HasTreasury = Convert.ToBoolean(n.SelectSingleNode("HasTreasury").InnerText);
            GoldByConstruction = Convert.ToInt32(n.SelectSingleNode("GoldByConstruction").InnerText);

            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);

            if (!HasTreasury)
            {
                Debug.Assert(GoldByConstruction == 0);
            }

            // Проверяем, что таких же ID и наименования нет
            foreach (Building b in FormMain.Config.Buildings)
            {
                if (b.ID == ID)
                    throw new Exception("В конфигурации зданий повторяется ID = " + ID);

                if (b.Name == Name)
                    throw new Exception("В конфигурации зданий повторяется Name = " + Name);

                if (b.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации зданий повторяется ImageIndex = " + ImageIndex.ToString());
            }

        }

        internal TypeHero TrainedHero { get; set; }
        internal bool HasTreasury { get; }// Имеет собственную казну
        internal int GoldByConstruction { get; }// Количество золота в казне при постройке
        internal CategoryBuilding CategoryBuilding { get; }
        internal TypeIncome TypeIncome { get; }
        internal PanelBuilding Panel { get; set; }
    }
}