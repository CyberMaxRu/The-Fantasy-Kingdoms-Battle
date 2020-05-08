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

    // Класс здания
    internal sealed class Building
    {
        public Building(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = n.SelectSingleNode("Description").InnerText.Replace("/", Environment.NewLine);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            DefaultLevel = Convert.ToInt32(n.SelectSingleNode("DefaultLevel").InnerText);
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);
            CategoryBuilding = (CategoryBuilding)Enum.Parse(typeof(CategoryBuilding), n.SelectSingleNode("CategoryBuilding").InnerText);
            Line = Convert.ToInt32(n.SelectSingleNode("Line").InnerText);
            Position = FormMain.Config.Buildings.Count;
            MaxHeroes = Convert.ToInt32(n.SelectSingleNode("MaxHeroes").InnerText);

            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);
            Debug.Assert(DefaultLevel >= 0);
            Debug.Assert(MaxLevel > 0);
            Debug.Assert(MaxLevel <= 3);
            Debug.Assert(DefaultLevel <= MaxLevel);

            // Проверяем, что таких же ID и наименования нет
            foreach (Building b in FormMain.Config.Buildings)
            {
                if (b.ID == ID)
                    throw new Exception("В конфигурации зданий повторяется ID = " + ID);

                if (b.Name == Name)
                    throw new Exception("В конфигурации зданий повторяется Name = " + Name);

                if (b.Description == Description)
                    throw new Exception("В конфигурации зданий повторяется Description = " + Description);

                if (b.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации зданий повторяется ImageIndex = " + ImageIndex.ToString());
            }

            // Загружаем информацию об уровнях
            Levels = new Level[MaxLevel + 1];// Для упрощения работы с уровнями, добавляем 1, чтобы уровень был равен индексу в массиве

            XmlNode nl = n.SelectSingleNode("Levels");
            if (nl != null)
            {
                Level level;

                foreach (XmlNode l in nl.SelectNodes("Level"))
                {
                    level = new Level(l);
                    Debug.Assert(Levels[level.Pos] == null);
                    Levels[level.Pos] = level;
                }

                for (int i = 1; i < Levels.Length; i++)
                { 
                    if (Levels[i] == null)
                        throw new Exception("В конфигурации зданий у " + ID + " нет информации об уровне " + i.ToString());
                }
            }
            else
                throw new Exception("В конфигурации зданий у " + ID + " нет информации об уровнях. ");
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal int DefaultLevel { get; }
        internal int MaxLevel { get; }

        internal Level[] Levels;
        internal int Position { get; }
        internal int MaxHeroes { get; }
        internal Hero TrainedHero { get; set; }
        internal CategoryBuilding CategoryBuilding { get; }
        internal int Line { get; }
        internal PanelBuilding Panel { get; set; }
    }
}