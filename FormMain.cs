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
        private enum SourceForDrag { None, ItemFromWarehouse, ItemFromHero, Hero }

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

        internal readonly Font fontQuantity = new Font("Courier New", 14, FontStyle.Bold);
        internal readonly Font fontCost = new Font("Arial", 11, FontStyle.Bold);
        internal readonly Color ColorCost = Color.White;
        internal readonly Color ColorQuantity = Color.Yellow;
        internal readonly Brush brushQuantity = new SolidBrush(Color.Yellow);
        internal readonly Brush brushCost = new SolidBrush(Color.White);

        internal readonly Font fontToolBar = new Font("Microsodt Sans Serif", 12, FontStyle.Bold);

        private Panel panelWarehouse;
        private Panel panelHeroes;

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
        internal const int GUI_PRODUCTS = 12;
        internal const int GUI_INHABITANTS = 13;
        internal const int GUI_INVENTORY = 14;
        internal const int GUI_ABILITY = 15;
        internal const int GUI_PARAMETERS = 16;

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

        internal static int SLOTS_IN_LINE = 4;
        internal static int SLOTS_LINES = 2;
        internal static int SLOT_IN_INVENTORY = SLOTS_IN_LINE * SLOTS_LINES;
        internal static int WH_SLOTS_IN_LINE = 10;
        internal static int WH_SLOT_LINES = 3;
        internal static int WH_MAX_SLOTS = WH_SLOTS_IN_LINE * WH_SLOT_LINES;
        internal const int BUILDING_MAX_LINES = 3;
        internal static Size PANEL_RESEARCH_SIZE = new Size(4, 3);

        private readonly Lobby lobby;
        private Player curAppliedPlayer;

        private List<PanelEntity> SlotsWarehouse = new List<PanelEntity>();
        private PanelHero[,] CellPanelHeroes;

        private PictureBox picBoxItemForDrag;// PictureBox с иконкой предмета для отображения под курсором при перетаскивании
        private Point shiftForMouseByDrag;// Смещение иконки предмета относится курсора мыши, чтобы она отображалась ровно так, как предмет взял пользователь
        private SourceForDrag placeItemForDrag = SourceForDrag.None;// Источник предмета - склад, герой и т.д.
        private PanelEntity panelItemForDrag;// Ячейка-источник предмета для переноса
        private PlayerItem itemForDrag;// Предмет для переноса. Отдельно его храним, так как если он один, в ячейке он не остается
        private PlayerItem itemTempForDrag;// Предмет для временного хранения одного экземпляра предмета при переносе
        internal PanelHero panelHeroForDrag;// Ячейка-источник героя для переноса
        private PlayerHero heroForDrag;// Герой для переноса

        internal readonly Bitmap bmpForBackground;
        internal readonly Bitmap bmpBackgroundButton;
        internal readonly Bitmap bmpBorderForIcon;
        internal readonly Bitmap bmpEmptyEntity;
        private readonly Bitmap bmpBackground;

        private readonly List<PanelControls> pages = new List<PanelControls>();
        private readonly PanelControls pageLobby;
        private readonly PanelControls pageGuilds;
        private readonly PanelControls pageBuildings;
        private readonly PanelControls pageTemples;
        private readonly PanelControls pageTowers;
        private readonly PanelControls pageHeroes;
        private readonly PanelControls pageBattle;
        private PanelControls currentPage;
        private readonly int leftForPages;
        private readonly Point pointMenu;
        private readonly PanelMenu panelMenu;
        private readonly PanelBuildingInfo panelBuildingInfo;
        private readonly PanelHeroInfo panelHeroInfo;

        private List<PictureBox> SlotSkill = new List<PictureBox>();

        internal FormHint formHint;
        internal PanelBuilding SelectedPanelBuilding { get; private set; }
        internal PanelHero SelectedPanelHero { get; private set; }

        internal static Random Rnd = new Random();

        public FormMain()
        {
            InitializeComponent();

            Program.formMain = this;

            // Настройка переменной с папкой ресурсов
            dirResources = Environment.CurrentDirectory;
            if (dirResources.Contains("Debug"))
            {
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
            }

            if (dirResources.Contains("Release"))
            {
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
            }

            dirResources += "Resources\\";

            _ = new Config(dirResources, this);

            // Подготавливаем иконки
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

            bmpForBackground = new Bitmap(dirResources + "Icons\\Background.png");
            bmpBackgroundButton = GuiUtils.MakeBackground(GuiUtils.SizeButtonWithImage(ilGui));
            bmpBorderForIcon = new Bitmap(dirResources + "Icons\\BorderIconEntity.png");
            bmpEmptyEntity = new Bitmap(dirResources + "Icons\\EmptyEntity.png");

            CellPanelHeroes = new PanelHero[Config.HERO_ROWS, Config.HERO_IN_ROW];

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

            PanelControls PreparePanel()
            {
                return new PanelControls(this, leftForPages, GuiUtils.NextTop(tabControl1));
            }

            // Создаем вкладку "Лобби"
            PanelAboutPlayer pap;

            int top = Config.GRID_SIZE;
            foreach (Player p in lobby.Players)
            {
                pap = new PanelAboutPlayer(p, ilResultBattle)
                {
                    Top = top,
                    Left = 0
                };
                pageLobby.AddControl(pap);

                p.PanelAbout = pap;

                top += pap.Height + Config.GRID_SIZE;
            }

            //
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

            //
            DrawGuilds();
            DrawBuildings();
            DrawTemples();
            DrawTowers();
            DrawHeroes();
            DrawWarehouse();

            ShowDataPlayer();

            // Определяем максимальную ширину окна            
            int rightSide = 0;

            foreach (PanelControls pc in pages)
            {
                rightSide = Math.Max(rightSide, pc.RightForParent);
            }

            // Создаем панель с меню
            panelMenu = new PanelMenu(this, dirResources);

            // Учитываем плиту под слоты
            pointMenu = new Point(rightSide + Config.GRID_SIZE, ClientSize.Height - panelMenu.Height - Config.GRID_SIZE);
            rightSide += panelMenu.Width + Config.GRID_SIZE;

            panelMenu.Location = pointMenu;

            Width = (Width - ClientSize.Width) + rightSide + Config.GRID_SIZE;
            Height = GuiUtils.NextTop(lobby.Players[lobby.Players.Length - 1].Panel) + (Height - ClientSize.Height);
            tabControl1.Width = ClientSize.Width - tabControl1.Left - Config.GRID_SIZE;
            pointMenu.Y = ClientSize.Height - panelMenu.Height - Config.GRID_SIZE;

            // Подготавливаем подложку
            bmpBackground = GuiUtils.MakeBackground(ClientSize);

            // Панель информации о здании
            panelBuildingInfo = new PanelBuildingInfo(panelMenu.Width, panelMenu.Top - GuiUtils.NextTop(tabControl1) - Config.GRID_SIZE)
            {
                Parent = this,
                Left = pointMenu.X,
                Top = GuiUtils.NextTop(tabControl1),
                Visible = false
            };

            //
            panelHeroInfo = new PanelHeroInfo(panelBuildingInfo.Width, panelBuildingInfo.Height)
            {
                Parent = this,
                Left = panelBuildingInfo.Left,
                Top = panelBuildingInfo.Top,
                Visible = false
            };

            // Перенести в класс
            for (int i = 0; i < panelHeroInfo.slots.Length; i++)
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

        private void DrawPageBuilding(PanelControls panel, CategoryBuilding category)
        {
            int top = 0;
            int left;
            int height = 0;

            for (int line = 1; line <= BUILDING_MAX_LINES; line++)
            {
                left = 0;

                foreach (Building b in Config.Buildings)
                {
                    if ((b.CategoryBuilding == category) && (b.Line == line))
                    {
                        b.Panel = new PanelBuilding(this, left, top, this);
                        panel.AddControl(b.Panel);

                        left += b.Panel.Width + Config.GRID_SIZE;
                        height = b.Panel.Height;
                    }
                }

                top += height + Config.GRID_SIZE;
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
            panelHeroes = new Panel()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Left = 0,
                Top = 64
            };
            pageHeroes.AddControl(panelHeroes);

            int width = 0;
            int height = 0;
            PanelHero ph;
            for (int y = 0; y < CellPanelHeroes.GetLength(0); y++)
                for (int x = 0; x < CellPanelHeroes.GetLength(1); x++)
                {
                    ph = new PanelHero(new Point(x, y), Config.GRID_SIZE + x * (ilGuiHeroes.ImageSize.Width + Config.GRID_SIZE * 2), Config.GRID_SIZE + y * (ilGuiHeroes.ImageSize.Height + Config.GRID_SIZE * 2), ilGuiHeroes, ilGui);
                    ph.Parent = panelHeroes;
                    CellPanelHeroes[y, x] = ph;
                    ph.Click += PanelHero_Click;
                    ph.MouseDown += CellHero_MouseDown;
                    ph.MouseUp += CellHero_MouseUp;
                    ph.MouseMove += CellHero_MouseMove;

                    width = Math.Max(width, GuiUtils.NextLeft(ph));
                    height = Math.Max(height, GuiUtils.NextTop(ph));
                }

            panelHeroes.Width = width;
            panelHeroes.Height = height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // Рисуем подложку
            e.Graphics.DrawImageUnscaled(bmpBackground, 0, 0);
        }

        private void CellHero_MouseMove(object sender, MouseEventArgs e)
        {
            if (panelHeroForDrag != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (picBoxItemForDrag.Visible == false)
                    {
                        panelHeroForDrag.ShowData(panelHeroForDrag.Hero);
                        BeginDrag();
                    }

                    UpdateDrag(e);
                }
                else
                {
                    EndDrag();
                }
            }
        }

        private void CellHero_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (panelHeroForDrag != null))
            {
                // Если клик на ячейке, то вызова BeginDrag не было
                if (panelHeroForDrag != null)
                {
                    // Определяем, куда бросили предмет
                    PanelHero ph = GetPanelHeroOnForm(RealCoordCursorHeroDragForCursor(e.Location));

                    if ((ph != null) && (ph != panelHeroForDrag))
                    {
                        PlayerHero h = ph.Hero;
                        lobby.CurrentPlayer.CellHeroes[ph.Point.Y, ph.Point.X] = panelHeroForDrag.Hero;
                        lobby.CurrentPlayer.CellHeroes[panelHeroForDrag.Point.Y, panelHeroForDrag.Point.X] = h;

                        ShowPageHeroes();
                    }
                }

                EndDrag();
            }
        }

        private void CellHero_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Debug.Assert(panelHeroForDrag == null);
                Debug.Assert(sender is PanelHero);

                if (((PanelHero)sender).Hero != null)
                {
                    panelHeroForDrag = (PanelHero)sender;
                    PrepareDrag(SourceForDrag.Hero, (PanelHero)sender, e.Location);
                }
            }
        }

        internal void ShowPageHeroes()
        {
            for (int y = 0; y < CellPanelHeroes.GetLength(0); y++)
                for (int x = 0; x < CellPanelHeroes.GetLength(1); x++)
                    if (CellPanelHeroes[y, x] != null)
                    {
                        CellPanelHeroes[y, x].ShowData(lobby.CurrentPlayer.CellHeroes[y, x]);
                        if (lobby.CurrentPlayer.CellHeroes[y, x] != null)
                            lobby.CurrentPlayer.CellHeroes[y, x].Panel = CellPanelHeroes[y, x];
                    }

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
            picBoxItemForDrag = new PictureBox()
            {
                Parent = this,
                Size = ilItems.ImageSize,
                Visible = false
            };
            picBoxItemForDrag.BringToFront();

            panelWarehouse = new Panel()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Left = 0,
                Top = 400
            };
            pageHeroes.AddControl(panelWarehouse);

            PanelEntity pi;

            for (int y = 0; y < WH_SLOT_LINES; y++)
                for (int x = 0; x < WH_SLOTS_IN_LINE; x++)
                {
                    pi = new PanelEntity();
                    pi.Parent = panelWarehouse;
                    pi.Left = Config.GRID_SIZE + (pi.Width + Config.GRID_SIZE) * x;
                    pi.Top = Config.GRID_SIZE + (pi.Height + Config.GRID_SIZE) * y;
                    pi.MouseMove += PanelCell_MouseMove;
                    pi.MouseDown += PanelCellWarehouse_MouseDown;
                    pi.MouseUp += PanelCellWarehouse_MouseUp;

                    SlotsWarehouse.Add(pi);
                }

            panelWarehouse.Width = WH_SLOTS_IN_LINE * (SlotsWarehouse[0].Width + Config.GRID_SIZE) + Config.GRID_SIZE;
            panelWarehouse.Height = WH_SLOT_LINES * (SlotsWarehouse[0].Height + Config.GRID_SIZE) + Config.GRID_SIZE;
        }

        internal void ShowWarehouse()
        {
            for (int i = 0; i < lobby.CurrentPlayer.Warehouse.Length; i++)
            {
                SlotsWarehouse[i].ShowPlayerItem(lobby.CurrentPlayer.Warehouse[i]);
            }
        }

        private void PanelHero_Click(object sender, EventArgs e)
        {
            SelectHero(sender as PanelHero);
        }

        private Point RealCoordCursorHeroDrag(Point locationMouse)
        {
            return new Point(panelHeroes.Left + panelHeroForDrag.Left + locationMouse.X - shiftForMouseByDrag.X, panelHeroes.Top + panelHeroForDrag.Top + locationMouse.Y - shiftForMouseByDrag.Y);
        }

        private Point RealCoordCursorHeroItemDrag(Point locationMouse)
        {
            return new Point(panelHeroInfo.Left + panelItemForDrag.Left + locationMouse.X - shiftForMouseByDrag.X, panelHeroInfo.Top + panelItemForDrag.Top + locationMouse.Y - shiftForMouseByDrag.Y);
        }

        private Point RealCoordCursorHeroItemDragForCursor(Point locationMouse)
        {
            return new Point(panelHeroInfo.Left + panelItemForDrag.Left + locationMouse.X, panelHeroInfo.Top + panelItemForDrag.Top + locationMouse.Y);
        }

        private PanelEntity GetPicBoxSlotOfHero(Point p)
        {
            if (panelHeroInfo != null)
            {
                PanelEntity pb;

                for (int i = 0; i < panelHeroInfo.slots.Length; i++)
                {
                    pb = panelHeroInfo.slots[i];
                    if ((p.Y >= panelHeroInfo.Top + pb.Top) && (p.Y <= panelHeroInfo.Top + pb.Top + pb.Height) && (p.X >= panelHeroInfo.Left + pb.Left) && (p.X <= panelHeroInfo.Left + pb.Left + pb.Width))
                        return pb;
                }
            }

            return null;
        }
        private int SlotHeroUnderCursor(Point locationMouse)
        {
            PanelEntity pb = GetPicBoxSlotOfHero(locationMouse);
            if (pb == null)
                return -1;

            return pb.NumberCell;
        }

        private bool PointInControl(int left, int top, int width, int height, Point p)
        {
            return (p.X >= left) && (p.X <= left + width) && (p.Y >= top) && (p.Y <= top + height);
        }

        private bool CursorUnderPanelAboutHero(Point p)
        {
            if (panelHeroInfo != null)
                return PointInControl(panelHeroInfo.Left, panelHeroInfo.Top, panelHeroInfo.Width, panelHeroInfo.Height, p);

            return false;
        }

        private bool CursorUnderPanelWarehouse(Point p)
        {
            return PointInControl(panelWarehouse.Left, panelWarehouse.Top, panelWarehouse.Width, panelWarehouse.Height, p);
        }

        // Подготовка будущего Drag&Drop. Сразу не забираем предмет, чтобы при обычном клике не было взятия/сброса предмета
        private void PrepareDrag(SourceForDrag place, Control panel, Point location)
        {
            Debug.Assert(picBoxItemForDrag.Visible == false);
            Debug.Assert(panelItemForDrag == null);
            Debug.Assert(itemTempForDrag == null);
            Debug.Assert(placeItemForDrag == SourceForDrag.None);

            shiftForMouseByDrag = location;
            placeItemForDrag = place;

            switch (place)
            {
                case SourceForDrag.ItemFromHero:
                case SourceForDrag.ItemFromWarehouse:
                    Debug.Assert(itemForDrag != null);
                    panelItemForDrag = panel as PanelEntity;

                    break;
                case SourceForDrag.Hero:
                    panelHeroForDrag = panel as PanelHero;

                    break;
                default:
                    throw new Exception("Неизвестный тип источника.");
            }
        }

        // Начала переноса. Показываем иконку переносимого предмета и забираем предметы с ячейки
        private void BeginDrag()
        {
            switch (placeItemForDrag)
            {
                case SourceForDrag.ItemFromWarehouse:
                    // Со склада по умолчанию показываем отбор одного предмета
                    itemTempForDrag = lobby.CurrentPlayer.TakeItemFromWarehouse(panelItemForDrag.NumberCell, 1);
                    ShowWarehouse();
                    picBoxItemForDrag.Image = ilItems.Images[itemTempForDrag.Item.ImageIndex];

                    break;
                case SourceForDrag.ItemFromHero:
                    // С инвентаря героя по умолчанию показываем отбор всех предметов
                    itemTempForDrag = panelHeroInfo.Hero.TakeItem(panelItemForDrag.NumberCell, panelHeroInfo.Hero.Slots[panelItemForDrag.NumberCell].Quantity);
                    panelHeroInfo.ShowData();
                    picBoxItemForDrag.Image = ilItems.Images[itemTempForDrag.Item.ImageIndex];

                    break;
                case SourceForDrag.Hero:
                    heroForDrag = panelHeroForDrag.Hero;
                    Debug.Assert(heroForDrag != null);
                    picBoxItemForDrag.Image = ilGuiHeroes.Images[heroForDrag.ClassHero.ImageIndex];

                    break;
                default:
                    throw new Exception("Неизвестный источник для предмета.");
            }

            picBoxItemForDrag.Show();
        }

        private void UpdateDrag(MouseEventArgs e)
        {
            switch (placeItemForDrag)
            {
                case SourceForDrag.ItemFromWarehouse:
                    picBoxItemForDrag.Location = RealCoordCursorWHDrag(e.Location);
                    break;
                case SourceForDrag.ItemFromHero:
                    picBoxItemForDrag.Location = RealCoordCursorHeroItemDrag(e.Location);
                    break;
                case SourceForDrag.Hero:
                    picBoxItemForDrag.Location = RealCoordCursorHeroDrag(e.Location);
                    break;
                default:
                    throw new Exception("Неизвестный источник для предмета.");
            }
        }

        private void EndDrag()
        {
            // Если пользователь нажал и отпустил кнопку, то иконка предмета не отобразилась
            if (picBoxItemForDrag.Visible == true)
                picBoxItemForDrag.Hide();

            PanelHero ph = panelHeroForDrag;
            placeItemForDrag = SourceForDrag.None;
            panelItemForDrag = null;
            panelHeroForDrag = null;
            itemForDrag = null;
            itemTempForDrag = null;
            heroForDrag = null;

            if (ph != null)
                ph.ShowData(ph.Hero);
        }

        private void PanelCellWarehouse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Debug.Assert(itemForDrag == null);
                Debug.Assert(sender is PanelEntity);

                itemForDrag = lobby.CurrentPlayer.Warehouse[((PanelEntity)sender).NumberCell];
                if (itemForDrag != null)
                    PrepareDrag(SourceForDrag.ItemFromWarehouse, (PanelEntity)sender, e.Location);
            }
        }

        private void PanelCellWarehouse_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (itemForDrag != null))
            {
                // Если клик на ячейке, то вызова BeginDrag не было
                if (itemTempForDrag != null)
                {
                    int fromSlot = panelItemForDrag.NumberCell;

                    // Определяем, куда бросили предмет
                    int numberCellWarehouse = SlotWarehouseUnderCursor(RealCoordCursorWHDragForCursor(e.Location));
                    int numberCellHero = SlotHeroUnderCursor(RealCoordCursorWHDragForCursor(e.Location));

                    if (numberCellWarehouse >= 0)// Бросили на другую ячейку склада
                    {
                        lobby.CurrentPlayer.AddItem(itemTempForDrag, fromSlot);

                        if (numberCellWarehouse != fromSlot)
                            lobby.CurrentPlayer.MoveItem(fromSlot, numberCellWarehouse);
                    }
                    else if (numberCellHero >= 0)// Бросили на ячейку инвентаря героя
                    {
                        lobby.CurrentPlayer.AddItem(itemTempForDrag, fromSlot);

                        // Предмет бросаем в первую очередь в ячейку, которая уже есть с такими предметами
                        int numberCellForExistItem = panelHeroInfo.Hero.FindSlotWithItem(itemTempForDrag.Item);
                        numberCellHero = numberCellForExistItem >= 0 ? numberCellForExistItem : numberCellHero;
                        lobby.CurrentPlayer.GiveItemToHero(fromSlot, panelHeroInfo.Hero, lobby.CurrentPlayer.Warehouse[fromSlot].Quantity, numberCellHero);

                        panelHeroInfo.ShowData();
                    }
                    else if ((panelHeroInfo != null) && (CursorUnderPanelAboutHero(RealCoordCursorWHDragForCursor(e.Location)) == true))// Бросили на панель игрока
                    {
                        lobby.CurrentPlayer.AddItem(itemTempForDrag, fromSlot);
                        lobby.CurrentPlayer.GiveItemToHero(fromSlot, panelHeroInfo.Hero, lobby.CurrentPlayer.Warehouse[fromSlot].Quantity);

                        panelHeroInfo.ShowData();
                    }
                    else if ((CursorUnderPanelWarehouse(RealCoordCursorWHDragForCursor(e.Location)) == false))
                    {
                        // Бросили вне панели склада
                        lobby.CurrentPlayer.SellItem(itemTempForDrag);
                    }
                    else
                    {
                        // Если предмет так никуда и не пристроили, возвращаем его обратно
                        lobby.CurrentPlayer.AddItem(itemTempForDrag, fromSlot);
                    }

                    ShowWarehouse();
                }

                EndDrag();
                // && (ModifierKeys.HasFlag(Keys.Control) == true)
            }
        }

        private void PanelCell_MouseMove(object sender, MouseEventArgs e)
        {
            if (itemForDrag != null)
            {
                if (picBoxItemForDrag.Visible == false)
                    BeginDrag();

                UpdateDrag(e);
            }
        }

        private void PanelCellHero_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Debug.Assert(itemForDrag == null);
                Debug.Assert(sender is PanelEntity);

                itemForDrag = panelHeroInfo.Hero.Slots[((PanelEntity)sender).NumberCell];
                // Дефолтный предмет нельзя перемещать
                if (itemForDrag != null)
                    if (itemForDrag.Item == panelHeroInfo.Hero.ClassHero.Slots[((PanelEntity)sender).NumberCell].DefaultItem)
                        itemForDrag = null;

                if (itemForDrag != null)
                    PrepareDrag(SourceForDrag.ItemFromHero, (PanelEntity)sender, e.Location);
            }
        }

        private void PanelCellHero_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (itemForDrag != null))
            {
                if (itemTempForDrag != null)
                {
                    int fromSlot = panelItemForDrag.NumberCell;

                    int numberCellWarehouse = SlotWarehouseUnderCursor(RealCoordCursorHeroItemDragForCursor(e.Location));
                    int numberCellHero = SlotHeroUnderCursor(RealCoordCursorHeroItemDragForCursor(e.Location));

                    if (numberCellWarehouse >= 0)// Бросили на ячейку склада
                    {
                        panelHeroInfo.Hero.AcceptItem(itemTempForDrag, itemTempForDrag.Quantity, fromSlot);

                        // Предмет бросаем в первую очередь в ячейку, которая уже есть с такими предметами
                        int numberCellForExistItem = lobby.CurrentPlayer.FindSlotWithItem(itemTempForDrag.Item);
                        numberCellWarehouse = numberCellForExistItem >= 0 ? numberCellForExistItem : numberCellWarehouse;
                        lobby.CurrentPlayer.GetItemFromHero(panelHeroInfo.Hero, fromSlot, numberCellWarehouse);

                        ShowWarehouse();
                    }
                    else if (numberCellHero >= 0)// Бросили на ячейку героя
                    {
                        panelHeroInfo.Hero.AcceptItem(itemTempForDrag, itemTempForDrag.Quantity, fromSlot);

                        if (numberCellHero != fromSlot)
                            panelHeroInfo.Hero.MoveItem(fromSlot, numberCellHero);
                    }
                    else if (CursorUnderPanelAboutHero(RealCoordCursorHeroItemDragForCursor(e.Location)) == false)
                    {
                        panelHeroInfo.Hero.AcceptItem(itemTempForDrag, itemTempForDrag.Quantity, fromSlot);
                        lobby.CurrentPlayer.GetItemFromHero(panelHeroInfo.Hero, fromSlot);

                        ShowWarehouse();
                        panelHeroInfo.ShowData();
                    }
                    else if (ModifierKeys.HasFlag(Keys.Control) == true)
                    {
                        //                        lobby.CurrentPlayer.SellItem(fromSlot);

                        //ShowWarehouse();
                    }
                    else
                        // Возвращаем предмет обратно
                        panelHeroInfo.Hero.AcceptItem(itemTempForDrag, itemTempForDrag.Quantity, fromSlot);

                    panelHeroInfo.ShowData();
                }

                EndDrag();
            }
        }

        private int SlotWarehouseUnderCursor(Point locationMouse)
        {
            PanelEntity pi = GetPanelItemSlotOfWarehouse(locationMouse);
            if (pi == null)
                return -1;

            return pi.NumberCell;
        }

        private PanelEntity GetPanelItemSlotOfWarehouse(Point p)
        {
            foreach (PanelEntity pi in SlotsWarehouse)
            {
                if ((p.Y >= panelWarehouse.Top + pi.Top) && (p.Y <= panelWarehouse.Top + pi.Top + pi.Height) && (p.X >= panelWarehouse.Left + pi.Left) && (p.X <= panelWarehouse.Left + pi.Left + pi.Width))
                    return pi;
            }

            return null;

        }
        private PanelHero GetPanelHeroOnForm(Point p)
        {
            PanelHero ph;

            for (int y = 0; y < CellPanelHeroes.GetLength(0); y++)
                for (int x = 0; x < CellPanelHeroes.GetLength(1); x++)
                {
                    ph = CellPanelHeroes[y, x];

                    if ((p.Y >= panelHeroes.Top + ph.Top) && (p.Y <= panelHeroes.Top + ph.Top + ph.Height) && (p.X >= panelHeroes.Left + ph.Left) && (p.X <= panelHeroes.Left + ph.Left + ph.Width))
                        return ph;
                }

            return null;
        }

        private Point RealCoordCursorWHDrag(Point locationMouse)
        {
            return new Point(panelWarehouse.Left + panelItemForDrag.Left + locationMouse.X - shiftForMouseByDrag.X, panelWarehouse.Top + panelItemForDrag.Top + locationMouse.Y - shiftForMouseByDrag.Y);
        }
        private Point RealCoordCursorWHDragForCursor(Point locationMouse)
        {
            return new Point(panelWarehouse.Left + panelItemForDrag.Left + locationMouse.X, panelWarehouse.Top + panelItemForDrag.Top + locationMouse.Y);
        }

        private Point RealCoordCursorHeroDragForCursor(Point locationMouse)
        {
            return new Point(panelHeroes.Left + panelHeroForDrag.Left + locationMouse.X, panelHeroes.Top + panelHeroForDrag.Top + locationMouse.Y);
        }

        private void FormMain_Deactivate(object sender, EventArgs e)
        {
            switch (placeItemForDrag)
            {
                case SourceForDrag.ItemFromWarehouse:
                    lobby.CurrentPlayer.AddItem(itemTempForDrag, panelItemForDrag.NumberCell);
                    ShowWarehouse();
                    break;
                case SourceForDrag.ItemFromHero:
                    lobby.CurrentPlayer.AddItem(itemTempForDrag, panelItemForDrag.NumberCell);
                    panelHeroInfo.ShowData();
                    break;
            }

            EndDrag();
        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            ActivatePage(pages[e.TabPageIndex]);
        }

        private void ActivatePage(PanelControls pc)
        {
            if (currentPage != null)
                currentPage.SetVisible(false);
            pc.SetVisible(true);
            currentPage = pc;

            //Invalidate();
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
            tsbEndTurn.Enabled = false;
            lobby.DoEndTurn();

            ShowDataPlayer();
            tsbEndTurn.Enabled = true;
        }

        internal void SelectBuilding(PanelBuilding pb)
        {
            if (SelectedPanelBuilding != pb)
            {
                if (SelectedPanelHero != null)
                    SelectHero(null);

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

        internal void SelectHero(PanelHero ph)
        {
            if (SelectedPanelHero != ph)
            {
                if (SelectedPanelBuilding != null)
                    SelectBuilding(null);

                PanelHero oldSelected = SelectedPanelHero;
                SelectedPanelHero = ph;

                UpdateMenu();

                if (oldSelected != null)
                    oldSelected.Invalidate(true);
                if (SelectedPanelHero != null)
                {
                    panelHeroInfo.Hero = SelectedPanelHero.Hero;
                    SelectedPanelHero.Invalidate(true);
                    panelHeroInfo.Show();
                }
                else
                    panelHeroInfo.Hide();

                panelMenu.Invalidate(true);// Это точно надо?
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
                for (int y = 0; y < PANEL_RESEARCH_SIZE.Height; y++)
                    for (int x = 0; x < PANEL_RESEARCH_SIZE.Width; x++)
                        panelMenu.CellsMenu[y, x].Research = null;
            }
        }
    }
}
