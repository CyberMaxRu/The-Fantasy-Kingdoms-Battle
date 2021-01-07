using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс сооружения в Королевстве
    internal abstract class TypeKingdomConstruction : TypeConstruction
    {
        public TypeKingdomConstruction(XmlNode n) : base(n)
        {
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

                    /*switch (TypeIncome)
                    {
                        case TypeIncome.None:
                            Debug.Assert(level.Income == 0);
                            break;
                        case TypeIncome.PerHeroes:
                            break;
                        case TypeIncome.Persistent:
                            Debug.Assert(level.Income > 0);
                            break;
                        default:
                            throw new Exception("Неизвестный тип дохода.");
                    }*/

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

            // Загружаем исследования
            XmlNode nr = n.SelectSingleNode("Researches");
            if (nr != null)
            {
                Researches = new Research[Convert.ToInt32(n.SelectSingleNode("LayersResearches").InnerText), FormMain.Config.PlateHeight, FormMain.Config.PlateWidth];

                Research research;

                foreach (XmlNode l in nr.SelectNodes("Research"))
                {
                    research = new Research(l);
                    Debug.Assert(Researches[research.Layer, research.Coord.Y, research.Coord.X] == null);
                    Researches[research.Layer, research.Coord.Y, research.Coord.X] = research;
                }
            }
        }

        internal Level[] Levels;
        internal Research[,,] Researches;

        internal void TuneResearches()
        {
            if (Researches != null)
                for (int z = 0; z < Researches.GetLength(0); z++)
                    for (int y = 0; y < Researches.GetLength(1); y++)
                        for (int x = 0; x < Researches.GetLength(2); x++)
                            Researches[z, y, x]?.FindItem();
        }
    }
}
