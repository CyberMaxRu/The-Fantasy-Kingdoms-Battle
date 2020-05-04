using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal abstract class Construction
    {
        public Construction(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            DefaultLevel = Convert.ToInt32(n.SelectSingleNode("DefaultLevel").InnerText);
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);

            Debug.Assert(DefaultLevel >= 0);
            Debug.Assert(MaxLevel > 0);
            Debug.Assert(MaxLevel <= 3);
            Debug.Assert(DefaultLevel <= MaxLevel);

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
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal int DefaultLevel { get; }
        internal int MaxLevel { get; }

        internal Level[] Levels;
    }
}