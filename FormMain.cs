using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Media;
using System.Text;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ProgramState { Started, ConfirmQuit, NeedQuit };
    internal enum DialogAction { None, OK, MainMenu, RestartGame, Quit };

    public partial class FormMain : Form
    {
        internal const string NAME_PROJECT = "The Fantasy Kingdoms Battle";
        internal const string VERSION = "0.3.14";
        internal const string DATE_VERSION = "27.09.2021";
        private const string VERSION_POSTFIX = "в разработке";
        internal readonly string dirCurrent;
        internal readonly string dirResources;

        internal ProgramState ProgramState { get; private set; } = ProgramState.Started;
        internal bool gameStarted = false;
        private bool needRepaintFrame = false;

        // Проигрывание звуков и музыки
        internal readonly PlayerMusic playerMusic;
        private readonly System.Windows.Media.MediaPlayer mpSoundSelect;
        private readonly System.Windows.Media.MediaPlayer mpSelectButton;
        private readonly System.Windows.Media.MediaPlayer mpPushButton;
        private readonly System.Windows.Media.MediaPlayer mpConstructionComplete;

        // ImageList'ы
        internal readonly BitmapList imListObjects128;
        internal BitmapList imListObjects48 { get; }
        internal readonly BitmapList ilGui16;
        internal readonly BitmapList ilGui24;
        internal readonly BitmapList ilParameters;
        internal readonly BitmapList ilStateHero;
        internal readonly BitmapList ilMenuCellFilters;
        internal readonly BitmapList blCheckBox;

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

        // Контролы главного меню
        private readonly VisualControl MainControl;

        private Point mousePos;
        private VisualControl controlWithHint;
        private VisualControl controlClicked;

        private readonly VisualControl panelPlayers;// Панель, на которой находятся панели игроков лобби

        private readonly VCToolLabel labelDay;
        private readonly VCToolLabel labelBuilders;
        private readonly VCToolLabel labelGold;
        private readonly VCToolLabel labelGreatness;
        private readonly VCToolLabel labelHeroes;
        private readonly VCToolLabel labelCorruption;
        private readonly VCLabel labelNamePlayer;

        private readonly VCIconButton48 btnInGameMenu;
        private readonly VCIconButton48 btnEndTurn;

        private readonly VisualControl panelLairWithFlags;
        private readonly List<VCButtonTargetLair> listBtnTargetLair = new List<VCButtonTargetLair>();
        private readonly List<VCImageLose> listBtnLoses = new List<VCImageLose>();

        // Рендеринг
        private Bitmap bmpRenderClientArea;// Фон клиентской области, на который налагается кадр
        private Bitmap bmpRenderBackgroundFrame;// Фон рисунка, на котором рисуются контролы
        private Bitmap bmpRenderFrame;// Рисунок, на котором рисуются контролы (без учета полноэкранного режима)
        private Graphics gfxRenderFrame;// Graphics кадра
        private Graphics gfxRenderClientArea;// Graphics клиентской области

        // Первый слой (главное меню)
        private readonly VisualControl layerMainMenu;
        private readonly VCBitmap bmpPreparedToolbar;
        private readonly VCBitmap bitmapLogo;
        private readonly VCBitmap bitmapNameGame;
        private readonly VCBitmap bitmapMenu;
        private readonly VCLabel labelVersion;
        private readonly VCBitmap bmpMainMenu;
        private readonly VCButton btnTournament;
        private readonly VCButton btnPlayerPreferences;
        private readonly VCButton btnGamePreferences;
        private readonly VCButton btnAboutProgram;
        private readonly VCButton btnExitToWindows;

        internal BigEntity selectedPlayerObject;

        // Главные страницы игры
        private readonly VCPageControl pageControl;
        private readonly VCPageButton pageResultTurn;
        private readonly VCPageButton pageGuilds;
        private readonly VCPageButton pageEconomicConstructions;
        private readonly VCPageButton pageTemples;
        private readonly VCPageButton pageHeroes;
        private readonly VCPageButton pageLairs;
        private readonly VCPageButton pageTournament;

        private readonly PanelConstruction[,] panelLairs;
        private PanelWithPanelEntity panelWarehouse;
        private PanelWithPanelEntity panelHeroes;
        private readonly VisualControl vcRightPanel;
        private PanelWithPanelEntity panelCombatHeroes;

        private readonly VisualControl panelNeighborhood;
        private readonly List<VCNeighborhood> listButtonsLayers = new List<VCNeighborhood>();
        private int currentNeighborhood = 0;

        private WindowAdvice winAdvice;

        private VCCell[] pageTournamentPlayers;
        private readonly List<VCResultRound> listResultRound = new List<VCResultRound>();

        private const int DEFAULT_DPI = 96;

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
        internal const int GUI_16_BUILDER = 2;
        internal const int GUI_16_GREATNESS = 3;
        internal const int GUI_16_HOLYLAND = 4;
        internal const int GUI_16_TRADEPOST = 5;
        internal const int GUI_16_FLAG_SCOUT = 6;
        internal const int GUI_16_PEASANT_HOUSE = 7;
        internal const int GUI_16_FLAG_ATTACK = 8;
        internal const int GUI_16_FLAG_DEFENSE = 9;
        internal const int GUI_16_HEROES = 10;
        internal const int GUI_16_COFFERS = 11;
        internal const int GUI_16_CORRUPTION = 12;

        internal const int GUI_24_FIRE = 0;
        internal const int GUI_24_HEROES = 1;
        internal const int GUI_24_STAR = 2;
        internal const int GUI_24_BUTTON_LEFT = 3;
        internal const int GUI_24_BUTTON_RIGHT = 4;
        internal const int GUI_24_NEUTRAL = 5;
        internal const int GUI_24_LOSE = 6;
        internal const int GUI_24_WIN = 7;
        internal const int GUI_24_TRANSP_LOSE = 8;
        internal const int GUI_24_TRANSP_WIN = 9;

        internal const int GUI_45_EMPTY = 0;
        internal const int GUI_45_BORDER = 0;

        internal static Size PANEL_MENU_CELLS = new Size(4, 3);
        private const int DISTANCE_BETWEEN_CELLS = 3;

        internal const int IMAGE_INDEX_NONE = 127;
        internal const int IMAGE_INDEX_UNKNOWN = 139;
        internal const int IMAGE_INDEX_CURRENT_AVATAR = -100;
        internal const int MAX_LAIR_LAYERS = 5;

        private Lobby lobby;
        private Player curAppliedPlayer;

        internal Lobby CurrentLobby { get { return lobby; } }

        private readonly float dpiX;
        private readonly float dpiY;
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
        internal readonly BitmapBorder bbIcon16;
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
        internal readonly Bitmap bmpBandQuest;
        internal readonly Bitmap bmpMenuInGame;

        internal readonly M2Font fontSmall;
        internal readonly M2Font fontSmallC;
        internal readonly M2Font fontMedCaptionC;
        internal readonly M2Font fontMedCaption;
        internal readonly M2Font fontBigCaptionC;
        internal readonly M2Font fontBigCaption;
        internal readonly M2Font fontSmallBC;
        internal readonly M2Font fontParagraph;
        internal readonly M2Font fontParagraphC;

        internal Size sizeGamespace { get; }
        private Point topLeftFrame;
        private bool inDrawFrame = false;
        private bool needRedrawFrame;

        private readonly List<VisualControl> Layers;
        private readonly VisualControl layerGame;
        private VisualControl currentLayer;

        private readonly VisualControl panelEmptyInfo;
        internal PanelConstructionInfo panelConstructionInfo { get; private set; }
        internal PanelLairInfo panelLairInfo { get; private set; }
        internal PanelHeroInfo panelHeroInfo { get; private set; }
        internal PanelMonsterInfo panelMonsterInfo { get; private set; }

        internal VCMenuCell[,] CellsMenu { get; }

        internal PanelHint formHint;

        //
        internal Settings Settings { get; private set; }
        internal MainConfig MainConfig { get; private set; }
        internal HumanPlayer CurrentHumanPlayer { get; private set; }

        private readonly Timer timerHover;

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

            // Определяем DPI для корректировки картинок
            Graphics gDpi = Graphics.FromHwnd(IntPtr.Zero);
            dpiX = gDpi.DpiX;
            dpiY = gDpi.DpiY;
            gDpi.Dispose();

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
                Settings = new Settings();
                Settings.PlayMusicChanged += Settings_PlayMusicChanged;

                MainConfig = new MainConfig(dirResources);
                // Проверяем требование по разрешению экрана
                if ((Screen.PrimaryScreen.Bounds.Width < MainConfig.ScreenMinSize.Width) || (Screen.PrimaryScreen.Bounds.Height < MainConfig.ScreenMinSize.Height))
                {
                    MessageBox.Show($"Для игры необходимо разрешение экрана {MainConfig.ScreenMinSize.Width} * {MainConfig.ScreenMinSize.Height}."
                        + Environment.NewLine + $"Текущее разрешение {Screen.PrimaryScreen.Bounds.Width} * {Screen.PrimaryScreen.Bounds.Height}.",
                        NAME_PROJECT, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                // Если включено автообновление, проверяем на их наличие
                if (Settings.CheckUpdateOnStartup)
                {
                    CheckForNewVersion();
                }

                fontSmall = new M2Font(dirResources, "small");
                fontSmallC = new M2Font(dirResources, "small_c");
                fontMedCaptionC = new M2Font(dirResources, "med_caption_c");
                fontMedCaption = new M2Font(dirResources, "med_caption");
                fontBigCaptionC = new M2Font(dirResources, "big_caption_c");
                fontBigCaption = new M2Font(dirResources, "big_caption");
                fontSmallBC = new M2Font(dirResources, "_small_b_c");
                fontParagraph = new M2Font(dirResources, "paragraph");
                fontParagraphC = new M2Font(dirResources, "paragraph_c");

                // Формируем и показываем сплэш-заставку
                Image splashBitmap = LoadBitmap("Splash.png");

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

                SelectHumanPlayer(Config.HumanPlayers[0]);

                // Загружаем иконки
                SetStage("Рассматриваем картины");

                bmpBorderBig = LoadBitmap("BorderBig.png");
                bmpMaskBig = LoadBitmap("MaskBig.png");
                bmpMaskSmall = LoadBitmap("MaskSmall.png");// Нужна ли еще?

                // Иконки игровых объектов. Также включает встроенные аватары игроков и пул пустых иконок под внешние аватары
                imListObjects128 = new BitmapList(LoadBitmap("Objects.png"), 128, true, true);
                // Добавляем места под внешние аватары
                imListObjects128.AddEmptySlots(Config.MaxQuantityExternalAvatars);
                imListObjects48 = new BitmapList(imListObjects128, 48, Config.BorderInBigIcons, bmpMaskSmall);
                LoadBitmapObjects();

                imListObjects48.AddBitmap(LoadBitmap("Gui48.png"));

                ilGui16 = new BitmapList(LoadBitmap("Gui16.png"), 16, true, false);
                ilGui24 = new BitmapList(LoadBitmap("Gui24.png"), 24, true, true);
                ilParameters = new BitmapList(LoadBitmap("Parameters.png"), 24, true, false);
                ilStateHero = new BitmapList(LoadBitmap("StateCreature.png"), 24, true, false);
                ilMenuCellFilters = new BitmapList(LoadBitmap("MenuCellFilters.png"), 48, true, false);
                blCheckBox = new BitmapList(LoadBitmap("CheckBox.png"), 24, false, false);

                //MakeAlpha();

                bmpForBackground = LoadBitmap("Background.png");
                bmpBorderForIcon = LoadBitmap("BorderIconEntity.png");
                bmpEmptyEntity = LoadBitmap("EmptyEntity.png");
                bmpBackgroundEntity = LoadBitmap("BackgroundEntity.png");
                bbBorderWindow = new BitmapBorder(LoadBitmap("BorderWindow.png"), false, 14, 14, 14, 14, 60, 14, 14, 60, 14, 14);
                bbObject = new BitmapBorder(LoadBitmap("BorderObject.png"), false, 10, 10, 9, 12, 25, 2, 5, 24, 3, 3);
                bbToolBarLabel = new BitmapBorder(LoadBitmap("ToolbarLabel.png"), true, 10, 10, 9, 10, 25, 9, 12, 25, 10, 10);
                bbGamespace = new BitmapBorder(LoadBitmap("BorderMain2.png"), false, 12, 12, 12, 12, 26, 7, 7, 26, 7, 7);
                bbSelect = new BitmapBorder(LoadBitmap("BorderSelect.png"), false, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10);
                bbIcon16 = new BitmapBorder(LoadBitmap("BorderIcon16.png"), true, 12, 12, 12, 12, 115, 12, 12, 0, 12, 12);
                //bbIcon16 = new BitmapBorder(LoadBitmap("BorderIcon16.png"), true, 10, 10, 10, 10, 28, 10, 10, 4, 10, 10);
                bmpToolbar = LoadBitmap("Toolbar.png");
                bmpToolbarBorder = LoadBitmap("ToolbarBorder.png");
                bmpSeparator = LoadBitmap("Separator.png");
                bmpBandWindowCaption = LoadBitmap("WindowCaption.png");
                bmpBandButtonNormal = LoadBitmap("ButtonNormal.png");
                bmpBandButtonHot = LoadBitmap("ButtonHot.png");
                bmpBandButtonDisabled = LoadBitmap("ButtonDisabled.png");
                bmpBandButtonPressed = LoadBitmap("ButtonPressed.png");
                bmpBandStateCreature = LoadBitmap("BandStateCreature.png");
                bmpBandQuest = LoadBitmap("BandQuest.png");
                bmpMenuInGame = LoadBitmap("MenuInGame.png");

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

                SetStage("Строим замок");

                // Создаем слой игрового поля
                Layers = new List<VisualControl>();
                layerMainMenu = new VisualControl();
                Layers.Add(layerMainMenu);
                currentLayer = layerMainMenu;

                // Лого
                bitmapLogo = new VCBitmap(layerMainMenu, 0, 0, LoadBitmap("Logo.png"));
                bitmapNameGame = new VCBitmap(bitmapLogo, 0, 0, LoadBitmap("NameGame.png"));

                labelVersion = new VCLabel(bitmapLogo, 0, 0, fontSmallC, Color.White, fontSmall.MaxHeightSymbol,
                    $"Сборка {VERSION} от {DATE_VERSION}");
                labelVersion.Width = labelVersion.Font.WidthText(labelVersion.Text);

                // Главное меню
                bmpMainMenu = new VCBitmap(bitmapLogo, 0, 0, LoadBitmap("MenuMain.png"));

                btnTournament = new VCButton(bmpMainMenu, 80, 88, "Турнир");
                btnTournament.Width = bmpMainMenu.Width - 80 - 80;
                btnTournament.Click += BtnTournament_Click;

                btnExitToWindows = new VCButton(bmpMainMenu, 80, bmpMainMenu.Height - 96, "Выход в Windows");
                btnExitToWindows.Width = bmpMainMenu.Width - 80 - 80;
                btnExitToWindows.Click += BtnExitToWindows_Click;

                btnAboutProgram = new VCButton(bmpMainMenu, 80, btnExitToWindows.ShiftY - 40, "О программе");
                btnAboutProgram.Width = bmpMainMenu.Width - 80 - 80;
                btnAboutProgram.Click += BtnAboutProgram_Click;

                btnGamePreferences = new VCButton(bmpMainMenu, 80, btnAboutProgram.ShiftY - 40, "Настройки игры");
                btnGamePreferences.Width = bmpMainMenu.Width - 80 - 80;
                btnGamePreferences.Click += BtnPreferences_Click;

                btnPlayerPreferences = new VCButton(bmpMainMenu, 80, btnGamePreferences.ShiftY - 40, "Настройки игрока");
                btnPlayerPreferences.Width = bmpMainMenu.Width - 80 - 80;
                btnPlayerPreferences.Click += BtnPlayerPreferences_Click;


                // Слой игры
                layerGame = new VisualControl();

                // Создаем панели игроков
                panelPlayers = new VisualControl(layerGame, 0, Config.GridSize);

                CellPlayer pp;
                int nextLeftPanelPlayer = 0;
                for (int i = 0; i < Config.TypeLobbies[0].QuantityPlayers; i++)
                {
                    pp = new CellPlayer(panelPlayers, nextLeftPanelPlayer);
                    nextLeftPanelPlayer = pp.NextLeft();
                }

                panelPlayers.ApplyMaxSize();

                // Полоса игрового тулбара
                bmpPreparedToolbar = new VCBitmap(layerGame, 0, 0, null);

                // Главное игровое поле
                MainControl = new VisualControl(layerGame, 0, 0);
                MainControl.Click += MainControl_Click;

                // Метки с информацией о Королевстве
                labelDay = new VCToolLabel(bmpPreparedToolbar, Config.GridSize, 6, "", GUI_16_DAY);
                labelDay.Click += LabelDay_Click;
                labelDay.ShowHint += LabelDay_ShowHint;
                labelDay.Width = 64;
                labelBuilders = new VCToolLabel(bmpPreparedToolbar, labelDay.NextLeft(), labelDay.ShiftY, "", GUI_16_BUILDER);
                labelBuilders.ShowHint += LabelBuilders_ShowHint;
                labelBuilders.Width = 112;
                labelGold = new VCToolLabel(bmpPreparedToolbar, labelBuilders.NextLeft(), labelDay.ShiftY, "", GUI_16_COFFERS);
                labelGold.ShowHint += LabelGold_ShowHint;
                labelGold.Width = 152;
                labelGreatness = new VCToolLabel(bmpPreparedToolbar, labelGold.NextLeft(), labelDay.ShiftY, "", GUI_16_GREATNESS);
                labelGreatness.ShowHint += LabelGreatness_ShowHint;
                labelGreatness.Width = 152;
                labelHeroes = new VCToolLabel(bmpPreparedToolbar, labelGreatness.NextLeft() + 288, labelDay.ShiftY, "", GUI_16_HEROES);
                labelHeroes.ShowHint += LabelHeroes_ShowHint;
                labelHeroes.Width = 96;
                labelCorruption = new VCToolLabel(bmpPreparedToolbar, labelHeroes.NextLeft(), labelDay.ShiftY, "", GUI_16_CORRUPTION);
                labelCorruption.ShowHint += LabelCorruption_ShowHint;
                labelCorruption.Width = 128;

                labelNamePlayer = new VCLabel(bmpPreparedToolbar, 0, 0, fontMedCaptionC, Color.White, fontMedCaptionC.MaxHeightSymbol, "");
                labelNamePlayer.StringFormat.LineAlignment = StringAlignment.Center;
                labelNamePlayer.Width = 16;

                btnInGameMenu = CreateButton(layerGame, Config.Gui48_Settings, Config.GridSize, Config.GridSize, BtnInGameMenu_Click, BtnInGameMenu_MouseHover);
                btnInGameMenu.HighlightUnderMouse = true;
                btnInGameMenu.ShowBorder = false;
                btnEndTurn = CreateButton(layerGame, Config.Gui48_Hourglass, 0, Config.GridSize, BtnEndTurn_Click, BtnEndTurn_MouseHover);
                btnEndTurn.HighlightUnderMouse = true;
                btnEndTurn.ShowBorder = true;
                panelLairWithFlags = new VisualControl(MainControl, 0, 0);
                panelLairWithFlags.Width = imListObjects48.Size;
                panelLairWithFlags.Height = imListObjects48.Size;

                // Отладочная информация
                vcDebugInfo = new VisualControl();
                labelTimeDrawFrame = new VCLabel(vcDebugInfo, Config.GridSize * 8, Config.GridSize, fontParagraph, Color.White, 16, "");
                labelTimeDrawFrame.StringFormat.Alignment = StringAlignment.Near;
                labelTimeDrawFrame.Width = 300;
                labelLayers = new VCLabel(vcDebugInfo, labelTimeDrawFrame.ShiftX, labelTimeDrawFrame.NextTop(), fontParagraph, Color.White, 16, "Layers");
                labelLayers.StringFormat.Alignment = StringAlignment.Near;
                labelLayers.Width = 300;
                vcDebugInfo.ApplyMaxSize();
                vcDebugInfo.ArrangeControls();

                // Правая панель с героями и меню
                vcRightPanel = new VisualControl(MainControl, 0, 0);

                // Создаем меню
                bitmapMenu = new VCBitmap(vcRightPanel, 0, 0, LoadBitmap("Menu.png"));
                //Debug.Assert(panelHeroInfo.Width >= bitmapMenu.Width);

                CellsMenu = new VCMenuCell[PANEL_MENU_CELLS.Height, PANEL_MENU_CELLS.Width];
                for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                    for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                        CellsMenu[y, x] = new VCMenuCell(bitmapMenu, 22 + (x * (imListObjects48.Size + DISTANCE_BETWEEN_CELLS)), 95 + (y * (imListObjects48.Size + DISTANCE_BETWEEN_CELLS)));

                // Панель со всеми героями
                panelCombatHeroes = new PanelWithPanelEntity(4, false, 12, 12);
                panelCombatHeroes.Click += PanelCombatHeroes_Click;
                vcRightPanel.AddControl(panelCombatHeroes);

                vcRightPanel.Width = Math.Max(bitmapMenu.Width, panelCombatHeroes.Width + Config.GridSize);

                // Панели информации об объектахs
                panelHeroInfo = new PanelHeroInfo(MainControl, Config.GridSize, panelLairWithFlags.NextTop());
                panelHeroInfo.Width = vcRightPanel.Width - Config.GridSize;
                panelHeroInfo.ApplyMaxSize();
                panelHeroInfo.Width = panelHeroInfo.Width + Config.GridSize;
                vcRightPanel.Width = Math.Max(vcRightPanel.Width, panelHeroInfo.Width);
                panelHeroInfo.Width = vcRightPanel.Width;

                panelConstructionInfo = new PanelConstructionInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelConstructionInfo.Width = panelHeroInfo.Width;
                panelConstructionInfo.ApplyMaxSize();
                panelLairInfo = new PanelLairInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelLairInfo.Width = panelHeroInfo.Width;
                panelLairInfo.ApplyMaxSize();
                panelMonsterInfo = new PanelMonsterInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelMonsterInfo.Width = panelHeroInfo.Width;
                panelMonsterInfo.ApplyMaxSize();
                panelEmptyInfo = new VisualControl(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY)
                {
                    Width = panelHeroInfo.Width,
                    Height = panelHeroInfo.Height,
                    ShowBorder = true
                };

                // Страницы игры
                pageControl = new VCPageControl(MainControl, 0, panelLairWithFlags.ShiftY);
                pageControl.PageChanged += PageControl_PageChanged;
                pageResultTurn = pageControl.AddPage(Config.Gui48_ResultDay, "Итоги хода", "Сводка", PageResultTurn_ShowHint);
                pageGuilds = pageControl.AddPage(Config.Gui48_Guilds, "Гильдии и военные сооружения", "В гильдиях нанимаются герои", PageGuilds_ShowHint);
                pageEconomicConstructions = pageControl.AddPage(Config.Gui48_Economy, "Экономические строения", "Надежная экономика - залог победы", PageEconomicConstructions_ShowHint);
                pageTemples = pageControl.AddPage(Config.Gui48_Temple, "Храмы", "Храмы позволяют нанимать самых сильных героев", PageTemples_ShowHint);
                pageHeroes = pageControl.AddPage(Config.Gui48_Heroes, "Герои", "Здесь можно посмотреть своих героев", PageHeroes_ShowHint);
                pageLairs = pageControl.AddPage(Config.Gui48_Map, "Окрестности", "В окрестностях замка водятся различные монстры", PageLairs_ShowHint);
                pageTournament = pageControl.AddPage(Config.Gui48_Tournament, "Турнир", "Здесь можно увидеть положение всех игроков на турнире", PageTournament_ShowHint);

                panelNeighborhood = new VisualControl(pageControl, 0, 0);
                panelNeighborhood.Visible = false;

                DrawPageConstructions();
                DrawHeroes();
                DrawWarehouse();
                panelLairs = new PanelConstruction[Config.TypeLobbies[0].LairsHeight, Config.TypeLobbies[0].LairsWidth];
                DrawPageLair();
                DrawPageTournament();

                // Вычисляем максимальный размер страниц
                pageControl.ApplyMaxSize();
                pageControl.ShiftX = panelEmptyInfo.NextLeft();

                vcRightPanel.ShiftX = pageControl.NextLeft();
                vcRightPanel.ShiftY = panelLairWithFlags.NextTop();

                //
                Debug.Assert(panelConstructionInfo.Height > 0);
                Debug.Assert(panelLairInfo.Height > 0);
                Debug.Assert(panelHeroInfo.Height > 0);
                Debug.Assert(panelMonsterInfo.Height > 0);

                int maxHeightPanelInfo = Math.Max(panelConstructionInfo.Height, panelLairInfo.Height);
                maxHeightPanelInfo = Math.Max(panelHeroInfo.Height, maxHeightPanelInfo);
                maxHeightPanelInfo = Math.Max(panelMonsterInfo.Height, maxHeightPanelInfo);
                int maxHeightControls = Math.Max(pageControl.Height, maxHeightPanelInfo);

                // Все контролы созданы, устанавливаем размеры bitmapMenu
                MainControl.Width = vcRightPanel.ShiftX + vcRightPanel.Width;
                bmpPreparedToolbar.Bitmap = PrepareToolbar();
                bmpPreparedToolbar.ShiftY = panelPlayers.NextTop();
                MainControl.ShiftY = bmpPreparedToolbar.NextTop();

                MainControl.Height = pageResultTurn.ShiftX + maxHeightControls + Config.GridSize;

                // Теперь когда известна ширина окна, можно создавать картинку тулбара
                labelNamePlayer.Height = bmpPreparedToolbar.Height;
                panelPlayers.ShiftX = (MainControl.Width - panelPlayers.Width) / 2;
                vcRightPanel.Height = MainControl.Height - panelLairWithFlags.NextTop();

                sizeGamespace = new Size(MainControl.Width, MainControl.ShiftY + MainControl.Height);
                Width = Width - ClientSize.Width + sizeGamespace.Width;
                Height = Height - ClientSize.Height + sizeGamespace.Height;
                layerGame.Width = sizeGamespace.Width;
                layerGame.Height = sizeGamespace.Height;

                bitmapMenu.ShiftX = vcRightPanel.Width - bitmapMenu.Width;
                bitmapMenu.ShiftY = vcRightPanel.Height - bitmapMenu.Height;
                panelCombatHeroes.ShiftX = vcRightPanel.Width - panelCombatHeroes.Width - Config.GridSize;

                panelConstructionInfo.Height = MainControl.Height - panelConstructionInfo.ShiftY - Config.GridSize;
                panelLairInfo.Height = panelConstructionInfo.Height;
                panelHeroInfo.Height = panelConstructionInfo.Height;
                panelMonsterInfo.Height = panelConstructionInfo.Height;
                panelEmptyInfo.Height = panelConstructionInfo.Height;

                btnEndTurn.ShiftX = btnEndTurn.Parent.Width - btnEndTurn.Width - Config.GridSize;

                bmpPreparedToolbar.ShiftX = 0;
                MainControl.ShiftX = 0;

                layerMainMenu.Width = sizeGamespace.Width;
                layerMainMenu.Height = sizeGamespace.Height;
                bitmapLogo.ShiftX = (layerMainMenu.Width - bitmapLogo.Width) / 2;
                bitmapLogo.ShiftY = (layerMainMenu.Height - bitmapLogo.Height) / 2;
                bitmapNameGame.ShiftX = (layerMainMenu.Width - bitmapNameGame.Width) / 2;
                bitmapNameGame.ShiftY = 32;//(bitmapLogo.ShiftY - bitmapNameGame.Height) / 2;
                labelVersion.ShiftX = sizeGamespace.Width - labelVersion.Width - Config.GridSize;
                labelVersion.ShiftY = sizeGamespace.Height - labelVersion.Height - Config.GridSize;
                bmpMainMenu.ShiftX = sizeGamespace.Width - bmpMainMenu.Width - Config.GridSize;
                bmpMainMenu.ShiftY = (sizeGamespace.Height - bmpMainMenu.Height) / 2 - (Config.GridSize * 1);

                layerMainMenu.ArrangeControls();
                layerGame.ArrangeControls();

                formHint = new PanelHint();
                //
                timerHover = new Timer()
                {
                    Interval = SystemInformation.MouseHoverTime,
                    Enabled = false
                };
                timerHover.Tick += TimerHover_Tick;

                SetStage("Прибираем после строителей");

                //
                pageControl.ActivatePage(pageResultTurn);
                ShowNamePlayer(pageControl.CurrentPage.Caption);

                //
                playerMusic = new PlayerMusic(dirResources, Settings);
                mpSoundSelect = new System.Windows.Media.MediaPlayer();
                mpSelectButton = new System.Windows.Media.MediaPlayer();
                mpSelectButton.Open(new Uri(dirResources + @"Sound\Interface\Button\SelectButton.wav"));
                mpPushButton = new System.Windows.Media.MediaPlayer();
                mpPushButton.Open(new Uri(dirResources + @"Sound\Interface\Button\PushButton.wav"));
                mpConstructionComplete = new System.Windows.Media.MediaPlayer();
                mpConstructionComplete.Open(new Uri(dirResources + @"Sound\Interface\Construction\ConstructionComplete.wav"));

                //formHint = new FormHint(ilGui16, ilParameters);

                splashForm.Dispose();

                // Курсор
                CustomCursor.CreateCursor(dirResources + @"Cursor\Cursor_simple.png");
                Cursor = CustomCursor.GetCursor();

                //
                Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);

                //MediaElement me = new MediaElement()
                //me.Parent = this;

                playerMusic.PlayMainTheme();

                //ImportNames();// Однократная операция

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

        private void LabelCorruption_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Воровство");
            formHint.AddStep5Description($"Всего процент: {curAppliedPlayer.PercentCorruption}" + Environment.NewLine
                + $"Изменение за день: {curAppliedPlayer.ChangeCorruption}");
        }

        private void Settings_PlayMusicChanged(object sender, EventArgs e)
        {
            playerMusic.TogglePlayMusic();
        }

        private void LabelHeroes_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Герои");
                formHint.AddStep5Description($"Нанято героев: {curAppliedPlayer.CombatHeroes.Count}" + Environment.NewLine
                + $"Максимум героев: {curAppliedPlayer.Lobby.TypeLobby.MaxHeroes}");
        }

        private void PageControl_PageChanged(object sender, EventArgs e)
        {
            panelNeighborhood.Visible = pageControl.CurrentPage == pageLairs;

            if (currentLayer == layerGame)
            {
                ShowNamePlayer(pageControl.CurrentPage.Caption);
                //if (winAdvice is null)
                //    winAdvice = new WindowAdvice();
                //winAdvice.ShowAdvice(pageControl.CurrentPage.Advice);
            }
        }

        private void LabelBuilders_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Строители");
            formHint.AddStep5Description("Всего строителей: " + curAppliedPlayer.Builders.ToString()
                + Environment.NewLine + "Свободно строителей: " + curAppliedPlayer.FreeBuilders.ToString()
                + Environment.NewLine + "Строителей на следующем ходу: " + curAppliedPlayer.BuildersAtNextDay.ToString());
        }

        private void PanelCombatHeroes_Click(object sender, EventArgs e)
        {
            SelectPlayerObject(null);
        }

        private void MainControl_Click(object sender, EventArgs e)
        {
            SelectPlayerObject(null);
        }

        private void PageTemples_ShowHint(object sender, EventArgs e)
        {
            formHint.AddSimpleHint("Храмы");
        }

        private void LabelGreatness_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Уровень величия: " + curAppliedPlayer.LevelGreatness.ToString());
            formHint.AddStep5Description($"Очков набрано: {curAppliedPlayer.PointGreatness} из {curAppliedPlayer.PointGreatnessForNextLevel}"
                    + Environment.NewLine
                    + "До следующего уровня: " + (curAppliedPlayer.PointGreatnessForNextLevel - curAppliedPlayer.PointGreatness).ToString()
                    + Environment.NewLine
                    + "Прибавление в день: " + curAppliedPlayer.PointGreatnessPerDay().ToString());
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
            formHint.AddStep2Header("Герои");
            formHint.AddStep5Description("Нанято героев: " + lobby.CurrentPlayer.CombatHeroes.Count.ToString());
        }

        private void PageEconomicConstructions_ShowHint(object sender, EventArgs e)
        {
            formHint.AddSimpleHint("Экономические строения");
        }
        
        private void PageResultTurn_ShowHint(object sender, EventArgs e)
        {
            formHint.AddSimpleHint("Итоги хода");
        }

        private void PageGuilds_ShowHint(object sender, EventArgs e)
        {
            formHint.AddSimpleHint("Гильдии и военные сооружения");
        }

        private void PageLairs_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Окрестности");
            formHint.AddStep5Description($"Разведано мест: {lobby.CurrentPlayer.LairsScouted}/{lobby.CurrentPlayer.LairsShowed}");
        }

        private void PageTournament_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Турнир");
            formHint.AddStep5Description(lobby.DaysLeftForBattle > 0 ? "Битва с другим игроком начнется через " + lobby.DaysLeftForBattle.ToString() + " дн." : 
                    curAppliedPlayer.SkipBattle ? "Битва пропускается" : "Сегодня битва с другим игроком");
        }

        private void LabelGold_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("Казна");
            formHint.AddStep5Description("Количество золота в казне и постоянный доход в день");
        }

        private void LabelDay_ShowHint(object sender, EventArgs e)
        {
            formHint.AddStep2Header("День игры");
            formHint.AddStep5Description("День игры: " + lobby.Day.ToString());
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

            //Debug.Assert(ClientRectangle.Width == MainConfig.ScreenMinSize.Width);
            //Debug.Assert(ClientRectangle.Height == MainConfig.ScreenMinSize.Height);
            //Debug.Assert(bitmapLogo.Width == ClientRectangle.Width);
            //Debug.Assert(bitmapLogo.Height == ClientRectangle.Height);

            ApplyFullScreen(true);
            gameStarted = true;

            ShowFrame(true);
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

            if ((bmpRenderClientArea == null) || !ClientSize.Equals(bmpRenderClientArea.Size))
            {
                if (Settings.FullScreenMode)
                {
                    topLeftFrame = new Point((ClientSize.Width - sizeGamespace.Width) / 2, (ClientSize.Height - sizeGamespace.Height) / 2);

                    Debug.Assert(topLeftFrame.X >= 0);
                    Debug.Assert(topLeftFrame.Y >= 0);
                }
                else
                {
                    topLeftFrame = new Point(0, 0);
                    Size = new Size(Width - ClientSize.Width + sizeGamespace.Width, Height - ClientSize.Height + sizeGamespace.Height);
                }
            }

            if (gameStarted)
                ShowFrame(true);
        }

        internal void ApplyFullScreen(bool force)
        {
            if (force || (MaximizeBox != Settings.FullScreenMode))
            {
                // Так как после перестройки экрана контрол оказывается в другом месте,
                // То во избежание повторного входа в него выходим из него
                if (controlWithHint != null)
                    ControlForHintLeave();
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

        private void BtnInGameMenu_Click(object sender, EventArgs e)
        {
            ShowInGameMenu();
        }

        private void BtnInGameMenu_MouseHover(object sender, EventArgs e)
        {
            formHint.Clear();
            formHint.AddStep2Header("Меню");
            formHint.AddStep5Description("Показать внутриигровое меню");
            formHint.DrawHint(btnInGameMenu);
        }

        private void BtnEndTurn_Click(object sender, EventArgs e)
        {
            StopSoundSelect();
            formHint.HideHint();
            timerHover.Stop();

            curAppliedPlayer.EndTurn();
        }

        private void BtnEndTurn_MouseHover(object sender, EventArgs e)
        {
            formHint.Clear();
            formHint.AddStep2Header("Конец хода");
            formHint.AddStep5Description("Завершение хода");
            formHint.DrawHint(btnEndTurn);
        }

        internal void ShowWindowPreferences()
        {
            WindowPreferences w = new WindowPreferences();
            w.ApplySettings(Settings);
            if (w.ShowDialog() == DialogAction.OK)
            {
                /*if (Settings.NamePlayer != lobby.CurrentPlayer.Name)
                {
                    lobby.CurrentPlayer.Name = Settings.NamePlayer;
                }*/

                ApplyFullScreen(false);
            }
        }

        internal void AddLayer(VisualControl vc, string name)
        {
            Debug.Assert(Layers.Count <= 5);
            Debug.Assert(currentLayer.Controls.Count > 0);

            // Так как переходим к новом слою, выходим мышью из контрола текущего слоя
            // При клике есть только controlClicked, его и смотрим
            // В некоторых случаях его может не быть - например, при выходе по Alt+F4 или крестик на окне
            controlClicked?.MouseLeave();
            controlClicked = null;

            Layers.Add(vc);
            currentLayer = vc;
        }

        internal void ExchangeLayer(VisualControl oldLayer, VisualControl newLayer)
        {
            Debug.Assert(Layers.Count == 1);
            Debug.Assert(Layers[0] == oldLayer);
            Debug.Assert(currentLayer == oldLayer);
            Layers[0] = newLayer;
            currentLayer = newLayer;
        }

        internal void RemoveLayer(VisualControl vl)
        {
            Debug.Assert(Layers.Count > 1);
            Debug.Assert(Layers[Layers.Count - 1] == vl);

            Layers.Remove(vl);
            currentLayer = Layers[Layers.Count - 1];
            formHint.HideHint();// Если слой убирается, убираем подсказку, если она там была

            SetNeedRedrawFrame();
        }

        internal void ShowWindowAboutProgram()
        {
            WindowAboutProgram w = new WindowAboutProgram();
            w.ShowDialog();
            w.Dispose();
            ShowFrame(true);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (ProgramState == ProgramState.NeedQuit)
            {
                Debug.Assert(lobby is null);
                return;
            }

            if (ProgramState == ProgramState.Started)
            {
                ProgramState = ProgramState.ConfirmQuit;
                WindowConfirmExit f = new WindowConfirmExit();
                ProgramState = f.ShowDialog() == DialogAction.OK ? ProgramState.NeedQuit : ProgramState.Started;
                e.Cancel = ProgramState == ProgramState.Started;

                ShowFrame(true);

                if (!e.Cancel)
                    if (!(lobby is null))
                        EndLobby();
            }
            else
                e.Cancel = true;
        }

        internal void LayerChanged()
        {
            ShowFrame(true);
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

        internal void ShowCurrentPlayerLobby()
        {
            if (lobby.CurrentPlayer == null)
            {
                MainControl.Visible = false;
            }
            else
            {
                if (lobby.CurrentPlayer.GetTypePlayer() == TypePlayer.Human)
                {
                    while (pageResultTurn.Page.Controls.Count > 0)
                        pageResultTurn.Page.RemoveControl(pageResultTurn.Page.Controls[0]);

                    btnEndTurn.ImageIsEnabled = true;
                    labelDay.Visible = true;
                    labelBuilders.Visible = true;
                    labelGold.Visible = true;
                    labelGreatness.Visible = true;
                    labelHeroes.Visible = true;
                    labelCorruption.Visible = true;
                    MainControl.Visible = true;
                    ShowDataPlayer();
                }
                else
                {
                    btnEndTurn.ImageIsEnabled = false;
                    labelDay.Visible = false;
                    labelBuilders.Visible = false;
                    labelGold.Visible = false;
                    labelGreatness.Visible = false;
                    labelHeroes.Visible = false;
                    labelCorruption.Visible = false;
                    MainControl.Visible = false;
                    foreach (VCImageLose il in listBtnLoses)
                        il.Visible = false;

                    ShowNamePlayer(lobby.CurrentPlayer.Descriptor.Name);
                }
            }

            ShowFrame(true);
        }

        internal void ShowNamePlayer(string name)
        {
            Debug.Assert(name.Length > 0);

            if (labelNamePlayer.Text != name)
            {
                labelNamePlayer.Text = name;
                labelNamePlayer.Width = labelNamePlayer.Font.WidthText(labelNamePlayer.Text);
                labelNamePlayer.ShiftX = (bmpPreparedToolbar.Width - labelNamePlayer.Width) / 2;
                bmpPreparedToolbar.ArrangeControl(labelNamePlayer);
            }
        }

        internal void ShowDataPlayer()
        {
            Debug.Assert(lobby.CurrentPlayer.GetTypePlayer() == TypePlayer.Human);

            labelDay.Text = lobby.Day.ToString();

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayer)
            {
                curAppliedPlayer = lobby.CurrentPlayer;
            }

            ShowLobby();

            LairsWithFlagChanged();
            LosesChanged();
            UpdateListHeroes();
            ShowWarehouse();
            AdjustPageTournament();
        }

        internal void UpdateListHeroes()
        {
            //List<ICell> listHeroes = new List<ICell>();
            //for (int y = 0; y < lobby.CurrentPlayer.CellHeroes.GetLength(0); y++)
            //    for (int x = 0; x < lobby.CurrentPlayer.CellHeroes.GetLength(1); x++)
            //        listHeroes.Add(lobby.CurrentPlayer.CellHeroes[y, x]);

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
            foreach (Construction pb in lobby.CurrentPlayer.Constructions)
            {
                if (pb.TypeConstruction.IsInternalConstruction)
                    pb.TypeConstruction.Panel.Entity = pb;
            }

            // Показываем логова
            UpdateNeighborhood();

            // Показываем героев
            ShowEvents();
            AdjustPanelLoses();
            AdjustPanelLairsWithFlags();
            ListHeroesChanged();
        }

        internal void UpdateNeighborhood()
        {
            for (int y = 0; y < panelLairs.GetLength(0); y++)
                for (int x = 0; x < panelLairs.GetLength(1); x++)
                {
                    panelLairs[y, x].Entity = lobby.CurrentPlayer.Lairs[currentNeighborhood, y, x];
                    panelLairs[y, x].Visible = !(panelLairs[y, x].Entity is null);
                }
        }

        private void ShowEvents()
        {
            Debug.Assert(curAppliedPlayer is PlayerHuman);

            PlayerHuman p = (PlayerHuman)curAppliedPlayer;
            int top = 0;

            foreach (VCEvent e in p.ListEvents)
            {
                e.SetParent(pageResultTurn.Page);
                e.ShiftX = 0;
                e.ShiftY = top;

                top = e.NextTop();
            }

            pageResultTurn.Page.ArrangeControls();
        }

        private void DrawPageConstructions()
        {
            // Создаем массив из страниц, линий и позиций
            PanelConstruction[,,] panels = new PanelConstruction[3, Config.ConstructionMaxLines, Config.ConstructionMaxPos];

            // Проходим по каждому зданию, создавая ему панель
            VisualControl parent;
            foreach (DescriptorConstruction tck in Config.Constructions)
            {
                if (tck.IsInternalConstruction)
                {
                    switch (tck.Page)
                    {
                        case ConstructionPage.Guild:
                            parent = pageGuilds.Page;
                            break;
                        case ConstructionPage.Economic:
                            parent = pageEconomicConstructions.Page;
                            break;
                        case ConstructionPage.Temple:
                            parent = pageTemples.Page;
                            break;
                        default:
                            throw new Exception("Неизвестная страница " + tck.Page.ToString());
                    }

                    Debug.Assert(panels[(int)tck.Page, tck.CoordInPage.Y, tck.CoordInPage.X] == null);

                    tck.Panel = new PanelConstruction(parent, 0, 0);
                    tck.Panel.ShiftX = (tck.Panel.Width + Config.GridSize) * (tck.CoordInPage.X);
                    tck.Panel.ShiftY = (tck.Panel.Height + Config.GridSize) * (tck.CoordInPage.Y);
                    panels[(int)tck.Page, tck.CoordInPage.Y, tck.CoordInPage.X] = tck.Panel;
                }
            }
        }

        private void DrawPageLair()
        {
            int top = 0;
            int left;
            int height = 0;

            for (int y = 0; y < Config.TypeLobbies[0].LairsHeight; y++)
            {
                left = 0;
                for (int x = 0; x < Config.TypeLobbies[0].LairsWidth; x++)
                {
                    Debug.Assert(panelLairs[y, x] == null);
                    panelLairs[y, x] = new PanelConstruction(pageLairs.Page, left, top);

                    left += panelLairs[y, x].Width + Config.GridSize;
                    height = panelLairs[y, x].Height;
                }

                top += height + Config.GridSize;
            }
        }

        private void DrawHeroes()
        {
            panelHeroes = new PanelWithPanelEntity(Config.HeroRows);
            pageHeroes.Page.AddControl(panelHeroes);
            panelHeroes.ShiftY = 0;

            List<Entity> list = new List<Entity>();
            for (int x = 0; x < Config.HeroRows * Config.HeroInRow; x++)
                list.Add(null);

            panelHeroes.ApplyList(list);
            panelHeroes.Height = panelHeroes.MaxSize().Height;
        }

        private void DrawPageTournament()
        {
            //private readonly VCCell[] pageTournamentPlayers;
            // Ячейки игроков
        }

        private void AdjustPageTournament()
        {
            if (pageTournamentPlayers is null)
            {
                int nextTop = 56;
                pageTournamentPlayers = new VCCell[lobby.Players.Length];
                for (int i = 0; i < pageTournamentPlayers.Length; i++)
                {
                    pageTournamentPlayers[i] = new VCCell(pageTournament.Page, 0, nextTop);
                    nextTop += 56;
                }

                pageTournament.Page.ArrangeControls();
            }

            foreach (VCResultRound rr in listResultRound)
                rr.Visible = false;

            while (listResultRound.Count < lobby.BattlesPlayers.Count)
            {
                listResultRound.Add(new VCResultRound(pageTournament.Page, pageTournamentPlayers[0].NextLeft() + (listResultRound.Count * 56), 0, lobby.Players.Length));
                pageTournament.Page.ArrangeControls();
            }

            foreach (Player lp in lobby.Players.OrderBy(lp => lp.PositionInLobby))
            {
                pageTournamentPlayers[lp.PositionInLobby - 1].Entity =  lp;
            }

            for (int i = 0; i < lobby.BattlesPlayers.Count; i++)
            {
                listResultRound[i].ShowPlayers(lobby.Players, lobby.BattlesPlayers[i]);
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
            panelWarehouse.ApplyList(lobby.CurrentPlayer.Warehouse.ToList<Entity>());
        }

        internal void ActivatePageResultTurn()
        {
            pageControl.ActivatePage(pageResultTurn);
        }

        internal void ActivatePageLairs()
        {
            pageControl.ActivatePage(pageLairs);
        }

        internal void StartNewLobby()
        {
            Debug.Assert(lobby == null);

            lobby = new Lobby(Config.TypeLobbies[0]);

            for (int i = 0; i < panelPlayers.Controls.Count; i++)
            {
                Debug.Assert(panelPlayers.Controls[i] is CellPlayer);
                ((CellPlayer)panelPlayers.Controls[i]).Entity = lobby.Players[i];
            }

            if (currentLayer != layerGame)
            {
                playerMusic.PlayMusic();
                ExchangeLayer(layerMainMenu, layerGame);
            }

            AdjustNeighborhood();
            listButtonsLayers[0].DoClick();
            pageControl.ActivatePage(pageResultTurn);
            ShowCurrentPlayerLobby();

            lobby.Start();
        }

        internal void RestartLobby()
        {
            Debug.Assert(lobby != null);
            lobby.ExitFromLobby();
            lobby = null;

            StartNewLobby();
        }

        internal void EndLobby()
        {
            Debug.Assert(lobby != null);
            lobby.ExitFromLobby();

            ReturnFromLobby();
        }

        internal void ReturnFromLobby()
        {
            Debug.Assert(lobby != null);
            lobby = null;

            if (ProgramState != ProgramState.NeedQuit)
            {
                ExchangeLayer(layerGame, layerMainMenu);
                ShowFrame(true);
                playerMusic.PlayMainTheme();
            }
        }

        internal void SetNeedRedrawFrame()
        {
            needRedrawFrame = true;
        }

        internal void UpdateMenu()
        {
            ClearMenu();

            if (selectedPlayerObject != null)
                selectedPlayerObject.MakeMenu(CellsMenu);

            for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                    if (!CellsMenu[y, x].Used)
                        CellsMenu[y, x].Research = null;
        }

        internal void ClearMenu()
        {
            for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                    CellsMenu[y, x].Used = false;
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
            Debug.Assert(ClientSize.Equals(bmpRenderClientArea.Size));

            base.OnPaint(e);
            
            if (needRepaintFrame)
            {
                DrawFrame();
                needRepaintFrame = false;
            }

            e.Graphics.DrawImage(bmpRenderClientArea, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        internal void ShowFrame(bool force)
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
            if (inDrawFrame)
                return;
            //Debug.Assert(inDrawFrame == false);

            inDrawFrame = true;

            if (debugMode)
            {
                startDebugAction = DateTime.Now;
            }

            // Готовим фон, если его надо поменять
            if ((bmpRenderClientArea == null) || (bmpRenderClientArea == null) || !bmpRenderFrame.Size.Equals(sizeGamespace) || !bmpRenderClientArea.Size.Equals(ClientSize))
                PrepareBackground();

            // Рисуем фон
            gfxRenderFrame.CompositingMode = CompositingMode.SourceCopy;
            gfxRenderFrame.DrawImageUnscaled(bmpRenderBackgroundFrame, 0, 0);
            gfxRenderFrame.CompositingMode = CompositingMode.SourceOver;

            //
            if ((Layers[0] == layerGame) && (lobby.CurrentPlayer != null) && MainControl.Visible)
            {
                labelGold.Text = lobby.CurrentPlayer.Gold.ToString() + " (+" + lobby.CurrentPlayer.Income().ToString() + ")";
                labelBuilders.Text = $"{curAppliedPlayer.FreeBuilders}/{curAppliedPlayer.Builders} (+{curAppliedPlayer.BuildersAtNextDay})";
                labelGreatness.Text = curAppliedPlayer.LevelGreatness.ToString()
                    + " (+" + curAppliedPlayer.PointGreatnessPerDay().ToString() + ")"
                    + ": " + curAppliedPlayer.PointGreatness.ToString() + "/"
                    + curAppliedPlayer.PointGreatnessForNextLevel.ToString();
                labelHeroes.Text = curAppliedPlayer.CombatHeroes.Count.ToString() + "/" + curAppliedPlayer.Lobby.TypeLobby.MaxHeroes.ToString();
                labelCorruption.Text = $"{curAppliedPlayer.PercentCorruption}% ({(curAppliedPlayer.ChangeCorruption > 0 ? "+" : "")}{curAppliedPlayer.ChangeCorruption}%)";

                pageTournament.Text = lobby.DaysLeftForBattle > 0 ? lobby.DaysLeftForBattle.ToString() + " д." :
                        curAppliedPlayer.SkipBattle ? "Проп." : "Битва";
            }

            // Рисуем контролы
            gfxRenderFrame.CompositingMode = CompositingMode.SourceOver;

            foreach (VisualControl vc in Layers)
            {
                Debug.Assert(vc.Visible);                
                vc.Paint(gfxRenderFrame);
            }

            // Рисуем подсказку поверх всех окон
            // Здесь исправляется баг - если после клика надо заново отрисовать подсказку, то во время рисования подсказки
            // контрол может оставаться видимым, а после отрисовки кадра - невидимым, так как в некоторых классах
            // настройка происходит в методе Draw
            if (formHint.Visible && !(controlWithHint is null) && controlWithHint.Visible)
            {
                formHint.Paint(gfxRenderFrame);
            }

            if (debugMode)
            {
                if (controlWithHint != null)
                    gfxRenderFrame.DrawRectangle(penDebugBorder, controlWithHint.Rectangle);

                durationDrawFrame = DateTime.Now - startDebugAction;
                labelTimeDrawFrame.Text = $"Draw frame: {durationDrawFrame.TotalMilliseconds}";
                vcDebugInfo.Paint(gfxRenderFrame);
            }

            gfxRenderClientArea.CompositingMode = CompositingMode.SourceCopy;
            gfxRenderClientArea.DrawImage(bmpRenderFrame, topLeftFrame.X, topLeftFrame.Y, sizeGamespace.Width, sizeGamespace.Height);

            //
            inDrawFrame = false;

            void PrepareBackground()
            {
                // Переформировываем картинку фона клиентской области
                if ((bmpRenderClientArea == null) || !bmpRenderClientArea.Size.Equals(GuiUtils.MakeBackground(ClientSize)))
                {
                    bmpRenderClientArea?.Dispose();

                    if (Settings.FullScreenMode)
                    {
                        bmpRenderClientArea = GuiUtils.MakeBackground(ClientSize);
                        Graphics g = Graphics.FromImage(bmpRenderClientArea);
                        bbGamespace.DrawBorder(g, topLeftFrame.X - 7, topLeftFrame.Y - 7, sizeGamespace.Width + 14, sizeGamespace.Height + 14);
                        g.Dispose();
                    }
                    else
                        bmpRenderClientArea = GuiUtils.MakeBackground(sizeGamespace);

                    gfxRenderClientArea?.Dispose();
                    gfxRenderClientArea = Graphics.FromImage(bmpRenderClientArea);
                }

                // Переформировываем картинку кадра
                if ((bmpRenderFrame == null) || !bmpRenderFrame.Equals(GuiUtils.MakeBackground(sizeGamespace)))
                {
                    bmpRenderFrame?.Dispose();
                    bmpRenderFrame = new Bitmap(sizeGamespace.Width, sizeGamespace.Height);

                    gfxRenderFrame?.Dispose();
                    gfxRenderFrame = Graphics.FromImage(bmpRenderFrame);

                    bmpRenderBackgroundFrame = GuiUtils.MakeBackground(sizeGamespace);
                }
            }
        }

        internal VCIconButton48 CreateButton(VisualControl parent, int imageIndex, int left, int top, EventHandler click, EventHandler showHint)
        {
            VCIconButton48 b = new VCIconButton48(parent, left, top, imageIndex);
            b.Click += click;
            b.ShowHint += showHint;

            return b;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            TreatMouseMove(e.Button == MouseButtons.Left);
        }

        private void ShowInGameMenu()
        {
            WindowMenuInGame w = new WindowMenuInGame();
            DialogAction dr = w.ShowDialog();
            switch (dr)
            {
                case DialogAction.None:
                    break;
                case DialogAction.Quit:
                    ProgramState = ProgramState.NeedQuit;
                    if (!(lobby is null))                        
                        Program.formMain.EndLobby();
                    Close();                        
                    break;
                case DialogAction.MainMenu:
                    EndLobby();
                    break;
                case DialogAction.RestartGame:
                    RestartLobby();
                    break;
                default:
                    throw new Exception($"Неизвестное действие: {dr}.");
            }

            if (ProgramState != ProgramState.NeedQuit)
                ShowFrame(true);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            currentLayer.KeyPress(e);
            ShowFrame(false);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if ((e.KeyCode == Keys.Escape) && (currentLayer == layerGame))
            {
                ShowInGameMenu();
            }
            else
            {
                currentLayer.KeyUp(e);
                ShowFrame(false);
            }
        }        

        private VisualControl ControlUnderMouse()
        {
            return currentLayer.GetControl(mousePos.X, mousePos.Y);
        }

        private void UpdateMousePos()
        {
            mousePos = PointToClient(Cursor.Position);
            mousePos.X -= topLeftFrame.X;
            mousePos.Y -= topLeftFrame.Y;
        }

        private void TreatMouseMove(bool leftDown)
        {
            Point oldMousePos = mousePos;
            UpdateMousePos();

            if (!mousePos.Equals(oldMousePos))
            {
                UpdateCurrentControl(leftDown);
            }
        }

        private void UpdateCurrentControl(bool leftDown)
        {
            VisualControl curControl = ControlUnderMouse();

            if (curControl == null)
            {
                timerHover.Stop();
                ControlForHintLeave();
            }
            else if (curControl == controlWithHint)
            {
                /*if (hintShowed)
                {
                    timerHover.Stop();
                    formHint.HideHint();
                }
                else
                {
                    // Если над контролом водят мышкой, отсчет времени начинаем только после остановки
                    timerHover.Stop();
                    timerHover.Start();
                }*/
            }
            else
            {
                // Если при переходе на новый контрол у него так же есть подсказка, просто перерисовываем текст, не скрываем её
                curControl.DoShowHint();
                if ((controlWithHint != null) && formHint.ExistHint)
                {
                    controlWithHint.MouseLeave();
                    controlWithHint = curControl;
                    controlWithHint.MouseEnter(leftDown);
                    formHint.Visible = true;
                }
                else
                {
                    ControlForHintLeave();
                    controlWithHint = curControl;
                    controlWithHint.MouseEnter(leftDown);
                    timerHover.Start();
                }

                SetNeedRedrawFrame();
            }

            ShowFrame(false);
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
                if (!(controlWithHint is null))
                {
                    Debug.Assert(controlWithHint.Visible);
                    controlWithHint.MouseDown();
                }

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
                    Debug.Assert(controlWithHint.Visible);
                    controlWithHint.MouseUp();
                    // При клике происходит перерисовка кадра, и текущий элемент может стать уже невидимым
                    // Но он будет все равно считаться активным, так как прописан в controlWithHint
                    // Поэтому перед кликом убираем его
                    controlClicked = controlWithHint;
                    controlWithHint = null;
                    controlClicked.DoClick();

                    // Во время нажатия кнопки мог произойти выход из программы
                    if (ProgramState == ProgramState.NeedQuit)
                    {
                        if (!(lobby is null))
                            EndLobby();
                        Close();
                    }
                    if (IsDisposed)
                        return;

                    // Если был клик на ячейке меню, обновляем меню, так как меняется список исследований и как следствие подсказки
                    // Так как ячейка может быть невидимой, обновляем меню перед проверкой, какой контрол сейчас под мышкой
                    if (controlClicked is VCMenuCell)
                        UpdateMenu();

                    // Смотрим какой контрол под мышкой сейчас. Если тот же самый, восстанавливаем его
                    // Перед этим актуализируем позицию курсора. Она могла поменяться, если игрок вызывал другое окно
                    UpdateMousePos();
                    VisualControl curControl = ControlUnderMouse();
                    if ((curControl != null) && (curControl == controlClicked))
                        controlWithHint = controlClicked;
                    else
                    {
                        controlWithHint = controlClicked;
                        ControlForHintLeave();// Контрол уже другой, отменяем подсказку

                        // Если сейчас есть новый контрол, входим в него мышью и стартуем таймер подсказки
                        // Когда закрывается слой (FormConfirmExit), в новый контрол происходит вход два раза
                        // Поэтому проверяем - если уже вошли, то повторяться не надо
                        controlWithHint = curControl;
                        if ((controlWithHint != null) && !controlWithHint.MouseOver)
                        {
                            controlWithHint.MouseEnter(false);
                            timerHover.Start();
                        }
                    }
                    controlClicked = null;

                    if (formHint.Visible)
                    {
                        controlWithHint.DoShowHint();

                        // После клика может оказаться, что подсказки нет (контрол скрыт, например)
                        if (!formHint.ExistHint)
                            formHint.HideHint();
                    }

                    ShowFrame(false);

                    // Если после отрисовки кадра контрола стал невидимым, выходим из него
                    if (!(controlWithHint is null) && !controlWithHint.Visible)
                    {
                        ControlForHintLeave();
                    }

                    /*if (formHint.Visible)
                    {
                        ControlForHintLeave();
                        mousePos = new Point(0, 0);
                        TreatMouseMove(false);
                    }*/
                }
            }
        }

        private void TimerHover_Tick(object sender, EventArgs e)
        {
            if (controlWithHint != null)
            {
                Debug.Assert(controlWithHint.Visible);
                /*if (controlWithHint.VisualLayer != currentLayer)
                {
                    timerHover.Stop();
                    MessageBox.Show($"{controlWithHint.VisualLayer.Name}, {currentLayer.Name}");
                }*/

                timerHover.Stop();
                controlWithHint.DoShowHint();

                if (formHint.ExistHint)
                {
                    Debug.Assert(controlWithHint.Visible);

                    formHint.Visible = true;
                    needRepaintFrame = true;
                    Invalidate(true);

                    Debug.Assert(controlWithHint.Visible);
                }
            }
        }

        internal void StopShowHint()
        {
            timerHover.Stop();
        }

        internal void NeedRedrawFrame()
        {
            needRedrawFrame = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            
            //ControlForHintLeave();
            //if (WindowState != FormWindowState.Minimized)
            //    ShowFrame(true);
        }

        internal int TreatImageIndex(int imageIndex, BattleParticipant p)
        {
            return imageIndex != IMAGE_INDEX_CURRENT_AVATAR ? imageIndex : p.GetImageIndexAvatar();
        }

        internal void LairsWithFlagChanged()
        {
            if (lobby.StateLobby == StateLobby.TurnHuman)
                AdjustPanelLairsWithFlags();
        }

        internal void LosesChanged()
        {
            if (lobby.StateLobby == StateLobby.TurnHuman)
                AdjustPanelLoses();
        }

        private void AdjustPanelLairsWithFlags()
        {
            Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);
            Debug.Assert(lobby.CurrentPlayer.ListFlags.Count > 0);

            // Приводим в соответствие количество кнопок и логов
            // Для этого скрываем все кнопки, а потом делаем их видимыми.
            // Это чтобы не создавать каждый раз заново кнопки при изменении их численности
            while (listBtnTargetLair.Count < lobby.CurrentPlayer.ListFlags.Count)
            {
                listBtnTargetLair.Add(new VCButtonTargetLair(panelLairWithFlags));
            }

            foreach (VCButtonTargetLair b in listBtnTargetLair)
                b.Visible = false;                     

            // Сортируем логова и переназначаем ссылки на них у кнопок
            int n = 0;
            int left = 0;
            foreach (Construction pl in lobby.CurrentPlayer.ListFlags)
            {
                listBtnTargetLair[n].ShiftX = left;
                listBtnTargetLair[n].Entity = pl;
                listBtnTargetLair[n].Visible = true;

                left = listBtnTargetLair[n].NextLeft();
                n++;
            }

            panelLairWithFlags.ShiftX = MainControl.Width - left;
            panelLairWithFlags.Width = left;
            MainControl.ArrangeControl(panelLairWithFlags);

            SetNeedRedrawFrame();
        }

        private void AdjustNeighborhood()
        {
            Debug.Assert(!(lobby is null));
            Debug.Assert(lobby.TypeLobby.LairsLayers > 0);
            
            // Убираем все кнопки окрестностей
            foreach (VCImage im in listButtonsLayers)
            {
                panelNeighborhood.RemoveControl(im);
            }
            listButtonsLayers.Clear();

            int nextLeft = 0;
            TypeLobbyLayerSettings ls;
            for (int i = 0; i < lobby.TypeLobby.LairsLayers; i++)
            {
                ls = lobby.TypeLobby.LayerSettings[i];
                VCNeighborhood im = new VCNeighborhood(panelNeighborhood, nextLeft, 0, ls);
                im.Click += Neighborhood_Click;
                nextLeft = im.NextLeft();
                listButtonsLayers.Add(im);
            }

            panelNeighborhood.Width = nextLeft;
            panelNeighborhood.Height = listButtonsLayers[0].Height;
            panelNeighborhood.ShiftX = pageControl.Width - panelNeighborhood.Width;

            pageControl.ArrangeControl(panelNeighborhood);
        }

        private void Neighborhood_Click(object sender, EventArgs e)
        {
            foreach (VCNeighborhood i in listButtonsLayers)
            {
                i.ManualSelected = i == sender;
            }

            currentNeighborhood = ((VCNeighborhood)sender).Layer.Number;
            UpdateNeighborhood();
        }

        private void AdjustPanelLoses()
        {
            Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);

            // Приводим в соответствие количество кнопок и логов
            // Для этого скрываем все кнопки, а потом делаем их видимыми.
            // Это чтобы не создавать каждый раз заново кнопки при изменении их численности
            while (listBtnLoses.Count < lobby.CurrentPlayer.LoseInfo.Count)
            {
                listBtnLoses.Add(new VCImageLose(bmpPreparedToolbar, 0, 6));
            }

            foreach (VCImageLose b in listBtnLoses)
                b.Visible = false;

            // Сортируем логова и переназначаем ссылки на них у кнопок
            int n = 0;
            int left = bmpPreparedToolbar.Width - listBtnLoses[0].Width - Config.GridSize;
            foreach (LoseInfo li in lobby.CurrentPlayer.LoseInfo)
            {
                listBtnLoses[n].ShiftX = left;
                listBtnLoses[n].Info = li;
                listBtnLoses[n].Visible = true;

                left -= listBtnLoses[n].Width + Config.GridSize;
                n++;
            }

            bmpPreparedToolbar.ArrangeControls();
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

        internal void PlaySoundSelect(Uri uri)
        {
            if (Settings.PlaySound)
            {
                mpSoundSelect.Open(uri);
                mpSoundSelect.Play();
            }
        }

        internal void StopSoundSelect()
        {
            mpSoundSelect.Stop();
        }

        internal void PlaySelectButton()
        {
            mpSelectButton.Stop();
            if (Settings.PlaySound)
            {
                mpSelectButton.Play();
            }
        }

        internal void PlayPushButton()
        {
            mpPushButton.Stop();
            if (Settings.PlaySound)
            {
                mpPushButton.Play();
            }
        }

        internal void PlayConstructionComplete()
        {
            if (Settings.PlaySound)
            {
                mpConstructionComplete.Stop();
                mpConstructionComplete.Play();
            }
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

        internal void SelectPlayerObject(BigEntity po)
        {
            if (selectedPlayerObject != po)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;

                if (selectedPlayerObject != null)
                {
                    selectedPlayerObject.HideInfo();
                }

                selectedPlayerObject = po;
                if (selectedPlayerObject != null)
                    selectedPlayerObject.ShowInfo();
                else
                    panelEmptyInfo.Visible = true;

                UpdateMenu();
                SetNeedRedrawFrame();
            }
        }

        internal bool PlayerObjectIsSelected(Entity po)
        {
            Debug.Assert(po != null);

            return po == selectedPlayerObject;
        }

        internal Bitmap LoadBitmap(string filename, string folder = "Icons")
        {
            Bitmap bmp = new Bitmap(dirResources + $"{folder}\\" + filename);
            Debug.Assert(Math.Round(bmp.HorizontalResolution) == DEFAULT_DPI);
            Debug.Assert(Math.Round(bmp.VerticalResolution) == DEFAULT_DPI);

            if ((dpiX != DEFAULT_DPI) || (dpiY != DEFAULT_DPI))
                bmp.SetResolution(dpiX, dpiY);

            return bmp;
        }

        internal void SetProgrameState(ProgramState ps)
        {
            ProgramState = ps;
        }

        internal void SelectHumanPlayer(HumanPlayer hp)
        {
            Debug.Assert(hp != null);
            Debug.Assert(Config.HumanPlayers.IndexOf(hp) != -1);

            CurrentHumanPlayer = hp;
        }

        internal bool AddAvatar(string filename)
        {
            Bitmap bmpAvatar = GuiUtils.PrepareAvatar(filename);
            if (bmpAvatar != null)
            {
                // Подбираем имя файла
                string newFilenameAvatar;
                for (int i = 0; ; i++)
                {
                    newFilenameAvatar = $"Avatar{i}.png";
                    if (!File.Exists(dirResources + @"ExternalAvatars\" + newFilenameAvatar))
                        break;
                }

                // Записываем аватар в папку аватаров
                if (!Directory.Exists(dirResources + @"ExternalAvatars\"))
                    Directory.CreateDirectory(dirResources + @"ExternalAvatars\");
                bmpAvatar.Save(dirResources + @"ExternalAvatars\" + newFilenameAvatar, ImageFormat.Png);
                Config.ExternalAvatars.Add(newFilenameAvatar);
                Config.SaveExternalAvatars();

                bmpAvatar.Dispose();
                // Загружаем заново иконки
                LoadBitmapObjects();

                return true;
            }
            else
                return false;
        }

        internal void DeleteAvatar(int index)
        {
            Debug.Assert(index >= Config.ImageIndexExternalAvatar);
            Debug.Assert(index <= Config.ImageIndexLastAvatar);

            // Удаляем из конфигурации
            int idx = index - Config.ImageIndexExternalAvatar;
            string filename = Config.ExternalAvatars[idx];
            Config.ExternalAvatars.RemoveAt(idx);
            Config.SaveExternalAvatars();

            // Удаляем файл
            if (File.Exists(dirResources + @"ExternalAvatars\" + filename))
                File.Delete(dirResources + @"ExternalAvatars\" + filename);

            LoadBitmapObjects();
        }

        internal bool ChangeAvatar(int index, string filename)
        {
            Debug.Assert(index >= Config.ImageIndexExternalAvatar);
            Debug.Assert(index <= Config.ImageIndexLastAvatar);

            Bitmap bmpAvatar = GuiUtils.PrepareAvatar(filename);
            if (bmpAvatar != null)
            {
                int idx = index - Config.ImageIndexExternalAvatar;
                string localFilename = Config.ExternalAvatars[idx];

                // Удаляем старый файл
                if (File.Exists(dirResources + @"ExternalAvatars\" + localFilename))
                    File.Delete(dirResources + @"ExternalAvatars\" + localFilename);

                // Записываем аватар в папку аватаров
                bmpAvatar.Save(dirResources + @"ExternalAvatars\" + localFilename, ImageFormat.Png);
                bmpAvatar.Dispose();

                // Загружаем заново иконки
                LoadBitmapObjects();

                return true;
            }
            else
                return false;
        }

        internal void LoadBitmapObjects()
        {
            imListObjects128.NullFromIndex(Config.ImageIndexExternalAvatar, Config.MaxQuantityExternalAvatars);
            imListObjects48.NullFromIndex(Config.ImageIndexExternalAvatar, Config.MaxQuantityExternalAvatars);

            // Загружаем внешние аватары
            Bitmap bmpAvatar;
            for (int i = 0; i < Config.ExternalAvatars.Count; i++)
            {
                bmpAvatar = GuiUtils.PrepareAvatar(dirResources + @"ExternalAvatars\" + Config.ExternalAvatars[i]);
                imListObjects128.ReplaceImage(bmpAvatar, Config.ImageIndexExternalAvatar + i);
                imListObjects48.ReplaceImageWithResize(imListObjects128, Config.ImageIndexExternalAvatar + i, 1, bmpMaskSmall);
            }

            Config.UpdateDataAboutAvatar();
        }

        private void ImportNames()
        {
            string[] rawNames = File.ReadAllLines(@"f:\Projects\C-Sharp\Fantasy King's Battle\text\locdata_dec.txt", Encoding.UTF8);

            /*foreach (TypeHero th in Config.TypeHeroes)
            {
                TreatNames(th.ID.ToUpper());
                TreatSurnames(th.ID.ToUpper());
            }

            TreatNames("DEATH_KNIGHT");
            TreatSurnames("DEATH_KNIGHT");
            */
            TreatNames("ICE_MAGE");
            TreatSurnames("ICE_MAGE");

            void TreatNames(string nameTypeHero)
            {
                string strBegin = $"#{nameTypeHero}N";
                List<string> names = new List<string>();
                string name;
                names.Add("    <Names>");

                foreach (string s in rawNames)
                {
                    if (s.StartsWith(strBegin))
                    {
                        name = s.Substring(s.IndexOf("hname") + 7);
                        name = name.Substring(0, name.Length - 7);
                        name = name.Replace("\n\r", "");

                        names.Add("      <Name>" + name + "</Name>");
                    }
                }
                names.Add("    </Names>");

                File.WriteAllLines(@"f:\Projects\C-Sharp\Fantasy King's Battle\Resources\Config\" + nameTypeHero + "_Name.txt", names);
            }

            void TreatSurnames(string nameTypeHero)
            {
                string strBegin = $"#{nameTypeHero}S";
                List<string> names = new List<string>();
                string name;
                names.Add("    <Surnames>");

                foreach (string s in rawNames)
                {
                    if (s.StartsWith(strBegin))
                    {
                        name = s.Substring(s.IndexOf("hname") + 7);
                        name = name.Substring(0, name.Length - 7);
                        name = name.Replace("\n\r", "");

                        names.Add("      <Surname>" + name + "</Surname>");
                    }
                }
                names.Add("    </Surnames>");

                File.WriteAllLines(@"f:\Projects\C-Sharp\Fantasy King's Battle\Resources\Config\" + nameTypeHero + "_Surname.txt", names);
            }
        }

        private void BtnPlayerPreferences_Click(object sender, EventArgs e)
        {
            WindowPlayerPreferences w = new WindowPlayerPreferences();
            w.ShowDialog();
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            WindowConfirmExit f = new WindowConfirmExit();
            if (f.ShowDialog() == DialogAction.OK)
            {
                SetProgrameState(ProgramState.NeedQuit);
                Close();
            }
        }

        private void BtnAboutProgram_Click(object sender, EventArgs e)
        {
            ShowWindowAboutProgram();
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            ShowWindowPreferences();
        }

        private void BtnTournament_Click(object sender, EventArgs e)
        {
            StartNewLobby();
            if (!(lobby is null))
                ReturnFromLobby();
        }

        internal void ControlShowed(VisualControl vc)
        {
            VisualControl curControl = ControlUnderMouse();
            if (curControl == vc)
            {
                UpdateCurrentControl(false);
            }
        }

        internal void ControlHided(VisualControl vc)
        {
            if (vc == controlWithHint)
            {
                ControlForHintLeave();
            }
        }

        internal void ObjectDestroyed(BigEntity entity)
        {
            Debug.Assert(entity != null);

            foreach (VCPageButton button in pageControl.Pages)
            {
                if (button.SelectedPlayerObject == entity)
                {
                    button.SelectedPlayerObject = null;
                }
            }
        }
    }
}
