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
            GridSize = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/GridSize").InnerText);
            Debug.Assert(GridSize >= 2);
            Debug.Assert(GridSize <= 20);
            Debug.Assert(GridSize % 2 == 0);
            GridSizeHalf = GridSize / 2;

            MaxLengthObjectName = XmlUtils.GetIntegerNotNull(xmlDoc, "Game/Interface/MaxLengthObjectName");
            Debug.Assert(MaxLengthObjectName > 20);
            Debug.Assert(MaxLengthObjectName <= 63);

            ShiftForBorder = new Point(Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ShiftForBorderX").InnerText),
                Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ShiftForBorderY").InnerText));
            Debug.Assert(ShiftForBorder.X >= 0);
            Debug.Assert(ShiftForBorder.X <= 10);
            Debug.Assert(ShiftForBorder.Y >= 0);
            Debug.Assert(ShiftForBorder.Y <= 10);

            // Т.к. ImageIndex у аватаров указан со смещением 1 и ImageIndexFirstAvatar указывается со смещением в 1, то уменьшаем итог на 1
            ImageIndexFirstAvatar = XmlUtils.GetIntegerNotNull(xmlDoc, "Game/Interface/ImageIndexFirstAvatar") - 1;
            QuantityInternalAvatars = XmlUtils.GetIntegerNotNull(xmlDoc, "Game/Interface/QuantityInternalAvatars");
            ImageIndexExternalAvatar = ImageIndexFirstAvatar + QuantityInternalAvatars;
            MaxQuantityExternalAvatars = XmlUtils.GetIntegerNotNull(xmlDoc, "Game/Interface/MaxQuantityExternalAvatars");
            Debug.Assert(ImageIndexFirstAvatar > 0);
            Debug.Assert(ImageIndexFirstAvatar < 240);
            Debug.Assert(QuantityInternalAvatars > 1);
            Debug.Assert(QuantityInternalAvatars < 64);
            Debug.Assert(MaxQuantityExternalAvatars > 1);
            Debug.Assert(MaxQuantityExternalAvatars < 64);
            ImageIndexFirstItems = ImageIndexExternalAvatar + MaxQuantityExternalAvatars;

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

            MaxElementInStartBonus = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/MaxElementInStartBonus").InnerText);
            Debug.Assert(ConstructionMaxLines >= 1);
            Debug.Assert(ConstructionMaxLines <= 10);

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

            MaxDurationBattleWithMonster = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battle/MaxDurationBattleWithMonster").InnerText);
            Debug.Assert(MaxDurationBattleWithMonster >= 30);
            Debug.Assert(MaxDurationBattleWithMonster <= 3600);

            MaxDurationBattleWithPlayer = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battle/MaxDurationBattleWithMonster").InnerText);
            Debug.Assert(MaxDurationBattleWithPlayer >= 30);
            Debug.Assert(MaxDurationBattleWithPlayer <= 3600);

            MaxFramesPerSecond = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Battle/MaxFramesPerSecond").InnerText);
            Debug.Assert(MaxFramesPerSecond >= 5);
            Debug.Assert(MaxFramesPerSecond <= 100);

            MaxDurationFrame = 1_000 / MaxFramesPerSecond;

            MaxStatPointPerLevel = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxStatPointPerLevel").InnerText);
            Debug.Assert(MaxStatPointPerLevel >= 5);
            Debug.Assert(MaxStatPointPerLevel <= 100);
            double timeInTumbstone = XmlUtils.GetDouble(xmlDoc, "Game/Heroes/TimeInTumbstone");
            Debug.Assert(timeInTumbstone >= 0);
            Debug.Assert(timeInTumbstone <= 10);
            StepsHeroInTumbstone = (int)(timeInTumbstone * StepsInSecond);
            Debug.Assert(StepsHeroInTumbstone >= 10);
            Debug.Assert(StepsHeroInTumbstone <= 1000);
            double timeToDisappearance = XmlUtils.GetDouble(xmlDoc, "Game/Heroes/TimeToDisappearance");
            Debug.Assert(timeToDisappearance >= 0);
            Debug.Assert(timeToDisappearance <= 10);
            UnitStepsTimeToDisappearance = (int)(timeToDisappearance * StepsInSecond);
            Debug.Assert(StepsHeroInTumbstone >= UnitStepsTimeToDisappearance);
            MaxCreatureAbilities = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxAbilities").InnerText);
            Debug.Assert(MaxCreatureAbilities >= 4);
            Debug.Assert(MaxCreatureAbilities <= 16);
            MaxCreatureSecondarySkills = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxSecondarySkills").InnerText);
            Debug.Assert(MaxCreatureSecondarySkills >= 4);
            Debug.Assert(MaxCreatureSecondarySkills <= 16);

            IDHeroAdvisor = xmlDoc.SelectSingleNode("Game/Links/HeroAdvisor").InnerText;
            Debug.Assert(IDHeroAdvisor.Length > 0);
            IDHeroPeasant = xmlDoc.SelectSingleNode("Game/Links/HeroPeasant").InnerText;
            Debug.Assert(IDHeroPeasant.Length > 0);
            IDConstructionCastle = xmlDoc.SelectSingleNode("Game/Links/ConstructionCastle").InnerText;
            Debug.Assert(IDConstructionCastle.Length > 0);
            IDPeasantHouse = xmlDoc.SelectSingleNode("Game/Links/PeasantHouse").InnerText;
            Debug.Assert(IDPeasantHouse.Length > 0);
            IDHolyPlace = xmlDoc.SelectSingleNode("Game/Links/HolyPlace").InnerText;
            Debug.Assert(IDHolyPlace.Length > 0);
            IDTradePost = xmlDoc.SelectSingleNode("Game/Links/TradePost").InnerText;
            Debug.Assert(IDTradePost.Length > 0);
            IDCityGraveyard = xmlDoc.SelectSingleNode("Game/Links/CityGraveyard").InnerText;
            Debug.Assert(IDCityGraveyard.Length > 0);
            NameResourceGold = xmlDoc.SelectSingleNode("Game/Links/Gold").InnerText;

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

            XmlNode xmlGui = xmlDoc.SelectSingleNode("Game/Gui48");
            Gui48_Heroes = GetGui48ImageIndex("Heroes");
            Gui48_Guilds = GetGui48ImageIndex("Guilds");
            Gui48_Economy = GetGui48ImageIndex("Economy");
            Gui48_Defense = GetGui48ImageIndex("Defense");
            Gui48_Temple = GetGui48ImageIndex("Temple");
            Gui48_LevelUp = GetGui48ImageIndex("LevelUp");
            Gui48_Buy = GetGui48ImageIndex("Buy");
            Gui48_Lobby = GetGui48ImageIndex("Lobby");
            Gui48_Dismiss = GetGui48ImageIndex("Dismiss");
            Gui48_Battle = GetGui48ImageIndex("Battle");
            Gui48_Peasant = GetGui48ImageIndex("Peasant");
            Gui48_Hourglass = GetGui48ImageIndex("Hourglass");
            Gui48_Goods = GetGui48ImageIndex("Goods");
            Gui48_Home = GetGui48ImageIndex("Home");
            Gui48_Inventory = GetGui48ImageIndex("Inventory");
            Gui48_Target = GetGui48ImageIndex("Target");
            Gui48_Book = GetGui48ImageIndex("Book");
            Gui48_Exit = GetGui48ImageIndex("Exit");
            Gui48_FlagAttack = GetGui48ImageIndex("FlagAttack");
            Gui48_Tournament = GetGui48ImageIndex("Tournament");
            Gui48_Scroll = GetGui48ImageIndex("Scroll");
            Gui48_Settings = GetGui48ImageIndex("Settings");
            Gui48_FlagScout = GetGui48ImageIndex("FlagScout");
            Gui48_FlagCancel = GetGui48ImageIndex("FlagCancel");
            Gui48_Build = GetGui48ImageIndex("Build");
            Gui48_FlagDefense = GetGui48ImageIndex("FlagDefense");
            Gui48_Map = GetGui48ImageIndex("Map");
            Gui48_Background = GetGui48ImageIndex("Background");
            Gui48_ResultDay = GetGui48ImageIndex("ResultDay");
            Gui48_Win = GetGui48ImageIndex("Win");
            Gui48_Draw = GetGui48ImageIndex("Draw");
            Gui48_Defeat = GetGui48ImageIndex("Defeat");
            Gui48_Battle2 = GetGui48ImageIndex("Battle2");
            Gui48_NeighborCastle = GetGui48ImageIndex("NeighborCastle");
            Gui48_Cheating = GetGui48ImageIndex("Cheating");

            int GetGui48ImageIndex(string name)
            {
                return XmlUtils.GetIntegerNotNull(xmlGui, name) + ImageIndexFirstItems - 1;
            }

            // Загружаем внешние аватары
            if (File.Exists(pathResources + @"\ExternalAvatars.xml"))
                xmlDoc = CreateXmlDocument(@"\ExternalAvatars.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/ExternalAvatars/ExternalAvatar"))
            {
                ExternalAvatars.Add(n.InnerText);
                if (ExternalAvatars.Count == MaxQuantityExternalAvatars)
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

            // Вспомогательные методы
            XmlDocument CreateXmlDocument(string pathToXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathResources + pathToXml);
                return doc;
            }
        }

        internal string PathResources { get; }
        internal List<DescriptorTypeLandscape> TypeLandscapes { get; } = new List<DescriptorTypeLandscape>();
        internal List<TypeLobby> TypeLobbies { get; } = new List<TypeLobby>();
        internal List<StartBonus> StartBonuses { get; } = new List<StartBonus>();
        internal List<ComputerPlayer> ComputerPlayers { get; } = new List<ComputerPlayer>();
        internal List<HumanPlayer> HumanPlayers { get; } = new List<HumanPlayer>();
        internal bool AutoCreatedPlayer { get; }

        internal int MaxLevelSkill { get; }

        //
        internal List<string> ExternalAvatars { get; } = new List<string>();

        //
        private List<(string, Bitmap)> Textures = new List<(string, Bitmap)>();

        // Константы
        internal int GridSize { get; private set; }// Размер ячейки сетки
        internal int GridSizeHalf { get; private set; }// Размер половины ячейки сетки
        internal int MaxLengthObjectName { get; set; }// Максимальная длина имени объекта
        internal Point ShiftForBorder { get; private set; }// Смещение иконки внутри рамки сущности
        internal int ImageIndexFirstAvatar { get; private set; }// ImageIndex первого аватара игроков
        internal int QuantityInternalAvatars { get; private set; }// Количество внутренних аватаров игроков
        internal int ImageIndexExternalAvatar { get; private set; }// ImageIndex первого аватара игроков
        internal int MaxQuantityExternalAvatars { get; private set; }// Количество внешних аватаров игроков
        internal int ImageIndexLastAvatar { get; private set; }// ImageIndex последнего аватара (включая внешний)
        internal int QuantityAllAvatars { get; private set; }// Общее количество аватаров
        internal int ImageIndexFirstItems { get; private set; }// ImageIndex первого аватара игроков
        internal int BorderInBigIcons => 1;// Прозрачный бордюр у иконок 128 * 128
        internal int HeroInRow { get; private set; }// Героев в ряду
        internal int HeroRows { get; private set; }// Рядов героев
        internal int RowsBetweenSides { get; private set; }// Рядов между сторонами
        internal int StepsInSecond { get; private set; }// Шагов в секунду
        internal int StepInMSec {get; private set; }// Время шага в миллисекундах
        internal int MaxDurationBattleWithMonster { get; private set; }// Максимальная длительность боя c монстрами в секундах
        internal int MaxDurationBattleWithPlayer { get; private set; }// Максимальная длительность боя с другим игроком в секундах
        internal int MaxFramesPerSecond { get; private set; }// Максимальная частота перерисовки кадров
        internal int MaxDurationFrame { get; private set; }// Максимальная длительность кадра
        internal int MaxStatPointPerLevel { get; private set; }
        internal int StepsHeroInTumbstone { get; private set; }// Сколько шагов герой в могиле перед исчезновением
        internal int UnitStepsTimeToDisappearance { get; private set; }// Сколько шагов могила юнита исчезает
        internal int MaxCreatureAbilities { get; private set; }// Максимальное количество умений у существа
        internal int MaxCreatureSecondarySkills { get; private set; }//Максимальное количество вторичных навыков у существа
        internal int PlateWidth { get; private set; }// Количество ячеек на панели справа по горизонтали
        internal int PlateHeight { get; private set; }// Количество ячеек на панели справа по вертикали
        internal int MinRowsEntities { get; private set; }// Минимальное количество строк сущностей в панели справа
        internal string IDHeroAdvisor { get; private set; }// ID типа героя - Советник
        internal string IDHeroPeasant { get; private set; }// ID типа героя - крестьянин
        internal string IDConstructionCastle { get; private set; }// ID Замка
        internal string IDPeasantHouse { get; private set; }// ID крестьянского дома
        internal string IDHolyPlace { get; private set; }// ID Святой земли
        internal string IDTradePost { get; private set; }// ID торгового поста
        internal string IDCityGraveyard { get; private set; }// ID торгового поста
        internal string NameResourceGold { get; private set; }// ID ресурса - золото
        internal int WarehouseWidth { get; private set; }// Количество ячеек в ряду склада
        internal int WarehouseHeight { get; private set; }// Количество рядов ячеек склада
        internal int WarehouseMaxCells { get; private set; }// Количество ячеек в складе
        internal int ConstructionMaxLines { get; private set; }// Максимальное количество линий сооружений
        internal int ConstructionMaxPos { get; private set; }// Максимальное количество позиций в линии сооружений
        internal int MaxElementInStartBonus { get; private set; }// Максимальное количество позиций в одном варианте стартового бонуса

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

        // Иконки GUI-48
        internal int Gui48_Heroes { get; }
        internal int Gui48_Guilds { get; }
        internal int Gui48_Economy { get; }
        internal int Gui48_Defense { get; }
        internal int Gui48_Temple { get; }
        internal int Gui48_LevelUp { get; }
        internal int Gui48_Buy { get; }
        internal int Gui48_Lobby { get; }
        internal int Gui48_Dismiss { get; }
        internal int Gui48_Battle { get; }
        internal int Gui48_Peasant { get; }
        internal int Gui48_Hourglass { get; }
        internal int Gui48_Goods { get; }
        internal int Gui48_Home { get; }
        internal int Gui48_Inventory { get; }
        internal int Gui48_Target { get; }
        internal int Gui48_Book { get; }
        internal int Gui48_Exit { get; }
        internal int Gui48_FlagAttack { get; }
        internal int Gui48_Tournament { get; }
        internal int Gui48_Scroll { get; }
        internal int Gui48_Settings { get; }
        internal int Gui48_FlagScout { get; }
        internal int Gui48_FlagCancel { get; }
        internal int Gui48_Build { get; }
        internal int Gui48_FlagDefense { get; }
        internal int Gui48_Map { get; }
        internal int Gui48_Background { get; }
        internal int Gui48_ResultDay { get; }
        internal int Gui48_Win { get; }
        internal int Gui48_Draw { get; }
        internal int Gui48_Defeat { get; }
        internal int Gui48_Battle2 { get; }
        internal int Gui48_NeighborCastle { get; }
        internal int Gui48_Cheating { get; }

        //
        internal Brush brushControl { get; private set; } = new SolidBrush(Color.White);
        internal Pen PenBorder { get; private set; }
        internal Pen PenSelectedBorder { get; private set; }

        //
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

        internal Color ColorIncome(bool existIncome)
        {
            return existIncome ? HintIncome : Color.Gray;
        }

        internal Color ColorGreatness(bool existGreatness)
        {
            return existGreatness ? HintIncome : Color.Gray;
        }

        internal Color ColorCost(bool allowed, bool goldEnough)
        {
            if (allowed && goldEnough)
                return Color.LimeGreen;
            if (allowed && !goldEnough)
                return Color.Red;

            return Color.Gray;
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

            HumanPlayers.Add(new HumanPlayer(id, name, "-", imageIndex + ImageIndexFirstAvatar));

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

        internal void UpdateDataAboutAvatar()
        {
            QuantityAllAvatars = QuantityInternalAvatars + ExternalAvatars.Count;
            ImageIndexLastAvatar = ImageIndexFirstAvatar + QuantityAllAvatars - 1;
        }

        private void LoadTextures(string pathResources)
        {
            string[] files = Directory.GetFiles(pathResources + @"Icons\Textures");
            foreach (string filename in files)
            {
                Bitmap bmp = Program.formMain.LoadBitmap(Path.GetFileName(filename), @"Icons\Textures");
                Textures.Add((Path.GetFileNameWithoutExtension(filename), bmp));
            }
        }

        internal Bitmap GetTexture(string id)
        {
            foreach ((string, Bitmap) t in Textures)
            {
                if (t.Item1 == id)
                    return t.Item2;
            }

            throw new Exception($"Текстура {id} не найдена.");
        }
    }
}
