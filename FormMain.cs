using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;

namespace Fantasy_Kingdoms_Battle
{
    public partial class FormMain : Form
    {
        private const string NAME_PROJECT = "The Fantasy Kingdoms Battle";
        internal const string VERSION = "0.2.6";
        internal const string DATE_VERSION = "03.01.2021";
        private const string VERSION_POSTFIX = "в разработке";
        internal readonly string dirCurrent;
        internal readonly string dirResources;

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
        private VCLabel labelTimePaintFrame;
        private DateTime startDebugAction;
        private TimeSpan durationDrawFrame;
        private TimeSpan durationPaintFrame;

        // Контролы главного меню
        private Bitmap bmpFrame;// Готовый кадр
        private Graphics gfxFrame;// Graphics кадра, чтобы контролы работали сразу с ним
        internal Bitmap bmpBackground;// Фон кадра

        private readonly VisualControl MainControl;

        private Point mousePos;
        private VisualControl controlWithHint;
        private VisualControl controlClicked;
        private bool hintShowed = false;

        private readonly VisualControl panelPlayers;// Панель, на которой находятся панели игроков лобби

        private readonly VCToolLabel labelDay;
        private readonly VCToolLabel labelGold;

        private readonly VCButton btnPreferences;
        private readonly VCButton btnHelp;
        private readonly VCButton btnQuit;
        private readonly VCButton btnEndTurn;

        private readonly VisualControl panelLairWithFlags;
        private readonly List<VCButtonTargetLair> listBtnTargetLair = new List<VCButtonTargetLair>();

        private readonly VCBitmap bitmapMenu;

        private bool allowResize = false;
        private VCCell selectedPanelEntity;

        // Главные страницы игры
        private readonly List<VCFormPage> pages = new List<VCFormPage>();
        private readonly VCFormPage pageGuilds;
        private readonly VCFormPage pageBuildings;
        private readonly VCFormPage pageTemples;
        private readonly VCFormPage pageHeroes;
        private readonly VCFormPage pageLairs;
        private readonly VCFormPage pageTournament;

        private PanelWithPanelEntity panelWarehouse;
        private PanelWithPanelEntity panelHeroes;

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

        internal const int GUI_16_DAY = 0;
        internal const int GUI_16_GOLD = 1;
        internal const int GUI_16_PEASANT = 2;

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
        internal readonly Bitmap bmpBorderBattlefield;
        internal readonly Bitmap bmpMaskBig;
        internal readonly Bitmap bmpMaskSmall;
        internal readonly M2Font fontCost;
        internal readonly M2Font fontLevel;
        internal int LengthSideBorderBattlefield { get; private set; }
        private Size sizeGamespace;
        private Point shiftControls;

        private bool inDrawFrame = false;
        private bool needRedrawFrame;

        private VCFormPage currentPage;
        private readonly VisualControl panelEmptyInfo;
        private readonly PanelBuildingInfo panelBuildingInfo;
        internal readonly PanelLairInfo panelLairInfo;
        private readonly PanelHeroInfo panelHeroInfo;
        private readonly PanelMonsterInfo panelMonsterInfo;
        internal VCCell SelectedPanelEntity
        {
            get => selectedPanelEntity;
            set
            {
                if (selectedPanelEntity != null)
                    selectedPanelEntity.Selected = false;
                selectedPanelEntity = value;
                if (selectedPanelEntity != null)
                    selectedPanelEntity.Selected = true;
            }
        }
        private Rectangle rectBorderAroungGamespace;
        private Point point1LineAfterPanelPlayers;
        private Point point2LineAfterPanelPlayers;

        internal VCMenuCell[,] CellsMenu { get; }

        internal FormHint formHint;
        internal PanelConstruction SelectedPanelBuilding { get; private set; }
        internal PanelLair SelectedPanelLair { get; private set; }
        internal PlayerHero SelectedHero { get; private set; }
        internal Monster SelectedMonster { get; private set; }
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

                fontCost = new M2Font(dirResources, "small_c");
                fontLevel = new M2Font(dirResources, "med_caption_c");

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
                ilStateHero = new BitmapList(dirResources, "StateHero.png", 24, true, false);
                ilMenuCellFilters = new BitmapList(dirResources, "MenuCellFilters.png", 48, true, false);

                ilGui = new BitmapList(dirResources, "Gui.png", 48, true, true);
                //MakeAlpha();

                bmpForBackground = new Bitmap(dirResources + "Icons\\Background.png");
                bmpBorderForIcon = new Bitmap(dirResources + "Icons\\BorderIconEntity.png");
                bmpEmptyEntity = new Bitmap(dirResources + "Icons\\EmptyEntity.png");
                bmpBackgroundEntity = new Bitmap(dirResources + "Icons\\BackgroundEntity.png");
                bmpBorderBattlefield = new Bitmap(dirResources + "Icons\\BorderBattlefield.png");
                LengthSideBorderBattlefield = bmpBorderBattlefield.Width - (Config.WidthBorderBattlefield * 2);
                Debug.Assert(LengthSideBorderBattlefield > 0);

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

                //
                MainControl = new VisualControl();

                // Метки с информацией о Королевстве
                labelDay = new VCToolLabel(MainControl, 0, 0, "", GUI_16_DAY);
                labelDay.Click += LabelDay_Click;
                labelDay.ShowHint += LabelDay_ShowHint;
                labelDay.Width = 48;
                labelGold = new VCToolLabel(MainControl, labelDay.NextLeft(), 0, "", GUI_16_GOLD);
                labelGold.ShowHint += LabelGold_ShowHint;
                labelGold.Width = 160;

                // Кнопки в правом верхнем углу
                btnPreferences = CreateButton(ilGui, GUI_INVENTORY, 0, labelDay.NextTop(), BtnPreferences_Click, BtnPreferences_MouseHover);
                btnHelp = CreateButton(ilGui, GUI_BOOK, 0, btnPreferences.ShiftY, BtnHelp_Click, BtnHelp_MouseHover);
                btnQuit = CreateButton(ilGui, GUI_EXIT, 0, btnPreferences.ShiftY, BtnQuit_Click, BtnQuit_MouseHover);

                btnEndTurn = CreateButton(ilGui, GUI_HOURGLASS, 0, btnPreferences.ShiftY, BtnEndTurn_Click, BtnEndTurn_MouseHover);
                panelLairWithFlags = new VisualControl(MainControl, 0, btnEndTurn.ShiftY);

                // Создаем панели игроков в верхней части окна
                panelPlayers = new VisualControl();

                PanelPlayer pp;
                int nextLeftPanelPlayer = 0;
                foreach (Player p in lobby.Players)
                {
                    pp = new PanelPlayer(panelPlayers, nextLeftPanelPlayer, 0);
                    // !!! Эту привязку переместить в StartNewLobby()
                    pp.LinkToLobby(p);
                    nextLeftPanelPlayer = pp.NextLeft();
                }
                panelPlayers.ApplyMaxSize();

                // Отладочная информация
                vcDebugInfo = new VisualControl();
                labelTimeDrawFrame = new VCLabel(vcDebugInfo, Config.GridSize, Config.GridSize, Config.FontToolbar, Color.White, 16, "");
                labelTimeDrawFrame.StringFormat.Alignment = StringAlignment.Near;
                labelTimePaintFrame = new VCLabel(vcDebugInfo, labelTimeDrawFrame.ShiftX, labelTimeDrawFrame.NextTop(), Config.FontToolbar, Color.White, 16, "Paint frame: 00000");
                labelTimePaintFrame.StringFormat.Alignment = StringAlignment.Near;
                vcDebugInfo.ArrangeControls();
                vcDebugInfo.ApplyMaxSize();

                // Страницы игры
                pageGuilds = new VCFormPage(MainControl, 0, 0, pages, ilGui, GUI_GUILDS, "Гильдии", BtnPage_Click);
                pageGuilds.ShowHint += PageGuilds_ShowHint;
                pageBuildings = new VCFormPage(MainControl, 0, 0, pages, ilGui, GUI_ECONOMY, "Экономические строения", BtnPage_Click);
                pageBuildings.ShowHint += PageBuildings_ShowHint;
                pageTemples = new VCFormPage(MainControl, 0, 0, pages, ilGui, GUI_TEMPLE, "Храмы", BtnPage_Click);
                pageHeroes = new VCFormPage(MainControl, 0, 0, pages, ilGui, GUI_HEROES, "Герои", BtnPage_Click);
                pageHeroes.ShowCostZero = true;
                pageHeroes.ShowHint += PageHeroes_ShowHint;
                pageLairs = new VCFormPage(MainControl, 0, 0, pages, ilGui, GUI_BATTLE, "Логова", BtnPage_Click);
                pageTournament = new VCFormPage(MainControl, 0, 0, pages, ilGui, GUI_TOURNAMENT, "Турнир", BtnPage_Click);
                pageTournament.ShowHint += PageTournament_ShowHint;
                pageTournament.Visible = false;

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
                int leftForNextButtonPage = 0;
                foreach (VCFormPage fp in pages)
                {
                    fp.ShiftX = leftForNextButtonPage;
                    fp.ShiftY = btnQuit.ShiftY;
                    fp.Page.Width = maxWidthPages;

                    leftForNextButtonPage = fp.NextLeft();
                }

                // Панели информации. Их располагаем после страниц
                panelHeroInfo = new PanelHeroInfo(MainControl, maxWidthPages + Config.GridSize, btnQuit.NextTop());
                panelBuildingInfo = new PanelBuildingInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelLairInfo = new PanelLairInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelMonsterInfo = new PanelMonsterInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
                panelEmptyInfo = new VisualControl(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY)
                {
                    Width = panelHeroInfo.Width,
                    Height = panelHeroInfo.Height,
                    ShowBorder = true
                };

                // Создаем меню
                bitmapMenu = new VCBitmap(MainControl, 0, panelHeroInfo.NextTop(), new Bitmap(dirResources + @"Icons\Menu.png"));
                Debug.Assert(panelHeroInfo.Width >= bitmapMenu.Width);

                CellsMenu = new VCMenuCell[PANEL_MENU_CELLS.Height, PANEL_MENU_CELLS.Width];
                for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                    for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                        CellsMenu[y, x] = new VCMenuCell(bitmapMenu, DISTANCE_BETWEEN_CELLS + (x * (ilItems.Size + DISTANCE_BETWEEN_CELLS)), DISTANCE_BETWEEN_CELLS + (y * (ilItems.Size + DISTANCE_BETWEEN_CELLS)), ilItems);

                bitmapMenu.ShiftX = panelHeroInfo.ShiftX + ((panelHeroInfo.Width - bitmapMenu.Width) / 2);

                // Все контролы созданы, устанавливаем размеры MainControl
                MainControl.Width = panelHeroInfo.ShiftX + panelEmptyInfo.Width;
                MainControl.Height = pageGuilds.NextTop() + maxHeightPages;

                sizeGamespace = new Size(Config.GridSize + MainControl.Width + Config.GridSize, panelPlayers.NextTop() + Config.GridSize + MainControl.NextTop() + Config.GridSize);
                Width = Width - ClientSize.Width + sizeGamespace.Width;
                Height = Height - ClientSize.Height + sizeGamespace.Height;

                bitmapMenu.ShiftY = MainControl.Height - bitmapMenu.Height;
                panelBuildingInfo.Height = MainControl.Height - panelBuildingInfo.ShiftY - bitmapMenu.Height - Config.GridSize;
                panelLairInfo.Height = panelBuildingInfo.Height;
                panelHeroInfo.Height = panelBuildingInfo.Height;
                panelMonsterInfo.Height = panelBuildingInfo.Height;
                panelEmptyInfo.Height = panelBuildingInfo.Height;

                btnQuit.ShiftX = MainControl.Width - btnQuit.Width;
                btnHelp.PlaceBeforeControl(btnQuit);
                btnPreferences.PlaceBeforeControl(btnHelp);
                btnEndTurn.ShiftX = panelBuildingInfo.ShiftX - btnEndTurn.Width - Config.GridSize;

                panelBuildingInfo.ShiftX = maxWidthPages + Config.GridSize;
                panelLairInfo.ShiftX = panelBuildingInfo.ShiftX;
                panelHeroInfo.ShiftX = panelBuildingInfo.ShiftX;
                panelMonsterInfo.ShiftX = panelBuildingInfo.ShiftX;
                panelEmptyInfo.ShiftX = panelBuildingInfo.ShiftX;

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

                formHint = new FormHint(null, null);
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

                //
                Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
                ApplyFullScreen(true);

                allowResize = true;

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

        private void LabelDay_Click(object sender, EventArgs e)
        {
            debugMode = !debugMode;
            ShowFrame(true);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // При старте игры в полноэкранном режиме, если курсор находится на пустом пространстве, окно игры состоит из белого фона
            // Показ кадра при старте отрисовывает окно
            ShowFrame(true);
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

                axWindowsMediaPlayer1.URL = dirResources + @"Video\Rebirth.ogg";
                axWindowsMediaPlayer1.uiMode = "none";
                axWindowsMediaPlayer1.Location = new Point(0, 0);
                axWindowsMediaPlayer1.enableContextMenu = false;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.MouseDownEvent += AxWindowsMediaPlayer1_MouseDownEvent;
                axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
            }
            else
                axWindowsMediaPlayer1.Dispose();
        }

        internal void ApplyFullScreen(bool force)
        {
            if (force || (MaximizeBox != Settings.FullScreenMode))
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

                //panelPlayers.Visible = false;
                //MainControl.Visible = false;
                //ShowFrame();
                //panelPlayers.Visible = true;
                //MainControl.Visible = true;
                ShowFrame(true);
                //Application.DoEvents();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (allowResize)
                ArrangeControls();

            if (axWindowsMediaPlayer1 != null)
                axWindowsMediaPlayer1.Size = ClientSize;
        }

        private void PrepareBackground()
        {
            if ((bmpBackground == null) || !bmpBackground.Size.Equals(ClientSize))
            {
                // Переформировываем картинку фона
                bmpBackground?.Dispose();
                bmpBackground = GuiUtils.MakeBackground(ClientSize);

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
            formHint.AddStep3Requirement("Не выбрано логово для атаки");
            formHint.ShowHint(btnEndTurn);
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

            e.Cancel = MessageBox.Show("Выйти из игры?", NAME_PROJECT, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
        }

        private void ArrangeControls()
        {
            shiftControls = new Point(Config.GridSize, Config.GridSize);

            if (Settings.FullScreenMode)
            {
                shiftControls.X = (ClientSize.Width - sizeGamespace.Width) / 2;
                shiftControls.Y = (ClientSize.Height - sizeGamespace.Height) / 2;

                Debug.Assert(shiftControls.X >= 0);
                Debug.Assert(shiftControls.Y >= 0);
            }
            else
            {
                Size = new Size(Width - ClientSize.Width + sizeGamespace.Width, Height - ClientSize.Height + sizeGamespace.Height);
            }

            panelPlayers.SetPos((ClientSize.Width - panelPlayers.Width) / 2, shiftControls.Y);
            MainControl.SetPos(shiftControls.X, panelPlayers.Top + panelPlayers.Height + Config.GridSize + Config.GridSize);
            MainControl.ArrangeControls();

            AdjustPanelLairsWithFlags();

            point1LineAfterPanelPlayers = new Point(MainControl.Left, panelPlayers.Top + panelPlayers.Height + Config.GridSize);
            point2LineAfterPanelPlayers = new Point(MainControl.Left + MainControl.Width, panelPlayers.Top + panelPlayers.Height + Config.GridSize);
            rectBorderAroungGamespace = new Rectangle(shiftControls.X - Config.GridSize - 1, shiftControls.Y - Config.GridSize - 1, sizeGamespace.Width + 2, sizeGamespace.Height + 2);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            KeyDown -= FormMain_KeyDown;
            KeyPreview = false;
        }

        private void AxWindowsMediaPlayer1_MouseDownEvent(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState != (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Dispose();
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
            Application.DoEvents();
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
        }

        internal void UpdateListHeroes()
        {
            List<ICell> listHeroes = new List<ICell>();
            for (int y = 0; y < lobby.CurrentPlayer.CellHeroes.GetLength(0); y++)
                for (int x = 0; x < lobby.CurrentPlayer.CellHeroes.GetLength(1); x++)
                    listHeroes.Add(lobby.CurrentPlayer.CellHeroes[y, x]);

            panelHeroes.ApplyList(listHeroes);
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
        }

        private void DrawPageConstructions()
        {
            DrawPage(pageGuilds, Config.TypeGuilds.ToList<TypeConstruction>());
            DrawPage(pageBuildings, Config.TypeEconomicConstructions.ToList<TypeConstruction>());
            DrawPage(pageTemples, Config.TypeTemples.ToList<TypeConstruction>());

            void DrawPage(VCFormPage panel, List<TypeConstruction> list)
            {
                int top = panel.TopForControls;
                int left;
                int height = 0;

                for (int line = 1; line <= Config.BuildingMaxLines; line++)
                {
                    left = 0;

                    foreach (TypeConstruction tck in list)
                    {
                        if (tck.Line == line)
                        {
                            tck.Panel = new PanelConstruction(panel.Page, left, top, tck);

                            left += tck.Panel.Width + Config.GridSize;
                            height = tck.Panel.Height;
                        }
                    }

                    top += height + Config.GridSize;
                }
            }
        }

        private void DrawPageLair()
        {
            int top = pageLairs.TopForControls;
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
            panelHeroes.ShiftY = pageHeroes.TopForControls;

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

                SetNeedRedrawFrame();
            }
        }

        internal void ActivatePageLairs()
        {
            ActivatePage(pageLairs);
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            // При деактивации пересоздаем окно, иначе оно отображается под главной формой
            formHint.Dispose();
            //formHint = new FormHint(ilGui16, ilParameters);
            formHint = new FormHint(null, null);
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

        internal void SelectBuilding(PanelConstruction pb)
        {
            if (SelectedPanelBuilding != pb)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;
                if (SelectedHero != null)
                    SelectHero(null);
                if (SelectedMonster != null)
                    SelectMonster(null);
                if (SelectedPanelLair != null)
                    SelectLair(null);

                if (pb != null)
                    SelectPanelEntity(null);

                PanelConstruction oldSelected = SelectedPanelBuilding;
                SelectedPanelBuilding = pb;

                //if (oldSelected != null)
                //    oldSelected.Repaint();
                if (SelectedPanelBuilding != null)
                {
                    panelBuildingInfo.Building = SelectedPanelBuilding.Building;
                    //SelectedPanelBuilding.Repaint();
                    panelBuildingInfo.Visible = true;
                }
                else
                    panelBuildingInfo.Visible = false;

                SetNeedRedrawFrame();
            }
        }

        internal void SelectLair(PanelLair pl)
        {
            if (SelectedPanelLair != pl)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;
                if (SelectedHero != null)
                    SelectHero(null);
                if (SelectedMonster != null)
                    SelectMonster(null);
                if (SelectedPanelBuilding != null)
                    SelectBuilding(null);

                if (pl != null)
                    SelectPanelEntity(null);

                PanelLair oldSelected = SelectedPanelLair;
                SelectedPanelLair = pl;

                //if (oldSelected != null)
                //    oldSelected.Invalidate(true);
                if (SelectedPanelLair != null)
                {
                    panelLairInfo.Lair = SelectedPanelLair.Lair;
                    //SelectedPanelLair.Invalidate(true);
                    panelLairInfo.Visible = true;
                }
                else
                    panelLairInfo.Visible = false;

                SetNeedRedrawFrame();
            }
        }

        internal void SelectMonster(Monster m)
        {
            if (SelectedMonster != m)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;
                if (SelectedHero != null)
                    SelectHero(null);
                if (SelectedPanelBuilding != null)
                    SelectBuilding(null);
                if (SelectedPanelLair != null)
                    SelectLair(null);

                Monster oldSelected = SelectedMonster;
                SelectedMonster = m;

                //SelectedPanelEntity = null;

                //if (oldSelected != null)
                //    ((ICell)oldSelected).Panel.Invalidate(true);
                if (SelectedMonster != null)
                {
                    panelMonsterInfo.Monster = SelectedMonster;
                    //SelectedPanelEntity = ((ICell)SelectedHero).Panel;
                    //   ((ICell)SelectedHero).Panel.Invalidate(true);
                    panelMonsterInfo.Visible = true;
                }
                else
                    panelMonsterInfo.Visible = false;

                SetNeedRedrawFrame();
            }
        }

        internal void SelectHero(PlayerHero ph)
        {
            if (SelectedHero != ph)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;
                if (SelectedMonster != null)
                    SelectMonster(null);
                if (SelectedPanelBuilding != null)
                    SelectBuilding(null);
                if (SelectedPanelLair != null)
                    SelectLair(null);

                PlayerHero oldSelected = SelectedHero;
                SelectedHero = ph;

                //SelectedPanelEntity = null;

                //if (oldSelected != null)
                //    ((ICell)oldSelected).Panel.Invalidate(true);
                if (SelectedHero != null)
                {
                    panelHeroInfo.Hero = SelectedHero;
                    //SelectedPanelEntity = ((ICell)SelectedHero).Panel;
                    //   ((ICell)SelectedHero).Panel.Invalidate(true);
                    panelHeroInfo.Visible = true;
                }
                else
                    panelHeroInfo.Visible = false;

                SetNeedRedrawFrame();
            }
        }

        internal void SetNeedRedrawFrame()
        {
            needRedrawFrame = true;
        }

        internal void SelectPanelEntity(VCCell pe)
        {
            if (SelectedPanelEntity != pe)
            {
                VCCell oldPe = SelectedPanelEntity;
                SelectedPanelEntity = pe;

                SetNeedRedrawFrame();
            }
        }

        internal void UpdateMenu()
        {
            // Рисуем содержимое ячеек
            if (SelectedPanelBuilding != null)
            {
                Debug.Assert(SelectedPanelBuilding.Building != null);
                panelBuildingInfo.Building = SelectedPanelBuilding.Building;

                PlayerBuilding plb = SelectedPanelBuilding.Building;

                ClearMenu();

                if (plb.Building.Researches != null)
                    foreach (PlayerResearch pr in plb.Researches)
                    {
                        if (CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research == null)
                            CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research = pr;
                        else if (CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research.Research.Layer > pr.Research.Layer)
                            CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research = pr;
                    }
            }
            else
            {
                panelBuildingInfo.Visible = false;
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

        private void ShowHintForToolButton(Control c, string text, string hint)
        {
            formHint.Clear();
            formHint.AddStep1Header(text, "", hint);
            formHint.ShowHint(c);
        }

        private void ShowHintForToolButton(VisualControl c, string text, string hint)
        {
            formHint.Clear();
            formHint.AddStep1Header(text, "", hint);
            formHint.ShowHint(c);
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
            base.OnPaint(e);

            e.Graphics.DrawImage(bmpFrame, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        private void ShowFrame(bool force)
        {
            if (force || needRedrawFrame)
            {
                needRedrawFrame = false;

                if (debugMode)
                    startDebugAction = DateTime.Now;

                DrawFrame();// Готовим кадр
                if (debugMode && (controlWithHint != null) && (controlWithHint != MainControl))
                {
                    gfxFrame.DrawRectangle(penDebugBorder, controlWithHint.Rectangle);
                }

                if (debugMode)
                {
                    durationDrawFrame = DateTime.Now - startDebugAction;
                    labelTimeDrawFrame.Text = "Draw frame: " + durationDrawFrame.TotalMilliseconds.ToString();
                    labelTimePaintFrame.Text = "Paint frame: " + durationPaintFrame.TotalMilliseconds.ToString();
                    vcDebugInfo.Draw(gfxFrame);

                    startDebugAction = DateTime.Now;
                }

                Invalidate();// Рисуем кадр

                if (debugMode)
                {
                    durationPaintFrame = DateTime.Now - startDebugAction;
                }
            }
        }

        // Рисование кадра главной формы
        private void DrawFrame()
        {
            Debug.Assert(inDrawFrame == false);

            inDrawFrame = true;

            // Рисуем фон
            if ((bmpBackground == null) || !bmpBackground.Size.Equals(ClientSize))
                PrepareBackground();

            gfxFrame.CompositingMode = CompositingMode.SourceCopy;
            gfxFrame.DrawImageUnscaled(bmpBackground, 0, 0);

            // Рисуем контролы
            gfxFrame.CompositingMode = CompositingMode.SourceOver;

            if (panelPlayers.Visible)
                panelPlayers.Draw(gfxFrame);

            if (MainControl.Visible)
            {
                //
                labelGold.Text = lobby.CurrentPlayer.Gold.ToString() + " (+" + lobby.CurrentPlayer.Income().ToString() + ")";

                pageGuilds.PopupQuantity = lobby.CurrentPlayer.PointConstructionGuild;
                pageBuildings.PopupQuantity = lobby.CurrentPlayer.PointConstructionEconomic;
                pageHeroes.Cost = lobby.CurrentPlayer.CombatHeroes.Count;

                //
                UpdateMenu();

                //
                foreach (VisualControl vc in MainControl.Controls)
                {
                    if (vc.Visible)
                        vc.Draw(gfxFrame);
                }
            }

            //
            if (panelPlayers.Visible)
            {
                gfxFrame.DrawLine(Config.GetPenBorder(false), point1LineAfterPanelPlayers, point2LineAfterPanelPlayers);

                if (Settings.FullScreenMode)
                    gfxFrame.DrawRectangle(Config.GetPenBorder(false), rectBorderAroungGamespace);
            }

            //
            inDrawFrame = false;
        }

        internal VCButton CreateButton(BitmapList bitmapList, int imageIndex, int left, int top, EventHandler click, EventHandler showHint)
        {
            VCButton b = new VCButton(MainControl, left, top, bitmapList, imageIndex);
            b.Click += click;
            b.ShowHint += showHint;

            return b;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            TreatMouseMove(e.Button == MouseButtons.Left);
        }

        private VisualControl ControlUnderMouse()
        {
            VisualControl curControl;
            curControl = panelPlayers.GetControl(mousePos.X, mousePos.Y);
            if (curControl == null)
            {
                curControl = MainControl.GetControl(mousePos.X, mousePos.Y);
                if (curControl == MainControl)
                {
                    curControl = currentPage.Page.GetControl(mousePos.X, mousePos.Y);
                    if (curControl == currentPage)
                        curControl = null;
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
                        // Если над контролов водят мышкой, отсчет времени начинаем только после остановки
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

            ShowFrame(false);
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
                        ControlForHintLeave();// Контрол уже другой, отменяет подсказку
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
    }
}
