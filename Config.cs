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

            // Загружаем внешние аватары
            if (File.Exists(pathResources + @"\ExternalAvatars.xml"))
                xmlDoc = CreateXmlDocument(@"\ExternalAvatars.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/ExternalAvatars/ExternalAvatar"))
            {
                ExternalAvatars.Add(n.InnerText);

                if (ExternalAvatars.Count == FormMain.MAX_AVATARS)
                    break;
            }

            // Загрузка компьютерных игроков
            xmlDoc = CreateXmlDocument("Config\\ComputerPlayers.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/ComputerPlayers/ComputerPlayer"))
            {
                ComputerPlayers.Add(new ComputerPlayer(n));
            }

            // Загрузка игроков-людей
            if (File.Exists(pathResources + "Players.xml"))
            {
                xmlDoc = CreateXmlDocument("Players.xml");
                foreach (XmlNode n in xmlDoc.SelectNodes("/Players/Player"))
                {
                    HumanPlayers.Add(new HumanPlayer(n));
                }
                AutoCreatedPlayer = false;
            }
            else
            {
                AddHumanPlayer("Игрок");
                AutoCreatedPlayer = true;
            }

            // Загрузка конфигураций лобби
            xmlDoc = CreateXmlDocument("Config\\TypeLobby.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeLobbies/TypeLobby"))
            {
                TypeLobbies.Add(new TypeLobby(n));
            }

            // Загрузка стартовых бонусов
            xmlDoc = CreateXmlDocument(@"Config\StartBonus.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/StartBonuses/StartBonus"))
            {
                StartBonuses.Add(new StartBonus(n));
            }

            // Загрузка конфигурации сооружений
            xmlDoc = CreateXmlDocument(@"Config\TypeConstructions.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/TypeConstructions/TypeConstruction"))
            {
                TypeConstructions.Add(new TypeConstruction(n));
            }

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

            foreach (TypeConstruction tc in TypeConstructions)
                tc.TuneDeferredLinks();

            foreach (TypeLobby tl in TypeLobbies)
                tl.TuneDeferredLinks();

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
        internal List<StartBonus> StartBonuses { get; } = new List<StartBonus>();
        internal List<ComputerPlayer> ComputerPlayers { get; } = new List<ComputerPlayer>();
        internal List<HumanPlayer> HumanPlayers { get; } = new List<HumanPlayer>();
        internal bool AutoCreatedPlayer { get; }
        internal List<TypeMonster> TypeMonsters { get; } = new List<TypeMonster>();
        internal List<TypeConstruction> TypeConstructions { get; } = new List<TypeConstruction>();

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

        //
        internal List<string> ExternalAvatars { get; } = new List<string>();

        // Константы
        internal int GridSize { get; private set; }// Размер ячейки сетки
        internal int GridSizeHalf { get; private set; }// Размер половины ячейки сетки
        internal int MaxLengthObjectName { get; set; }// Максимальная длина имени объекта
        internal Point ShiftForBorder { get; private set; }// Смещение иконки внутри рамки сущности
        internal int BorderInBigIcons => 1;// Прозрачный бордюр у иконок 128 * 128
        internal int HeroInRow { get; private set; }// Героев в ряду
        internal int HeroRows { get; private set; }// Рядов героев
        internal int RowsBetweenSides { get; private set; }// Рядов между сторонами
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
        internal string IDConstructionCastle { get; private set; }// ID Замка
        internal string IDPeasantHouse { get; private set; }// ID крестьянского дома
        internal string IDEmptyPlace { get; private set; }// ID пустого места
        internal string IDHolyPlace { get; private set; }// ID Святой земли
        internal string IDTradePlace { get; private set; }// ID торгового места
        internal int WarehouseWidth { get; private set; }// Количество ячеек в ряду склада
        internal int WarehouseHeight { get; private set; }// Количество рядов ячеек склада
        internal int WarehouseMaxCells { get; private set; }// Количество ячеек в складе
        internal int ConstructionMaxLines { get; private set; }// Максимальное количество линий сооружений
        internal int ConstructionMaxPos { get; private set; }// Максимальное количество позиций в линии сооружений

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

        //
        internal Brush brushControl { get; private set; } = new SolidBrush(Color.White);
        internal Pen PenBorder { get; private set; }
        internal Pen PenSelectedBorder { get; private set; }

        //
        internal TypeConstruction FindTypeConstruction(string ID, bool mustBeExists = true)
        {
            foreach (TypeConstruction tck in TypeConstructions)
            {
                if (tck.ID == ID)
                    return tck;
            }
            if (mustBeExists)
                throw new Exception("Сооружение " + ID + " не найдено.");

            return null;
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

        internal Item FindItem(string ID, bool mustBeExists = true)
        {
            foreach (Item i in Items)
            {
                if (i.ID == ID)
                    return i;
            }

            if (mustBeExists)
                throw new Exception("Предмет " + ID + " не найден.");

            return null;
        }

        internal Ability FindAbility(string ID, bool mustBeExists = true)
        {
            foreach (Ability a in Abilities)
            {
                if (a.ID == ID)
                    return a;
            }

            if (mustBeExists)
                throw new Exception("Способность " + ID + " не найдена.");

            return null;
        }

        internal GroupWeapon FindGroupWeapon(string ID, bool mustBeExists = true)
        {
            foreach (GroupWeapon gw in GroupWeapons)
                if (gw.ID == ID)
                    return gw;

            if (mustBeExists)
                throw new Exception("Группа оружия " + ID + " не найдена.");

            return null;
        }

        internal GroupArmour FindGroupArmour(string ID, bool mustBeExists = true)
        {
            foreach (GroupArmour ga in GroupArmours)
                if (ga.ID == ID)
                    return ga;

            if (mustBeExists)
                throw new Exception("Группа доспехов " + ID + " не найдена.");

            return null;
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

        private void LoadConfigGame(XmlDocument xmlDoc)
        {
            GridSize = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/GridSize").InnerText);
            Debug.Assert(GridSize >= 2);
            Debug.Assert(GridSize <= 20);
            Debug.Assert(GridSize % 2 == 0);
            GridSizeHalf = GridSize / 2;

            MaxLengthObjectName = XmlUtils.GetIntegerNotNull(xmlDoc.SelectSingleNode("Game/Interface/MaxLengthObjectName"));
            Debug.Assert(MaxLengthObjectName > 20);
            Debug.Assert(MaxLengthObjectName <= 63);

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

            ConstructionMaxLines = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ConstructionMaxLines").InnerText);
            Debug.Assert(ConstructionMaxLines >= 2);
            Debug.Assert(ConstructionMaxLines <= 5);

            ConstructionMaxPos = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ConstructionMaxPos").InnerText);
            Debug.Assert(ConstructionMaxLines >= 2);
            Debug.Assert(ConstructionMaxLines <= 5);

            HeroInRow = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/HeroInRow").InnerText);
            Debug.Assert(HeroInRow >= 3);
            Debug.Assert(HeroInRow <= 14);
            HeroRows = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/HeroRows").InnerText);
            Debug.Assert(HeroRows >= 3);
            Debug.Assert(HeroRows <= 14);
            RowsBetweenSides = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battlefield/RowsBetweenSides").InnerText);
            Debug.Assert(RowsBetweenSides >= 0);
            Debug.Assert(RowsBetweenSides <= 4);

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
            IDConstructionCastle = xmlDoc.SelectSingleNode("Game/Links/ConstructionCastle").InnerText;
            Debug.Assert(IDConstructionCastle.Length > 0);
            IDPeasantHouse = xmlDoc.SelectSingleNode("Game/Links/PeasantHouse").InnerText;
            Debug.Assert(IDPeasantHouse.Length > 0);
            IDEmptyPlace = xmlDoc.SelectSingleNode("Game/Links/EmptyPlace").InnerText;
            Debug.Assert(IDEmptyPlace.Length > 0);
            IDHolyPlace = xmlDoc.SelectSingleNode("Game/Links/HolyPlace").InnerText;
            Debug.Assert(IDHolyPlace.Length > 0);
            IDTradePlace = xmlDoc.SelectSingleNode("Game/Links/TradePlace").InnerText;
            Debug.Assert(IDTradePlace.Length > 0);

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

        internal Color ColorBorderPlayer(LobbyPlayer p)
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

        internal Color ColorIncome(bool existIncome)
        {
            return existIncome ? HintIncome : Color.Gray;
        }

        internal Color ColorGreatness(bool existGreatness)
        {
            return existGreatness ? HintIncome : Color.Gray;
        }

        internal void AddHumanPlayer(string name)
        {
            string id;
            bool exist;
            for (int i = 1; ; i++)
            {
                exist = false;
                id = $"HumanPlayer{i}";
                foreach (HumanPlayer hp in HumanPlayers)
                {
                    if (hp.ID == id)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                    break;
            }

            int imageIndex;
            for (imageIndex = 0; ; imageIndex++)
            {
                exist = false;
                foreach (HumanPlayer hp in HumanPlayers)
                {
                    if (hp.ImageIndex == imageIndex)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                    break;
            }

            HumanPlayers.Add(new HumanPlayer(id, name, "-", imageIndex));

            SaveHumanPlayers();
        }

        internal void SaveHumanPlayers()
        {
            XmlTextWriter textWriter = new XmlTextWriter(Program.formMain.dirResources + "Players.xml", Encoding.UTF8);
            textWriter.WriteStartDocument();
            textWriter.Formatting = Formatting.Indented;

            textWriter.WriteStartElement("Players");

            // Записываем информацию об игроках
            foreach (HumanPlayer hp in HumanPlayers)
            {
                hp.SaveToXml(textWriter);
            }

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }

        internal void SaveExternalAvatars()
        {
            XmlTextWriter textWriter = new XmlTextWriter(Program.formMain.dirResources + "ExternalAvatars.xml", Encoding.UTF8);
            textWriter.WriteStartDocument();
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteStartElement("ExternalAvatars");

            foreach (string ea in ExternalAvatars)
            {
                textWriter.WriteElementString("ExternalAvatar", ea); 
            }

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }

        internal bool CheckNonExistsNamePlayer(string name)
        {
            foreach (ComputerPlayer cp in ComputerPlayers)
                if (cp.Name == name)
                    return false;

            foreach (HumanPlayer hp in HumanPlayers)
                if (hp.Name == name)
                    return false;

            return true;
        }
    }
}
