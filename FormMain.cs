﻿using System;
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
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ProgramState { Started, ConfirmQuit, NeedQuit };
    internal enum DialogAction { None, OK, MainMenu, NewMission, RestartGame, Quit };

    public partial class FormMain : Form
    {
        internal const string NAME_PROJECT = "The Fantasy Kingdoms Battle";
        internal const string VERSION = "0.4.00";
        internal const string DATE_VERSION = "27.07.2024";
        internal const string VERSION_POSTFIX = "developer build";

        internal ProgramState ProgramState { get; private set; } = ProgramState.Started;
        internal bool gameStarted = false;
        private bool needRepaintFrame = false;
        private bool inMouseClick;

        // Проигрывание звуков
        private readonly System.Windows.Media.MediaPlayer mpSoundSelect;
        private readonly System.Windows.Media.MediaPlayer mpSelectButton;
        private readonly System.Windows.Media.MediaPlayer mpPressButton;
        private readonly System.Windows.Media.MediaPlayer mpConstructionComplete;

        // Контролы главного меню
        private Point mousePos;
        private VisualControl controlWithHint;
        private VisualControl controlClicked;
        private VisualControl clickedControl;
        private VisualControl controlWithDropDown;

        // Рендеринг
        private Bitmap bmpRenderClientArea;// Фон клиентской области, на который налагается кадр
        private Bitmap bmpRenderFrame;// Рисунок, на котором рисуются контролы (без учета полноэкранного режима)
        private Graphics gfxRenderFrame;// Graphics кадра
        private Graphics gfxRenderClientArea;// Graphics клиентской области

        internal VCLabel labelFPS;// Отладочная информация о FPS и прочем

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
        internal const int GUI_16_GOLD = 43;
        internal const int GUI_16_REPAIR = 2;
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
        internal const int GUI_16_MORALE = 13;
        internal const int GUI_16_LUCK = 14;
        internal const int GUI_16_INTEREST_DEFENSE = 15;
        internal const int GUI_16_NEEDS_FOOD = 16;
        internal const int GUI_16_ENTHUSIASM = 17;
        internal const int GUI_16_HONOR = 18;
        internal const int GUI_16_NEEDS_GOLD = 19;
        internal const int GUI_16_NEEDS_REST = 20;
        internal const int GUI_16_NEEDS_ENTERTAINMENT = 21;
        internal const int GUI_16_INTEREST_EXPLORE = 22;
        internal const int GUI_16_INTEREST_ATTACK = 23;
        internal const int GUI_16_INTEREST_OTHER = 24;
        internal const int GUI_16_PURSE = 25;
        internal const int GUI_16_WOOD = 26;
        internal const int GUI_16_IRON = 27;
        internal const int GUI_16_STONE = 28;
        internal const int GUI_16_SUN = 40;
        internal const int GUI_16_CRESCENT = 41;
        internal const int GUI_16_RESEARCH_POINTS = 45;
        internal const int GUI_16_DURABILITY = 47;
        internal const int GUI_16_KNOWLEDGE = 51;
        internal const int GUI_16_BUILDER = 58;
        internal const int GUI_16_PEOPLE = 53;
        internal const int GUI_16_TRADITIONS = 57;
        internal const int GUI_16_MANA = 61;

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
        internal const int GUI_24_BROKEN_HOUSE = 10;
        internal const int GUI_24_HAMMER = 11;

        internal const int GUI_32_CLOSE = 0;

        internal const int GUI_45_EMPTY = 0;
        internal const int GUI_45_BORDER = 0;

        internal static Size PANEL_MENU_CELLS = new Size(4, 3);
        internal const int DISTANCE_BETWEEN_CELLS = 3;

        internal const int IMAGE_INDEX_NONE = 127;
        internal const int IMAGE_INDEX_UNKNOWN = 139;
        internal const int IMAGE_INDEX_CURRENT_AVATAR = -100;

        internal readonly Bitmap bmpForBackground;
        internal readonly Bitmap bmpBorderForIcon;
        internal readonly Bitmap bmpBorderForIconAlly;
        internal readonly Bitmap bmpBorderForIconEnemy;
        internal readonly Bitmap bmpEmptyEntity;
        internal readonly Bitmap bmpBackgroundEntity;
        internal readonly Bitmap bmpBackgroundEntityInQueue;
        internal readonly BitmapBorder bbBorderWindow;
        internal readonly BitmapBorder bbObject;
        internal readonly BitmapBorder bbToolBarLabel;
        internal readonly BitmapBorder bbGamespace;
        internal readonly BitmapBorder bbSelect;
        internal readonly BitmapBorder bbIcon16;
        internal readonly Bitmap bmpBorderBig;
        internal readonly Bitmap bmpBorderBigForProgressBar;
        internal readonly Bitmap bmpBorder96ForProgressBar;
        internal readonly Bitmap bmpMaskBig;
        internal readonly Bitmap bmpMask96;
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
        internal readonly Bitmap bmpBandProgressBar;
        internal readonly Bitmap bmpBandProgressBarFore;
        internal readonly Bitmap bmpBandProgressBarBack;

        internal Size sizeGamespace { get; set; }
        internal Size MinSizeGamespace { get; set; }
        private Point topLeftFrame;
        private bool inDrawFrame = false;

        internal readonly List<LayerCustom> Layers;
        internal readonly LayerMainMenu layerMainMenu;
        internal readonly LayerGameSingle layerGame;
        internal LayerCustom currentLayer { get; set; }
        internal CollectionBackgroundImage CollectionBackgroundImage { get; }

        //
        internal Settings Settings { get; private set; }
        internal MainConfig MainConfig { get; private set; }

        public FormMain()
        {
            InitializeComponent();

            Program.formMain = this;

            Text = NAME_PROJECT + " (сборка " + VERSION + ")";

            // Обновляем обновлятор
            string newName;
            foreach (string file in System.IO.Directory.EnumerateFiles(Program.WorkFolder))
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
            Settings = new Settings();
            Settings.PlayMusicChanged += Settings_PlayMusicChanged;
            Settings.VolumeSoundChanged += Settings_VolumeSoundChanged;
            Settings.VolumeMusicChanged += Settings_VolumeMusicChanged;

            MainConfig = new MainConfig();
            // Проверяем требование по разрешению экрана
            bool defaultGridSize = true;
            if ((Screen.PrimaryScreen.Bounds.Width < MainConfig.ScreenMinSize.Width) || (Screen.PrimaryScreen.Bounds.Height < MainConfig.ScreenMinSize.Height))
            {
                MessageBox.Show($"Для игры необходимо разрешение экрана {MainConfig.ScreenMinSize.Width} * {MainConfig.ScreenMinSize.Height}."
                    + Environment.NewLine + $"Текущее разрешение {Screen.PrimaryScreen.Bounds.Width} * {Screen.PrimaryScreen.Bounds.Height}."
                    + Environment.NewLine + Environment.NewLine + $"Программа автоматически уменьшит размер используемого пространства до минимума."
                    + Environment.NewLine + $"Это внесет визуальные искажения в оригинальный дизайн.",
                    NAME_PROJECT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                defaultGridSize = false;
            }

            // Если включено автообновление, проверяем на их наличие
            if (Settings.CheckUpdateOnStartup)
            {
                CheckForNewVersion();
            }

            CollectionBackgroundImage = new CollectionBackgroundImage();

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
            SetStage("Читаем книгу");
            _ = new Config(this, defaultGridSize);
            SetStage("Изучаем справочник");
            _ = new Descriptors(this);

            FontSmall = new M2Font("small");
            FontSmallC = new M2Font("small_c");
            FontMedCaptionC = new M2Font("med_caption_c");
            FontMedCaption = new M2Font("med_caption");
            FontBigCaptionC = new M2Font("big_caption_c");
            FontBigCaption = new M2Font("big_caption");
            FontSmallBC = new M2Font("_small_b_c");
            FontParagraph = new M2Font("paragraph");
            FontParagraphC = new M2Font("paragraph_c");

            SelectHumanPlayer(Descriptors.HumanPlayers[0]);

            // Загружаем иконки
            SetStage("Рассматриваем картины");

            bmpBorderBig = LoadBitmap("BorderBig.png");
            bmpBorderBigForProgressBar = LoadBitmap("BorderBigWithPB.png");
            bmpBorder96ForProgressBar = LoadBitmap("BorderBigOrigin.png");
            bmpMaskBig = LoadBitmap("MaskBig.png");
            bmpMask96 = LoadBitmap("Mask96.png");
            bmpMaskSmall = LoadBitmap("MaskSmall.png");// Нужна ли еще?

            // Иконки игровых объектов. Также включает встроенные аватары игроков и пул пустых иконок под внешние аватары
            BmpListObjects128 = new BitmapList(LoadBitmap("Objects.png"), new Size(128, 128), true, true);
            // Добавляем места под внешние аватары
            BmpListObjects128.AddEmptySlots(Config.MaxQuantityExternalAvatars);
            BmpListObjects48 = new BitmapList(BmpListObjects128, new Size(48, 48), Config.BorderInBigIcons, bmpMaskSmall);
            BmpListObjects96 = new BitmapList(BmpListObjects128, new Size(96, 96), Config.BorderInBigIcons, bmpMask96);
            LoadBitmapObjects();

            BmpListObjects48.AddBitmap(LoadBitmap("Gui48.png"));
            BmpListObjects32 = new BitmapList(BmpListObjects48, new Size(32, 32), 0, null);

            BmpListGui16 = new BitmapList(LoadBitmap("Gui16.png"), new Size(16, 16), true, false);
            BmpListGui24 = new BitmapList(LoadBitmap("Gui24.png"), new Size(24, 24), true, true);
            BmpListGui32 = new BitmapList(LoadBitmap("Gui32.png"), new Size(32, 32), true, true);
            BmpListParameters = new BitmapList(LoadBitmap("Parameters.png"), new Size(24, 24), true, false);
            BmpListStateHero = new BitmapList(LoadBitmap("StateCreature.png"), new Size(24, 24), true, false);
            BmpListMenuCellFilters = new BitmapList(LoadBitmap("MenuCellFilters.png"), new Size(48, 48), true, false);
            BmpListCheckBox = new BitmapList(LoadBitmap("CheckBox.png"), new Size(24, 24), true, true);

            //MakeAlpha();

            bmpForBackground = LoadBitmap("Background.png");
            bmpBorderForIcon = LoadBitmap("BorderIconEntity.png");
            bmpEmptyEntity = LoadBitmap("EmptyEntity.png");
            bmpBackgroundEntity = LoadBitmap("BackgroundEntity.png");
            bmpBackgroundEntityInQueue = LoadBitmap("BackgroundEntityInQueue.png");
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
            bmpBandProgressBar = LoadBitmap("ProgressBar.png");
            bmpBandProgressBarFore = LoadBitmap("ProgressBarFore.png");
            bmpBandProgressBarBack = LoadBitmap("ProgressBarBack.png");

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

            // Создаем список слоев игрового поля
            Layers = new List<LayerCustom>();

            // Слой главного меню
            layerMainMenu = new LayerMainMenu();
            Layers.Add(layerMainMenu);
            currentLayer = layerMainMenu;

            // Слой игры
            layerGame = new LayerGameSingle();

            Width = Width - Program.formMain.ClientSize.Width + MinSizeGamespace.Width;
            Height = Height - Program.formMain.ClientSize.Height + MinSizeGamespace.Height;

            layerMainMenu.Width = MinSizeGamespace.Width;
            layerMainMenu.Height = MinSizeGamespace.Height;

            layerMainMenu.ArrangeControls();
            layerGame.ArrangeControls();


            labelFPS = new VCLabel(null, Config.GridSize, Config.GridSize, FontSmallC, Color.White, 16, "");
            labelFPS.Width = 200;
            labelFPS.ApplyMaxSize();
            labelFPS.ShowBorder = false;

            //
            SetStage("Прибираем после строителей");

            //
            PlayerMusic = new PlayerMusic(Settings);
            mpSoundSelect = new System.Windows.Media.MediaPlayer();
            mpSelectButton = new System.Windows.Media.MediaPlayer();
            mpSelectButton.Open(new Uri(Program.FolderResources + @"Sound\Interface\Button\SelectButton.wav"));
            mpPressButton = new System.Windows.Media.MediaPlayer();
            mpPressButton.Open(new Uri(Program.FolderResources + @"Sound\Interface\Button\PushButton.wav"));
            mpConstructionComplete = new System.Windows.Media.MediaPlayer();
            mpConstructionComplete.Open(new Uri(Program.FolderResources + @"Sound\Interface\Construction\ConstructionComplete.wav"));
            UpdateVolumeSound();

            //formHint = new FormHint(ilGui16, ilParameters);

            splashForm.Dispose();

            // Курсор
            CustomCursor.CreateCursor(Program.FolderResources + @"Cursor\Cursor_simple.png");
            Cursor = CustomCursor.GetCursor();

            //
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);

            //MediaElement me = new MediaElement()
            //me.Parent = this;

            PlayerMusic.PlayMainTheme();

            //ImportNames();// Однократная операция

            // 
            void SetStage(string text)
            {
                lblStage.Text = text + "...";
                lblStage.Refresh();
            }

        }

        // ImageList'ы
        internal BitmapList BmpListObjects32 { get; }
        internal BitmapList BmpListObjects48 { get; }
        internal BitmapList BmpListObjects96 { get; }
        internal BitmapList BmpListObjects128 { get; }
        internal BitmapList BmpListGui16 { get; }
        internal BitmapList BmpListGui24 { get; }
        internal BitmapList BmpListGui32 { get; }
        internal BitmapList BmpListParameters { get; }
        internal BitmapList BmpListStateHero { get; }
        internal BitmapList BmpListMenuCellFilters { get; }
        internal BitmapList BmpListCheckBox { get; }

        // Шрифты
        internal M2Font FontSmall { get; }
        internal M2Font FontSmallC { get; }
        internal M2Font FontSmallBC { get; }
        internal M2Font FontMedCaption { get; }
        internal M2Font FontMedCaptionC { get; }
        internal M2Font FontBigCaption { get; }
        internal M2Font FontBigCaptionC { get; }
        internal M2Font FontParagraph { get; }
        internal M2Font FontParagraphC { get; }

        // Проигрывание музыки
        internal PlayerMusic PlayerMusic { get; }


        internal HumanPlayer CurrentHumanPlayer { get; private set; }

        private void Settings_VolumeMusicChanged(object sender, EventArgs e)
        {
            PlayerMusic.UpdateVolumeSound();
        }

        private void Settings_VolumeSoundChanged(object sender, EventArgs e)
        {
            UpdateVolumeSound();
        }

        private void UpdateVolumeSound()
        {
            mpSoundSelect.Volume = (float)Settings.VolumeSound / 100;
            mpSelectButton.Volume = mpSoundSelect.Volume;
            mpPressButton.Volume = mpSoundSelect.Volume;
            mpConstructionComplete.Volume = mpSoundSelect.Volume;
        }

        private void Settings_PlayMusicChanged(object sender, EventArgs e)
        {
            PlayerMusic.TogglePlayMusic();
        }

        internal TimeSpan durationDrawFrame;
        internal DateTime firstFrameOfSecond;
        internal int countFrames;
        internal int framesPerSecond;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // При старте игры в полноэкранном режиме, если курсор находится на пустом пространстве, окно игры состоит из белого фона
            // Показ кадра при старте отрисовывает окно
            ValidateFrame();
            DrawFrame();// Делаем вызов, иначе при первой же обработке Application.DoEvents() будет исключение при отрисовке

            while (true)
            {
                DateTime curTime = DateTime.Now;
                TimeSpan delta1 = curTime - firstFrameOfSecond;
                if (delta1.TotalMilliseconds >= 1000)
                {
                    firstFrameOfSecond = DateTime.Now;
                    framesPerSecond = countFrames;
                    countFrames = 0;
                }

                // Обрабатываем события - клики мышью, нажатия клавиатуры
                Application.DoEvents();

                // Если был подтвержден выход из программы, выходим
                if (ProgramState == ProgramState.NeedQuit)
                    break;

                // Расчет перед кадром
                currentLayer.PrepareFrame();

                // Формируем и показываем кадр
                if (WindowState != FormWindowState.Minimized)
                {
                    DrawFrame();
                    Refresh();
                }

                TimeSpan ts = DateTime.Now - curTime;
                int delta = Config.DurationFrame - ts.Milliseconds;
                if (delta > 0)
                    System.Threading.Thread.Sleep(delta);
            }

            Close();
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

                axWindowsMediaPlayer1.URL = Program.FolderResources + @"Video\Rebirth.avi";
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
        }

        private void ValidateFrame()
        {
            if (axWindowsMediaPlayer1 != null)
                axWindowsMediaPlayer1.Size = ClientSize;

            switch (Settings.ScreenMode())
            {
                case ScreenMode.FullScreenStretched:
                    topLeftFrame = new Point(0, 0);
                    sizeGamespace = Size;
                    break;
                case ScreenMode.FillScreenWindowed:
                    topLeftFrame = new Point((ClientSize.Width - MinSizeGamespace.Width) / 2, (ClientSize.Height - MinSizeGamespace.Height) / 2);
                    sizeGamespace = MinSizeGamespace;

                    Debug.Assert(topLeftFrame.X >= 0, $"topLeftFrame.X = {topLeftFrame.X}");
                    Debug.Assert(topLeftFrame.Y >= 0, $"topLeftFrame.Y = {topLeftFrame.Y}");
                    break;
                case ScreenMode.Window:
                    topLeftFrame = new Point(0, 0);
                    Size = new Size(Width - ClientSize.Width + MinSizeGamespace.Width, Height - ClientSize.Height + MinSizeGamespace.Height);
                    sizeGamespace = MinSizeGamespace;
                    break;
                default:
                    throw new Exception("Неизвестный режим");
            }

            layerMainMenu.ApplyCurrentWindowSize(sizeGamespace);
            layerGame.ApplyCurrentWindowSize(sizeGamespace);
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


                //panelPlayers.Visible = false;
                //MainControl.Visible = false;
                //panelPlayers.Visible = true;
                //MainControl.Visible = true;
                //Application.DoEvents();
                ValidateFrame();
                //Show();
            }
        }


        internal void ShowWindowPreferences()
        {
            ScreenMode sm = Settings.ScreenMode();
            WindowPreferences w = new WindowPreferences();
            w.ApplySettings(Settings);
            w.Show();
        }

        internal void AddLayer(LayerCustom vc, bool deactivateLayer = true)
        {
            Debug.Assert(Layers.Count <= 5);
            Debug.Assert(currentLayer.Controls.Count > 0);
            Debug.Assert(!vc.Active);

            // Если новый слой - выпадающий, то запоминаем, на какой контроле выпало окно
            if ((vc is CustomWindow cw) && cw.IsDropDown)
            {
                Assert(controlWithDropDown is null);
                controlWithDropDown = clickedControl;
                Assert(controlWithDropDown != null);
            }

            // Так как переходим к новом слою, выходим мышью из контрола текущего слоя
            // При клике есть только controlClicked, его и смотрим
            // В некоторых случаях его может не быть - например, при выходе по Alt+F4 или крестик на окне
            controlClicked?.MouseLeave();
            controlClicked = null;

            Layers.Add(vc);
            if (deactivateLayer)
                currentLayer.Deactivated();
            ControlForHintLeave();// Убираем активный контрол, если он был
            currentLayer = vc;
            currentLayer.Activated();
        }

        internal void ExchangeLayer(LayerCustom oldLayer, LayerCustom newLayer)
        {
            Debug.Assert(Layers.Count == 1);
            Debug.Assert(Layers[0] == oldLayer);
            Debug.Assert(currentLayer == oldLayer);

            // Слой меняется, поэтому тут же делаем выход контрола старого слоя
            ControlForHintLeave();

            Layers[0] = newLayer;
            currentLayer = newLayer;
        }

        internal void RemoveLayer(LayerCustom vl, DialogAction da)
        {
            Debug.Assert(Layers.Count > 1);
            if (Layers[Layers.Count - 1] != vl)
            {
                string layers = "";
                foreach (LayerCustom l in Layers)
                {
                    if (l is VCForm lv)
                        layers += $"{lv.GetCaption()}; ";
                    else
                        layers += $"{l}; ";
                }

                string nameLayer = vl is VCForm vlf ? vlf.GetCaption() : vl.ToString();
                Debug.Assert(false, $"Последний слой не равен удаляемому (всего слоев {Layers.Count}, слои [{layers}], удаляется {nameLayer})");
            }
            currentLayer.Deactivated();
            ControlForHintLeave();// Убираем активный контрол, если он был
            Layers.Remove(vl);
            currentLayer = Layers[Layers.Count - 1];
            if (!currentLayer.Active)
                currentLayer.Activated();
            currentLayer.Focused(da);

            if (controlWithDropDown != null)
            {
                controlWithDropDown.ResultFromDropDown(da);
                controlWithDropDown = null;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            switch (ProgramState)
            {
                case ProgramState.ConfirmQuit:
                    e.Cancel = true;
                    break;
                case ProgramState.NeedQuit:
                    if (Program.formMain.layerGame.CurrentLobby != null)
                        Program.formMain.layerGame.EndLobby();
                    break;
                default:
                    WindowConfirmExit.ConfirmExit();
                    e.Cancel = true;
                    break;
            }
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
        internal static Descriptors Descriptors { get; set; }

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

            base.OnPaint(e);

            if (bmpRenderClientArea == null)
                return;
            Assert(ClientSize.Equals(bmpRenderClientArea.Size));
            if (needRepaintFrame)
            {
                DrawFrame();
                needRepaintFrame = false;
            }

            e.Graphics.DrawImage(bmpRenderClientArea, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        // Рисование кадра главной формы
        private void DrawFrame()
        {
            if (inDrawFrame)
                return;
            //Debug.Assert(inDrawFrame == false);

            inDrawFrame = true;

            if (layerGame.debugMode)
            {
                layerGame.startDebugAction = DateTime.Now;
            }

            // Готовим фон, если его надо поменять
            if ((bmpRenderClientArea == null) || (bmpRenderClientArea == null) || !bmpRenderFrame.Size.Equals(sizeGamespace) || !bmpRenderClientArea.Size.Equals(ClientSize))
                PrepareBackground();

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
            if (!(controlWithHint is null) && controlWithHint.Visible)
            {
                if (!VisualControl.PanelHint.Visible)
                    VisualControl.PanelHint.CheckHover();

                // Необходимо перерисовывать подсказку, т.к. после тика могли появиться деньги, выполниться условия
                if (VisualControl.PanelHint.Visible)
                {
                    VisualControl.PanelHint.Clear();
                    controlWithHint.DoShowHint();
                    VisualControl.PanelHint.DrawHint();
                    VisualControl.PanelHint.Paint(gfxRenderFrame);
                }
            }

            if (layerGame.debugMode)
            {
                if (controlWithHint != null)
                    gfxRenderFrame.DrawRectangle(layerGame.penDebugBorder, controlWithHint.Rectangle);

                layerGame.durationDrawFrame = DateTime.Now - layerGame.startDebugAction;
                layerGame.labelTimeDrawFrame.Text = $"FPS: {layerGame.framesPerSecond}, TPS: {layerGame.ticksPerSecond}, Draw frame: {layerGame.durationDrawFrame.TotalMilliseconds}";
                layerGame.vcDebugInfo.Paint(gfxRenderFrame);
            }

            labelFPS.Text = "Delta: ";
            labelFPS.Paint(gfxRenderFrame);

            gfxRenderClientArea.CompositingMode = CompositingMode.SourceCopy;
            gfxRenderClientArea.DrawImage(bmpRenderFrame, topLeftFrame.X, topLeftFrame.Y, sizeGamespace.Width, sizeGamespace.Height);

            //
            inDrawFrame = false;

            void PrepareBackground()
            {
                // Переформировываем картинку фона клиентской области
                if ((bmpRenderClientArea == null) || !bmpRenderClientArea.Size.Equals(sizeGamespace))
                {
                    bmpRenderClientArea?.Dispose();

                    if (Settings.FullScreenMode)
                    {
                        bmpRenderClientArea = GuiUtils.MakeBackground(ClientSize);
                        Graphics g = Graphics.FromImage(bmpRenderClientArea);
                        if (!Settings.StretchControlsInFSMode)
                            bbGamespace.DrawBorder(g, topLeftFrame.X - 7, topLeftFrame.Y - 7, sizeGamespace.Width + 14, sizeGamespace.Height + 14, Color.Transparent);
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
                }
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            currentLayer.KeyPress(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            currentLayer.KeyUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            TreatMouseMove(e.Button == MouseButtons.Left, e.Button == MouseButtons.Right);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            ControlForHintLeave();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (controlWithHint != null)
                {
                    //Assert(clickedControl is null);// Need restore
                        
                    clickedControl = controlWithHint;
                    clickedControl.MouseDown();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (controlWithHint != null)
                {
                    //Assert(clickedControl is null);// Need restore

                    controlWithHint.MouseRightDown(MousePosToControl(controlWithHint));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (clickedControl != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    inMouseClick = true;
                    clickedControl.MouseUp(MousePosToControl(clickedControl));
                    clickedControl = null;
                    inMouseClick = false;

                    // Во время нажатия кнопки мог произойти выход из программы
                    if (ProgramState == ProgramState.NeedQuit)
                    {
                        if (layerGame.CurrentLobby != null)
                            layerGame.EndLobby();
                        Close();
                    }

                    if (IsDisposed)
                        return;
                }
            }
            else
            {
                if (currentLayer is CustomWindow cw)
                    if (cw.IsDropDown)
                    {
                        cw.CloseForm(DialogAction.None);
                    }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (controlWithHint != null)
                {
                    Debug.Assert(controlWithHint.Visible);
                    controlWithHint.RightButtonClick();
                }
            }


            //ShowFrame(true);
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

        internal Point MousePosToControl(VisualControl vc)
        {
            return new Point(mousePos.X - vc.Left, mousePos.Y - vc.Top);
        }

        private void TreatMouseMove(bool leftDown, bool rightDown)
        {
            Point oldMousePos = mousePos;
            UpdateMousePos();

            if (!mousePos.Equals(oldMousePos))
            {
                UpdateCurrentControl(leftDown, rightDown);
            }
        }

        private void UpdateCurrentControl(bool leftDown, bool rightDown)
        {
            VisualControl curControl = ControlUnderMouse();

            if (curControl == null)
            {
                ControlForHintLeave();
            }
            else if (curControl == controlWithHint)
            {
                curControl.MouseMove(MousePosToControl(curControl), leftDown, rightDown);
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
                //curControl.DoShowHint();
                if (controlWithHint != null)
                {
                    controlWithHint.MouseLeave();
                    controlWithHint = curControl;
                    controlWithHint.MouseEnter(leftDown);
                    //formHint.Visible = true;
                }
                else
                {
                    ControlForHintLeave();
                    controlWithHint = curControl;
                    controlWithHint.MouseEnter(leftDown);
                }
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

            clickedControl = null;

            VisualControl.PanelHint.HideHint();
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

        internal void PlayPressButton()
        {
            mpPressButton.Stop();
            if (Settings.PlaySound)
            {
                mpPressButton.Play();
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

        internal void SetProgrameState(ProgramState ps)
        {
            ProgramState = ps;
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
                    if (!File.Exists(Program.FolderResources + @"ExternalAvatars\" + newFilenameAvatar))
                        break;
                }

                // Записываем аватар в папку аватаров
                if (!Directory.Exists(Program.FolderResources + @"ExternalAvatars\"))
                    Directory.CreateDirectory(Program.FolderResources + @"ExternalAvatars\");
                bmpAvatar.Save(Program.FolderResources + @"ExternalAvatars\" + newFilenameAvatar, ImageFormat.Png);
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
            if (File.Exists(Program.FolderResources + @"ExternalAvatars\" + filename))
                File.Delete(Program.FolderResources + @"ExternalAvatars\" + filename);

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
                if (File.Exists(Program.FolderResources + @"ExternalAvatars\" + localFilename))
                    File.Delete(Program.FolderResources + @"ExternalAvatars\" + localFilename);

                // Записываем аватар в папку аватаров
                bmpAvatar.Save(Program.FolderResources + @"ExternalAvatars\" + localFilename, ImageFormat.Png);
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
            BmpListObjects128.NullFromIndex(Config.ImageIndexExternalAvatar, Config.MaxQuantityExternalAvatars);
            BmpListObjects48.NullFromIndex(Config.ImageIndexExternalAvatar, Config.MaxQuantityExternalAvatars);

            // Загружаем внешние аватары
            Bitmap bmpAvatar;
            for (int i = 0; i < Config.ExternalAvatars.Count; i++)
            {
                bmpAvatar = GuiUtils.PrepareAvatar(Program.FolderResources + @"ExternalAvatars\" + Config.ExternalAvatars[i]);
                BmpListObjects128.ReplaceImage(bmpAvatar, Config.ImageIndexExternalAvatar + i);
                BmpListObjects48.ReplaceImageWithResize(BmpListObjects128, Config.ImageIndexExternalAvatar + i, 1, bmpMaskSmall);
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

        internal void ControlShowed(VisualControl vc)
        {
            VisualControl curControl = ControlUnderMouse();
            if (curControl == vc)
            {
                UpdateCurrentControl(false, false);
            }
        }

        internal void ControlHided(VisualControl vc)
        {
            if (vc == controlWithHint)
            {
                ControlForHintLeave();
            }
        }

        internal void SelectHumanPlayer(HumanPlayer hp)
        {
            Debug.Assert(hp != null);
            Debug.Assert(Descriptors.HumanPlayers.IndexOf(hp) != -1);

            CurrentHumanPlayer = hp;
        }

        internal int CalcRestTime(int restMilliTicks, int milliTicksPerTick)
        {
            // Прибавляем секунду, чтобы когда оставалось менее 1 секунды, индикатор не становился 0, а продолжал показывать 1
            int restTimeExecuting = (int)Math.Truncate((double)restMilliTicks / (milliTicksPerTick * Config.TicksInSecond) + 0.99);
            Assert(restTimeExecuting > 0);

            return restTimeExecuting;
        }

        internal int CalcPercentExecuting(int passedMilliTicks, int totalMilliTicks)
        {
            int percent = (int)Math.Truncate(passedMilliTicks * 100.0 / totalMilliTicks + 0.99);
            Assert(percent >= 0);
            Assert(percent <= 100);
            return percent;
        }

        internal bool InGame() => layerGame.CurrentLobby != null;
    }
}