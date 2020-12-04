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

namespace Fantasy_King_s_Battle
{
    public partial class FormMain : Form
    {
        private readonly string dirResources;

        internal readonly ImageList ilPlayerAvatars;
        internal readonly ImageList ilPlayerAvatarsBig;
        private readonly ImageList ilSkills;
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

        internal readonly Font fontLevel = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        internal readonly Font fontQuantity = new Font("Arial", 13, FontStyle.Bold);
        internal readonly Font fontCost = new Font("Arial", 12, FontStyle.Bold);
        internal readonly Color ColorLevel = Color.Yellow;
        internal readonly Color ColorCost = Color.White;
        internal readonly Color ColorQuantity = Color.Yellow;
        internal readonly Brush brushQuantity = new SolidBrush(Color.Yellow);
        internal readonly Brush brushCost = new SolidBrush(Color.White);

        internal readonly Font fontToolBar = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);

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

        internal const int GUI_16_GOLD = 0;
        internal const int GUI_16_PEASANT = 1;

        internal const int GUI_24_FIRE = 0;
        internal const int GUI_24_HEROES = 1;
        internal const int GUI_24_STAR = 2;

        internal const int GUI_45_EMPTY = 0;
        internal const int GUI_45_BORDER = 0;

        internal static Size PANEL_MENU_CELLS = new Size(4, 3);

        private Lobby lobby;
        private Player curAppliedPlayer;

        internal static bool ShowGrid { get; set; } = true;

        internal readonly Bitmap bmpForBackground;
        internal readonly Bitmap bmpBackgroundButton;
        internal readonly Bitmap bmpBorderForIcon;
        internal readonly Bitmap bmpEmptyEntity;
        private readonly Bitmap bmpBackground;
        internal readonly Bitmap bmpBorderBattlefield;
        internal int LengthSideBorderBattlefield { get; private set; }

        private readonly List<PanelPage> pages = new List<PanelPage>();
        private readonly PanelPage pageLobby;
        private readonly PanelPage pageGuilds;
        private readonly PanelPage pageBuildings;
        private readonly PanelPage pageTemples;
        private readonly PanelPage pageTowers;
        private readonly PanelPage pageHeroes;
        private readonly PanelPage pageBattle;
        private PanelPage currentPage;
        private readonly int leftForPages;
        private readonly Point pointMenu;
        private readonly PanelMenu panelMenu;
        private readonly PanelBuildingInfo panelBuildingInfo;
        private readonly PanelHeroInfo panelHeroInfo;
        internal PanelEntity SelectedPanelEntity;

        private List<PictureBox> SlotSkill = new List<PictureBox>();

        internal FormHint formHint;
        internal PanelBuilding SelectedPanelBuilding { get; private set; }
        internal PlayerHero SelectedHero { get; private set; }

        internal static Random Rnd = new Random();

        public FormMain()
        {
            InitializeComponent();

            Program.formMain = this;

            // Настройка переменной с папкой ресурсов
            dirResources = Environment.CurrentDirectory;

            if (dirResources.Contains("Debug"))
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
            else if (dirResources.Contains("Release"))
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
            else
                dirResources += "\\";

            dirResources += "Resources\\";

            // Загружаем конфигурацию
            _ = new Config(dirResources, this);

            // Загружаем иконки
            ilPlayerAvatars = PrepareImageList("PlayerAvatars.png", 48, 48, true);
            ilPlayerAvatarsBig = PrepareImageList("PlayerAvatarsBig.png", 128, 128, true);
            ilSkills = PrepareImageList("Skills.png", 82, 94, false);
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
            lobby = new Lobby(Config.TypeLobbies[0]);

            // Подготавливаем тулбар
            toolStripMain.ImageList = ilGui;
            toolStripMain.ImageScalingSize = ilGui.ImageSize;

            tslDay.Font = fontToolBar;
            tslGold.ImageIndex = GUI_BUY;
            tslGold.Font = fontToolBar;
            tslBuilders.ImageIndex = GUI_PEASANT;
            tslBuilders.Font = fontToolBar;
            tslHeroes.ImageIndex = GUI_HEROES;
            tslHeroes.Font = fontToolBar;
            tsbEndTurn.ImageIndex = GUI_HOURGLASS;

            // Создаем иконки игроков в левой части окна
            foreach (Player p in lobby.Players)
            {
                new PanelPlayer(p, this);
            }

            leftForPages = GuiUtils.NextLeft(lobby.Players[0].Panel);

            tabControl1.Top = GuiUtils.NextTop(toolStripMain);
            tabControl1.Left = leftForPages;

            pageLobby = PreparePanel();
            pageGuilds = PreparePanel();
            pageBuildings = PreparePanel();
            pageTemples = PreparePanel();
            pageTowers = PreparePanel();
            pageHeroes = PreparePanel();
            pageBattle = PreparePanel();
            pages.Add(pageLobby);
            pages.Add(pageGuilds);
            pages.Add(pageBuildings);
            pages.Add(pageTemples);
            pages.Add(pageTowers);
            pages.Add(pageHeroes);
            pages.Add(pageBattle);
            
            tabControl1.ImageList = ilGui;
            tabPageLobby.ImageIndex = GUI_LOBBY;
            tabPageLobby.Text = "";
            tabPageGuilds.ImageIndex = GUI_GUILDS;
            tabPageGuilds.Text = "";
            tabPageBuildings.ImageIndex = GUI_ECONOMY;
            tabPageBuildings.Text = "";
            tabPageTemples.ImageIndex = GUI_TEMPLE;
            tabPageTemples.Text = "";
            tabPageTowers.ImageIndex = GUI_DEFENSE;
            tabPageTowers.Text = "";
            tabPageHeroes.ImageIndex = GUI_HEROES;
            tabPageHeroes.Text = "";
            tabPageBattle.ImageIndex = GUI_BATTLE;
            tabPageBattle.Text = "";

            DrawLobby();
            DrawGuilds();
            DrawBuildings();
            DrawTemples();
            DrawTowers();
            DrawHeroes();
            DrawWarehouse();

            PanelPage PreparePanel()
            {
                return new PanelPage()
                {
                    Parent = this,
                    Left = leftForPages,
                    Top = GuiUtils.NextTop(tabControl1)
                };
            }

            ShowDataPlayer();

            // Определяем максимальную ширину страниц
            int maxWidthPages = 0;
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
            panelBuildingInfo = new PanelBuildingInfo(panelMenu.Top - GuiUtils.NextTop(tabControl1) - Config.GridSize)
            {
                Parent = this,
                Top = GuiUtils.NextTop(tabControl1),
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
            int calcedWidth = leftForPages + maxWidthPages + widthRightPanel + Config.GridSize;

            panelMenu.Location = pointMenu;

            Width = (Width - ClientSize.Width) + calcedWidth + Config.GridSize;
            Height = GuiUtils.NextTop(lobby.Players[lobby.Players.Length - 1].Panel) + (Height - ClientSize.Height);
            tabControl1.Width = ClientSize.Width - tabControl1.Left - Config.GridSize;
            pointMenu.Y = ClientSize.Height - panelMenu.Height - Config.GridSize;

            // Подготавливаем подложку
            bmpBackground = GuiUtils.MakeBackground(ClientSize);

            //
            toolStripMain.BackgroundImage = GuiUtils.MakeBackground(toolStripMain.Size);
            toolStripMain.ForeColor = Color.White;

            // Перенести в класс
            for (int i = 0; i < panelHeroInfo.slots.Count; i++)
            {
                //panelHeroInfo.slots[i].MouseDown += PanelCellHero_MouseDown;
                //panelHeroInfo.slots[i].MouseUp += PanelCellHero_MouseUp;
                //panelHeroInfo.slots[i].MouseMove += PanelCell_MouseMove;
            }

            //
            ActivatePage(pageLobby);

            formHint = new FormHint(bmpForBackground, ilGui16, ilParameters);
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
            tslDay.Text = "День: " + lobby.Turn.ToString();
            
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
            foreach (Player p in lobby.Players)
            {
                p.PanelAbout.ShowData();
            }

            int top = tabControl1.Top;
            foreach (Player p in lobby.Players.OrderBy(p => p.PositionInLobby))
            {
                Debug.Assert(p.PositionInLobby >= 1);
                Debug.Assert(p.PositionInLobby <= lobby.TypeLobby.QuantityPlayers);

                p.Panel.Top = top;
                top = GuiUtils.NextTopHalf(p.Panel);
            }

            Refresh();
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

        private void DrawLobby()
        {
            PanelAboutPlayer pap;

            int top = Config.GridSize;
            foreach (Player p in lobby.Players)
            {
                pap = new PanelAboutPlayer(p, ilResultBattle)
                {
                    Parent = pageLobby,
                    Top = top,
                    Left = 0
                };

                p.PanelAbout = pap;

                top += pap.Height + Config.GridSize;
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

        private void DrawTowers()
        {
            
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
            base.OnPaintBackground(e);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // Рисуем подложку
            e.Graphics.DrawImageUnscaled(bmpBackground, 0, 0);
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
            tslGold.Text = lobby.CurrentPlayer.Gold.ToString() + " (+" + lobby.CurrentPlayer.Income().ToString() + ")";
            tslBuilders.Text = lobby.CurrentPlayer.FreeBuilders.ToString() + "/" + lobby.CurrentPlayer.TotalBuilders.ToString();
            tslHeroes.Text = lobby.CurrentPlayer.CombatHeroes.Count.ToString() + "/" + lobby.TypeLobby.MaxHeroes.ToString();
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
            formHint = new FormHint(bmpForBackground, ilGui16, ilParameters);
        }

        private void tsbEndTurn_Click(object sender, EventArgs e)
        {
            formHint.HideHint();

            tsbEndTurn.Enabled = false;
            lobby.DoEndTurn();

            ShowDataPlayer();
            tsbEndTurn.Enabled = true;

            // Если вылетели из лобби, то показываем итоговое место и начинаем новое лобби
            if (!lobby.CurrentPlayer.IsLive)
            {
                MessageBox.Show("Поражение..." + Environment.NewLine + "Вы заняли " + lobby.CurrentPlayer.PositionInLobby.ToString() + " место.");

                lobby = new Lobby(Config.TypeLobbies[0]);
                ShowDataPlayer();
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

        private void tslDay_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(tslDay, "День игры", "День игры: " + lobby.Turn.ToString());
        }

        private void tslGold_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(tslGold, "Казна", "Количество золота в казне и постоянный доход в день");
        }

        private void tslBuilders_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(tslBuilders, "Крестьяне", "Количество свободных строителей");
        }

        private void tslHeroes_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(tslHeroes, "Герои", "Количество героев в Королевстве - текущее и максимальное");
        }

        private void tsbEndTurn_MouseHover(object sender, EventArgs e)
        {
            ShowHintForToolButton(tsbEndTurn, "Конец хода", "Завершение хода");
        }

        private void ShowHintForToolButton(ToolStripItem tsi, string text, string hint)
        {
            formHint.Clear();
            formHint.AddStep1Header(text, "", hint);

            Point l = toolStripMain.PointToScreen(new Point(0, toolStripMain.Height + 2));
            foreach (ToolStripItem i in toolStripMain.Items)
            {
                if (i == tsi)
                    break;
                l.X += i.Width;
            }

            formHint.ShowHint(l);
        }

        private void tsl_MouseLeave(object sender, EventArgs e)
        {
            formHint.HideHint();
        }
    }
}
