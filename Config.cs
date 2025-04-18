﻿using System;
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
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Config
    {
        public Config(FormMain fm, bool defaultGridSize)
        {
            FormMain.Config = this;
            Descriptor.Config = this;
            LayerCustom.Config = this;

            // 
            MaxLevelSkill = 3;

            //
            XmlDocument xmlDoc;

            // Загружаем конфигурацию игры
            xmlDoc = CreateXmlDocument("Config\\Game.xml");
            if (defaultGridSize)
                GridSize = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/GridSize").InnerText);
            else
                GridSize = 2;
            Debug.Assert(GridSize >= 2);
            Debug.Assert(GridSize <= 20);
            Debug.Assert(GridSize % 2 == 0);
            GridSizeHalf = GridSize / 2;

            MaxLengthIDEntity = XmlUtils.GetIntegerNotNull(xmlDoc, "Game/Interface/MaxLengthIDEntity");
            Debug.Assert(MaxLengthIDEntity > 20);
            Debug.Assert(MaxLengthIDEntity <= 63);

            MaxLengthNameEntity = XmlUtils.GetIntegerNotNull(xmlDoc, "Game/Interface/MaxLengthNameEntity");
            Debug.Assert(MaxLengthNameEntity > 20);
            Debug.Assert(MaxLengthNameEntity <= 63);

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
            MaxValueProperty = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/MaxValueProperty").InnerText);
            Debug.Assert(MaxValueProperty >= 1_000);
            Debug.Assert(MaxValueProperty <= 10_000);

            ConstructionMaxLines = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ConstructionMaxLines").InnerText);
            Debug.Assert(ConstructionMaxLines >= 2);
            Debug.Assert(ConstructionMaxLines <= 5);

            ConstructionMaxPos = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ConstructionMaxPos").InnerText);
            Debug.Assert(ConstructionMaxLines >= 2);
            Debug.Assert(ConstructionMaxLines <= 5);

            MaxElementInStartBonus = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/MaxElementInStartBonus").InnerText);
            Debug.Assert(MaxElementInStartBonus >= 1);
            Debug.Assert(MaxElementInStartBonus <= 5);
            MaxHeroForSelectBonus = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/MaxHeroForSelectBonus").InnerText);
            Debug.Assert(MaxHeroForSelectBonus >= 1);
            Debug.Assert(MaxHeroForSelectBonus <= 5);

            MaxLengthQueue = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/GlobalSettings/Construction/MaxLengthQueue").InnerText);
            Debug.Assert(MaxLengthQueue >= 1);
            Debug.Assert(MaxLengthQueue <= 10);

            MouseHoverTime = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/MouseHoverTime").InnerText);
            Debug.Assert(MouseHoverTime >= 0);
            Debug.Assert(MouseHoverTime <= 10_000);
            if (MouseHoverTime == 0)
                MouseHoverTime = SystemInformation.MouseHoverTime;

            PanelHintWidth = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/PanelHintWidth").InnerText);
            Debug.Assert(PanelHintWidth >= 200);
            Debug.Assert(PanelHintWidth <= 1000);

            FlashCursorTime = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/FlashCursorTime").InnerText);
            Debug.Assert(FlashCursorTime > 100);
            Debug.Assert(FlashCursorTime <= 5_000);

            TraditionsPerColumn = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/TraditionsPerColumn").InnerText);
            Debug.Assert(TraditionsPerColumn > 1);
            Debug.Assert(TraditionsPerColumn <= 20);

            ObjectMenuWidth = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ObjectMenuWidth").InnerText);
            Debug.Assert(ObjectMenuWidth >= 50);
            Debug.Assert(ObjectMenuWidth <= 500);

            ShiftXButtonsInMenu = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/ShiftXButtonsInMenu").InnerText);
            Debug.Assert(ShiftXButtonsInMenu >= 0);
            Debug.Assert(ShiftXButtonsInMenu <= 200);            

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

            FramesPerSecond = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Interface/FramesPerSecond").InnerText);
            Debug.Assert(FramesPerSecond >= 5);
            Debug.Assert(FramesPerSecond <= 100);

            DurationFrame = 1_000 / FramesPerSecond;

            SecondsInTurn = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/GlobalSettings/SecondsInTurn").InnerText);
            Debug.Assert(SecondsInTurn > 0);
            Debug.Assert(SecondsInTurn < 600);

            TicksInSecond = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/GlobalSettings/TicksInSecond").InnerText);
            Debug.Assert(TicksInSecond >= 10);
            Debug.Assert(TicksInSecond <= 200);

            DurationTickInMilliSeconds = 1000 / TicksInSecond;
            Debug.Assert(DurationTickInMilliSeconds * TicksInSecond == 1000);

            TicksInTurn = TicksInSecond * SecondsInTurn;

            DaysInWeek = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/GlobalSettings/DaysInWeek").InnerText);
            Assert(DaysInWeek > 1);
            Assert(DaysInWeek < 10);
            WeeksInMonth = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/GlobalSettings/WeeksInMonth").InnerText);
            Assert(WeeksInMonth > 1);
            Assert(WeeksInMonth < 10);

            MaxTraditions = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/MaxTraditions").InnerText);
            Assert(MaxTraditions >= 3);
            Assert(MaxTraditions <= 64);
            MaxLevelTradition = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/MaxLevelTradition").InnerText);
            Assert(MaxLevelTradition > 1);
            Assert(MaxLevelTradition <= 50);
            CostFirstTradition = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/CostFirstTradition").InnerText);
            Assert(CostFirstTradition >= 1);
            Assert(CostFirstTradition <= 1000);
            CoefForNextTradition = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/CoefForNextTradition").InnerText);
            Assert(CoefForNextTradition > 1);
            Assert(CoefForNextTradition < 1000);
            CoefForNextTradition = CoefForNextTradition / 100;

            PercentCostFirstTypeTradition = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/PercentCostFirstTypeTradition").InnerText);
            Assert(PercentCostFirstTypeTradition > 0);
            Assert(PercentCostFirstTypeTradition < 100);
            PercentCostSecondTypeTradition = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/PercentCostSecondTypeTradition").InnerText);
            Assert(PercentCostSecondTypeTradition > 0);
            Assert(PercentCostSecondTypeTradition < 100);
            PercentCostThirdTypeTradition = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Traditions/PercentCostThirdTypeTradition").InnerText);
            Assert(PercentCostThirdTypeTradition > 0);
            Assert(PercentCostThirdTypeTradition < 100);

            Assert(PercentCostFirstTypeTradition < PercentCostSecondTypeTradition);
            Assert(PercentCostSecondTypeTradition < PercentCostThirdTypeTradition);

            // MainMenu
            MainMenuMinAlphaBanner = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/MainMenu/MinAlphaBanner").InnerText);
            MainMenuFramesAnimationBanner = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/MainMenu/FramesAnimationBanner").InnerText);

            //
            MaxStatPointPerLevel = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxStatPointPerLevel").InnerText);
            Debug.Assert(MaxStatPointPerLevel >= 5, $"MaxStatPointPerLevel: {MaxStatPointPerLevel}");
            Debug.Assert(MaxStatPointPerLevel <= 100, $"MaxStatPointPerLevel: {MaxStatPointPerLevel}");
            double timeInTumbstone = XmlUtils.GetDouble(xmlDoc, "Game/Heroes/TimeInTumbstone");
            Debug.Assert(timeInTumbstone >= 0, $"timeInTumbstone: {timeInTumbstone}");
            Debug.Assert(timeInTumbstone <= 10, $"timeInTumbstone: {timeInTumbstone}");
            StepsHeroInTumbstone = (int)(timeInTumbstone * StepsInSecond);
            Debug.Assert(StepsHeroInTumbstone >= 10, $"StepsHeroInTumbstone: {StepsHeroInTumbstone}");
            Debug.Assert(StepsHeroInTumbstone <= 1000, $"StepsHeroInTumbstone: {StepsHeroInTumbstone}");
            double timeToDisappearance = XmlUtils.GetDouble(xmlDoc, "Game/Heroes/TimeToDisappearance");
            Debug.Assert(timeToDisappearance >= 0, $"timeToDisappearance: {timeToDisappearance}");
            Debug.Assert(timeToDisappearance <= 10, $"timeToDisappearance: {timeToDisappearance}");
            UnitStepsTimeToDisappearance = (int)(timeToDisappearance * StepsInSecond);
            Debug.Assert(StepsHeroInTumbstone >= UnitStepsTimeToDisappearance);
            MaxCreatureAbilities = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxAbilities").InnerText);
            Debug.Assert(MaxCreatureAbilities >= 4, $"MaxCreatureAbilities: {MaxCreatureAbilities}");
            Debug.Assert(MaxCreatureAbilities <= 16, $"MaxCreatureAbilities: {MaxCreatureAbilities}");
            MaxCreatureSecondarySkills = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Heroes/MaxSecondarySkills").InnerText);
            Debug.Assert(MaxCreatureSecondarySkills >= 4, $"MaxCreatureSecondarySkills: {MaxCreatureSecondarySkills}");
            Debug.Assert(MaxCreatureSecondarySkills <= 16, $"MaxCreatureSecondarySkills: {MaxCreatureSecondarySkills}");

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
            IDGuildOfBuilders = xmlDoc.SelectSingleNode("Game/Links/GuildOfBuilders").InnerText;
            Debug.Assert(IDGuildOfBuilders.Length > 0);
            IDReasonOfDeathInBattle = xmlDoc.SelectSingleNode("Game/Links/ReasonOfDeathInBattle").InnerText;
            Debug.Assert(IDReasonOfDeathInBattle.Length > 0);
            NameDefaultLevelTax = xmlDoc.SelectSingleNode("Game/Interface/DefaultLevelTax").InnerText;
            Debug.Assert(NameDefaultLevelTax.Length > 0);

            CityParameterCitizens = xmlDoc.SelectSingleNode("Game/Links/CityParameters/Citizens").InnerText;
            Debug.Assert(CityParameterCitizens.Length > 0);


            WarehouseWidth = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Warehouse/Width").InnerText);
            Debug.Assert(WarehouseWidth >= 5);
            Debug.Assert(WarehouseWidth <= 30);
            WarehouseHeight = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Warehouse/Height").InnerText);
            Debug.Assert(WarehouseHeight >= 1);
            Debug.Assert(WarehouseHeight <= 10);
            WarehouseMaxCells = WarehouseWidth * WarehouseHeight;

            //
            StateCreatureDoScoutFlag = "DoScoutFlag";

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


            NoticeSecondsBeforeHide = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Notice/SecondsBeforeHide").InnerText);
            Debug.Assert(NoticeSecondsBeforeHide >= 1);
            Debug.Assert(NoticeSecondsBeforeHide <= 300);
            NoticeSecondsHide = Convert.ToInt32(xmlDoc.SelectSingleNode("Game/Notice/SecondsHide").InnerText);
            Debug.Assert(NoticeSecondsHide >= 1);
            Debug.Assert(NoticeSecondsHide <= 300);


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
            Gui48_Mail = GetGui48ImageIndex("Mail");
            Gui48_Win = GetGui48ImageIndex("Win");
            Gui48_Draw = GetGui48ImageIndex("Draw");
            Gui48_Defeat = GetGui48ImageIndex("Defeat");
            Gui48_Battle2 = GetGui48ImageIndex("Battle2");
            Gui48_NeighborCastle = GetGui48ImageIndex("NeighborCastle");
            Gui48_Cheating = GetGui48ImageIndex("Cheating");
            Gui48_NoCheating = GetGui48ImageIndex("NoCheating");
            Gui48_Money = GetGui48ImageIndex("Money");
            Gui48_Finance = GetGui48ImageIndex("Finance");
            Gui48_RandomSelect = GetGui48ImageIndex("RandomSelect");
            Gui48_ManualSelect = GetGui48ImageIndex("ManualSelect");
            Gui48_ComputerPlayer = GetGui48ImageIndex("ComputerPlayer");
            Gui48_HumanPlayer = GetGui48ImageIndex("HumanPlayer");
            Gui48_Quest = GetGui48ImageIndex("Quest");
            Gui48_Tradition = GetGui48ImageIndex("Tradition");
            Gui48_CastSpell = GetGui48ImageIndex("CastSpell");
            Gui48_Mana = GetGui48ImageIndex("Mana");

            int GetGui48ImageIndex(string name)
            {
                return XmlUtils.GetIntegerNotNull(xmlGui, name) + ImageIndexFirstItems - 1;
            }

            // Загружаем внешние аватары
            if (File.Exists(Program.FolderResources + @"\ExternalAvatars.xml"))
                xmlDoc = CreateXmlDocument(@"\ExternalAvatars.xml");
            foreach (XmlNode n in xmlDoc.SelectNodes("/ExternalAvatars/ExternalAvatar"))
            {
                ExternalAvatars.Add(n.InnerText);
                if (ExternalAvatars.Count == MaxQuantityExternalAvatars)
                    break;
            }

            // Вспомогательные методы
            XmlDocument CreateXmlDocument(string pathToXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Program.FolderResources + pathToXml);
                return doc;
            }
        }

        internal int MaxLevelSkill { get; }

        //
        internal List<string> ExternalAvatars { get; } = new List<string>();

        // Константы
        internal const int INDEX_FIRST_RESOURCE = 1;

        internal int GridSize { get; private set; }// Размер ячейки сетки
        internal int GridSizeHalf { get; private set; }// Размер половины ячейки сетки
        internal int MaxLengthIDEntity { get; set; }// Максимальная длина ID сущности
        internal int MaxLengthNameEntity { get; set; }// Максимальная длина имени сущности
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
        internal int StepInMSec { get; private set; }// Время шага в миллисекундах
        internal int MaxDurationBattleWithMonster { get; private set; }// Максимальная длительность боя c монстрами в секундах
        internal int MaxDurationBattleWithPlayer { get; private set; }// Максимальная длительность боя с другим игроком в секундах
        internal int MaxStatPointPerLevel { get; private set; }
        internal int StepsHeroInTumbstone { get; private set; }// Сколько шагов герой в могиле перед исчезновением
        internal int UnitStepsTimeToDisappearance { get; private set; }// Сколько шагов могила юнита исчезает
        internal int MaxCreatureAbilities { get; private set; }// Максимальное количество умений у существа
        internal int MaxCreatureSecondarySkills { get; private set; }//Максимальное количество вторичных навыков у существа
        internal int PlateWidth { get; private set; }// Количество ячеек на панели справа по горизонтали
        internal int PlateHeight { get; private set; }// Количество ячеек на панели справа по вертикали
        internal int MinRowsEntities { get; private set; }// Минимальное количество строк сущностей в панели справа
        internal int MaxValueProperty { get; private set; }// 
        internal int MouseHoverTime { get; set; }// Через сколько миллисекунд после наведения курсора на контрол отображается подсказка
        internal int PanelHintWidth { get; set; }// Ширина панели с подсказкой
        internal int FlashCursorTime { get; set; }// Cколько миллисекунд отображается/не отображается текстовый курсор 
        internal int TraditionsPerColumn { get; set; }// Сколько традиций отображать в одной колонке на вкладке "Традиции"
        internal int ObjectMenuWidth { get; set; }
        internal int ShiftXButtonsInMenu { get; set; }

        internal int FramesPerSecond { get; set; }// Отрисовка количества кадров в секунду
        internal int DurationFrame { get; set; }// Длительность кадра в миллисекундах
        internal int SecondsInTurn { get; set; }// Сколько реальных секунд длятся одни игровые сутки
        internal int TicksInSecond { get; set; }// Количество тиков игры в реальной секунде
        internal int DurationTickInMilliSeconds { get; set; }// Длительность одного тика игры в миллисекундах
        internal int TicksInTurn{ get; set; }// Сколько тиков в игровых сутках
        internal int DaysInWeek { get; private set; }// Дней в неделе
        internal int WeeksInMonth { get; private set; }// Недель в месяце

        // Традиции
        internal int MaxTraditions { get; private set; }// Максимальное количество традиций у игрока
        internal int MaxLevelTradition { get; private set; }// Максимальный уровень традиций
        internal int CostFirstTradition { get; private set; }// Стоимость принятия первой традиции
        internal double CoefForNextTradition { get; private set; }// Коэффициент для стоимости второй и последующих традиций
        internal int PercentCostFirstTypeTradition { get; private set; }// Процент от полного количества очков для первого типа традиции
        internal int PercentCostSecondTypeTradition { get; private set; }// Процент от полного количества очков для первого типа традиции
        internal int PercentCostThirdTypeTradition { get; private set; }// Процент от полного количества очков для первого типа традиции

        internal int MainMenuMinAlphaBanner { get; set; }// Минимальная прозрачность баннера с названием игры
        internal int MainMenuFramesAnimationBanner { get; set; }// Количество кадров анимации баннера

        internal string IDHeroAdvisor { get; private set; }// ID типа героя - Советник
        internal string IDHeroPeasant { get; private set; }// ID типа героя - крестьянин
        internal string IDConstructionCastle { get; private set; }// ID Замка
        internal string IDPeasantHouse { get; private set; }// ID крестьянского дома
        internal string IDHolyPlace { get; private set; }// ID Святой земли
        internal string IDTradePost { get; private set; }// ID торгового поста
        internal string IDCityGraveyard { get; private set; }// ID торгового поста
        internal string IDGuildOfBuilders { get; private set; }// ID гильдии строителей
        internal string IDReasonOfDeathInBattle { get; private set; }// ID причины смерти - в бою
        internal string NameDefaultLevelTax { get; private set; }// Уровень налогов по умолчанию
        internal int WarehouseWidth { get; private set; }// Количество ячеек в ряду склада
        internal int WarehouseHeight { get; private set; }// Количество рядов ячеек склада
        internal int WarehouseMaxCells { get; private set; }// Количество ячеек в складе
        internal int ConstructionMaxLines { get; private set; }// Максимальное количество линий сооружений
        internal int ConstructionMaxPos { get; private set; }// Максимальное количество позиций в линии сооружений
        internal int MaxElementInStartBonus { get; private set; }// Максимальное количество позиций в одном варианте стартового бонуса
        internal int MaxHeroForSelectBonus { get; private set; }// Максимальное количество типов героев для выбора постоянного бонуса
        internal int MaxLengthQueue { get; private set; }// Максимальная длина очереди

        internal string CityParameterCitizens { get; private set; }// Параметр города - количество горожан

        // Состояния существ
        internal string StateCreatureDoScoutFlag { get; private set; }// Выполняет флаг разведеи

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

        internal int NoticeSecondsBeforeHide { get; private set; }
        internal int NoticeSecondsHide { get; private set; }


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
        internal int Gui48_Mail { get; }
        internal int Gui48_Win { get; }
        internal int Gui48_Draw { get; }
        internal int Gui48_Defeat { get; }
        internal int Gui48_Battle2 { get; }
        internal int Gui48_NeighborCastle { get; }
        internal int Gui48_Cheating { get; }
        internal int Gui48_NoCheating { get; }
        internal int Gui48_Money { get; }
        internal int Gui48_Finance { get; }
        internal int Gui48_RandomSelect { get; }
        internal int Gui48_ManualSelect { get; }
        internal int Gui48_ComputerPlayer { get; }
        internal int Gui48_HumanPlayer { get; }
        internal int Gui48_Quest { get; }
        internal int Gui48_Tradition { get; }
        internal int Gui48_CastSpell { get; }
        internal int Gui48_Mana { get; }

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

        internal void UpdateDataAboutAvatar()
        {
            QuantityAllAvatars = QuantityInternalAvatars + ExternalAvatars.Count;
            ImageIndexLastAvatar = ImageIndexFirstAvatar + QuantityAllAvatars - 1;
        }

        internal void SaveExternalAvatars()
        {
            XmlTextWriter textWriter = new XmlTextWriter(Program.FolderResources + "ExternalAvatars.xml", Encoding.UTF8);
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
    }
}
