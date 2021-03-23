using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using System.Media;

namespace Fantasy_Kingdoms_Battle
{
    public partial class FormMain : Form
    {
        private const string NAME_PROJECT = "The Fantasy Kingdoms Battle";
        internal const string VERSION = "0.3.3";
        internal const string DATE_VERSION = "16.03.2021";
        private const string VERSION_POSTFIX = "в разработке";
        internal readonly string dirCurrent;
        internal readonly string dirResources;

        internal bool gameStarted = false;
        internal bool inQuit = false;
        private bool needRepaintFrame = false;

        // Проигрывание звуков и музыки 
        private readonly SoundPlayer spSoundSelect = new SoundPlayer();
        private readonly System.Windows.Media.MediaPlayer mpSelectButton;
        private readonly System.Windows.Media.MediaPlayer mpPushButton;
        private readonly System.Windows.Media.MediaPlayer mpConstructionComplete;

        // ImageList'ы
        internal BitmapList blPlayerAvatars;
        internal readonly BitmapList imListObjectsBig;
        internal readonly BitmapList imListObjectsCell;
        internal readonly BitmapList ilGui;
        internal readonly BitmapList ilGui16;
        internal readonly BitmapList ilGui24;
        internal readonly BitmapList ilParameters;
        internal readonly BitmapList ilItems;
        internal readonly BitmapList ilStateHero;
        internal readonly BitmapList ilMenuCellFilters;

        internal Brush brushQuantity;
        internal Brush brushCost;

        // Поддержка режима отладки
        private bool debugMode = false;
        private Pen penDebugBorder = new Pen(Color.Red);
        private VisualControl vcDebugInfo;
        private VCLabel labelTimeDrawFrame;
        private VCLabel labelLayers;
        private DateTime startDebugAction;
        private TimeSpan durationDrawFrame;
        private int playsSelectButton;

        // Контролы главного меню
        private Bitmap bmpFrame;// Готовый кадр
        private Graphics gfxFrame;// Graphics кадра, чтобы контролы работали сразу с ним
        internal Bitmap bmpBackground;// Фон кадра

        private readonly VisualControl TopControl;
        private readonly VisualControl MainControl;

        private Point mousePos;
        private VisualControl controlWithHint;
        private VisualControl controlClicked;
        private bool hintShowed = false;

        private readonly VisualControl panelPlayers;// Панель, на которой находятся панели игроков лобби

        private readonly VCToolLabel labelDay;
        private readonly VCToolLabel labelGreatness;
        private readonly VCToolLabel labelGold;

        private readonly VCIconButton btnPreferences;
        private readonly VCIconButton btnHelp;
        private readonly VCIconButton btnQuit;

        private readonly VCIconButton btnEndTurn;

        private readonly VisualControl panelLairWithFlags;
        private readonly List<VCButtonTargetLair> listBtnTargetLair = new List<VCButtonTargetLair>();

        private readonly VCBitmap bitmapMenu;

        private PlayerObject selectedPlayerObject;

        // Главные страницы игры
        private readonly List<VCFormPage> pages = new List<VCFormPage>();
        private readonly VCFormPage pageGuilds;
        private readonly VCFormPage pageBuildings;
        private readonly VCFormPage pageTemples;
        private readonly VCFormPage pageHeroes;
        private readonly VCFormPage pageLairs;
        private readonly VCFormPage pageTournament;
        private readonly VCLabelM2 labelCaptionPage;

        private PanelWithPanelEntity panelWarehouse;
        private PanelWithPanelEntity panelHeroes;
        private PanelWithPanelEntity panelCombatHeroes;

        internal const int GUI_HEROES = 0;
        internal const int GUI_GUILDS = 1;
        internal const int GUI_ECONOMY = 2;
        internal const int GUI_DEFENSE = 3;
        internal const int GUI_TEMPLE = 4;
        internal const int GUI_LEVELUP = 5;
        internal const int GUI_BUY = 6;
        internal const int GUI_LOBBY = 7;
        internal const int GUI_DISMISS = 8;
        internal const int GUI_BATTLE = 9;
        internal const int GUI_PEASANT = 10;
        internal const int GUI_HOURGLASS = 11;
        internal const int GUI_GOODS = 12;
        internal const int GUI_HOME = 13;
        internal const int GUI_INVENTORY = 14;
        internal const int GUI_TARGET = 15;
        internal const int GUI_BOOK = 16;
        internal const int GUI_EXIT = 17;
        internal const int GUI_FLAG_ATTACK = 18;
        internal const int GUI_TOURNAMENT = 19;
        internal const int GUI_SCROLL = 20;
        internal const int GUI_SETTINGS = 21;
        internal const int GUI_FLAG_SCOUT = 22;
        internal const int GUI_FLAG_CANCEL = 23;
        internal const int GUI_BUILD = 24;

        internal const int GUI_PARAMETER_STRENGTH = 6;
        internal const int GUI_PARAMETER_DEXTERITY = 7;
        internal const int GUI_PARAMETER_MAGIC = 8;
        internal const int GUI_PARAMETER_VITALITY = 9;
        internal const int GUI_PARAMETER_SPEED_ATTACK = 10;
        internal const int GUI_PARAMETER_ATTACK_MELEE = 0;
        internal const int GUI_PARAMETER_ATTACK_RANGE = 1;
        internal const int GUI_PARAMETER_ATTACK_MAGIC = 2;
        internal const int GUI_PARAMETER_DEFENSE_MELEE = 3;
        internal const int GUI_PARAMETER_DEFENSE_RANGE = 4;
        internal const int GUI_PARAMETER_DEFENSE_MAGIC = 5;

        internal const int II_STATE_HERO_DO_ATTACK_FLAG = 11;
        internal const int II_STATE_HERO_DO_SCOUT_FLAG = 12;
        internal const int II_STATE_HERO_IN_HOME = 13;
        internal const int II_STATE_HERO_NOTHING = 14;
        internal const int II_STATE_HERO_THERAPY = 15;
        internal const int II_STATE_HERO_KING = 16;
        internal const int II_STATE_HERO_ADVISOR = 17;
        internal const int II_STATE_HERO_CAPTAIN = 18;
        internal const int II_STATE_HERO_TREASURER = 19;

        internal const int GUI_16_DAY = 0;
        internal const int GUI_16_GOLD = 1;
        internal const int GUI_16_PEASANT = 2;
        internal const int GUI_16_GREATNESS = 3;
        internal const int GUI_16_INCOME = 4;

        internal const int GUI_24_FIRE = 0;
        internal const int GUI_24_HEROES = 1;
        internal const int GUI_24_STAR = 2;

        internal const int GUI_45_EMPTY = 0;
        internal const int GUI_45_BORDER = 0;

        internal static Size PANEL_MENU_CELLS = new Size(4, 3);
        private const int DISTANCE_BETWEEN_CELLS = 3;

        internal const int IMAGE_INDEX_NONE = 127;
        internal const int IMAGE_INDEX_CURRENT_AVATAR = -100;
        internal const int MAX_LAIR_LAYERS = 5;

        private Lobby lobby;
        private Player curAppliedPlayer;

        internal Lobby CurrentLobby { get { return lobby; } }

        internal readonly Bitmap bmpForBackground;
        internal readonly Bitmap bmpBorderForIcon;
        internal readonly Bitmap bmpBorderForIconAlly;
        internal readonly Bitmap bmpBorderForIconEnemy;
        internal readonly Bitmap bmpEmptyEntity;
        internal readonly Bitmap bmpBackgroundEntity;
        internal readonly BitmapBorder bbBorderWindow;
        internal readonly BitmapBorder bbObject;
        internal readonly BitmapBorder bbToolBarLabel;
        internal readonly BitmapBorder bbGamespace;
        internal readonly BitmapBorder bbSelect;
        internal readonly Bitmap bmpBorderBig;
        internal readonly Bitmap bmpMaskBig;
        internal readonly Bitmap bmpMaskSmall;
        internal readonly Bitmap bmpToolbar;
        internal readonly Bitmap bmpToolbarBorder;
        internal readonly Bitmap bmpSeparator;
        internal readonly Bitmap bmpBandWindowCaption;
        internal readonly Bitmap bmpBandButtonNormal;
        internal readonly Bitmap bmpBandButtonHot;
        internal readonly Bitmap bmpBandButtonDisabled;
        internal readonly Bitmap bmpBandButtonPressed;
        internal readonly Bitmap bmpBandStateCreature;
        private VCBitmap bmpPreparedToolbar;
        internal readonly M2Font fontSmall;
        internal readonly M2Font fontSmallC;
        internal readonly M2Font fontMedCaptionC;
        internal readonly M2Font fontMedCaption;
        internal readonly M2Font fontBigCaption;
        internal readonly M2Font fontSmallBC;
        internal readonly M2Font fontParagraph;

        internal Size sizeGamespace { get; set; }
        internal Point ShiftControls { get; set; }

        private bool inDrawFrame = false;
        private bool needRedrawFrame;

        private readonly List<VisualLayer> Layers;
        private readonly VisualLayer layerGame;
        private VisualLayer currentLayer;

        private VCFormPage currentPage;
        private readonly VisualControl panelEmptyInfo;
        internal PanelBuildingInfo panelBuildingInfo { get; private set; }
        internal PanelLairInfo panelLairInfo { get; private set; }
        internal PanelHeroInfo panelHeroInfo { get; private set; }
        internal PanelMonsterInfo panelMonsterInfo { get; private set; }

        internal VCMenuCell[,] CellsMenu { get; }

        internal PanelHint formHint;
        internal int ImageIndexFirstAvatar { get; }

        internal static Random Rnd = new Random();

        //
        internal Settings Settings { get; private set; }
        internal MainConfig MainConfig { get; private set; }
        internal int AvatarCount { get; private set; }

        private Timer timerHover;

        public FormMain()
        {
            InitializeComponent();

            Program.formMain = this;

            Text = NAME_PROJECT + " (сборка " + VERSION + ")";
            
            // Настройка переменной с папкой ресурсов
            dirCurrent = Environment.CurrentDirectory;

            if (dirCurrent.Contains("Debug"))
                dirCurrent = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
            else if (dirCurrent.Contains("Release"))
                dirCurrent = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
            else
                dirCurrent += "\\";

            // Ищем главную пользовательскую модификацию
            dirResources = Directory.Exists(dirCurrent + @"User_mods\Main") ? dirCurrent + @"User_mods\Main\" : dirCurrent + @"Resources\";

            // Обновляем обновлятор
            string newName;
            foreach (string file in System.IO.Directory.EnumerateFiles(dirCurrent))
            {
                if (Path.GetFileName(file).StartsWith("Updater.") && Path.GetFileName(file).EndsWith(".new"))
                    try
                    {
                        newName = file.Substring(0, file.Length - 4);
                        if (File.Exists(newName))
                            File.Delete(newName);
                        File.Move(file, newName);
                    }
                    catch (Exception exc)
                    {
                        GuiUtils.ShowError(exc.Message);
                        break;
                    }
            }

            // Загружаем настройки
            try
            {
                Settings = new Settings(dirResources);

                MainConfig = new MainConfig(dirResources);

                // Если включено автообновление, проверяем на их наличие
                if (Settings.CheckUpdateOnStartup)
                {
                    CheckForNewVersion();
                }

                fontSmall = new M2Font(dirResources, "small");
                fontSmallC = new M2Font(dirResources, "small_c");
                fontMedCaptionC = new M2Font(dirResources, "med_caption_c");
                fontMedCaption = new M2Font(dirResources, "med_caption");
                fontBigCaption = new M2Font(dirResources, "big_caption");
                fontSmallBC = new M2Font(dirResources, "_small_b_c");
                fontParagraph = new M2Font(dirResources, "paragraph");

                // Формируем и показываем сплэш-заставку
                Image splashBitmap = new Bitmap(dirResources + "\\Icons\\Splash.png");

                Form splashForm = new Form()
                {
                    StartPosition = FormStartPosition.CenterScreen,
                    ShowInTaskbar = false,
                    FormBorderStyle = FormBorderStyle.None,
                    ClientSize = splashBitmap.Size,
                    BackgroundImage = splashBitmap,
                    TopMost = true
                };

                Label lblCaption = new Label()
                {
                    Parent = splashForm,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Top = 8,
                    Left = 0,
                    Height = 32,
                    Width = splashForm.ClientSize.Width,
                    ForeColor = Color.SkyBlue,
                    BackColor = Color.Transparent,
                    Font = new Font("Times New Roman", 16, FontStyle.Bold),
                    Text = "The Fantasy Kingdoms Battle"
                };

                Label lblStage = new Label()
                {
                    Parent = splashForm,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Top = splashForm.ClientSize.Height - 32,
                    Left = 0,
                    Width = splashForm.ClientSize.Width,
                    ForeColor = Color.LightBlue,
                    BackColor = Color.Transparent,
                    Font = new Font("Times New Roman", 13)
                };

                splashForm.Show();
                splashForm.Refresh();

                // Загружаем конфигурацию
                SetStage("Открываем сундуки");
                _ = new Config(dirResources, this);

                brushQuantity = new SolidBrush(Config.CommonQuantity);
                brushCost = new SolidBrush(Config.CommonCost);

                // Загружаем иконки
                SetStage("Рассматриваем картины");

                bmpBorderBig = new Bitmap(dirResources + @"Icons\BorderBig.png");
                bmpMaskBig = new Bitmap(dirResources + @"Icons\MaskBig.png");
                bmpMaskSmall = new Bitmap(dirResources + @"Icons\MaskSmall.png");// Нужна ли еще?

                imListObjectsBig = new BitmapList(dirResources, "Objects.png", 128, true, true);

                // Добавляем в список иконок аватарки игроков
                // Для этого создаем отдельный список оригинальных аватарок, из которого уже будем составлять итоговый
                ImageIndexFirstAvatar = imListObjectsBig.Count;
                blPlayerAvatars = new BitmapList(dirResources, "Avatars.png", 128, true, true);
                for (int i = 0; i < blPlayerAvatars.Count; i++)
                    imListObjectsBig.Add(blPlayerAvatars.GetImage(i, true, false));

                ValidateAvatars();

                imListObjectsCell = new BitmapList(imListObjectsBig, 48, Config.BorderInBigIcons, bmpMaskSmall);

                ilGui16 = new BitmapList(dirResources, "Gui16.png", 16, true, false);
                ilGui24 = new BitmapList(dirResources, "Gui24.png", 24, true, false);
                ilParameters = new BitmapList(dirResources, "Parameters.png", 24, true, false);
                ilItems = new BitmapList(dirResources, "Items.png", 48, true, true);
                ilStateHero = new BitmapList(dirResources, "StateCreature.png", 24, true, false);
                ilMenuCellFilters = new BitmapList(dirResources, "MenuCellFilters.png", 48, true, false);

                ilGui = new BitmapList(dirResources, "Gui.png", 48, true, true);
                //MakeAlpha();

                bmpForBackground = new Bitmap(dirResources + "Icons\\Background.png");
                bmpBorderForIcon = new Bitmap(dirResources + "Icons\\BorderIconEntity.png");
                bmpEmptyEntity = new Bitmap(dirResources + "Icons\\EmptyEntity.png");
                bmpBackgroundEntity = new Bitmap(dirResources + "Icons\\BackgroundEntity.png");
                bbBorderWindow = new BitmapBorder(dirResources + @"Icons\BorderWindow.png", false, 14, 14, 14, 14, 60, 14, 14, 60, 14, 14);
                bbObject = new BitmapBorder(dirResources + "Icons\\BorderObject.png", false, 10, 10, 9, 12, 25, 2, 5, 24, 3, 3);
                bbToolBarLabel = new BitmapBorder(dirResources + @"Icons\ToolbarLabel.png", true, 10, 10, 9, 10, 25, 9, 12, 25, 10, 10);
                bbGamespace = new BitmapBorder(dirResources + @"Icons\BorderMain2.png", false, 12, 12, 12, 12, 26, 7, 7, 26, 7, 7);
                bbSelect = new BitmapBorder(dirResources + @"Icons\BorderSelect.png", false, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10);
                bmpToolbar = new Bitmap(dirResources + @"Icons\Toolbar.png");
                bmpToolbarBorder = new Bitmap(dirResources + @"Icons\ToolbarBorder.png");
                bmpSeparator = new Bitmap(dirResources + @"Icons\Separator.png");
                bmpBandWindowCaption = new Bitmap(dirResources + @"Icons\WindowCaption.png");
                bmpBandButtonNormal = new Bitmap(dirResources + @"Icons\ButtonNormal.png");
                bmpBandButtonHot = new Bitmap(dirResources + @"Icons\ButtonHot.png");
                bmpBandButtonDisabled = new Bitmap(dirResources + @"Icons\ButtonDisabled.png");
                bmpBandButtonPressed = new Bitmap(dirResources + @"Icons\ButtonPressed.png");
                bmpBandStateCreature = new Bitmap(dirResources + @"Icons\BandStateCreature.png");

                // Делаем рамки для союзников и врагов
                bmpBorderForIconAlly = new Bitmap(bmpBorderForIcon);
                bmpBorderForIconEnemy = new Bitmap(bmpBorderForIcon);
                for (int y = 0; y < bmpBorderForIcon.Height; y++)
                    for (int x = 0; x < bmpBorderForIcon.Width; x++)
                    {
                        // получаем (i, j) пиксель
                        uint pixel = (uint)(bmpBorderForIcon.GetPixel(x, y).ToArgb());

                        // получаем компоненты цветов пикселя
                        float R = (pixel & 0x00FF0000) >> 16; // красный
                        float G = (pixel & 0x0000FF00) >> 8; // зеленый
                        float B = pixel & 0x000000FF; // синий
                                                      // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;

                        // собираем новый пиксель по частям (по каналам)
                        float G2 = G + 64.0f;
                        if (G2 > 255)
                            G2 = 255;
                        uint newPixel = ((uint)bmpBorderForIcon.GetPixel(x, y).A << 24) | ((uint)G2 << 8);
                        //                    uint newPixel = ((uint)bmpBorderForIcon.GetPixel(x, y).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                        // добавляем его в Bitmap нового изображения
                        bmpBorderForIconAlly.SetPixel(x, y, Color.FromArgb((int)newPixel));

                        float R2 = R + 64.0f;
                        if (R2 > 255)
                            R2 = 255;
                        newPixel = ((uint)bmpBorderForIcon.GetPixel(x, y).A << 24) | ((uint)R2 << 16);
                        bmpBorderForIconEnemy.SetPixel(x, y, Color.FromArgb((int)newPixel));

                        //orgColor = bmpBorderForIcon.GetPixel(x, y);
                        //bmpBorderForIconAlly.SetPixel(x, y, Color.FromArgb(orgColor.A, orgColor.R, 192, orgColor.B));
                        //bmpBorderForIconEnemy.SetPixel(x, y, Color.FromArgb(orgColor.A, 192, orgColor.G, orgColor.B));
                    }

                // Создаем лобби
                // Переместить уже после создания всех контролов, чтобы обеспечить связь лобби-контролы
                lobby = new Lobby(Config.TypeLobbies[0]);

                SetStage("Строим замок");

                // Создаем слой игрового поля
                Layers = new List<VisualLayer>();
                layerGame = new VisualLayer();
                Layers.Add(layerGame);
                currentLayer = layerGame;

                // Верхняя панель
                TopControl = new VisualControl(layerGame);

                // Создаем панели игроков в верхней панели
                panelPlayers = new VisualControl(TopControl, 0, Config.GridSize);

                PanelPlayer pp;
                int nextLeftPanelPlayer = 0;
                foreach (Player p in lobby.Players)
                {
                    pp = new PanelPlayer(panelPlayers, nextLeftPanelPlayer);
                    // !!! Эту привязку переместить в StartNewLobby()
                    pp.LinkToLobby(p);
                    nextLeftPanelPlayer = pp.NextLeft();
                }
                panelPlayers.ApplyMaxSize();

                // Кнопки в правом верхнем углу
                btnPreferences = CreateButton(TopControl, ilGui, GUI_INVENTORY, 0, Config.GridSize, BtnPreferences_Click, BtnPreferences_MouseHover);
                btnHelp = CreateButton(TopControl, ilGui, GUI_BOOK, 0, btnPreferences.ShiftY, BtnHelp_Click, BtnHelp_MouseHover);
                btnQuit = CreateButton(TopControl, ilGui, GUI_EXIT, 0, btnPreferences.ShiftY, BtnQuit_Click, BtnQuit_MouseHover);

                TopControl.ApplyMaxSize();

                // Главное игровое поле
                MainControl = new VisualControl(layerGame);

                // Тулбар
                bmpPreparedToolbar = new VCBitmap(MainControl, 0, 0, null);

                // Метки с информацией о Королевстве
                labelDay = new VCToolLabel(bmpPreparedToolbar, Config.GridSize, 6, "", GUI_16_DAY);
                labelDay.Click += LabelDay_Click;
                labelDay.ShowHint += LabelDay_ShowHint;
                labelDay.Width = 64;
                labelGreatness = new VCToolLabel(bmpPreparedToolbar, labelDay.NextLeft(), labelDay.ShiftY, "", GUI_16_GREATNESS);
                labelGreatness.ShowHint += LabelGreatness_ShowHint;
                labelGreatness.Width = 112;
                labelGold = new VCToolLabel(bmpPreparedToolbar, labelGreatness.NextLeft(), labelDay.ShiftY, "", GUI_16_GOLD);
                labelGold.ShowHint += LabelGold_ShowHint;
                labelGold.Width = 168;

                btnEndTurn = CreateButton(MainControl, ilGui, GUI_HOURGLASS, 0, bmpToolbar.Height + Config.GridSize, BtnEndTurn_Click, BtnEndTurn_MouseHover);
                panelLairWithFlags = new VisualControl(MainControl, 0, btnEndTurn.ShiftY);

                // Отладочная информация
                vcDebugInfo = new VisualControl();
                labelTimeDrawFrame = new VCLabel(vcDebugInfo, Config.GridSize, Config.GridSize, Config.FontToolbar, Color.White, 16, "");
                labelTimeDrawFrame.StringFormat.Alignment = StringAlignment.Near;
                labelTimeDrawFrame.Width = 160;
                labelLayers = new VCLabel(vcDebugInfo, labelTimeDrawFrame.ShiftX, labelTimeDrawFrame.NextTop(), Config.FontToolbar, Color.White, 16, "Layers");
                labelLayers.StringFormat.Alignment = StringAlignment.Near;
                labelLayers.Width = 160;
                vcDebugInfo.ArrangeControls();
                vcDebugInfo.ApplyMaxSize();

                // Панели информации об объектахs
                panelHeroInfo = new PanelHeroInfo(MainControl, Config.GridSize, btnEndTurn.NextTop());
                panelHeroInfo.ApplyMaxSize();
                panelBuildingInfo = new PanelBuildingInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelBuildingInfo.ApplyMaxSize();
                panelLairInfo = new PanelLairInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelLairInfo.ApplyMaxSize();
                panelMonsterInfo = new PanelMonsterInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelMonsterInfo.ApplyMaxSize();
                panelEmptyInfo = new VisualControl(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY)
                {
                    Width = panelHeroInfo.Width,
                    Height = panelHeroInfo.Height,
                    ShowBorder = true
                };

                // Панель со всеми героями
                panelCombatHeroes = new PanelWithPanelEntity(5, false, 8, 4);
                panelCombatHeroes.ShiftY = btnEndTurn.NextTop();
                MainControl.AddControl(panelCombatHeroes);

                // Страницы игры
                pageGuilds = new VCFormPage(MainControl, 0, btnEndTurn.ShiftY, pages, ilGui, GUI_GUILDS, "Гильдии и военные сооружения", BtnPage_Click);
                pageGuilds.ShowHint += PageGuilds_ShowHint;
                pageBuildings = new VCFormPage(MainControl, 0, pageGuilds.ShiftY, pages, ilGui, GUI_ECONOMY, "Экономические строения", BtnPage_Click);
                pageBuildings.ShowHint += PageBuildings_ShowHint;
                pageTemples = new VCFormPage(MainControl, 0, pageGuilds.ShiftY, pages, ilGui, GUI_TEMPLE, "Храмы", BtnPage_Click);
                pageHeroes = new VCFormPage(MainControl, 0, pageGuilds.ShiftY, pages, ilGui, GUI_HEROES, "Герои", BtnPage_Click);
                pageHeroes.ShowCostZero = true;
                pageHeroes.ShowHint += PageHeroes_ShowHint;
                pageLairs = new VCFormPage(MainControl, 0, pageGuilds.ShiftY, pages, ilGui, GUI_BATTLE, "Окрестности", BtnPage_Click);
                pageTournament = new VCFormPage(MainControl, 0, pageGuilds.ShiftY, pages, ilGui, GUI_TOURNAMENT, "Турнир", BtnPage_Click);
                pageTournament.ShowHint += PageTournament_ShowHint;

                labelCaptionPage = new VCLabelM2(MainControl, 0, pageGuilds.ShiftY, fontMedCaptionC, Config.CommonCaptionPage, pageGuilds.Height, "");
                labelCaptionPage.Width = 240;
                labelCaptionPage.StringFormat.Alignment = StringAlignment.Near;
                labelCaptionPage.StringFormat.LineAlignment = StringAlignment.Center;

                DrawPageConstructions();
                DrawHeroes();
                DrawWarehouse();
                DrawPageLair();

                ShowDataPlayer();

                // Вычисляем максимальный размер страниц
                int maxHeightPages = 0;
                int maxWidthPages = 0;

                foreach (VCFormPage pc in pages)
                {
                    Size maxSizePanelPage = pc.Page.MaxSize();
                    maxWidthPages = Math.Max(maxWidthPages, maxSizePanelPage.Width);
                    maxHeightPages = Math.Max(maxHeightPages, maxSizePanelPage.Height);
                }

                // Располагаем страницы на главной форме
                int leftForNextButtonPage = panelEmptyInfo.NextLeft();
                foreach (VCFormPage fp in pages)
                {
                    fp.ShiftX = leftForNextButtonPage;
                    fp.Page.Width = maxWidthPages;

                    leftForNextButtonPage = fp.NextLeft();
                }

                labelCaptionPage.ShiftX = leftForNextButtonPage + Config.GridSize * 3;
                panelCombatHeroes.ShiftX = pageGuilds.ShiftX + maxWidthPages + Config.GridSize;

                // Создаем меню
                bitmapMenu = new VCBitmap(MainControl, 0, 0, new Bitmap(dirResources + @"Icons\Menu.png"));
                //Debug.Assert(panelHeroInfo.Width >= bitmapMenu.Width);

                CellsMenu = new VCMenuCell[PANEL_MENU_CELLS.Height, PANEL_MENU_CELLS.Width];
                for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                    for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                        CellsMenu[y, x] = new VCMenuCell(bitmapMenu, 77 + (x * (ilItems.Size + DISTANCE_BETWEEN_CELLS)), 95 + (y * (ilItems.Size + DISTANCE_BETWEEN_CELLS)), ilItems);

                //
                Debug.Assert(panelBuildingInfo.Height > 0);
                Debug.Assert(panelLairInfo.Height > 0);
                Debug.Assert(panelHeroInfo.Height > 0);
                Debug.Assert(panelMonsterInfo.Height > 0);

                int maxHeightPanelInfo = Math.Max(panelBuildingInfo.Height, panelLairInfo.Height);
                maxHeightPanelInfo = Math.Max(panelHeroInfo.Height, maxHeightPanelInfo);
                maxHeightPanelInfo = Math.Max(panelMonsterInfo.Height, maxHeightPanelInfo);
                int maxHeightControls = Math.Max(maxHeightPages, maxHeightPanelInfo);

                // Все контролы созданы, устанавливаем размеры bitmapMenu
                MainControl.Width = panelCombatHeroes.ShiftX + panelCombatHeroes.Width + Config.GridSize;
                MainControl.Height = pageGuilds.NextTop() + maxHeightControls + Config.GridSize;
                TopControl.Width = MainControl.Width;

                // Теперь когда известна ширина окна, можно создавать картинку тулбара
                bmpPreparedToolbar.Bitmap = PrepareToolbar();
                panelPlayers.ShiftX = (TopControl.Width - panelPlayers.Width) / 2;
                panelCombatHeroes.Height = maxHeightPages - bitmapMenu.Height - Config.GridSize;

                sizeGamespace = new Size(MainControl.Width, TopControl.Height + MainControl.NextTop());
                Width = Width - ClientSize.Width + sizeGamespace.Width;
                Height = Height - ClientSize.Height + sizeGamespace.Height;

                bitmapMenu.ShiftX = MainControl.Width - bitmapMenu.Width;
                bitmapMenu.ShiftY = MainControl.Height - bitmapMenu.Height;

                panelBuildingInfo.Height = MainControl.Height - panelBuildingInfo.ShiftY - Config.GridSize;
                panelLairInfo.Height = panelBuildingInfo.Height;
                panelHeroInfo.Height = panelBuildingInfo.Height;
                panelMonsterInfo.Height = panelBuildingInfo.Height;
                panelEmptyInfo.Height = panelBuildingInfo.Height;

                btnQuit.ShiftX = MainControl.Width - btnQuit.Width - Config.GridSize;
                btnHelp.PlaceBeforeControl(btnQuit);
                btnPreferences.PlaceBeforeControl(btnHelp);
                btnEndTurn.ShiftX = MainControl.Width - btnEndTurn.Width - Config.GridSize;

                //pageGuilds.ShiftX + maxWidthPages + Config.GridSize;

                ArrangeControls();

                SetStage("Прибираем после строителей");
                // Перенести в класс
                for (int i = 0; i < panelHeroInfo.slots.Count; i++)
                {
                    //panelHeroInfo.slots[i].MouseDown += PanelCellHero_MouseDown;
                    //panelHeroInfo.slots[i].MouseUp += PanelCellHero_MouseUp;
                    //panelHeroInfo.slots[i].MouseMove += PanelCell_MouseMove;
                }

                //
                ActivatePage(pageGuilds);

                //
                mpSelectButton = new System.Windows.Media.MediaPlayer();
                mpSelectButton.Open(new Uri(dirResources + @"Sound\Interface\Button\SelectButton.wav"));
                mpPushButton = new System.Windows.Media.MediaPlayer();
                mpPushButton.Open(new Uri(dirResources + @"Sound\Interface\Button\PushButton.wav"));
                mpConstructionComplete = new System.Windows.Media.MediaPlayer();
                mpConstructionComplete.Open(new Uri(dirResources + @"Sound\Interface\Construction\ConstructionComplete.wav"));

                formHint = new PanelHint();
                //formHint = new FormHint(ilGui16, ilParameters);

                ValidateAvatars();

                splashForm.Dispose();

                //
                timerHover = new Timer()
                {
                    Interval = SystemInformation.MouseHoverTime,
                    Enabled = false
                };
                timerHover.Tick += TimerHover_Tick;

                // Курсор
                CustomCursor.CreateCursor(dirResources + @"Cursor\Cursor_simple.png");
                Cursor = CustomCursor.GetCursor();

                //
                Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);

                //MediaElement me = new MediaElement()
                //me.Parent = this;
                

                // 
                void SetStage(string text)
                {
                    lblStage.Text = text + "...";
                    lblStage.Refresh();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message + Environment.NewLine + exc.StackTrace);
                Environment.Exit(-1);
            }
        }

        private void LabelGreatness_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(labelGreatness, "Величие", "Уровень величия и количество очков до следующего уровня");
        }

        private void LabelDay_Click(object sender, EventArgs e)
        {
            debugMode = !debugMode;
            labelTimeDrawFrame.Visible = debugMode;
            labelLayers.Visible = debugMode;
            ShowFrame(true);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // При старте игры в полноэкранном режиме, если курсор находится на пустом пространстве, окно игры состоит из белого фона
            // Показ кадра при старте отрисовывает окно
            ValidateFrame();
            //ShowFrame(true);
        }

        private void PageHeroes_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(pageHeroes, pageHeroes.Caption, "Нанято героев: " + lobby.CurrentPlayer.CombatHeroes.Count.ToString());
        }

        private void PageBuildings_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(pageBuildings, pageBuildings.Caption, "Доступно построек/апгрейдов зданий: " + lobby.CurrentPlayer.PointConstructionEconomic.ToString());
        }

        private void PageGuilds_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(pageGuilds, pageGuilds.Caption, "Доступно построек/апгрейдов гильдий: " + lobby.CurrentPlayer.PointConstructionGuild.ToString());
        }

        private void PageTournament_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(pageTournament, pageTournament.Caption, "Турнир начнется через " + lobby.DaysForTournament().ToString() + " дн.");
        }

        private void LabelGold_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(labelGold, "Казна", "Количество золота в казне и постоянный доход в день");
        }

        private void LabelDay_ShowHint(object sender, EventArgs e)
        {
            ShowHintForToolButton(labelDay, "День игры", "День игры: " + lobby.Turn.ToString());
        }

        private void BtnQuit_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(btnQuit, "Выход", "Выход из игры");
        }

        private void BtnHelp_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(btnHelp, "Справка", "Справка об игре");
        }

        private void BtnPreferences_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(btnPreferences, "Настройки", "Настройки игры");
        }

        internal bool CheckForNewVersion()
        {
            if (MainConfig.CheckForNewVersion())
            {
                if (MessageBox.Show("Обнаружена новая версия " + MainConfig.ActualVersion.ToString() + "."
                    + Environment.NewLine + "Выполнить обновление?", "Обновление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Process p = new Process();
                    p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = @"Updater.exe";
                    p.StartInfo.Arguments = "-silence";
                    p.Start();

                    Environment.Exit(0);
                }
                return true;
            }
            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Settings.ShowSplashVideo)
            {
                KeyDown += FormMain_KeyDown;
                KeyPreview = true;

                axWindowsMediaPlayer1.URL = dirResources + @"Video\Rebirth.avi";
                axWindowsMediaPlayer1.uiMode = "none";
                axWindowsMediaPlayer1.Location = new Point(0, 0);
                axWindowsMediaPlayer1.enableContextMenu = false;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.MouseDownEvent += AxWindowsMediaPlayer1_MouseDownEvent;
                axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
            }
            else
                axWindowsMediaPlayer1.Dispose();

            ApplyFullScreen(true);
            gameStarted = true;
        }

        private void ApplyFullScreenModeToWindow()
        {
            if (Settings.FullScreenMode)
            {
                MaximizeBox = true;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                MaximizeBox = false;
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Normal;
            }
        }

        private void ValidateFrame()
        {
            if (axWindowsMediaPlayer1 != null)
                axWindowsMediaPlayer1.Size = ClientSize;

            if ((bmpFrame == null) || !ClientSize.Equals(bmpFrame.Size))
                ArrangeControls();

            if (gameStarted)
                ShowFrame(true);
        }

        internal void ApplyFullScreen(bool force)
        {
            if (force || (MaximizeBox != Settings.FullScreenMode))
            {
                //Hide();
                ApplyFullScreenModeToWindow();


                //panelPlayers.Visible = false;
                //MainControl.Visible = false;
                //ShowFrame();
                //panelPlayers.Visible = true;
                //MainControl.Visible = true;
                //Application.DoEvents();
                ValidateFrame();
                //Show();
            }
        }

        private void PrepareBackground()
        {
            if ((bmpBackground == null) || !bmpBackground.Size.Equals(ClientSize))
            {
                // Переформировываем картинку фона
                bmpBackground?.Dispose();
                bmpBackground = GuiUtils.MakeBackground(ClientSize);

                if (Settings.FullScreenMode)
                {
                    Bitmap border = bbGamespace.DrawBorder(sizeGamespace.Width + 14, sizeGamespace.Height + 14);
                    Graphics gbg = Graphics.FromImage(bmpBackground);
                    gbg.DrawImageUnscaled(border, TopControl.Left - 7, TopControl.Top - 7);

                    border.Dispose();
                    gbg.Dispose();
                }

                // Переформировываем картинку кадра
                if ((bmpFrame == null) || (bmpFrame.Width != ClientSize.Width) || (bmpFrame.Height != ClientSize.Height))
                {
                    bmpFrame?.Dispose();
                    bmpFrame = new Bitmap(ClientSize.Width, ClientSize.Height);

                    gfxFrame?.Dispose();
                    gfxFrame = Graphics.FromImage(bmpFrame);
                }
            }
        }

        private void BtnEndTurn_Click(object sender, EventArgs e)
        {
            formHint.HideHint();

            lobby.DoEndTurn();

            if (lobby.CurrentPlayer == null)
            {
                // Лобби для текущего игрока закончено
                if (lobby.HumanIsWin)
                {
                    MessageBox.Show("Поздравляем, вы победитель!", "ПОБЕДА!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    StartNewLobby();
                    return;
                }
                else// Если вылетели из лобби, то показываем итоговое место и начинаем новое лобби
                {
                    // Здесь заложено, что реальный игрок под номером 0. Это может быть не так
                    MessageBox.Show("Поражение..." + Environment.NewLine + "Вы заняли " + lobby.Players[0].PositionInLobby.ToString() + " место.", "ПОРАЖЕНИЕ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    StartNewLobby();
                    return;
                }

            }

            Debug.Assert(lobby.CurrentPlayer.IsLive);
            if (lobby.CurrentPlayer.IsLive)
            {
                ShowDataPlayer();
            }
        }

        private void BtnEndTurn_MouseHover(object sender, EventArgs e)
        {
            formHint.Clear();
            formHint.AddStep1Header("Конец хода", "", "Завершение хода");
            formHint.DrawHint(btnEndTurn);
        }

        private void LabelGold_MouseHover(object sender, EventArgs e)
        {
        }

        private void BtnPage_Click(object sender, EventArgs e)
        {
            ActivatePage((VCFormPage)sender);
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            FormSettings f = new FormSettings();
            f.ApplySettings(Settings);
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (Settings.NamePlayer != lobby.CurrentPlayer.Name)
                {
                    lobby.CurrentPlayer.Name = Settings.NamePlayer;
                }

                ApplyFullScreen(false);
            }
        }

        internal VisualLayer AddLayer(VisualControl vc)
        {
            Debug.Assert(Layers.Count <= 5);
            Debug.Assert(currentLayer.Controls.Count > 0);

            VisualLayer vl = new VisualLayer();
            Layers.Add(vl);
            vl.AddControl(vc);
            currentLayer = vl;

            return vl;
        }

        internal void RemoveLayer(VisualLayer vl)
        {
            Debug.Assert(Layers.Count >= 2);
            Debug.Assert(Layers[Layers.Count - 1] == vl);

            Layers.Remove(vl);
            currentLayer = Layers[Layers.Count - 1];
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            FormAbout f = new FormAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!inQuit)
            {
                inQuit = true;
                FormConfirmExit f = new FormConfirmExit();
                e.Cancel = f.ShowModal() == DialogResult.No;
                inQuit = false;

                ShowFrame(true);
            }
            else
                e.Cancel = true;
        }

        internal void LayerChanged()
        {
            ShowFrame(true);
        }

        private void ArrangeControls()
        {
            ShiftControls = new Point(0, 0);

            if (Settings.FullScreenMode)
            {
                ShiftControls = new Point((ClientSize.Width - sizeGamespace.Width) / 2, (ClientSize.Height - sizeGamespace.Height) / 2);

                Debug.Assert(ShiftControls.X >= 0);
                Debug.Assert(ShiftControls.Y >= 0);
            }
            else
            {
                Size = new Size(Width - ClientSize.Width + sizeGamespace.Width, Height - ClientSize.Height + sizeGamespace.Height);
            }

            TopControl.SetPos((ClientSize.Width - TopControl.Width) / 2, ShiftControls.Y);
            MainControl.SetPos(ShiftControls.X, TopControl.Top + TopControl.Height + Config.GridSize);
            MainControl.ArrangeControls();

            AdjustPanelLairsWithFlags();
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void AxWindowsMediaPlayer1_MouseDownEvent(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState != (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Parent = null;
                axWindowsMediaPlayer1.close();
                axWindowsMediaPlayer1 = null;
                KeyDown -= FormMain_KeyDown;
                KeyPreview = false;
            }
        }

        internal static Config Config { get; set; }

        private void AddBitmapToImageList(ImageList il, Bitmap bitmap, int height)
        {
            int lines = bitmap.Height / height;

            if (lines > 1)
            {
                int pics = bitmap.Width / il.ImageSize.Width;
                for (int i = 0; i < lines; i++)
                {
                    for (int j = 0; j < pics; j++)
                    {
                        Bitmap bmpSingleline = new Bitmap(il.ImageSize.Width, height);
                        Graphics g = Graphics.FromImage(bmpSingleline);
                        g.DrawImage(bitmap, 0, 0, new Rectangle(j * il.ImageSize.Width, i * height, il.ImageSize.Width, il.ImageSize.Height), GraphicsUnit.Pixel);
                        il.Images.Add(bmpSingleline);
                        g.Dispose();
                    }
                }
            }
            else
            {
                if (il.Images.AddStrip(bitmap) == -1)
                    throw new Exception("Не удалось добавить полосу изображения.");
            }
        }

        private Bitmap GreyBitmap(Bitmap bmp)
        {
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);

            // Перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < bmp.Height; j++)
                for (int i = 0; i < bmp.Width; i++)
                {
                    // получаем (i, j) пиксель
                    uint pixel = (uint)(bmp.GetPixel(i, j).ToArgb());

                    // получаем компоненты цветов пикселя
                    float R = (pixel & 0x00FF0000) >> 16; // красный
                    float G = (pixel & 0x0000FF00) >> 8; // зеленый
                    float B = pixel & 0x000000FF; // синий
                                                  // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = G = B = (R + G + B) / 3.0f;

                    // собираем новый пиксель по частям (по каналам)
                    uint newPixel = ((uint)bmp.GetPixel(i, j).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            return output;
        }

        private Bitmap BrightBitmap(Bitmap bmp)
        {
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);

            // Перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < bmp.Height; j++)
                for (int i = 0; i < bmp.Width; i++)
                {
                    // получаем (i, j) пиксель
                    uint pixel = (uint)(bmp.GetPixel(i, j).ToArgb());

                    // получаем компоненты цветов пикселя
                    float R = (pixel & 0x00FF0000) >> 16; // красный
                    float G = (pixel & 0x0000FF00) >> 8; // зеленый
                    float B = pixel & 0x000000FF; // синий
                                                  // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = Math.Min(R * 1.2f, 255);
                    G = Math.Min(G * 1.2f, 255);
                    B = Math.Min(B * 1.2f, 255);

                    // собираем новый пиксель по частям (по каналам)
                    uint newPixel = ((uint)bmp.GetPixel(i, j).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            return output;
        }

        internal void ShowCurrentPlayerLobby()
        {
            if (lobby.CurrentPlayer.TypePlayer == TypePlayer.Human)
            {
                MainControl.Visible = true;
                ShowDataPlayer();
            }
            else
            {
                MainControl.Visible = false;
            }

            ShowFrame(true);
        }

        internal void ShowDataPlayer()
        {
            Debug.Assert(lobby.CurrentPlayer.TypePlayer == TypePlayer.Human);

            labelDay.Text = lobby.Turn.ToString();

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayer)
            {
                //DrawExternalBuilding();

                curAppliedPlayer = lobby.CurrentPlayer;
            }

            ShowLobby();

            UpdateListHeroes();
            ShowWarehouse();

            labelGreatness.Text = curAppliedPlayer.LevelGreatness.ToString()
                + " (" + curAppliedPlayer.PointGreatness.ToString() + "/"
                + curAppliedPlayer.PointGreatnessForNextLevel.ToString() + ")";
        }

        internal void UpdateListHeroes()
        {
            List<ICell> listHeroes = new List<ICell>();
            for (int y = 0; y < lobby.CurrentPlayer.CellHeroes.GetLength(0); y++)
                for (int x = 0; x < lobby.CurrentPlayer.CellHeroes.GetLength(1); x++)
                    listHeroes.Add(lobby.CurrentPlayer.CellHeroes[y, x]);

            //panelHeroes.ApplyList(listHeroes);
        }

        private void ShowLobby()
        {
            /*int top = 0;
            foreach (Player p in lobby.Players.OrderBy(p => p.PositionInLobby))
            {
                Debug.Assert(p.PositionInLobby >= 1);
                Debug.Assert(p.PositionInLobby <= lobby.TypeLobby.QuantityPlayers);

                p.Panel.ShiftY = top;
                top += p.Panel.Height + Config.GridSize;
            }*/

            //panelPlayers.ArrangeControls();

            // Показываем сооружения
            foreach (PlayerBuilding pb in lobby.CurrentPlayer.Buildings)
            {
                pb.Building.Panel.LinkToPlayer(pb);
            }

            // Показываем логова
            foreach (PlayerLair pl in lobby.CurrentPlayer.Lairs)
            {
                pl.TypeLair.Panel.LinkToPlayer(pl);
            }

            // Показываем героев
            ListHeroesChanged();
        }

        private void DrawPageConstructions()
        {
            // Создаем массив из страниц, линий и позиций
            PanelConstruction[,,] panels = new PanelConstruction[3, Config.BuildingMaxLines, Config.BuildingMaxPos];

            // Проходим по каждому зданию, создавая ему панель
            VisualControl parent;
            foreach (TypeConstruction tck in Config.TypeConstructionsOfKingdom)
            {
                switch (tck.Page)
                {
                    case Page.Guild:
                        parent = pageGuilds.Page;
                        break;
                    case Page.Economic:
                        parent = pageBuildings.Page;
                        break;
                    case Page.Temple:
                        parent = pageTemples.Page;
                        break;
                    default:
                        throw new Exception("Неизвестная страница " + tck.Page.ToString());
                }

                Debug.Assert(panels[(int)tck.Page, tck.Line - 1, tck.Pos - 1] == null);

                tck.Panel = new PanelConstruction(parent, 0, 0, tck);
                tck.Panel.ShiftX = (tck.Panel.Width + Config.GridSize) * (tck.Pos - 1);
                tck.Panel.ShiftY = (tck.Panel.Height + Config.GridSize) * (tck.Line - 1);
                panels[(int)tck.Page, tck.Line - 1, tck.Pos - 1] = tck.Panel;
            }
        }

        private void DrawPageLair()
        {
            int top = 0;
            int left;
            int height = 0;

            for (int line = 1; line <= 4; line++)
            {
                left = 0;

                foreach (TypeLair l in Config.TypeLairs)
                {
                    if (l.Line == line)
                    {
                        l.Panel = new PanelLair(pageLairs.Page, left, top, l);

                        left += l.Panel.Width + Config.GridSize;
                        height = l.Panel.Height;
                    }
                }

                top += height + Config.GridSize;
            }
        }

        private void DrawHeroes()
        {
            panelHeroes = new PanelWithPanelEntity(Config.HeroRows);
            pageHeroes.Page.AddControl(panelHeroes);
            panelHeroes.ShiftY = 0;

            List<ICell> list = new List<ICell>();
            for (int x = 0; x < Config.HeroRows * Config.HeroInRow; x++)
                list.Add(null);

            panelHeroes.ApplyList(list);
            panelHeroes.Height = panelHeroes.MaxSize().Height;
        }

        internal void ShowAboutHero(PlayerHero ph)
        {
            panelHeroInfo.Hero = ph;
        }

        private void ShowBattle()
        {
            if (lobby.Turn > 1)
            {
                Battle b = lobby.GetBattle(lobby.CurrentPlayer, lobby.Turn - 1);

                FormBattle fb = new FormBattle();
                fb.ShowBattle(b);
            }
        }

        private void DrawWarehouse()
        {
            panelWarehouse = new PanelWithPanelEntity(Config.WarehouseWidth);
            pageHeroes.Page.AddControl(panelWarehouse);
            panelWarehouse.ShiftY = panelHeroes.NextTop();
        }

        internal void ShowWarehouse()
        {
            panelWarehouse.ApplyList(lobby.CurrentPlayer.Warehouse.ToList<ICell>());
        }

        private void ActivatePage(VCFormPage pc)
        {
            if (pc != currentPage)
            {
                if (currentPage != null)
                    currentPage.Page.Visible = false;
                currentPage = pc;
                currentPage.Page.Visible = true;
                labelCaptionPage.Text = currentPage.Caption;

                SetNeedRedrawFrame();
            }
        }

        internal void ActivatePageLairs()
        {
            ActivatePage(pageLairs);
        }

        private void StartNewLobby()
        {
            lobby = new Lobby(Config.TypeLobbies[0]);

            DrawLobby();

            ShowDataPlayer();
        }

        private void DrawLobby()
        {
            foreach (Player p in lobby.Players)
            {
                p.Panel.ShiftY = (p.PositionInLobby - 1) * (p.Panel.Height + Config.GridSize);
            }

            panelPlayers.ArrangeControls();
        }
        internal void SetNeedRedrawFrame()
        {
            needRedrawFrame = true;
        }

        internal void UpdateMenu()
        {
            // Рисуем содержимое ячеек
            if ((selectedPlayerObject != null) && (selectedPlayerObject is PlayerBuilding pb))
            {
                Debug.Assert(pb.Building != null);

                ClearMenu();

                if (pb.Building.Researches != null)
                    foreach (PlayerResearch pr in pb.Researches)
                    {
                        if (CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research == null)
                            CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research = pr;
                        else if (CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research.Research.Layer > pr.Research.Layer)
                            CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research = pr;
                    }
            }
            else
            {
                ClearMenu();
            }

            void ClearMenu()
            {
                for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                    for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                        CellsMenu[y, x].Research = null;
            }
        }

        internal void UpdateBuildingInfo()
        {
            Debug.Assert(panelBuildingInfo != null);

            SetNeedRedrawFrame();
        }

        private void ShowHintForToolButton(VisualControl c, string text, string hint)
        {
            formHint.Clear();
            formHint.AddStep1Header(text, "", hint);
            formHint.DrawHint(c);
        }

        internal void ValidateAvatars()
        {
            Settings.LoadAvatar();

            if (lobby != null)
            {
                foreach (Player ph1 in lobby.Players)
                    foreach (Player ph2 in lobby.Players)
                        if (ph1 != ph2)
                            Debug.Assert(ph1.ImageIndexAvatar != ph2.ImageIndexAvatar);
            }

            if (lobby != null)
                SetNeedRedrawFrame();
        }

        internal ImageList BigIconToSmall(ImageList ilBig)
        {
            ImageList ilSmall = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(48, 48)
            };

            foreach (Image i in ilBig.Images)
            {
                Bitmap bmpDest = new Bitmap(48, 48);
                Graphics gDest = Graphics.FromImage(bmpDest);
                gDest.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gDest.SmoothingMode = SmoothingMode.HighQuality;
                gDest.DrawImage(i, new Rectangle(0, 0, 48, 48), new Rectangle(0, 0, 128, 128), GraphicsUnit.Pixel);
                //gDest.DrawImageUnscaled(MaskSmall, 0, 0);
                ilSmall.Images.Add(bmpDest);
                gDest.Dispose();
            }
            ilSmall.Tag = ilSmall.Images.Count;

            return ilSmall;
        }

        private void MakeAlpha()
        {
            //Bitmap b = new Bitmap(ilItems.Images[1]);
            /*Bitmap b = new Bitmap(dirResources + @"Icons\sklep.png");
            Bitmap a = new Bitmap(dirResources + @"Icons\MaskBig.png");
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    b.SetPixel(x, y, Color.FromArgb(a.GetPixel(x, y).A, b.GetPixel(x, y).R, b.GetPixel(x, y).G, b.GetPixel(x, y).B));
                }
            b.Save(@"f:\Projects\C-Sharp\Fantasy King's Battle\Resources\Icons\1.png");*/

            /*Bitmap b = new Bitmap(ilPlayerAvatarsBig.Images[0]);
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    b.SetPixel(x, y, Color.FromArgb(b.GetPixel(x, y).A, 0, 0, 0));
                }
            b.Save(@"f:\Projects\C-Sharp\Fantasy King's Battle\Resources\Icons\1.png");*/
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Debug.Assert(ClientSize.Equals(bmpFrame.Size));

            base.OnPaint(e);

            if (needRepaintFrame)
            {
                DrawFrame();
                needRepaintFrame = false;
            }

            e.Graphics.DrawImage(bmpFrame, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        private void ShowFrame(bool force)
        {
            if ((force || needRedrawFrame) && (WindowState != FormWindowState.Minimized))
            {
                needRedrawFrame = false;

                DrawFrame();// Готовим кадр

                Refresh();// Сразу же рисуем кадр
            }
        }

        // Рисование кадра главной формы
        private void DrawFrame()
        {
//            if (inDrawFrame)
//                return;
            Debug.Assert(inDrawFrame == false);

            inDrawFrame = true;

            if (debugMode)
            {
                startDebugAction = DateTime.Now;
                labelLayers.Text = $"Layers: {Layers.Count}; PlaySelectButton: {playsSelectButton}";
            }

            // Рисуем фон
            if ((bmpBackground == null) || !bmpBackground.Size.Equals(ClientSize))
                PrepareBackground();

            gfxFrame.CompositingMode = CompositingMode.SourceCopy;
            gfxFrame.DrawImageUnscaled(bmpBackground, 0, 0);

            if (MainControl.Visible)
            {
                labelGold.Text = lobby.CurrentPlayer.Gold.ToString() + " (+" + lobby.CurrentPlayer.Income().ToString() + ")";

                pageGuilds.PopupQuantity = lobby.CurrentPlayer.PointConstructionGuild;
                pageBuildings.PopupQuantity = lobby.CurrentPlayer.PointConstructionEconomic;
                pageHeroes.Cost = lobby.CurrentPlayer.CombatHeroes.Count;

                //
                UpdateMenu();
            }

            // Рисуем контролы
            gfxFrame.CompositingMode = CompositingMode.SourceOver;

            foreach (VisualLayer vl in Layers)
            {
                vl.DrawBackground(gfxFrame);
            }

            foreach (VisualLayer vl in Layers)
            {
                vl.Draw(gfxFrame); 
            }

            // Рисуем подсказку поверх всех окон
            if (formHint.Visible)
                formHint.Draw(gfxFrame);

            if (debugMode)
            {
                if (controlWithHint != null)
                    gfxFrame.DrawRectangle(penDebugBorder, controlWithHint.Rectangle);

                durationDrawFrame = DateTime.Now - startDebugAction;
                labelTimeDrawFrame.Text = "Draw frame: " + durationDrawFrame.TotalMilliseconds.ToString();
                vcDebugInfo.Draw(gfxFrame);
            }

            //
            inDrawFrame = false;
        }

        internal VCIconButton CreateButton(VisualControl parent, BitmapList bitmapList, int imageIndex, int left, int top, EventHandler click, EventHandler showHint)
        {
            VCIconButton b = new VCIconButton(parent, left, top, bitmapList, imageIndex);
            b.Click += click;
            b.ShowHint += showHint;

            return b;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            TreatMouseMove(e.Button == MouseButtons.Left);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            currentLayer.KeyUp(e);
        }        

        private VisualControl ControlUnderMouse()
        {
            VisualControl curControl = null;

            if (currentLayer == layerGame)
            {
                curControl = TopControl.GetControl(mousePos.X, mousePos.Y);
                if (curControl == null)
                {
                    curControl = MainControl.GetControl(mousePos.X, mousePos.Y);
                    if (curControl == MainControl)
                    {
                        curControl = currentPage.Page.GetControl(mousePos.X, mousePos.Y);
                        if (curControl == null)
                            curControl = MainControl;
                        //if (curControl == currentPage)
                        //    curControl = null;
                    }
                }
            }
            else
            {
                foreach (VisualControl vc in currentLayer.Controls)
                {
                    curControl = vc.GetControl(mousePos.X, mousePos.Y);
                    if (curControl != null)
                        break;
                }
            }

            return curControl;
        }

        private void TreatMouseMove(bool leftDown)
        {
            Point newMousePos = PointToClient(Cursor.Position);

            if (!mousePos.Equals(newMousePos))
            {
                mousePos = newMousePos;
                VisualControl curControl = ControlUnderMouse();

                if (curControl == null)
                {
                    timerHover.Stop();
                    ControlForHintLeave();
                }
                else if (curControl == controlWithHint)
                {
                    if (hintShowed)
                    {
                        timerHover.Stop();
                        formHint.HideHint();
                    }
                    else
                    {
                        // Если над контролом водят мышкой, отсчет времени начинаем только после остановки
                        timerHover.Stop();
                        timerHover.Start();
                    }
                }
                else
                {
                    ControlForHintLeave();
                    controlWithHint = curControl;
                    controlWithHint.MouseEnter(leftDown);
                    timerHover.Start();
                    SetNeedRedrawFrame();
                }

                ShowFrame(false);
            }
        }

        private void ControlForHintLeave()
        {
            if ((controlWithHint != null) && (controlWithHint == controlClicked))
                controlClicked = null;

            if (controlWithHint != null)
            {
                controlWithHint.MouseLeave();
                controlWithHint = null;
            }

            if (controlClicked != null)
            {
                controlClicked.MouseLeave();
                controlClicked = null;
            }

            hintShowed = false;
            formHint.HideHint();

            ShowFrame(false);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            ControlForHintLeave();
            formHint.HideHint();

            // Если мышь покидает пределы окна, надо отрисовать его, т.к. теряется фокус у активного контрола
            ShowFrame(true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                controlWithHint?.MouseDown();

                ShowFrame(false);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                // Обработка клика перемещена из MouseClick, так как если кликать часто, то системой
                // это распознается как двойной клик и вызывает OnMouseDoubleClick вместо OnMouseClick
                if (controlWithHint != null)
                {
                    controlWithHint.MouseUp();
                    // При клике происходит перерисовка кадра, и текущий элемент может стать уже невидимым
                    // Но он будет все равно считаться активным, так как прописан в controlWithHint
                    // Поэтому перед кликом убираем его
                    controlClicked = controlWithHint;
                    controlWithHint = null;
                    controlClicked.DoClick();

                    // Смотрим какой контрол под мышкой сейчас. Если тот же самый, восстанавливаем его
                    VisualControl curControl = ControlUnderMouse();
                    if ((curControl != null) && (curControl == controlClicked))
                        controlWithHint = controlClicked;
                    else
                    {
                        controlWithHint = controlClicked;
                        ControlForHintLeave();// Контрол уже другой, отменяем подсказку
                    }
                    controlClicked = null;

                    ShowFrame(false);

                    if (formHint.Visible)
                    {
                        ControlForHintLeave();
                        mousePos = new Point(0, 0);
                        TreatMouseMove(false);
                    }
                }
            }
        }

        private void TimerHover_Tick(object sender, EventArgs e)
        {
            if (controlWithHint != null)
            {
                timerHover.Stop();
                controlWithHint.DoShowHint();
                hintShowed = true;
                if (formHint.ExistHint)
                {
                    formHint.Visible = true;
                    needRepaintFrame = true;
                    Invalidate(true);
                }
            }
        }

        internal void NeedRedrawFrame()
        {
            needRedrawFrame = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);

            ControlForHintLeave();
            if (WindowState != FormWindowState.Minimized)
                ShowFrame(true);
        }

        internal int TreatImageIndex(int imageIndex, BattleParticipant p)
        {
            return imageIndex != IMAGE_INDEX_CURRENT_AVATAR ? imageIndex : p.ImageIndexAvatar;
        }

        internal void LairsWithFlagChanged()
        {
            AdjustPanelLairsWithFlags();
        }

        private void AdjustPanelLairsWithFlags()
        {
            Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);

            // Приводим в соответствие количество кнопок и логов
            // Для этого скрываем все кнопки, а потом делаем их видимыми.
            // Это чтобы не создавать каждый раз заново кнопки при изменении их численности
            if (listBtnTargetLair.Count < lobby.CurrentPlayer.LairsWithFlag.Count)
                listBtnTargetLair.Add(new VCButtonTargetLair(panelLairWithFlags));

            foreach (VCButtonTargetLair b in listBtnTargetLair)
                b.Visible = false;                     

            // Сортируем логова и переназначаем ссылки на них у кнопок
            int n = 0;
            int left = 0;
            foreach (PlayerLair pl in lobby.CurrentPlayer.LairsWithFlag.OrderByDescending(l => l.PriorityFlag).OrderByDescending(l => l.listAttackedHero.Count))
            {
                listBtnTargetLair[n].ShiftX = left;
                listBtnTargetLair[n].Lair = pl;
                listBtnTargetLair[n].Visible = true;

                left = listBtnTargetLair[n].NextLeft();
                n++;
            }

            panelLairWithFlags.ShiftX = btnEndTurn.ShiftX - left - Config.GridSize;
            MainControl.ArrangeControl(panelLairWithFlags);

            SetNeedRedrawFrame();
        }

        internal void ListHeroesChanged()
        {
            if (lobby != null)
            {
                Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);

                panelCombatHeroes.ApplyList(curAppliedPlayer.CombatHeroes);

                SetNeedRedrawFrame();
            }
        }

        internal void PlaySoundSelect(string filename)
        {
            spSoundSelect.SoundLocation = dirResources + @"Sound\Interface\ConstructionSelect\" + filename;
            spSoundSelect.Load();
            spSoundSelect.Play();
        }

        internal void PlaySelectButton()
        {
            playsSelectButton++;
            mpSelectButton.Stop();
            mpSelectButton.Play();
        }

        internal void PlayPushButton()
        {
            mpPushButton.Stop();
            mpPushButton.Play();
        }

        internal void PlayConstructionComplete()
        {
            mpConstructionComplete.Stop();
            mpConstructionComplete.Play();
        }

        private Bitmap PrepareToolbar()
        {
            Bitmap bmp = new Bitmap(MainControl.Width, bmpToolbar.Height);

            Graphics g = Graphics.FromImage(bmp);

            DrawBitmap(0, bmpToolbar);
            DrawBitmap(0, bmpToolbarBorder);
            DrawBitmap(bmp.Height - bmpToolbarBorder.Height, bmpToolbarBorder);

            g.Dispose();
            return bmp;

            void DrawBitmap(int top, Bitmap b)
            {
                int repeats = bmp.Width / b.Width;
                int restBorder = bmp.Width - (b.Width * repeats);

                for (int i = 0; i < repeats; i++)
                {
                    g.DrawImageUnscaled(b, i * b.Width, top);
                }

                g.DrawImageUnscaledAndClipped(b, new Rectangle(repeats * b.Width, top, restBorder, b.Height));
            }
        }

        internal void SelectPlayerObject(PlayerObject po)
        {
            if (selectedPlayerObject != po)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;

                if (selectedPlayerObject != null)
                    selectedPlayerObject.HideInfo();

                selectedPlayerObject = po;
                selectedPlayerObject.ShowInfo();

                SetNeedRedrawFrame();
            }
        }

        internal bool PlayerObjectIsSelected(PlayerObject po)
        {
            Debug.Assert(po != null);

            return po == selectedPlayerObject;
        }
    }
}
