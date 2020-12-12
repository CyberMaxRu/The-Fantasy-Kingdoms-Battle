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
using System.Web;
using System.Net;
using System.IO;
using System.IO.Compression;


namespace Fantasy_King_s_Battle
{
    public partial class FormMain : Form
    {
        private const string NAME_PROJECT = "The Fantasy Kingdoms Battle";
        private const string VERSION = "0.2.1";
        private const string VERSION_POSTFIX = "в разработке";
        internal readonly string dirResources;

        // ImageList'ы
        internal readonly ImageList ilPlayerAvatars;
        internal readonly ImageList ilPlayerAvatarsBig;
        internal readonly ImageList ilResultBattle;
        internal readonly ImageList ilBuildings;
        internal readonly ImageList ilHeroes;
        internal readonly ImageList ilGui;
        internal readonly ImageList ilGui16;
        internal readonly ImageList ilGui24;
        internal readonly ImageList ilGui45;
        internal readonly ImageList ilGuiHeroes;
        internal readonly ImageList ilParameters;
        internal readonly ImageList ilItems;
        internal readonly ImageList ilStateHero;
        internal readonly ImageList ilMenuCellFilters;
        internal readonly ImageList ilPages;

        internal Brush brushQuantity;
        internal Brush brushCost;

        // Контролы главного меню
        private readonly Label labelDay;
        private readonly Label labelGold;
        private readonly Label labelPeasants;

        private readonly Button btnPreferences;
        private readonly Button btnHelp;
        private readonly Button btnQuit;

        private readonly Button btnPageGuilds;
        private readonly Button btnPageBuildings;
        private readonly Button btnPageTemples;
        private readonly Button btnPageHeroes;
        private readonly Button btnEndTurn;

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

        private Lobby lobby;
        private Player curAppliedPlayer;

        internal Lobby CurrentLobby { get { return lobby; } }

        internal readonly Bitmap bmpForBackground;
        internal readonly Bitmap bmpBackgroundButton;
        internal readonly Bitmap bmpBorderForIcon;
        internal readonly Bitmap bmpEmptyEntity;
        private Bitmap bmpBackground;
        internal readonly Bitmap bmpBorderBattlefield;
        internal int LengthSideBorderBattlefield { get; private set; }
        private int calcedWidth;
        private int calcedHeight;
        private Size minSizeForm;
        private Point shiftControls;
        private int maxWidthPages;


        private readonly List<PanelPage> pages = new List<PanelPage>();
        private readonly PanelPage pageGuilds;
        private readonly PanelPage pageBuildings;
        private readonly PanelPage pageTemples;
        private readonly PanelPage pageHeroes;
        private PanelPage currentPage;
        private readonly int leftForPages;
        private readonly int heightToolBar;
        private readonly int heightBandLobby;
        private readonly int heightBandBuildings;
        private readonly int heightBandInfoAndMenu;
        private readonly Point pointMenu;
        private readonly PanelMenu panelMenu;
        private readonly PanelBuildingInfo panelBuildingInfo;
        private readonly PanelHeroInfo panelHeroInfo;
        internal PanelEntity SelectedPanelEntity;
        private readonly List<PanelPlayer> panelPlayers = new List<PanelPlayer>();
        private readonly List<PanelBuilding> listPanelBuildings = new List<PanelBuilding>();

        private List<PictureBox> SlotSkill = new List<PictureBox>();

        internal FormHint formHint;
        internal PanelBuilding SelectedPanelBuilding { get; private set; }
        internal PlayerHero SelectedHero { get; private set; }

        internal static Random Rnd = new Random();

        //
        internal Settings Settings { get; private set; }
        internal MainConfig MainConfig { get; private set; }

        public FormMain()
        {
            InitializeComponent();

            Program.formMain = this;

            Text = NAME_PROJECT + " (сборка " + VERSION + ")";

            // Настройка переменной с папкой ресурсов
            dirResources = Environment.CurrentDirectory;

            if (dirResources.Contains("Debug"))
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
            else if (dirResources.Contains("Release"))
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
            else
                dirResources += "\\";

            dirResources += "Resources\\";

            // Загружаем настройки
            Settings = new Settings(dirResources);

            MainConfig = new MainConfig(dirResources);

            // Если включено автообновление, проверяем на их наличие
            if (Settings.CheckUpdateOnStartup)
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
                }
            }

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
            ilPlayerAvatars = PrepareImageList("PlayerAvatars.png", 48, 48, true);
            ilPlayerAvatarsBig = PrepareImageList("PlayerAvatarsBig.png", 128, 128, true);
            ilResultBattle = PrepareImageList("ResultBattle.png", 24, 24, false);
            ilBuildings = PrepareImageList("Buildings.png", 126, 126, true);
            ilHeroes = PrepareImageList("Heroes.png", 126, 126, false);
            ilGui = PrepareImageList("Gui.png", 48, 48, true);
            ilGuiHeroes = PrepareImageList("GuiHeroes.png", 48, 48, true);
            ilGui16 = PrepareImageList("Gui16.png", 16, 16, false);
            ilGui24 = PrepareImageList("Gui24.png", 24, 24, false);
            ilGui45 = PrepareImageList("Gui45.png", 45, 45, false);
            ilParameters = PrepareImageList("Parameters.png", 24, 24, false);
            ilItems = PrepareImageList("Items.png", 48, 48, true);
            ilStateHero = PrepareImageList("StateHero.png", 24, 24, false);
            ilMenuCellFilters = PrepareImageList("MenuCellFilters.png", 48, 48, true);
            ilPages = PrepareImageList("Pages.png", 48, 48, true);

            bmpForBackground = new Bitmap(dirResources + "Icons\\Background.png");
            bmpBackgroundButton = GuiUtils.MakeBackground(GuiUtils.SizeButtonWithImage(ilGui));
            bmpBorderForIcon = new Bitmap(dirResources + "Icons\\BorderIconEntity.png");
            bmpEmptyEntity = new Bitmap(dirResources + "Icons\\EmptyEntity.png");
            bmpBorderBattlefield = new Bitmap(dirResources + "Icons\\BorderBattlefield.png");
            LengthSideBorderBattlefield = bmpBorderBattlefield.Width - (Config.WidthBorderBattlefield * 2);
            Debug.Assert(LengthSideBorderBattlefield > 0);

            // Создаем лобби
            // Переместить уже после создания всех контролов, чтобы обеспечить связь лобби-контролы
            lobby = new Lobby(Config.TypeLobbies[0]);

            SetStage("Строим замок");

            // Кнопки в правом верхнем углу
            btnPreferences = GuiUtils.CreateButtonWithIcon(this, 0, Config.GridSize, GUI_INVENTORY);
            btnPreferences.Click += BtnPreferences_Click;
            btnHelp = GuiUtils.CreateButtonWithIcon(this, 0, Config.GridSize, GUI_BOOK);
            btnHelp.Click += BtnHelp_Click;
            btnQuit = GuiUtils.CreateButtonWithIcon(this, 0, Config.GridSize, GUI_EXIT);
            btnQuit.Click += BtnQuit_Click;

            heightToolBar = btnQuit.Height + (Config.GridSize * 2);

            // Создаем иконки игроков в левой части окна
            PanelPlayer pp;
            foreach (Player p in lobby.Players)
            {
                pp = new PanelPlayer(this);
                pp.Player = p;
                panelPlayers.Add(pp);
                heightBandLobby = pp.Top + pp.Height;
            }

            leftForPages = GuiUtils.NextLeft(lobby.Players[0].Panel);

            // Текст с информацией о Королевстве
            labelDay = GuiUtils.CreateLabel(this, Config.GridSize, Config.GridSize, 80, "День");
            labelDay.Height = 20;
            labelDay.ImageList = ilGui16;
            labelDay.ImageIndex = GUI_16_DAY;
            labelDay.Font = Config.FontToolbar;
            labelDay.ImageAlign = ContentAlignment.MiddleLeft;
            labelDay.ForeColor = Color.White;
            labelDay.BackColor = Color.Transparent;
            labelDay.MouseHover += LabelDay_MouseHover;
            labelGold = GuiUtils.CreateLabel(this, Config.GridSize, labelDay.Top + labelDay.Height, 80, "");
            labelGold.Height = 20;
            labelGold.ImageList = ilGui16;
            labelGold.ImageIndex = GUI_16_GOLD;
            labelGold.Font = Config.FontToolbar;
            labelGold.ForeColor = Color.White;
            labelGold.BackColor = Color.Transparent;
            labelGold.ImageAlign = ContentAlignment.MiddleLeft;
            labelGold.MouseHover += LabelGold_MouseHover;
            labelPeasants = GuiUtils.CreateLabel(this, Config.GridSize, labelGold.Top + labelGold.Height, 80, "");
            labelPeasants.Height = 20;
            labelPeasants.ImageList = ilGui16;
            labelPeasants.ImageIndex = GUI_16_PEASANT;
            labelPeasants.Font = Config.FontToolbar;
            labelPeasants.ForeColor = Color.White;
            labelPeasants.BackColor = Color.Transparent;
            labelPeasants.ImageAlign = ContentAlignment.MiddleLeft;
            labelPeasants.MouseHover += LabelPeasants_MouseHover;

            // Страницы меню
            pageGuilds = PreparePanel();
            pageBuildings = PreparePanel();
            pageTemples = PreparePanel();
            pageHeroes = PreparePanel();
            pages.Add(pageGuilds);
            pages.Add(pageBuildings);
            pages.Add(pageTemples);
            pages.Add(pageHeroes);

            PanelPage PreparePanel()
            {
                return new PanelPage()
                {
                    Parent = this,
                    Left = leftForPages,
                    Top = GuiUtils.NextTop(btnQuit)
                };
            }

            // Кнопки страниц и конца хода
            btnPageGuilds = CreateButtonPage(pageGuilds, GUI_GUILDS);
            btnPageBuildings = CreateButtonPage(pageBuildings, GUI_ECONOMY);
            btnPageTemples = CreateButtonPage(pageTemples, GUI_TEMPLE);
            btnPageHeroes = CreateButtonPage(pageHeroes, GUI_HEROES);

            btnEndTurn = GuiUtils.CreateButtonWithIcon(this, 0, Config.GridSize, GUI_BATTLE);
            btnEndTurn.Text = "Конец хода";
            btnEndTurn.Width = 160;
            btnEndTurn.ImageAlign = ContentAlignment.MiddleLeft;
            btnEndTurn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnEndTurn.Font = Config.FontCaptionPage;
            btnEndTurn.ForeColor = Config.CommonCaptionPage;
            btnEndTurn.MouseHover += BtnEndTurn_MouseHover;
            btnEndTurn.Click += BtnEndTurn_Click;

            Button CreateButtonPage(PanelPage p, int imageIndex)
            {
                Button b = GuiUtils.CreateButtonWithIcon(this, 0, Config.GridSize, imageIndex);
                b.FlatAppearance.BorderColor = Color.DarkBlue;
                b.Tag = p;
                b.Click += BtnPage_Click;

                return b;
            }

            DrawGuilds();
            DrawBuildings();
            DrawTemples();
            DrawHeroes();
            DrawWarehouse();

            ShowDataPlayer();

            // Определяем максимальную ширину страниц
            int maxHeightPages = 0;

            foreach (PanelPage pc in pages)
            {
                foreach (Control c in pc.Controls)
                {
                    maxWidthPages = Math.Max(maxWidthPages, c.Left + c.Width);
                    maxHeightPages = Math.Max(maxHeightPages, c.Top + c.Height);
                }
            }

            foreach (PanelPage pc in pages)
            {
                pc.Width = maxWidthPages;
                pc.Height = maxHeightPages;
            }

            // Создаем панель с меню
            panelMenu = new PanelMenu(this, dirResources);
            panelMenu.Top = ClientSize.Height - panelMenu.Height - Config.GridSize;

            // Панель информации о здании
            panelBuildingInfo = new PanelBuildingInfo(panelMenu.Top - GuiUtils.NextTop(btnPageGuilds) - Config.GridSize)
            {
                Parent = this,
                Top = GuiUtils.NextTop(btnPageGuilds),
                Visible = false
            };

            //
            panelHeroInfo = new PanelHeroInfo(panelBuildingInfo.Height)
            {
                Parent = this,
                Top = panelBuildingInfo.Top,
                Visible = false
            };

            // Подбираем ширину правой части
            panelBuildingInfo.Width = panelHeroInfo.Width;
            int widthRightPanel = Math.Max(panelMenu.Width, panelHeroInfo.Width);
            Debug.Assert(widthRightPanel > panelMenu.Width);

            panelBuildingInfo.Left = leftForPages + maxWidthPages + Config.GridSize;
            panelHeroInfo.Left = panelBuildingInfo.Left;

            // Учитываем плиту под слоты
            pointMenu = new Point(leftForPages + maxWidthPages + Config.GridSize, ClientSize.Height - panelMenu.Height - Config.GridSize);
            pointMenu.X = pointMenu.X + ((widthRightPanel - panelMenu.Width) / 2);
            calcedWidth = leftForPages + maxWidthPages + widthRightPanel + Config.GridSize;

            // Определяем высоту бэнда зданий
            heightBandBuildings = 0;
            foreach (Building b in Config.Buildings)
                if (b.Panel != null)
                    heightBandBuildings = Math.Max(heightBandBuildings, b.Panel.Height + b.Panel.Top);

            // Определяем высоту бэнда информации
            //heightBandInfoAndMenu = panelMenu.Height + Math.Max(panelBuildingInfo.Height, panelHeroInfo.Height);

            //
            Width = (Width - ClientSize.Width) + calcedWidth + Config.GridSize;
            // Высота - это наибольшая высота бэндов лобби, зданий и информации с меню
            calcedHeight = heightToolBar + Math.Max(heightBandLobby, heightBandBuildings);
            Height = (Height - ClientSize.Height) + calcedHeight + Config.GridSize;
            minSizeForm = new Size(Width, Height);

            pointMenu.Y = ClientSize.Height - panelMenu.Height - Config.GridSize;

            panelMenu.Location = pointMenu;

            panelBuildingInfo.Height = ClientSize.Height - panelBuildingInfo.Top - panelMenu.Height - (Config.GridSize * 2);
            panelHeroInfo.Height = panelBuildingInfo.Height;

            SetStage("Прибираем после строителей");
            // Перенести в класс
            for (int i = 0; i < panelHeroInfo.slots.Count; i++)
            {
                //panelHeroInfo.slots[i].MouseDown += PanelCellHero_MouseDown;
                //panelHeroInfo.slots[i].MouseUp += PanelCellHero_MouseUp;
                //panelHeroInfo.slots[i].MouseMove += PanelCell_MouseMove;
            }

            //

            PrepareBackground();
            ArrangeControls();

            ActivatePage(pageGuilds);

            formHint = new FormHint(ilGui16, ilParameters);

            splashForm.Dispose();

            //
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
            ApplyFullScreen(true);

            // 
            if (Settings.ShowSplashVideo)
            {
                KeyDown += FormMain_KeyDown;
                KeyPreview = true;

                axWindowsMediaPlayer1.URL = dirResources + @"Video\Rebirth.ogg";
                axWindowsMediaPlayer1.uiMode = "none";
                axWindowsMediaPlayer1.Location = new Point(0, 0);
                axWindowsMediaPlayer1.Size = ClientSize;
                axWindowsMediaPlayer1.MouseDownEvent += AxWindowsMediaPlayer1_MouseDownEvent;
                axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else
                axWindowsMediaPlayer1.Dispose();

            void SetStage(string text)
            {
                lblStage.Text = text + "...";
                lblStage.Refresh();
            }
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
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                    MaximizeBox = false;
                    WindowState = FormWindowState.Normal;
                }

                ApplySize();
                PrepareBackground();
                ArrangeControls();

                Refresh();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Program.formMain != null)
            {
                PrepareBackground();
                ArrangeControls();
            }
        }

        private void PrepareBackground()
        {
            if ((bmpBackground == null) || !bmpBackground.Size.Equals(ClientSize))
            {
                bmpBackground?.Dispose();
                bmpBackground = GuiUtils.MakeBackground(ClientSize);
            }
        }

        private void ApplySize()
        {
            if (minSizeForm != default)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    Size = minSizeForm;
                }
            }
        }

        private void BtnEndTurn_Click(object sender, EventArgs e)
        {
            formHint.HideHint();

            btnEndTurn.Enabled = false;
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
                btnEndTurn.Enabled = true;
            }
        }

        private void BtnEndTurn_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(btnEndTurn, "Конец хода", "Завершение хода");
        }

        private void LabelPeasants_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(labelPeasants, "Крестьяне", "Количество свободных строителей");
        }

        private void LabelGold_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(labelGold, "Казна", "Количество золота в казне и постоянный доход в день");
        }

        private void LabelDay_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(labelDay, "День игры", "День игры: " + lobby.Turn.ToString());
        }

        private void BtnPage_Click(object sender, EventArgs e)
        {
            ActivatePage((PanelPage)((Button)sender).Tag);
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            FormSettings f = new FormSettings();
            f.ApplySettings(Settings);
            if (f.ShowDialog() == DialogResult.OK)
            {
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
                shiftControls.X = (ClientSize.Width - calcedWidth) / 2;
                shiftControls.Y = (ClientSize.Height - calcedHeight) / 2;
            }

            labelDay.Left = shiftControls.X;
            labelGold.Left = shiftControls.X;
            labelPeasants.Left = shiftControls.X;

            ShowLobby();

            pageGuilds.Left = shiftControls.X + leftForPages - Config.GridSize;
            pageBuildings.Left = pageGuilds.Left;
            pageTemples.Left = pageGuilds.Left;
            pageHeroes.Left = pageGuilds.Left;

            btnQuit.Left = shiftControls.X + minSizeForm.Width - btnQuit.Width - (Config.GridSize * 4);
            btnHelp.Left = btnQuit.Left - btnQuit.Width - Config.GridSize;
            btnPreferences.Left = btnHelp.Left - btnHelp.Width - Config.GridSize;

            btnPageGuilds.Left = leftForPages + shiftControls.X - Config.GridSize;
            btnPageBuildings.Left = GuiUtils.NextLeft(btnPageGuilds);
            btnPageTemples.Left = GuiUtils.NextLeft(btnPageBuildings);
            btnPageHeroes.Left = GuiUtils.NextLeft(btnPageTemples);

            panelBuildingInfo.Left = shiftControls.X + leftForPages + maxWidthPages;
            panelHeroInfo.Left = panelBuildingInfo.Left;

            btnEndTurn.Left = panelBuildingInfo.Left - btnEndTurn.Width - Config.GridSize;

            panelMenu.Left = shiftControls.X + pointMenu.X - Config.GridSize;
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
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsStopped)
                axWindowsMediaPlayer1.Dispose();
        }

        internal static Config Config { get; set; }
        internal ImageList PrepareImageList(string filename, int width, int height, bool convertToGrey)
        {
            ImageList il = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(width, height)
            };

            Bitmap bmp = new Bitmap(dirResources + "Icons\\" + filename);
            // Если это многострочная картинка, нарезаем ее в однострочную картинку
            if (bmp.Height % height != 0)
                throw new Exception("Высота многострочной картинки не кратна высоте строки: " + filename);

            AddBitmapToImageList(bmp);

            if (convertToGrey == true)
                AddBitmapToImageList(GreyBitmap());

            return il;

            void AddBitmapToImageList(Bitmap bitmap)
            {
                int lines = bitmap.Height / height;
                if (lines > 1)
                {
                    for (int i = 0; i < lines; i++)
                    {
                        Bitmap bmpSingleline = new Bitmap(bitmap.Width, height);
                        Graphics g = Graphics.FromImage(bmpSingleline);
                        g.DrawImage(bitmap, 0, 0, new Rectangle(0, i * height, bitmap.Width, height), GraphicsUnit.Pixel);
                        _ = il.Images.AddStrip(bmpSingleline);
                        g.Dispose();
                    }
                }
                else
                {
                    _ = il.Images.AddStrip(bitmap);
                }
            }

            Bitmap GreyBitmap()
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
        }

        internal void ShowDataPlayer()
        {
            Debug.Assert(lobby.CurrentPlayer.TypePlayer == TypePlayer.Human);

            labelDay.Text = "     " + lobby.Turn.ToString();
            
            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayer)
            {
                //DrawExternalBuilding();

                curAppliedPlayer = lobby.CurrentPlayer;
            }

            ShowLobby();
            ShowGuilds();
            ShowBuildings();
            ShowTemples();
            ShowPageHeroes();
            ShowGold();
        }

        private void ShowLobby()
        {
            int top = heightToolBar;
            foreach (Player p in lobby.Players.OrderBy(p => p.PositionInLobby))
            {
                Debug.Assert(p.PositionInLobby >= 1);
                Debug.Assert(p.PositionInLobby <= lobby.TypeLobby.QuantityPlayers);

                p.Panel.Left = shiftControls.X;
                p.Panel.Top = top;
                top = GuiUtils.NextTop(p.Panel);
            }

            //Refresh();
        }

        private void DrawPageBuilding(PanelPage panel, CategoryBuilding category)
        {
            int top = 0;
            int left;
            int height = 0;

            for (int line = 1; line <= Config.BuildingMaxLines; line++)
            {
                left = 0;

                foreach (Building b in Config.Buildings)
                {
                    if ((b.CategoryBuilding == category) && (b.Line == line))
                    {
                        b.Panel = new PanelBuilding()
                        {
                            Parent = panel,
                            Location = new Point(left, top)
                        };

                        left += b.Panel.Width + Config.GridSize;
                        height = b.Panel.Height;
                    }
                }

                top += height + Config.GridSize;
            }
        }

        private void DrawGuilds()
        {
            DrawPageBuilding(pageGuilds, CategoryBuilding.Guild);
        }

        private void ShowGuilds()
        {
            foreach (PlayerBuilding pg in lobby.CurrentPlayer.Buildings)
            {
                if (pg.Building.CategoryBuilding == CategoryBuilding.Guild)
                    pg.UpdatePanel();
            }
        }

        private void DrawBuildings()
        {
            DrawPageBuilding(pageBuildings, CategoryBuilding.Castle);
        }

        private void ShowBuildings()
        {
            foreach (PlayerBuilding pb in lobby.CurrentPlayer.Buildings)
            {
                pb.UpdatePanel();
            }
        }

        private void DrawTemples()
        {
            DrawPageBuilding(pageTemples, CategoryBuilding.Temple);
        }

        private void ShowTemples()
        {
            foreach (PlayerBuilding pb in lobby.CurrentPlayer.Buildings)
            {
                if (pb.Building.CategoryBuilding == CategoryBuilding.Temple)
                    pb.UpdatePanel();
            }
        }

        private void DrawHeroes()
        {
            panelHeroes = new PanelWithPanelEntity(Config.HeroInRow)
            {
                Parent = pageHeroes,
                Left = 0,
                Top = 0
            };

            List<ICell> list = new List<ICell>();
            for (int x = 0; x < Config.HeroInRow * Config.HeroInRow; x++)
                list.Add(null);

            panelHeroes.ApplyList(list);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Debug.Assert(bmpBackground.Size.Equals(ClientSize));

            base.OnPaintBackground(e);

            if (bmpBackground != null)
            {
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                // Рисуем подложку
                e.Graphics.DrawImage(bmpBackground, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
        }

        internal void ShowPageHeroes()
        {
            List<ICell> listHeroes = new List<ICell>();
            for (int y = 0; y < lobby.CurrentPlayer.CellHeroes.GetLength(0); y++)
                for (int x = 0; x < lobby.CurrentPlayer.CellHeroes.GetLength(1); x++)
                    listHeroes.Add(lobby.CurrentPlayer.CellHeroes[y, x]);

            panelHeroes.ApplyList(listHeroes);

            ShowWarehouse();
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

        internal void ShowGold()
        {
            Debug.Assert(lobby.CurrentPlayer.TypePlayer == TypePlayer.Human);

            labelGold.Text = "     " + lobby.CurrentPlayer.Gold.ToString() + " (+" + lobby.CurrentPlayer.Income().ToString() + ")";
            labelPeasants.Text = "     " + lobby.CurrentPlayer.FreeBuilders.ToString() + "/" + lobby.CurrentPlayer.TotalBuilders.ToString();
        }

        internal void ShowAllBuildings()
        {
            ShowGuilds();
            ShowBuildings();
            ShowTemples();

            UpdateMenu();
        }

        private void DrawWarehouse()
        {
            panelWarehouse = new PanelWithPanelEntity(Config.WarehouseWidth)
            {
                Parent = pageHeroes,
                Left = 0,
                Top = panelHeroes.Height + Config.GridSize
            };
        }

        internal void ShowWarehouse()
        {
            panelWarehouse.ApplyList(lobby.CurrentPlayer.Warehouse.ToList<ICell>());
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            ActivatePage(pages[e.TabPageIndex]);
        }

        private void ActivatePage(PanelPage pc)
        {
            currentPage?.Hide();
            pc.Show();
            currentPage = pc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowBattle();
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            // При деактивации пересоздаем окно, иначе оно отображается под главной формой
            formHint.Dispose();
            formHint = new FormHint(ilGui16, ilParameters);
        }

        private void StartNewLobby()
        {
            lobby = new Lobby(Config.TypeLobbies[0]);

            DrawLobby();

            ShowDataPlayer();

            btnEndTurn.Enabled = true;
        }

        private void DrawLobby()
        {
            for (int i = 0; i < panelPlayers.Count; i++)
            {
                panelPlayers[i].Player = lobby.Players[i];
            }
        }

        internal void SelectBuilding(PanelBuilding pb)
        {
            if (SelectedPanelBuilding != pb)
            {
                if (SelectedHero != null)
                    SelectHero(null);

                SelectPanelEntity(null);

                PanelBuilding oldSelected = SelectedPanelBuilding;
                SelectedPanelBuilding = pb;

                UpdateMenu();

                if (oldSelected != null)
                    oldSelected.Invalidate(true);
                if (SelectedPanelBuilding != null)
                {
                    panelBuildingInfo.Building = SelectedPanelBuilding.Building;
                    SelectedPanelBuilding.Invalidate(true);
                    panelBuildingInfo.Show();
                }
                else
                    panelBuildingInfo.Hide();

                panelMenu.Invalidate(true);// Это точно надо?
            }
        }

        internal void SelectHero(PlayerHero ph)
        {
            if (SelectedHero != ph)
            {
                if (SelectedPanelBuilding != null)
                    SelectBuilding(null);

                PlayerHero oldSelected = SelectedHero;
                SelectedHero = ph;

                //SelectedPanelEntity = null;

                UpdateMenu();

                if (oldSelected != null)
                    ((ICell)oldSelected).Panel.Invalidate(true);
                if (SelectedHero != null)
                {
                    panelHeroInfo.Hero = SelectedHero;
                    //SelectedPanelEntity = ((ICell)SelectedHero).Panel;
                    ((ICell)SelectedHero).Panel.Invalidate(true);
                    panelHeroInfo.Show();
                }
                else
                    panelHeroInfo.Hide();

                panelMenu.Invalidate(true);// Это точно надо?

            }
        }

        internal void SelectPanelEntity(PanelEntity pe)
        {
            if (SelectedPanelEntity != pe)
            {
                PanelEntity oldPe = SelectedPanelEntity;
                SelectedPanelEntity = pe;

                oldPe?.Invalidate();
                SelectedPanelEntity?.Invalidate();
            }
        }

        internal void UpdateMenu()
        {
            // Рисуем содержимое ячеек
            if (SelectedPanelBuilding != null)
            {
                panelBuildingInfo.Building = SelectedPanelBuilding.Building;

                PlayerBuilding plb = SelectedPanelBuilding.Building;

                ClearMenu();

                if (plb.Building.Researches != null)
                    foreach (PlayerResearch pr in plb.Researches)
                    {
                        if (panelMenu.CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research == null)
                            panelMenu.CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research = pr;
                        else if (panelMenu.CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research.Research.Layer > pr.Research.Layer)
                            panelMenu.CellsMenu[pr.Research.Coord.Y, pr.Research.Coord.X].Research = pr;
                    }
            }
            else
            {
                panelBuildingInfo.Hide();
                ClearMenu();
            }

            void ClearMenu()
            {
                for (int y = 0; y < PANEL_MENU_CELLS.Height; y++)
                    for (int x = 0; x < PANEL_MENU_CELLS.Width; x++)
                        panelMenu.CellsMenu[y, x].Research = null;
            }
        }

        internal void UpdateBuildingInfo()
        {
            Debug.Assert(panelBuildingInfo != null);

            panelBuildingInfo.ShowData();
        }


        private void ShowHintForToolButton(Control c, string text, string hint)
        {
            formHint.Clear();
            formHint.AddStep1Header(text, "", hint);
            formHint.ShowHint(c);
        }

        private void tsl_MouseLeave(object sender, EventArgs e)
        {
            formHint.HideHint();
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            formHint.HideHint();
        }
    }
}
