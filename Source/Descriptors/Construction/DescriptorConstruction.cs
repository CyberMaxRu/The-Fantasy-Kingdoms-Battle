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
    internal enum CategoryConstruction { Guild, Economic, Military, Temple, External, Lair, Place, BasePlace, ElementLandscape };// Категория сооружения
    internal enum ConstructionPage { Guild, Economic, Temple, None };// Страница для размещения сооружения
    internal enum PriorityExecution { None = -1, Normal = 0, Warning = 1, High = 2, Exclusive = 3 };// Приоритет выполнения флага
    internal enum TypeFlag { None, Scout, Attack, Defense, Battle };// Тип флага

    // Тип сооружения - базовый класс для всех зданий, построек и мест
    internal sealed class DescriptorConstruction : DescriptorEntity
    {
        private readonly Uri uriSoundSelect;// Звук при выборе объекта
        private string nameTypePlaceForConstruct;

        public DescriptorConstruction(XmlNode n) : base(n)
        {
            if (GetString(n, "TypeConstruction").Length > 0)
                TypeConstruction = Config.FindTypeConstruction(GetString(n, "TypeConstruction"));
            Category = (CategoryConstruction)Enum.Parse(typeof(CategoryConstruction), GetStringNotNull(n, "Category"));
            IsInternalConstruction = (Category == CategoryConstruction.Guild) || (Category == CategoryConstruction.Economic) || (Category == CategoryConstruction.Temple) || (Category == CategoryConstruction.Military);
            IsOurConstruction = IsInternalConstruction || (Category == CategoryConstruction.External);
            HasTreasury = (Category == CategoryConstruction.Guild) || (Category == CategoryConstruction.Temple) || (ID == Config.IDConstructionCastle);
            uriSoundSelect = new Uri(Program.formMain.dirResources + @"Sound\Interface\ConstructionSelect\" + GetStringNotNull(n, "SoundSelect"));
            nameTypePlaceForConstruct = GetString(n, "TypePlaceForConstruct");
            Debug.Assert(Name != nameTypePlaceForConstruct);

            if (IsInternalConstruction)
            {
                Page = (ConstructionPage)Enum.Parse(typeof(ConstructionPage), GetStringNotNull(n, "Page"));
                CoordInPage = GetPoint(n, "Pos");
            }
            else
            {
                XmlFieldNotExist(n, "Page");
                XmlFieldNotExist(n, "Line");
                XmlFieldNotExist(n, "Pos");
                Page = ConstructionPage.None;
            }

            if (IsOurConstruction)
            {
                DefaultLevel = GetIntegerNotNull(n, "DefaultLevel");
                MaxLevel = GetIntegerNotNull(n, "MaxLevel");
                PlayerCanBuild = GetBoolean(n, "PlayerCanBuild", true);
            }
            else
            {
                DefaultLevel = 1;
                MaxLevel = 1;

                if (Category != CategoryConstruction.ElementLandscape)
                {
                    XmlFieldNotExist(n, "DefaultLevel");
                    XmlFieldNotExist(n, "MaxLevel");
                }
                XmlFieldNotExist(n, "PlayerCanBuild");
                XmlFieldNotExist(n, "LayersCellMenu");
            }

            if (HasTreasury)
            {
                GoldByConstruction = GetIntegerNotNull(n, "GoldByConstruction");
            }
            else
            {
                XmlFieldNotExist(n, "GoldByConstruction");
            }

            // Проверяем, что таких же ID и наименования нет
            foreach (DescriptorConstruction tec in Config.Constructions)
            {
                Debug.Assert(tec.ID != ID);
                Debug.Assert(tec.Name != Name);
                //Debug.Assert(tec.ImageIndex != ImageIndex);
            }

            // Загружаем информацию об уровнях
            if ((IsOurConstruction || (n.SelectSingleNode("Levels") != null)) && (MaxLevel > 0))
            {
                // Для удобства уровень равен номеру позиции в массиве
                Levels = new DescriptorCellMenuForConstructionLevel[MaxLevel + 1];

                XmlNode nl = n.SelectSingleNode("Levels");
                if (nl != null)
                {
                    DescriptorCellMenuForConstructionLevel level;
                    int number;

                    foreach (XmlNode l in nl.SelectNodes("Level"))
                    {
                        number = GetIntegerNotNull(l, "Number");
                        Debug.Assert(number > 0);
                        Debug.Assert(Levels[number] == null);
                        level = new DescriptorCellMenuForConstructionLevel(this, number, new Point(0, number - 1), l);
                        if (number > 1)
                            level.Requirements.Insert(0, new RequirementConstruction(level, ID, number - 1));

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

                        Levels[number] = level;
                        CheckFreeCellMenu(level.Coord);
                        ListResearches.Add(level);
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
                    ce = new DescriptorConstructionExtension(this, l);

                    foreach(DescriptorConstructionExtension ce2 in Extensions)
                    {
                        Debug.Assert(ce2.ID != ce.ID);
                    }

                    Extensions.Add(ce);
                }
            }

            // Загружаем информацию о мероприятиях
            XmlNode nodeEvents = n.SelectSingleNode("Events");
            if (nodeEvents != null)
            {
                DescriptorConstructionEvent dcEvent;
                foreach (XmlNode l in nodeEvents.SelectNodes("Event"))
                {
                    dcEvent = new DescriptorConstructionEvent(this, l);

                    foreach (DescriptorConstructionEvent dcEvent2 in Events)
                    {
                        Debug.Assert(dcEvent2.ID != dcEvent.ID);
                    }

                    Events.Add(dcEvent);
                }
            }

            // Загружаем меню
            XmlNode nr = n.SelectSingleNode("CellsMenu");
            if (nr != null)
            {
                DescriptorCellMenuForConstruction research;

                foreach (XmlNode l in nr.SelectNodes("CellMenu"))
                {
                    research = new DescriptorCellMenuForConstruction(this, l);
                    CheckFreeCellMenu(research.Coord);

                    foreach (DescriptorCellMenu tcm in ListResearches)
                    {
                        //Debug.Assert(research.Construction. NameTypeObject != tcm.NameTypeObject, $"У {ID} в меню повторяется объект {research.NameTypeObject}.");
                    }

                    ListResearches.Add(research);
                }
            }

            // Информация о монстрах
            ne = n.SelectSingleNode("Monsters");
            if (ne != null)
            {
                MonsterLevelLair mll;
                foreach (XmlNode l in ne.SelectNodes("Monster"))
                {
                    mll = new MonsterLevelLair(l);
                    Monsters.Add(mll);
                }
            }

            // Информация о награде
            if (n.SelectSingleNode("Reward") != null)
                Reward = new DescriptorReward(n.SelectSingleNode("Reward"));
            if (n.SelectSingleNode("HiddenReward") != null)
                HiddenReward = new DescriptorReward(n.SelectSingleNode("HiddenReward"));

            if (IsInternalConstruction)
            {
                Debug.Assert(DefaultLevel >= 0);
                Debug.Assert(DefaultLevel <= 5);
                Debug.Assert(MaxLevel > 0);
                Debug.Assert(MaxLevel <= 10);
                Debug.Assert(DefaultLevel <= MaxLevel);

                if (Category != CategoryConstruction.Temple)
                {
                    Debug.Assert(nameTypePlaceForConstruct.Length == 0);
                }
                else
                {
                    Debug.Assert(nameTypePlaceForConstruct.Length > 0);
                }
            }
            else
            {
                Debug.Assert(DefaultLevel >= 0);
                Debug.Assert(DefaultLevel <= 5);
                //Debug.Assert(MaxLevel == 1);
                Debug.Assert(DefaultLevel <= MaxLevel);

                if (Category == CategoryConstruction.External)
                {
                    //Debug.Assert(nameTypePlaceForConstruct != "");
                }
                else if (Category == CategoryConstruction.Place)
                {
                    Debug.Assert(nameTypePlaceForConstruct.Length > 0);
                }
                else if (Category == CategoryConstruction.BasePlace)
                {
                    Debug.Assert(nameTypePlaceForConstruct.Length == 0);
                }
                else
                {
                    Debug.Assert(nameTypePlaceForConstruct.Length > 0);
                }
            }



            //else
            //    throw new Exception("В конфигурации логова у " + ID + " нет информации об уровнях. ");

            void CheckFreeCellMenu(Point p)
            {
                foreach (DescriptorCellMenuForConstruction cm in ListResearches)
                {
                    Debug.Assert(!cm.Coord.Equals(p), $"У {ID} в ячейке ({p.X + 1}, {p.Y + 1}) уже есть сущность.");
                }
            }
        }

        internal DescriptorTypeConstruction TypeConstruction { get; }// Тип сооружения
        internal CategoryConstruction Category { get; }// Категория сооружения
        internal bool IsInternalConstruction { get; }// Это внутреннее сооружение
        internal bool IsOurConstruction { get; }// Это сооружение, относящееся к Королевству

        // Свойства, относящиеся только к зданиям Королевства
        internal ConstructionPage Page { get; }// Страница игрового интерфейса
        internal Point CoordInPage { get; }// Позиция на странице игрового интерфейса
        internal int DefaultLevel { get; }// Уровень сооружения по умолчанию
        internal int MaxLevel { get; }// Максимальный уровень сооружения
        internal bool PlayerCanBuild { get; }// Игрок может строить сооружение
        internal bool HasTreasury { get; }// Имеет собственную казну (Замок, гильдии, храмы)
        internal int GoldByConstruction { get; }// Количество золота в казне при постройке
        internal List<DescriptorConstructionExtension> Extensions { get; } = new List<DescriptorConstructionExtension>();
        internal List<DescriptorConstructionEvent> Events { get; } = new List<DescriptorConstructionEvent>();
        internal List<DescriptorCellMenuForConstruction> ListResearches { get; } = new List<DescriptorCellMenuForConstruction>();
        internal DescriptorCellMenuForConstructionLevel[] Levels { get; }

        //
        internal PanelConstruction Panel { get; set; }
        internal DescriptorCreature TrainedHero { get; set; }

        // Свойства, относящиеся к логовам монстров
        internal List<MonsterLevelLair> Monsters { get; } = new List<MonsterLevelLair>();
        internal DescriptorReward Reward { get; }// Награда за зачистку логова
        internal DescriptorReward HiddenReward { get; }// Скрытая награда за зачистку логова
        internal DescriptorConstruction TypePlaceForConstruct { get; private set; }// Тип сооружения, на котором строится сооружение

        internal void PlaySoundSelect()
        {
            Program.formMain.PlaySoundSelect(uriSoundSelect);
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            /*if (Levels != null)
            {
                foreach (DescriptorCellMenuForConstructionLevel l in Levels)
                {
                    if (l != null)
                        l.TuneDeferredLinks();
                }
            }*/

            foreach (DescriptorConstructionExtension ce in Extensions)
                ce.TuneDeferredLinks();

            foreach (DescriptorConstructionEvent ce in Events)
                ce.TuneDeferredLinks();

            foreach (DescriptorCellMenuForConstruction cm in ListResearches)
                cm.TuneDeferredLinks();

            foreach (MonsterLevelLair mll in Monsters)
            {
                mll.TuneDeferredLinks();

                // Проверяем, что тип монстра не повторяется
                foreach (MonsterLevelLair mlev in Monsters)
                    if ((mlev != mll) && (mlev.Monster != null))
                        if (mlev.Monster == mll.Monster)
                            throw new Exception("Тип монстра " + mll.Monster.ID + " повторяется.");
            }

            if (nameTypePlaceForConstruct.Length > 0)
                TypePlaceForConstruct = Config.FindConstruction(nameTypePlaceForConstruct);

            nameTypePlaceForConstruct = null;

            if ((DefaultLevel == 1) && (Levels != null) && (Levels[1] != null))// Убрать вторую проверку после доработки логов
                ListResearches.Remove(Levels[1]);
        }

        internal override void AfterTune()
        {
            base.AfterTune();

            foreach (DescriptorConstructionExtension ce in Extensions)
                ce.AfterTune();
        }

        internal string GetTextConstructionNotBuilded()
        {
            switch (Category)
            {
                case CategoryConstruction.Guild:
                    return "Гильдия не построена";
                case CategoryConstruction.Economic:
                    return "Здание не построено";
                case CategoryConstruction.Temple:
                    return "Храм не построен";
                default:
                    throw new Exception("Нельзя строить категорию сооружения: " + Category.ToString());
            }
        }

        internal string GetTextConstructionIsFull()
        {
            switch (Category)
            {
                case CategoryConstruction.Guild:
                    return "Гильдия заполнена";
                case CategoryConstruction.Economic:
                    throw new Exception("В экономическом здании не может быть героев для найма.");
                case CategoryConstruction.Temple:
                    return "Храм заполнен";
                default:
                    throw new Exception("Нельзя строить категорию сооружения: " + Category.ToString());
            }
        }

        internal DescriptorConstructionExtension FindExtension(string IDExtension, bool mustBeFound)
        {
            foreach (DescriptorConstructionExtension ce in Extensions)
            {
                if (ce.ID == IDExtension)
                    return ce;
            }

            if (mustBeFound)
                throw new Exception($"Доп. сооружение {IDExtension} не найдено в {ID}.");

            return null;
        }

        internal DescriptorConstructionEvent FindConstructionEvent(string IDEvent, bool mustBeExists = true)
        {
            foreach (DescriptorConstructionEvent dce in Events)
            {
                if (dce.ID == IDEvent)
                    return dce;
            }

            if (mustBeExists)
                throw new Exception($"Мероприятие {IDEvent} не найдено в {ID}.");

            return null;
        }

    }
}