using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Тип сооружения - базовый класс для всех зданий, построек и мест
    internal sealed class DescriptorConstruction : DescriptorActiveEntity
    {
        public DescriptorConstruction(XmlNode n) : base(n)
        {
            Debug.Assert(UriSoundSelect != null);

            if (GetString(n, "TypeConstruction").Length > 0)
                TypeConstruction = Descriptors.FindTypeConstruction(GetString(n, "TypeConstruction"));

            MaxLevel = GetIntegerNotNull(n, "MaxLevel");

            // Проверяем, что таких же ID и наименования нет
            foreach (DescriptorConstruction tec in Descriptors.Constructions)
            {
                Debug.Assert(tec.ID != ID);
                Debug.Assert(tec.Name != Name);
                //Debug.Assert(tec.ImageIndex != ImageIndex);
            }

            // Загружаем информацию об уровнях
            if ((n.SelectSingleNode("Levels") != null) && (MaxLevel > 0))
            {
                // Для удобства уровень равен номеру позиции в массиве
                Levels = new DescriptorConstructionLevel[MaxLevel + 1];

                XmlNode nl = n.SelectSingleNode("Levels");
                if (nl != null)
                {
                    DescriptorConstructionLevel level;

                    foreach (XmlNode l in nl.SelectNodes("Level"))
                    {
                        level = new DescriptorConstructionLevel(this, l);
                        Debug.Assert(Levels[level.Number] is null);
                        Levels[level.Number] = level;

                        //if (number > 1)
                        //    level.Requirements.Insert(0, new RequirementConstruction(level, ID, number - 1));

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

                        //Levels[number] = level;
                        //CheckFreeCellMenu(level.Coord);
                        //CellsMenu.Add(level);
                    }

                    Debug.Assert(Levels[0] is null);

                    for (int i = 1; i < Levels.Length; i++)
                    {
                        if (Levels[i] is null)
                            throw new Exception($"В конфигурации зданий у {ID} нет информации об уровне {i}.");
                    }
                }
                else
                    throw new Exception("В конфигурации зданий у " + ID + " нет информации об уровнях. ");
            }

            // Загружаем информацию о дополнительных сооружениях
            XmlNode ne = n.SelectSingleNode("Extensions");
            if (ne != null)
            {
                DescriptorConstructionExtension ce;
                foreach (XmlNode l in ne.SelectNodes("Extension"))
                {
                    new DescriptorConstructionExtension(this, l);
                }
            }

            // Загружаем информацию о мероприятиях
            XmlNode nodeEvents = n.SelectSingleNode("Events");
            if (nodeEvents != null)
            {
                foreach (XmlNode l in nodeEvents.SelectNodes("Event"))
                    new DescriptorConstructionMassEvent(this, l);
            }

            // Загружаем информацию об улучшениях
            XmlNode nodeImprovements = n.SelectSingleNode("Improvements");
            if (nodeImprovements != null)
            {
                foreach (XmlNode l in nodeImprovements.SelectNodes("Improvement"))
                    new DescriptorConstructionImprovement(this, l);
            }

            // Загружаем информацию об услугах
            XmlNode nodeServices = n.SelectSingleNode("Services");
            if (nodeServices != null)
            {
                foreach (XmlNode l in nodeServices.SelectNodes("Service"))
                    new DescriptorConstructionService(this, l);
            }

            // Загрузка информацию о заклинаниях
            XmlNode nodeSpells = n.SelectSingleNode("Spells");
            if (nodeSpells != null)
            {
                foreach (XmlNode l in nodeSpells.SelectNodes("Spell"))
                    new DescriptorConstructionSpell(this, l);
            }

            // Загружаем информацию о товарах
            XmlNode np = n.SelectSingleNode("Products");
            if (np != null)
            {
                foreach (XmlNode l in np.SelectNodes("Product"))
                    new DescriptorProduct(this, l);
            }

            //else
            //    throw new Exception("В конфигурации логова у " + ID + " нет информации об уровнях. ");
        }

        internal DescriptorTypeConstruction TypeConstruction { get; }// Тип сооружения

        // Свойства, относящиеся только к зданиям Королевства
        internal int MaxLevel { get; }// Максимальный уровень сооружения
        internal bool PlayerCanBuild { get; }// Игрок может строить сооружение
        internal DescriptorConstructionLevel[] Levels { get; }

        //
        //internal PanelConstruction Panel { get; set; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            foreach (DescriptorActionForEntity cm in CellsMenu)
                cm.TuneLinks();


            //if ((DefaultLevel == 1) && (Levels != null) && (Levels[1] != null))// Убрать вторую проверку после доработки логов
            //    CellsMenu.Remove(Levels[1]);
        }


        internal string GetTextConstructionIsFull()
        {
            return "Гильдия заполнена";
            /*switch (Category)
            {
                case CategoryConstruction.Guild:
                    return "Гильдия заполнена";
                case CategoryConstruction.Military:
                    return "Здание заполнено";
                case CategoryConstruction.Economic:
                    throw new Exception("В экономическом здании не может быть героев для найма.");
                case CategoryConstruction.Temple:
                    return "Храм заполнен";
                default:
                    throw new Exception("Нельзя строить категорию сооружения: " + Category.ToString());
            }*/
        }

        internal override string GetTypeEntity() => TypeConstruction.Name;
    }
}