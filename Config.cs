using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Config
    {
        //private PrivateFontCollection pfc = new PrivateFontCollection();
        //private FontFamily ffMajesty2;

        public Config(string pathResources, FormMain fm)
        {
            FormMain.Config = this;
            PathResources = pathResources;

            // 
            MaxLevelSkill = 3;

            //
            XmlDocument xmlDoc;

            // Загружаем конфигурацию игры
            xmlDoc = CreateXmlDocument("Config\\Game.xml");
            LoadConfigGame(xmlDoc);

            // Загрузка конфигураций лобби
            xmlDoc = CreateXmlDocument("Config\\TypeLobby.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeLobbies/TypeLobby"))
            {
                TypeLobbies.Add(new TypeLobby(n));
            }

            // Загрузка конфигурации гильдий
            xmlDoc = CreateXmlDocument(@"Config\TypeGuilds.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeGuilds/TypeGuild"))
            {
                TypeGuilds.Add(new TypeGuild(n));
            }

            // Загрузка конфигурации экономических зданий
            xmlDoc = CreateXmlDocument(@"Config\TypeEconomicConstructions.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeConstructionEconomics/TypeConstructionEconomic"))
            {
                TypeEconomicConstructions.Add(new TypeEconomicConstruction(n));
            }

            // Загрузка конфигурации храмов
            xmlDoc = CreateXmlDocument(@"Config\TypeTemples.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeTemples/TypeTemple"))
            {
                TypeTemples.Add(new TypeTemple(n));
            }

            // Загрузка логов монстров
            xmlDoc = CreateXmlDocument("Config\\TypeLairs.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeLairs/TypeLair"))
            {
                TypeLairs.Add(new TypeLair(n));
            }

            // Создаем единую конфигурацию сооружений
            TypeConstructionsOfKingdom.AddRange(TypeGuilds);
            TypeConstructionsOfKingdom.AddRange(TypeEconomicConstructions);
            TypeConstructionsOfKingdom.AddRange(TypeTemples);
            TypeConstructions.AddRange(TypeConstructionsOfKingdom);
            TypeConstructions.AddRange(TypeLairs);

            // Загрузка предметов
            xmlDoc = CreateXmlDocument("Config\\Items.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Items/Item"))
            {
                Items.Add(new Item(n));
            }

            // Загрузка оружия
            xmlDoc = CreateXmlDocument("Config\\Weapons.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/GroupWeapons/GroupWeapon"))
            {
                GroupWeapons.Add(new GroupWeapon(n));
            }

            // Загрузка доспехов
            xmlDoc = CreateXmlDocument("Config\\Armours.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/GroupArmours/GroupArmour"))
            {
                GroupArmours.Add(new GroupArmour(n));
            }

            // Загрузка конфигурации способностей
            xmlDoc = CreateXmlDocument("Config\\Abilities.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Abilities/Ability"))
            {
                Abilities.Add(new Ability(n));
            }

            // Загрузка конфигурации специализаций
            xmlDoc = CreateXmlDocument(@"Config\Specializations.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/Specializations/Specialization"))
            {
                Specializations.Add(new Specialization(n));
            }

            // Загрузка конфигурации вторичных навыков
            xmlDoc = CreateXmlDocument(@"Config\SecondarySkills.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/SecondarySkills/SecondarySkill"))
            {
                SecondarySkills.Add(new SecondarySkill(n));
            }

            // Загрузка конфигурации состояний существ
            xmlDoc = CreateXmlDocument(@"Config\StateCreature.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/StatesCreature/StateCreature"))
            {
                StatesCreature.Add(new StateCreature(n));
            }

            // Загрузка конфигурации типов существ
            xmlDoc = CreateXmlDocument("Config\\KindCreatures.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/KindCreatures/KindCreature"))
            {
                KindCreatures.Add(new KindCreature(n));
            }

            // Загрузка конфигурации горожан
            xmlDoc = CreateXmlDocument("Config\\TypeCitizens.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeCitizens/TypeCitizen"))
            {
                TypeCitizens.Add(new TypeCitizen(n));
            }

            // Загрузка конфигурации героев
            xmlDoc = CreateXmlDocument("Config\\TypeHeroes.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeHeroes/TypeHero"))
            {
                TypeHeroes.Add(new TypeHero(n));
            }

            // Загрузка монстров
            xmlDoc = CreateXmlDocument("Config\\TypeMonsters.xml");

            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeMonsters/TypeMonster"))
            {
                TypeMonsters.Add(new TypeMonster(n));
            }

            // Составляем общий пул существ
            TypeCreatures.AddRange(TypeHeroes);
            TypeCreatures.AddRange(TypeCitizens);
            TypeCreatures.AddRange(TypeMonsters);

            // Настраиваем связи
            foreach (Ability a in Abilities)
                a.TuneDeferredLinks();

            foreach (Specialization s in Specializations)
                s.TuneDeferredLinks();

            foreach (SecondarySkill ss in SecondarySkills)
                ss.TuneDeferredLinks();

            foreach (GroupWeapon gw in GroupWeapons)
                gw.TuneDeferredLinks();

            foreach (GroupArmour ga in GroupArmours)
                ga.TuneDeferredLinks();

            foreach (TypeCreature tc in TypeCreatures)
                tc.TuneDeferredLinks();

            foreach (TypeMapObject tc in TypeConstructions)
                tc.TuneDeferredLinks();

            foreach (TypeLobby tl in TypeLobbies)
                tl.TuneDeferredLinks();

            //foreach (TypeLair l in TypeLairs)
            //    l.TuneDeferredLinks();

            // Вспомогательные методы
            XmlDocument CreateXmlDocument(string pathToXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathResources + pathToXml);
                return doc;
            }
        }

        internal string PathResources { get; }
        internal List<TypeLobby> TypeLobbies { get; } = new List<TypeLobby>();       
        internal List<TypeMonster> TypeMonsters { get; } = new List<TypeMonster>();

        // Сооружения, постройки, логова
        internal List<TypeGuild> TypeGuilds { get; } = new List<TypeGuild>();
        internal List<TypeEconomicConstruction> TypeEconomicConstructions { get; } = new List<TypeEconomicConstruction>();
        internal List<TypeTemple> TypeTemples { get; } = new List<TypeTemple>();
        internal List<TypeLair> TypeLairs { get; } = new List<TypeLair>();
        internal List<TypeConstruction> TypeConstructionsOfKingdom { get; } = new List<TypeConstruction>();
        internal List<TypeMapObject> TypeConstructions { get; } = new List<TypeMapObject>();

        //
        internal List<Ability> Abilities { get; } = new List<Ability>();
        internal List<Specialization> Specializations { get; } = new List<Specialization>();
        internal List<SecondarySkill> SecondarySkills { get; } = new List<SecondarySkill>();
        internal List<StateCreature> StatesCreature { get; } = new List<StateCreature>();
        internal List<KindCreature> KindCreatures { get; } = new List<KindCreature>();
        internal List<TypeCitizen> TypeCitizens { get; } = new List<TypeCitizen>();
        internal List<TypeHero> TypeHeroes { get; } = new List<TypeHero>();
        internal List<Item> Items { get; } = new List<Item>();
        internal List<GroupWeapon> GroupWeapons { get; } = new List<GroupWeapon>();
        internal List<GroupArmour> GroupArmours { get; } = new List<GroupArmour>();
        internal int MaxLevelSkill { get; }
        internal List<TypeCreature> TypeCreatures { get; } = new List<TypeCreature>();

        // Константы
        internal int GridSize { get; private set; }// Размер ячейки сетки
        internal int GridSizeHalf { get; private set; }// Размер половины ячейки сетки
        internal Point ShiftForBorder { get; private set; }// Смещение иконки внутри рамки сущности
        internal int BorderInBigIcons => 1;// Прозрачный бордюр у иконок 128 * 128
        internal int HeroInRow { get; private set; }// Героев в ряду
        internal int HeroRows { get; private set; }// Рядов героев
        internal int RowsBetweenSides { get; private set; }// Рядов между сторонами
        internal int WidthBorderBattlefield { get; set; }// Ширина бордюра поля боя
        internal int StepsInSecond { get; private set; }// Шагов в секунду
        internal int StepInMSec {get; private set; }// Время шага в миллисекундах
        internal int MaxDurationBattle { get; private set; }// Максимальная длительность боя в секундах
        internal int MaxStepsInBattle { get; private set; }// Максимальная длительность боя в тактах
        internal int MaxFramesPerSecond { get; private set; }// Максимальная частота перерисовки кадров
        internal int MaxDurationFrame { get; private set; }// Максимальная длительность кадра
        internal int MaxStatPointPerLevel { get; private set; }
        internal int StepsHeroInTumbstone { get; private set; }// Сколько шагов герой в могиле перед исчезновением
        internal int UnitStepsTimeToDisappearance { get; private set; }// Сколько шагов могила юнита исчезает
        internal int PlateWidth { get; private set; }// Количество ячеек на панели справа по горизонтали
        internal int PlateHeight { get; private set; }// Количество ячеек на панели справа по вертикали
        internal int MinRowsEntities { get; private set; }// Минимальное количество строк сущностей в панели справа
        internal string IDHeroPeasant { get; private set; }// ID типа героя - крестьянин
        internal string IDBuildingCastle { get; private set; }// ID Замка
        internal int WarehouseWidth { get; private set; }// Количество ячеек в ряду склада
        internal int WarehouseHeight { get; private set; }// Количество рядов ячеек склада
        internal int WarehouseMaxCells { get; private set; }// Количество ячеек в складе
        internal int BuildingMaxLines { get; private set; }// Максимальное количество линий зданий
        internal int BuildingMaxPos { get; private set; }// Максимальное количество позиций в линии зданий

        // Цвета
        internal Color CommonBorder { get; private set; }
        internal Color CommonSelectedBorder { get; private set; }
        internal Color CommonCaptionPage { get; private set; }
        internal Color CommonLevel { get; private set; }
        internal Color CommonCost { get; private set; }
        internal Color CommonQuantity { get; private set; }
        internal Color CommonPopupQuantity { get; private set; }
        internal Color CommonPopupQuantityBack { get; private set; }

        internal Color SelectedPlayerBorder { get; private set; }
        internal Color DamageToCastlePositive { get; private set; }
        internal Color DamageToCastleNegative { get; private set; }

        internal Color HintHeader { get; private set; }
        internal Color HintAction { get; private set; }
        internal Color HintDescription { get; private set; }
        internal Color HintIncome { get; private set; }
        internal Color HintParameter { get; private set; }
        internal Color HintRequirementsMet { get; private set; }
        internal Color HintRequirementsNotMet { get; private set; }

        internal Color BattlefieldSystemInfo { get; private set; }
        internal Color BattlefieldPlayerName { get; private set; }
        internal Color BattlefieldPlayerHealth { get; private set; }
        internal Color BattlefieldPlayerHealthNone { get; private set; }
        internal Color BattlefieldGrid { get; private set; }
        internal Color BattlefieldAllyColor { get; private set; }
        internal Color BattlefieldEnemyColor { get; private set; }
        internal Color BattlefieldTextWin { get; private set; }
        internal Color BattlefieldTextDraw { get; private set; }
        internal Color BattlefieldTextLose { get; private set; }

        internal Color UnitHealth { get; private set; }
        internal Color UnitHealthNone { get; private set; }
        internal Color UnitNormalParam { get; private set; }
        internal Color UnitLowNormalParam { get; private set; }
        internal Color UnitHighNormalParam { get; private set; }

        // Шрифты
        internal Font FontToolbar { get; private set; }
        internal Font FontLevel { get; private set; }
        internal Font FontQuantity { get; private set; }
        internal Font FontPopupQuantity { get; private set; }
        internal Font FontCost { get; private set; }
        internal Font FontCaptionPage { get; private set; }
        internal Font FontNamePage { get; private set; }
        internal Font FontBattlefieldPlayer { get; private set; }
        internal Font FontBattlefieldTimer { get; private set; }
        internal Font FontBattlefieldDamage { get; private set; }
        internal Font FontHintMainText { get; private set; }
        internal Font FontHintAdditionalText { get; private set; }
        internal Font FontBuildingLevel { get; private set; }
        internal Font FontBuildingCaption { get; private set; }

        //
        internal Brush brushControl { get; private set; } = new SolidBrush(Color.White);
        internal Pen PenBorder { get; private set; }
        internal Pen PenSelectedBorder { get; private set; }

        //
        internal TypeConstruction FindTypeConstructionOfKingdom(string ID)
        {
            foreach (TypeConstruction tck in TypeConstructionsOfKingdom)
            {
                if (tck.ID == ID)
                    return tck;
            }

            throw new Exception("Сооружение Королевства " + ID + " не найдено.");
        }

        internal TypeEconomicConstruction FindTypeEconomicConstruction(string ID)
        {
            foreach (TypeEconomicConstruction tec in TypeEconomicConstructions)
            {
                if (tec.ID == ID)
                    return tec;
            }

            throw new Exception("Экономическое сооружение " + ID + " не найдено.");
        }

        internal TypeGuild FindTypeGuild(string ID)
        {
            foreach (TypeGuild tg in TypeGuilds)
            {
                if (tg.ID == ID)
                    return tg;
            }

            throw new Exception("Экономическое сооружение " + ID + " не найдено.");
        }

        internal TypeHero FindTypeHero(string ID)
        {
            foreach (TypeHero th in TypeHeroes)
            {
                if (th.ID == ID)
                    return th;
            }

            throw new Exception("Герой " + ID + " не найден.");
        }

        internal TypeMonster FindTypeMonster(string ID)
        {
            foreach (TypeMonster tm in TypeMonsters)
            {
                if (tm.ID == ID)
                    return tm;
            }

            throw new Exception("Тип монстра " + ID + " не найден.");
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

        internal StateCreature FindStateCreature(string ID)
        {
            foreach (StateCreature sc in StatesCreature)
            {
                if (sc.ID == ID)
                    return sc;
            }

            throw new Exception("Состояние существа " + ID + " не найдено.");
        }

        internal KindCreature FindKindCreature(string ID)
        {
            foreach (KindCreature tu in KindCreatures)
            {
                if (tu.ID == ID)
                    return tu;
            }

            throw new Exception("Вид существа " + ID + " не найден.");
        }

        internal TypeLair FindTypeLair(string ID)
        {
            foreach (TypeLair tl in TypeLairs)
            {
                if (tl.ID == ID)
                    return tl;
            }

            throw new Exception($"Тип логова {ID} не найден.");
        }

        internal Ability FindAbility(string ID)
        {
            foreach (Ability a in Abilities)
            {
                if (a.ID == ID)
                    return a;
            }

            throw new Exception("Способность " + ID + " не найдена.");
        }

        internal Specialization FindSpecialization(string ID)
        {
            foreach (Specialization s in Specializations)
            {
                if (s.ID == ID)
                    return s;
            }

            throw new Exception("Специализация " + ID + " не найдена.");
        }

        internal SecondarySkill FindSecondarySkill(string ID)
        {
            foreach (SecondarySkill ss in SecondarySkills)
            {
                if (ss.ID == ID)
                    return ss;
            }

            throw new Exception("Вторичный навык " + ID + " не найден.");
        }

        internal TypeCreature FindTypeCreature(string ID)
        {
            foreach (TypeCreature tc in TypeCreatures)
            {
                if (tc.ID == ID)
                    return tc;
            }

            throw new Exception("Тип существ " + ID + " не найден.");
        }

        internal GroupWeapon FindGroupWeapon(string ID)
        {
            foreach (GroupWeapon gw in GroupWeapons)
                if (gw.ID == ID)
                    return gw;

            throw new Exception("Группа оружия " + ID + " не найдена.");
        }

        internal GroupArmour FindGroupArmour(string ID)
        {
            foreach (GroupArmour ga in GroupArmours)
                if (ga.ID == ID)
                    return ga;

            throw new Exception("Группа доспехов " + ID + " не найдена.");
        }

        internal Weapon FindWeapon(string ID)
        {
            foreach (GroupWeapon gw in GroupWeapons)
                foreach (Weapon w in gw.Weapons)
                    if (w.ID == ID)
                        return w;

            throw new Exception("Оружие " + ID + " не найдено.");
        }

        internal Armour FindArmour(string ID)
        {
            foreach (GroupArmour ga in GroupArmours)
                foreach (Armour a in ga.Armours)
                    if (a.ID == ID)
                        return a;

            throw new Exception("Доспех " + ID + " не найден.");
        }

        private void LoadConfigGame(XmlDocument xmlDoc)
        {
            GridSize = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/GridSize").InnerText);
            Debug.Assert(GridSize >= 2);
            Debug.Assert(GridSize <= 20);
            Debug.Assert(GridSize % 2 == 0);
            GridSizeHalf = GridSize / 2;

            ShiftForBorder = new Point(Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ShiftForBorderX").InnerText),
                Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ShiftForBorderY").InnerText));
            Debug.Assert(ShiftForBorder.X >= 0);
            Debug.Assert(ShiftForBorder.X <= 10);
            Debug.Assert(ShiftForBorder.Y >= 0);
            Debug.Assert(ShiftForBorder.Y <= 10);

            PlateWidth = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/PlateWidth").InnerText);
            Debug.Assert(PlateWidth >= 2);
            Debug.Assert(PlateWidth <= 8);
            PlateHeight = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/PlateHeight").InnerText);
            Debug.Assert(PlateHeight >= 2);
            Debug.Assert(PlateHeight <= 8);
            MinRowsEntities = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/MinRowsEntityInPlate").InnerText);
            Debug.Assert(MinRowsEntities >= 2);
            Debug.Assert(MinRowsEntities <= 6);

            BuildingMaxLines = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/BuildingMaxLines").InnerText);
            Debug.Assert(BuildingMaxLines >= 2);
            Debug.Assert(BuildingMaxLines <= 5);

            BuildingMaxPos = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/BuildingMaxPos").InnerText);
            Debug.Assert(BuildingMaxLines >= 2);
            Debug.Assert(BuildingMaxLines <= 5);

            HeroInRow = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/HeroInRow").InnerText);
            Debug.Assert(HeroInRow >= 3);
            Debug.Assert(HeroInRow <= 14);
            HeroRows = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/HeroRows").InnerText);
            Debug.Assert(HeroRows >= 3);
            Debug.Assert(HeroRows <= 14);
            RowsBetweenSides = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/RowsBetweenSides").InnerText);
            Debug.Assert(RowsBetweenSides >= 0);
            Debug.Assert(RowsBetweenSides <= 4);

            WidthBorderBattlefield = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/WidthBorder").InnerText);
            Debug.Assert(HeroRows >= 1);
            Debug.Assert(HeroRows <= 32);

            StepsInSecond = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battle/StepsInSecond").InnerText);
            Debug.Assert(StepsInSecond >= 5);
            Debug.Assert(StepsInSecond <= 50);
            Debug.Assert(1000 % StepsInSecond == 0);
            StepInMSec = 1000 / StepsInSecond;

            MaxDurationBattle = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battle/MaxDurationBattle").InnerText);
            Debug.Assert(MaxDurationBattle >= 30);
            Debug.Assert(MaxDurationBattle <= 3600);

            MaxStepsInBattle = MaxDurationBattle * StepsInSecond;

            MaxFramesPerSecond = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battle/MaxFramesPerSecond").InnerText);
            Debug.Assert(MaxFramesPerSecond >= 5);
            Debug.Assert(MaxFramesPerSecond <= 100);

            MaxDurationFrame = 1_000 / MaxFramesPerSecond;

            MaxStatPointPerLevel = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxStatPointPerLevel").InnerText);
            Debug.Assert(MaxStatPointPerLevel >= 5);
            Debug.Assert(MaxStatPointPerLevel <= 100);
            double timeInTumbstone = XmlUtils.GetDouble(xmlDoc.SelectSingleNode("Game/Heroes/TimeInTumbstone"));
            Debug.Assert(timeInTumbstone >= 0);
            Debug.Assert(timeInTumbstone <= 10);
            StepsHeroInTumbstone = (int)(timeInTumbstone * StepsInSecond);
            Debug.Assert(StepsHeroInTumbstone >= 10);
            Debug.Assert(StepsHeroInTumbstone <= 1000);
            double timeToDisappearance = XmlUtils.GetDouble(xmlDoc.SelectSingleNode("Game/Heroes/TimeToDisappearance"));
            Debug.Assert(timeToDisappearance >= 0);
            Debug.Assert(timeToDisappearance <= 10);
            UnitStepsTimeToDisappearance = (int)(timeToDisappearance * StepsInSecond);
            Debug.Assert(StepsHeroInTumbstone >= UnitStepsTimeToDisappearance);

            IDHeroPeasant = xmlDoc.SelectSingleNode("Game/Links/HeroPeasant").InnerText;
            Debug.Assert(IDHeroPeasant.Length > 0);
            IDBuildingCastle = xmlDoc.SelectSingleNode("Game/Links/BuildingCastle").InnerText;
            Debug.Assert(IDBuildingCastle.Length > 0);

            WarehouseWidth = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Warehouse/Width").InnerText);
            Debug.Assert(WarehouseWidth >= 5);
            Debug.Assert(WarehouseWidth <= 30);
            WarehouseHeight = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Warehouse/Height").InnerText);
            Debug.Assert(WarehouseHeight >= 1);
            Debug.Assert(WarehouseHeight <= 10);
            WarehouseMaxCells = WarehouseWidth * WarehouseHeight;

            // Цвета
            CommonBorder = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/Border").InnerText);
            CommonSelectedBorder = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/SelectedBorder").InnerText);
            CommonCaptionPage = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/CaptionPage").InnerText);
            CommonLevel = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/Level").InnerText);
            CommonCost = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/Cost").InnerText);
            CommonQuantity = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/Quantity").InnerText);
            CommonPopupQuantity = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/PopupQuantity").InnerText);
            CommonPopupQuantityBack = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Common/PopupQuantityBack").InnerText);

            SelectedPlayerBorder = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Player/SelectedBorder").InnerText);
            DamageToCastlePositive = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Player/DamageToCastlePositive").InnerText);
            DamageToCastleNegative = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Player/DamageToCastleNegative").InnerText);

            HintHeader = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/Header").InnerText);
            HintAction = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/Action").InnerText);
            HintDescription = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/Description").InnerText);
            HintIncome = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/Income").InnerText);
            HintParameter = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/Parameter").InnerText);
            HintRequirementsMet = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/RequirementsMet").InnerText);
            HintRequirementsNotMet = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Hint/RequirementsNotMet").InnerText);

            BattlefieldSystemInfo = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/SystemInfo").InnerText);
            BattlefieldPlayerName = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/PlayerName").InnerText);
            BattlefieldPlayerHealth = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/PlayerHealth").InnerText);
            BattlefieldPlayerHealthNone = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/PlayerHealthNone").InnerText);
            BattlefieldGrid = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/Grid").InnerText);
            BattlefieldAllyColor = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/AllyColor").InnerText);
            BattlefieldEnemyColor = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/EnemyColor").InnerText);
            BattlefieldTextWin = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/TextWin").InnerText);
            BattlefieldTextDraw = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/TextDraw").InnerText);
            BattlefieldTextLose = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Battlefield/TextLose").InnerText);

            UnitHealth = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Unit/Health").InnerText);
            UnitHealthNone = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Unit/HealthNone").InnerText);
            UnitNormalParam = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Unit/NormalParam").InnerText);
            UnitLowNormalParam = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Unit/LowNormalParam").InnerText);
            UnitHighNormalParam = Color.FromName(xmlDoc.SelectSingleNode("Game/Colors/Unit/HighNormalParam").InnerText);

            /*// Добавляем шрифт из ресурсов
            FontFamily GetResourceFontFamily(byte[] fontbytes)
            {
                // Шрифт Majesty2 не хочет работать из ресурсов. Другие шрифты работают.
                // Причину этого так и не выяснил. Поэтому шрифт выгружается в файл, загружается в шрифты и удаляется
                string fileFont = Path.GetTempFileName();
                File.WriteAllBytes(fileFont, fontbytes);
                pfc.AddFontFile(fileFont);
                return pfc.Families[0];

                /*IntPtr fontMemPointer = Marshal.AllocCoTaskMem(fontbytes.Length);
                Marshal.Copy(fontbytes, 0, fontMemPointer, fontbytes.Length);
                pfc.AddMemoryFont(fontMemPointer, fontbytes.Length);
                Marshal.FreeCoTaskMem(fontMemPointer);
                return pfc.Families[0];*//*
            }*/

            // Шрифты            
            //ffMajesty2 = GetResourceFontFamily(Properties.Resources.Majesty2);

            FontToolbar = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Toolbar"));
            FontLevel = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Level"));
            FontQuantity = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Quantity"));
            FontPopupQuantity = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/PopupQuantity"));
            FontCost = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Cost"));
            FontCaptionPage = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/CaptionPage"));
            FontNamePage = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/NamePage"));
            FontBattlefieldPlayer = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Battlefield/Player"));
            FontBattlefieldTimer = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Battlefield/Timer"));
            FontBattlefieldDamage = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Battlefield/Damage"));
            FontHintMainText = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Hint/MainText"));
            FontHintAdditionalText = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Hint/AdditionalText"));
            FontBuildingLevel = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Building/Level"));
            FontBuildingCaption = CreateFont(xmlDoc.SelectSingleNode("Game/Fonts/Building/Caption"));

            PenBorder = new Pen(CommonBorder);
            PenSelectedBorder = new Pen(CommonSelectedBorder);

            Font CreateFont(XmlNode n)
            {
                string name = n.SelectSingleNode("Name").InnerText;
                
/*                if (name == "Majesty2")
                {
                    if (XmlUtils.GetBool(n.SelectSingleNode("Bold"), false))
                        return new Font(ffMajesty2, Convert.ToInt32(n.SelectSingleNode("Size").InnerText), FontStyle.Bold);
                    else
                        return new Font(ffMajesty2, Convert.ToInt32(n.SelectSingleNode("Size").InnerText));
                }
                else
                {*/
                    if (XmlUtils.GetBool(n.SelectSingleNode("Bold"), false))
                        return new Font(name, Convert.ToInt32(n.SelectSingleNode("Size").InnerText), FontStyle.Bold);
                    else
                        return new Font(name, Convert.ToInt32(n.SelectSingleNode("Size").InnerText));
//                }
            }
        }

        internal Color ColorEntity(bool ally)
        {
            return ally ? BattlefieldAllyColor : BattlefieldEnemyColor;
        }

        internal Color ColorBorder(bool selected)
        {
            return selected ? CommonSelectedBorder : CommonBorder;
        }

        internal Color ColorBorderPlayer(Player p)
        {
            return p == p.Lobby.CurrentPlayer ? SelectedPlayerBorder : CommonBorder;
        }

        internal Color ColorMapObjectCaption(bool allowed)
        {
            return allowed ? Color.LimeGreen : Color.Gray;
        }

        internal Pen GetPenBorder(bool selected)
        {
            return selected ? PenSelectedBorder : PenBorder;
        }
    }
}
