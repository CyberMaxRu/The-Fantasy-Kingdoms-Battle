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
    internal enum Page { Guild, Economic, Temple, None };// Страница для размещения сооружения
    internal enum PriorityExecution { None = -1, Normal = 0, Warning = 1, High = 2, Exclusive = 3 };// Приоритет выполнения флага
    internal enum TypeFlag { None, Scout, Attack, Defense, Battle };// Тип флага

    // Тип сооружения - базовый класс для всех зданий, построек и мест
    internal class TypeConstruction : TypeObject
    {
        private Uri uriSoundSelect;// Звук при выборе объекта
        private string nameTypePlaceForConstruct;

        public TypeConstruction(XmlNode n) : base(n)
        {
            Category = (CategoryConstruction)Enum.Parse(typeof(CategoryConstruction), GetStringNotNull(n, "Category"));
            IsInternalConstruction = (Category == CategoryConstruction.Guild) || (Category == CategoryConstruction.Economic) || (Category == CategoryConstruction.Temple) || (Category == CategoryConstruction.Military);
            IsOurConstruction = IsInternalConstruction || (Category == CategoryConstruction.External);

            if (IsInternalConstruction)
            {
                Page = (Page)Enum.Parse(typeof(Page), GetStringNotNull(n, "Page"));
                CoordInPage = new Point(GetInteger(n, "Pos") - 1, GetInteger(n, "Line") - 1);
            }
            else
            {
                XmlFieldNotExist(n, "Page");
                XmlFieldNotExist(n, "Line");
                XmlFieldNotExist(n, "Pos");
                Page = Page.None;
            }

            HasTreasury = GetBoolean(n, "HasTreasury", false);
            GoldByConstruction = GetInteger(n, "GoldByConstruction");

            if (IsOurConstruction)
            {
                DefaultLevel = GetIntegerNotNull(n, "DefaultLevel");
                MaxLevel = GetIntegerNotNull(n, "MaxLevel");
            }
            else
            {
                XmlFieldNotExist(n, "DefaultLevel");
                XmlFieldNotExist(n, "MaxLevel");
            }

            ResearchesPerDay = GetInteger(n, "ResearchesPerDay");
            PlayerCanBuild = GetBoolean(n, "PlayerCanBuild", true);

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeConstruction tec in FormMain.Config.TypeConstructions)
            {
                Debug.Assert(tec.ID != ID);
                Debug.Assert(tec.Name != Name);
                Debug.Assert(tec.ImageIndex != ImageIndex);
            }

            uriSoundSelect = new Uri(Program.formMain.dirResources + @"Sound\Interface\ConstructionSelect\" + GetStringNotNull(n, "SoundSelect"));

            // Загружаем информацию об уровнях
            if ((IsInternalConstruction || (Category == CategoryConstruction.External) || (n.SelectSingleNode("Levels") != null)) && (MaxLevel > 0))
            {
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
            }

            // Загружаем исследования
            int layersResearches = GetInteger(n, "LayersCellMenu");
            XmlNode nr = n.SelectSingleNode("CellsMenu");
            if (nr != null)
            {
                Debug.Assert(layersResearches > 0);
                Researches = new TypeCellMenu[layersResearches, FormMain.Config.PlateHeight, FormMain.Config.PlateWidth];

                TypeCellMenu research;

                foreach (XmlNode l in nr.SelectNodes("CellMenu"))
                {
                    research = new TypeCellMenu(l);
                    Debug.Assert(Researches[research.Layer, research.Coord.Y, research.Coord.X] == null);
                    Researches[research.Layer, research.Coord.Y, research.Coord.X] = research;
                }
            }
            else
            {
                Debug.Assert(layersResearches == 0);
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

            MaxHeroes = GetInteger(n, "MaxHeroes");
            nameTypePlaceForConstruct = GetString(n, "TypePlaceForConstruct");
            Debug.Assert(Name != nameTypePlaceForConstruct);

            // Информация о награде
            if (n.SelectSingleNode("Reward") != null)
                TypeReward = new TypeReward(n.SelectSingleNode("Reward"));
            if (n.SelectSingleNode("HiddenReward") != null)
                HiddenReward = new TypeReward(n.SelectSingleNode("HiddenReward"));

            Debug.Assert(MaxHeroes < 50);

            if (IsInternalConstruction || (Category == CategoryConstruction.External))
            {
                //Debug.Assert(MaxHeroes == 0);
            }
            else
            {
                //Debug.Assert(MaxHeroes > 0);
            }

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

        internal CategoryConstruction Category { get; }// Категория сооружения
        internal bool IsInternalConstruction { get; }// Это внутреннее сооружение
        internal bool IsOurConstruction { get; }// Это сооружение, относящееся к Королевству
        internal Page Page { get; }// Страница игрового интерфейса
        internal Point CoordInPage { get; }// Позиция на странице игрового интерфейса
        internal int DefaultLevel { get; }// Уровень сооружения по умолчанию
        internal int MaxLevel { get; }// Максимальный уровень сооружения
        internal Level[] Levels;
        internal bool PlayerCanBuild { get; }// Игрок может строить сооружение
        internal int ResearchesPerDay { get; }// Количество исследований в сооружении в день
        internal PanelConstruction Panel { get; set; }
        internal bool HasTreasury { get; }// Имеет собственную казну
        internal int GoldByConstruction { get; }// Количество золота в казне при постройке
        internal TypeCreature TrainedHero { get; set; }

        internal TypeCellMenu[,,] Researches;
        internal List<MonsterLevelLair> Monsters { get; } = new List<MonsterLevelLair>();
        internal int MaxHeroes { get; }// Максимальное количество героев, которое может атаковать логово
        internal TypeReward TypeReward { get; }// Награда за зачистку логова
        internal TypeReward HiddenReward { get; }// Скрытая награда за зачистку логова
        internal TypeConstruction TypePlaceForConstruct { get; private set; }// Тип сооружения, на котором строится сооружение

        internal void PlaySoundSelect()
        {
            Program.formMain.PlaySoundSelect(uriSoundSelect);
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if (Levels != null)
            {
                foreach (Level l in Levels)
                {
                    if (l != null)
                        foreach (Requirement r in l.Requirements)
                            r.FindConstruction();
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
                TypePlaceForConstruct = FormMain.Config.FindTypeConstruction(nameTypePlaceForConstruct);

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

    // Класс монстров уровня логова
    internal sealed class MonsterLevelLair
    {
        private string idMonster;

        public MonsterLevelLair(XmlNode n)
        {
            idMonster = n.SelectSingleNode("ID").InnerText;
            StartQuantity = GetInteger(n, "StartQuantity");
            MaxQuantity = GetInteger(n, "MaxQuantity");
            Level = GetInteger(n, "Level");
            DaysRespawn = GetInteger(n, "DaysRespawn");
            QuantityRespawn = GetInteger(n, "QuantityRespawn");

            Debug.Assert(idMonster.Length > 0);
            Debug.Assert(StartQuantity >= 0);
            Debug.Assert(MaxQuantity > 0);
            Debug.Assert(StartQuantity <= MaxQuantity);
            Debug.Assert(Level > 0);
            Debug.Assert(DaysRespawn >= 0);
            Debug.Assert(DaysRespawn <= 25);
            Debug.Assert(QuantityRespawn >= 0);
            //Debug.Assert(QuantityRespawn <= 49);
        }

        internal TypeCreature Monster { get; private set; }
        internal int StartQuantity { get; }
        internal int MaxQuantity { get; }
        internal int Level { get; }
        internal int DaysRespawn { get; }
        internal int QuantityRespawn { get; }
        internal List<Monster> Monsters { get; } = new List<Monster>();

        internal void TuneDeferredLinks()
        {
            Monster = FormMain.Config.FindTypeCreature(idMonster);
            idMonster = null;
            Debug.Assert(Level <= Monster.MaxLevel);
        }
    }
}