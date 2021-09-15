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
    internal enum CategoryConstruction { Guild, Economic, Military, Temple, External, Lair, Place, BasePlace };// Категория сооружения
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

            int layersResearches = 0;

            if (IsInternalConstruction)
            {
                Page = (ConstructionPage)Enum.Parse(typeof(ConstructionPage), GetStringNotNull(n, "Page"));
                CoordInPage = new Point(GetIntegerNotNull(n, "Pos") - 1, GetIntegerNotNull(n, "Line") - 1);
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
                layersResearches = GetInteger(n, "LayersCellMenu");

                if (IsInternalConstruction)
                {
                    ResearchesPerDay = GetIntegerNotNull(n, "ResearchesPerDay");
                }
                else
                {
                    XmlFieldNotExist(n, "ResearchesPerDay");
                }
            }
            else
            {
                XmlFieldNotExist(n, "DefaultLevel");
                XmlFieldNotExist(n, "MaxLevel");
                XmlFieldNotExist(n, "ResearchesPerDay");
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
                Debug.Assert(tec.ImageIndex != ImageIndex);
            }

            // Загружаем информацию об уровнях
            if ((IsOurConstruction || (n.SelectSingleNode("Levels") != null)) && (MaxLevel > 0))
            {
                Levels = new LevelConstruction[MaxLevel + 1];// Для упрощения работы с уровнями, добавляем 1, чтобы уровень был равен индексу в массиве

                XmlNode nl = n.SelectSingleNode("Levels");
                if (nl != null)
                {
                    LevelConstruction level;

                    foreach (XmlNode l in nl.SelectNodes("Level"))
                    {
                        level = new LevelConstruction(l);
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
            }

            // Загружаем исследования
            if (IsOurConstruction)
            {
                XmlNode nr = n.SelectSingleNode("CellsMenu");
                if (nr != null)
                {
                    Debug.Assert(layersResearches > 0, $"У {ID} не указано количество слоев меню, но есть меню.");
                    Researches = new DescriptorCellMenu[layersResearches, Config.PlateHeight, Config.PlateWidth];
                    List<DescriptorCellMenu> listMenu = new List<DescriptorCellMenu>();

                    DescriptorCellMenu research;

                    foreach (XmlNode l in nr.SelectNodes("CellMenu"))
                    {
                        research = new DescriptorCellMenu(l);
                        Debug.Assert(Researches[research.Layer, research.Coord.Y, research.Coord.X] == null,
                            $"У {ID} в слое {research.Layer} в ячейке ({research.Coord.Y}, {research.Coord.X}) уже есть сущность.");

                        foreach (DescriptorCellMenu tcm in listMenu)
                        {
                            Debug.Assert(research.NameTypeObject != tcm.NameTypeObject, $"У {ID} в меню повторяется объект {research.NameTypeObject}.");
                        }

                        Researches[research.Layer, research.Coord.Y, research.Coord.X] = research;
                        listMenu.Add(research);
                    }
                }
            }

            // Информация о монстрах
            XmlNode ne = n.SelectSingleNode("Monsters");
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
                Debug.Assert(ResearchesPerDay > 0);
                Debug.Assert(ResearchesPerDay <= 10);

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
                //Debug.Assert(ResearchesPerDay == 0);

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
        internal int ResearchesPerDay { get; }// Количество исследований в сооружении в день
        internal bool HasTreasury { get; }// Имеет собственную казну (Замок, гильдии, храмы)
        internal int GoldByConstruction { get; }// Количество золота в казне при постройке
        internal DescriptorCellMenu[,,] Researches;

        //
        internal LevelConstruction[] Levels;

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

            if (Levels != null)
            {
                foreach (LevelConstruction l in Levels)
                {
                    if (l != null)
                        foreach (Requirement r in l.Requirements)
                            r.TuneDeferredLinks();
                }
            }

            if (Researches != null)
            {
                for (int z = 0; z < Researches.GetLength(0); z++)
                    for (int y = 0; y < Researches.GetLength(1); y++)
                        for (int x = 0; x < Researches.GetLength(2); x++)
                            Researches[z, y, x]?.TuneDeferredLinks();
            }

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
    }
}