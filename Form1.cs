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

namespace Fantasy_King_s_Battle
{
    public partial class FormMain : Form
    {
        private enum PlaceItemForDrag { None, Warehouse, Hero }

        private readonly string dirResources;

        private readonly ImageList ilFractions;
        private readonly ImageList ilSkills;
        private readonly ImageList ilResultBattle;
        private readonly ImageList ilTypeBattle;
        private readonly ImageList ilGuilds;
        private readonly ImageList ilBuildings;
        private readonly ImageList ilTemples;
        private readonly ImageList ilHeroes;
        internal readonly ImageList ilGui;
        private readonly ImageList ilGui16;
        private readonly ImageList ilGuiHeroes;
        internal readonly ImageList ilParameters;
        internal readonly ImageList ilItems;

        private readonly ToolStripStatusLabel StatusLabelDay;
        private readonly ToolStripStatusLabel StatusLabelGold;
        private Panel panelWarehouse;
        private PanelHeroInfo panelHeroInfo;

        internal const int GUI_HEROES = 0;
        internal const int GUI_GUILDS = 1;
        internal const int GUI_ECONOMY = 2;
        internal const int GUI_DEFENSE = 3;
        internal const int GUI_TEMPLE = 4;
        internal const int GUI_LEVELUP = 5;
        internal const int GUI_BUY = 6;
        internal const int GUI_LOBBY = 7;
        internal const int GUI_DISMISS = 8;

        internal const int GUI_PARAMETER_STRENGTH = 6;
        internal const int GUI_PARAMETER_DEXTERITY = 7;
        internal const int GUI_PARAMETER_WISDOM = 8;
        internal const int GUI_PARAMETER_STAMINA = 9;
        internal const int GUI_PARAMETER_SPEED = 10;
        internal const int GUI_PARAMETER_ATTACK_MELEE = 0;
        internal const int GUI_PARAMETER_ATTACK_RANGE = 1;
        internal const int GUI_PARAMETER_ATTACK_MAGIC = 2;
        internal const int GUI_PARAMETER_DEFENSE_MELEE = 3;
        internal const int GUI_PARAMETER_DEFENSE_RANGE = 4;
        internal const int GUI_PARAMETER_DEFENSE_MAGIC = 5;

        internal const int GUI_16_GOLD = 0;
        internal static int MAX_HEROES_AT_PLAYER = 16;
        internal static int SLOTS_IN_LINE = 4;
        internal static int SLOTS_LINES = 2;
        internal static int SLOT_IN_INVENTORY = SLOTS_IN_LINE * SLOTS_LINES;
        internal static int WH_SLOTS_IN_LINE = 10;
        internal static int WH_SLOT_LINES = 3;
        internal static int WH_MAX_SLOTS = WH_SLOTS_IN_LINE * WH_SLOT_LINES;

        private readonly Lobby lobby;
        private int curAppliedPlayer = -1;

        private List<PanelItem> SlotsWarehouse = new List<PanelItem>();

        private PictureBox picBoxItemForDrag;// PictureBox с иконкой предмета для отображения под курсором при перетаскивании
        private Point shiftForMouseByDrag;// Смещение иконки предмета относится курсора мыши, чтобы она отображалась ровно так, как предмет взял пользователь
        private PlaceItemForDrag placeItemForDrag = PlaceItemForDrag.None;// Источник предмета - склад, герой и т.д.
        private PanelItem panelItemForDrag;// Ячейка-источник предмета для переноса
        private PlayerItem itemForDrag;// Предмет для переноса. Отдельно его храним, так как если он один, в ячейке он не остается
        private PlayerItem itemTempForDrag;// Предмет для временного хранения одного экземпляра предмета при переносе

        private List<PictureBox> SlotSkill = new List<PictureBox>();

        public FormMain()
        {
            InitializeComponent();

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
            ilFractions = PrepareImageList("Fractions.png", 78, 52, true);
            
            ilSkills = PrepareImageList("Skills.png", 82, 94, false);
            ilResultBattle = PrepareImageList("ResultBattle52.png", 45, 52, false);
            ilTypeBattle = PrepareImageList("TypeBattle52.png", 52, 52, false);
            ilGuilds = PrepareImageList("Guilds.png", 126, 126, true);
            ilBuildings = PrepareImageList("Buildings.png", 126, 126, true);
            ilTemples = PrepareImageList("Temples.png", 126, 126, true);
            ilHeroes = PrepareImageList("Heroes.png", 126, 126, false);
            ilGui = PrepareImageList("Gui.png", 48, 48, true);
            ilGuiHeroes = PrepareImageList("GuiHeroes.png", 48, 48, true);
            ilGui16 = PrepareImageList("Gui16.png", 16, 16, false);
            ilParameters = PrepareImageList("Parameters.png", 24, 24, false);
            ilItems = PrepareImageList("Items.png", 48, 48, false);

            //    
            lobby = new Lobby(8);

            // Создаем метку под золото
            StatusStrip.ImageList = ilGui16;

            StatusLabelDay = new ToolStripStatusLabel()
            {
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Width = 200
            };
            StatusStrip.Items.Add(StatusLabelDay);

            StatusLabelGold = new ToolStripStatusLabel(StatusStrip.ImageList.Images[GUI_16_GOLD])
            {
                ImageAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Width = 100
            };
            StatusStrip.Items.Add(StatusLabelGold);

            // Создаем панели игроков
            PanelAboutPlayer pap;
            int top = Config.GRID_SIZE;
            foreach (Player p in lobby.Players)
            {
                pap = new PanelAboutPlayer(p, ilFractions, ilResultBattle, ilTypeBattle)
                {
                    Parent = tabPageLobby,
                    Top = top
                };

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
            tabPageHeroes.ImageIndex = GUI_HEROES;
            tabPageHeroes.Text = "";

            //
            DrawGuilds();
            DrawBuildings();
            DrawTemples();
            DrawWarehouse();

            ShowDataPlayer();
        }

        internal static Config Config { get; set; }
        internal ImageList ILFractions { get { return ilFractions; } }
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

            int lines = bmp.Height / height;
            if (lines > 1)
            {
                for (int i = 0; i < lines; i++)
                {
                    Bitmap bmpSingleline = new Bitmap(bmp.Width, height);
                    Graphics g = Graphics.FromImage(bmpSingleline);
                    g.DrawImage(bmp, 0, 0, new Rectangle(0, i * height, bmp.Width, height), GraphicsUnit.Pixel);
                    _ = il.Images.AddStrip(bmpSingleline);
                    g.Dispose();
                }
            }
            else
            {
                _ = il.Images.AddStrip(bmp);
            }

            // Добавляем серые иконки
            if (convertToGrey == true)
            {
                // Создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(bmp.Width, bmp.Height);

                // Перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < bmp.Height; j++)
                    for (int i = 0; i < bmp.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(bmp.GetPixel(i, j).ToArgb());
                        
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                        // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;

                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = ((UInt32)bmp.GetPixel(i, j).A << 24) | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
//                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);

                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                il.Images.AddStrip(output);
            }

            return il;
        }

        internal void ShowDataPlayer()
        {
            StatusLabelDay.Text = "День: " + lobby.Turn.ToString();

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayerIndex)
            {
                //DrawExternalBuilding();

                curAppliedPlayer = lobby.CurrentPlayerIndex;
            }

            ShowLobby();
            ShowGuilds();
            ShowBuildings();
            ShowTemples();
            ShowHeroes();
            ShowBattle();
            ShowGold();
        }

        private void ShowLobby()
        {
            foreach(Player p in lobby.Players)
            {
                p.PanelAbout.ShowData();
            }
        }

        private void DrawGuilds()
        {
            int top = Config.GRID_SIZE;
            int left;
            int height = 0;
            bool found;

            for (int levelCastle = 1; ; levelCastle++)
            {
                left = Config.GRID_SIZE;
                found = false;

                foreach (Guild g in Config.Guilds)
                {
                    if (g.LevelCastle == levelCastle)
                    {
                        found = true;

                        g.Panel = new PanelGuild(left, top, ilGuilds, ilGui, ilGuiHeroes);
                        g.Panel.Parent = tabPageGuilds;

                        left += g.Panel.Width + Config.GRID_SIZE;
                        height = g.Panel.Height;
                    }
                }

                if (found == false)
                    break;

                top += height + Config.GRID_SIZE;
            }
        }

        private void ShowGuilds()
        {
            foreach (PlayerGuild pg in lobby.CurrentPlayer.Guilds)
            {
                pg.UpdatePanel();
            }
        }

        private void DrawBuildings()
        {
            int top = Config.GRID_SIZE;
            int left;
            int height = 0;

            foreach (TypeBuilding tb in Enum.GetValues(typeof(TypeBuilding)))
            {
                left = Config.GRID_SIZE;

                foreach (Building b in Config.Buildings)
                {
                    if (b.TypeBuilding == tb)
                    {
                        b.Panel = new PanelBuilding(left, top, ilBuildings, ilGui, ilGui16);
                        b.Panel.Parent = tabPageBuildings;

                        left += b.Panel.Width + Config.GRID_SIZE;
                        height = b.Panel.Height;
                    }
                }

                top += height + Config.GRID_SIZE;
            }
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
            int top = Config.GRID_SIZE;
            int height;
            int left = Config.GRID_SIZE;
            int cnt = 0;

            foreach (Temple t in Config.Temples)
            {
                t.Panel = new PanelTemple(left, top, ilTemples, ilGui);
                t.Panel.Parent = tabPageTemples;

                height = t.Panel.Height;
                cnt++;
                if (cnt == 3)
                {
                    cnt = 0;
                    left = Config.GRID_SIZE;
                    top += height + Config.GRID_SIZE;
                }
                else
                {
                    left += t.Panel.Width + Config.GRID_SIZE;
                }
            }
        }

        private void ShowTemples()
        {
            foreach (PlayerTemple pt in lobby.CurrentPlayer.Temples)
            {
                pt.UpdatePanel();
            }
        }

        internal void ShowHeroes()
        {
            int top = Config.GRID_SIZE;
            int left = Config.GRID_SIZE;
            int height;
            int cnt = 0;

            foreach (PlayerGuild pg in lobby.CurrentPlayer.Guilds)
            {
                foreach (PlayerHero ph in pg.Heroes)
                {
                    if (ph.Panel == null)
                    {
                        ph.Panel = new PanelHero(ph, left, top, ilGuiHeroes, ilGui)
                        {
                            Parent = tabPageHeroes
                        };
                        ph.Panel.Click += PanelHero_Click;
                    }
                    else
                    {
                        ph.Panel.Top = top;
                        ph.Panel.Left = left;
                    }

                    ph.Panel.ShowData();

                    height = ph.Panel.Height;
                    cnt++;
                    if (cnt == 4)
                    {
                        cnt = 0;
                        left = Config.GRID_SIZE;
                        top += height + Config.GRID_SIZE;
                    }
                    else
                    {
                        left += ph.Panel.Width + Config.GRID_SIZE;
                    }
                }
            }

            ShowWarehouse();
        }

        internal void ShowAboutHero(PlayerHero ph)
        {
            if (panelHeroInfo == null)
            {
                panelHeroInfo = new PanelHeroInfo(ilHeroes, ilParameters, ilItems)
                {
                    Left = 632,
                    Top = Config.GRID_SIZE,
                    Parent = tabPageHeroes
                };

                for (int i = 0; i < panelHeroInfo.slots.Length; i++)
                {
                    panelHeroInfo.slots[i].MouseDown += PanelCellHero_MouseDown;
                    panelHeroInfo.slots[i].MouseUp += PanelCellHero_MouseUp;
                    panelHeroInfo.slots[i].MouseMove += PanelCell_MouseMove;
                }
            }

            panelHeroInfo.ShowHero(ph);
        }

        private void ShowBattle()
        {
            CourseBattle cb = lobby.GetBattle(lobby.CurrentPlayer, lobby.Turn - 1);

            textBoxResultBattle.Text = cb != null ? cb.LogBattle : "";
        }

        private void ButtonEndTurn_Click(object sender, EventArgs e)
        {
            lobby.DoEndTurn();

            ShowDataPlayer();
        }

        internal void ShowGold()
        {
            StatusLabelGold.Text = lobby.CurrentPlayer.Gold.ToString() + " (+" + lobby.CurrentPlayer.Income().ToString() + ")";
        }

        internal void ShowAllBuildings()
        {
            ShowGuilds();
            ShowBuildings();
            ShowTemples();
        }

        private void DrawWarehouse()
        {
            picBoxItemForDrag = new PictureBox()
            {
                Parent = tabPageHeroes,
                Size = ilItems.ImageSize,
                Visible = false,
                Name = "PB_For_Drag"
            };

            panelWarehouse = new Panel()
            {
                Parent = tabPageHeroes,
                BorderStyle = BorderStyle.FixedSingle,
                Left = 0,
                Top = 400
            };

            PanelItem pi;

            for (int y = 0; y < WH_SLOT_LINES; y++)
                for (int x = 0; x < WH_SLOTS_IN_LINE; x++)
                {
                    pi = new PanelItem(panelWarehouse, ilItems, x + y * WH_SLOTS_IN_LINE);
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
                SlotsWarehouse[i].ShowItem(lobby.CurrentPlayer.Warehouse[i]);
            }
        }

        private void PanelHero_Click(object sender, EventArgs e)
        {
            if (panelHeroInfo == null)
            {
                panelHeroInfo = new PanelHeroInfo(ilHeroes, ilParameters, ilItems)
                {
                    Left = 600,
                    Top = Config.GRID_SIZE,
                    Parent = tabPageHeroes
                };

                for (int i = 0; i < panelHeroInfo.slots.Length; i++)
                {
                    panelHeroInfo.slots[i].MouseDown += PanelCellHero_MouseDown;
                    panelHeroInfo.slots[i].MouseUp += PanelCellHero_MouseUp;
                    panelHeroInfo.slots[i].MouseMove += PanelCell_MouseMove;
                }
            }

            panelHeroInfo.ShowHero((sender as PanelHero).Hero);
        }

        private Point RealCoordCursorHeroDrag(Point locationMouse)
        {
            return new Point(panelHeroInfo.Left + panelItemForDrag.Left + locationMouse.X - shiftForMouseByDrag.X, panelHeroInfo.Top + panelItemForDrag.Top + locationMouse.Y - shiftForMouseByDrag.Y);
        }
        private Point RealCoordCursorHeroDragForCursor(Point locationMouse)
        {
            return new Point(panelHeroInfo.Left + panelItemForDrag.Left + locationMouse.X, panelHeroInfo.Top + panelItemForDrag.Top + locationMouse.Y);
        }

        private PanelItem GetPicBoxSlotOfHero(Point p)
        {
            if (panelHeroInfo != null)
            {
                PanelItem pb;

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
            PanelItem pb = GetPicBoxSlotOfHero(locationMouse);
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
        private void PrepareDrag(PlaceItemForDrag place, PanelItem panel, Point location)
        {
            Debug.Assert(picBoxItemForDrag.Visible == false);
            Debug.Assert(panelItemForDrag == null);
            Debug.Assert(itemTempForDrag == null);
            Debug.Assert(placeItemForDrag == PlaceItemForDrag.None);
            Debug.Assert(itemForDrag != null);

            shiftForMouseByDrag = location;
            placeItemForDrag = place;
            panelItemForDrag = panel;
        }

        // Начала переноса. Показываем иконку переносимого предмета и забираем предметы с ячейки
        private void BeginDrag()
        {
            switch (placeItemForDrag)
            {
                case PlaceItemForDrag.Warehouse:
                    // Со склада по умолчанию показываем отбор одного предмета
                    itemTempForDrag = lobby.CurrentPlayer.TakeItemFromWarehouse(panelItemForDrag.NumberCell, 1);
                    ShowWarehouse();

                    break;
                case PlaceItemForDrag.Hero:
                    // С инвентаря героя по умолчанию показываем отбор всех предметов
                    itemTempForDrag = panelHeroInfo.Hero.TakeItem(panelItemForDrag.NumberCell, panelHeroInfo.Hero.Slots[panelItemForDrag.NumberCell].Quantity);
                    panelHeroInfo.RefreshHero();

                    break;
                default:
                    throw new Exception("Неизвестный источник для предмета.");
            }

            picBoxItemForDrag.Image = ilItems.Images[itemTempForDrag.Item.ImageIndex];
            picBoxItemForDrag.Show();
        }

        private void UpdateDrag(MouseEventArgs e)
        {
            switch (placeItemForDrag)
            {
                case PlaceItemForDrag.Warehouse:
                    picBoxItemForDrag.Location = RealCoordCursorWHDrag(e.Location);
                    break;
                case PlaceItemForDrag.Hero:
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

            placeItemForDrag = PlaceItemForDrag.None;
            panelItemForDrag = null;
            itemForDrag = null;
            itemTempForDrag = null;
        }

        private void PanelCellWarehouse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Debug.Assert(itemForDrag == null);
                Debug.Assert(sender is PanelItem);

                itemForDrag = lobby.CurrentPlayer.Warehouse[((PanelItem)sender).NumberCell];
                if (itemForDrag != null)
                    PrepareDrag(PlaceItemForDrag.Warehouse, (PanelItem)sender, e.Location);
            }
        }

        private void PanelCellWarehouse_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (itemForDrag != null))
            {
                // Если клик на ячейки, то вызова BeginDrag не было
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

                        panelHeroInfo.RefreshHero();
                    }
                    else if ((panelHeroInfo != null) && (CursorUnderPanelAboutHero(RealCoordCursorWHDragForCursor(e.Location)) == true))// Бросили на панель игрока
                    {
                        lobby.CurrentPlayer.AddItem(itemTempForDrag, fromSlot);
                        lobby.CurrentPlayer.GiveItemToHero(fromSlot, panelHeroInfo.Hero, lobby.CurrentPlayer.Warehouse[fromSlot].Quantity);

                        panelHeroInfo.RefreshHero();
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
                Debug.Assert(sender is PanelItem);

                itemForDrag = panelHeroInfo.Hero.Slots[((PanelItem)sender).NumberCell];
                // Дефолтный предмет нельзя перемещать
                if (itemForDrag != null)
                    if (itemForDrag.Item == panelHeroInfo.Hero.Hero.Slots[((PanelItem)sender).NumberCell].DefaultItem)
                        itemForDrag = null;

                if (itemForDrag != null)
                    PrepareDrag(PlaceItemForDrag.Hero, (PanelItem)sender, e.Location);
            }
        }

        private void PanelCellHero_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (itemForDrag != null))
            {
                if (itemTempForDrag != null)
                {
                    int fromSlot = panelItemForDrag.NumberCell;

                    int numberCellWarehouse = SlotWarehouseUnderCursor(RealCoordCursorHeroDragForCursor(e.Location));
                    int numberCellHero = SlotHeroUnderCursor(RealCoordCursorHeroDragForCursor(e.Location));

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
                    else if (CursorUnderPanelAboutHero(RealCoordCursorHeroDragForCursor(e.Location)) == false)
                    {
                        panelHeroInfo.Hero.AcceptItem(itemTempForDrag, itemTempForDrag.Quantity, fromSlot);
                        lobby.CurrentPlayer.GetItemFromHero(panelHeroInfo.Hero, fromSlot);

                        ShowWarehouse();
                        panelHeroInfo.RefreshHero();
                    }
                    else if (ModifierKeys.HasFlag(Keys.Control) == true)
                    {
                        //                        lobby.CurrentPlayer.SellItem(fromSlot);

                        //ShowWarehouse();
                    }
                    else
                        // Возвращаем предмет обратно
                        panelHeroInfo.Hero.AcceptItem(itemTempForDrag, itemTempForDrag.Quantity, fromSlot);

                    panelHeroInfo.RefreshHero();
                }

                EndDrag();
            }
        }

        private int SlotWarehouseUnderCursor(Point locationMouse)
        {
            PanelItem pi = GetPanelItemSlotOfWarehouse(locationMouse);
            if (pi == null)
                return -1;

            return pi.NumberCell;
        }

        private PanelItem GetPanelItemSlotOfWarehouse(Point p)
        {
            foreach (PanelItem pi in SlotsWarehouse)
            {
                if ((p.Y >= panelWarehouse.Top + pi.Top) && (p.Y <= panelWarehouse.Top + pi.Top + pi.Height) && (p.X >= panelWarehouse.Left + pi.Left) && (p.X <= panelWarehouse.Left + pi.Left + pi.Width))
                    return pi;
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

        private void FormMain_Deactivate(object sender, EventArgs e)
        {
            switch (placeItemForDrag)
            {
                case PlaceItemForDrag.Warehouse:
                    lobby.CurrentPlayer.AddItem(itemTempForDrag, panelItemForDrag.NumberCell);
                    ShowWarehouse();
                    break;
                case PlaceItemForDrag.Hero:
                    lobby.CurrentPlayer.AddItem(itemTempForDrag, panelItemForDrag.NumberCell);
                    panelHeroInfo.RefreshHero();
                    break;
            }

            EndDrag();
        }
    }
}
