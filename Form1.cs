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

        private List<PictureBox> SlotsWarehouse = new List<PictureBox>();
        private PictureBox pbForDragDrop;
        private PlayerItem playerItemDragged;
        private PictureBox pbDragged;
        private Point shiftDrag;

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
                Width = 160
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

            tabPageHeroes.MouseMove += TabPageHeroes_MouseMove;
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

        internal void ShowWarehouse()
        {
            for (int i = 0; i < lobby.CurrentPlayer.Warehouse.Length; i++)
            {
                if (lobby.CurrentPlayer.Warehouse[i] != null)
                    SlotsWarehouse[i].Image = ilItems.Images[lobby.CurrentPlayer.Warehouse[i].Item.ImageIndex];
                else
                    SlotsWarehouse[i].Image = null;

                SlotsWarehouse[i].Tag = i;
            }
        }

        private void PanelHero_Click(object sender, EventArgs e)
        {
            if (panelHeroInfo == null)
            {
                panelHeroInfo = new PanelHeroInfo(ilHeroes, ilParameters, ilItems)
                {
                    Left = 568,
                    Top = Config.GRID_SIZE,
                    Parent = tabPageHeroes
                };
            }

            panelHeroInfo.ShowHero((sender as PanelHero).Hero);
        }

        internal void ShowAboutHero(PlayerHero ph)
        {
            if (panelHeroInfo == null)
            {
                panelHeroInfo = new PanelHeroInfo(ilHeroes, ilParameters, ilItems)
                {
                    Left = 568,
                    Top = Config.GRID_SIZE,
                    Parent = tabPageHeroes
                };
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
            pbForDragDrop = new PictureBox()
            {
                Parent = tabPageHeroes,
                Size = ilItems.ImageSize,
                Visible = false,
                Name = "PB_For_Drag"
            };

            PictureBox pb;

            for (int y = 0; y < WH_SLOT_LINES; y++)
                for (int x = 0; x < WH_SLOTS_IN_LINE; x++)
                {
                    pb = new PictureBox()
                    {
                        Parent = tabPageHeroes,
                        BorderStyle = BorderStyle.FixedSingle,
                        Left = Config.GRID_SIZE + (ilItems.ImageSize.Width + Config.GRID_SIZE) * x,
                        Top = 400 + Config.GRID_SIZE + (ilItems.ImageSize.Height + Config.GRID_SIZE) * y,
                        Width = ilItems.ImageSize.Width + 2,
                        Height = ilItems.ImageSize.Height + 2,
                        Name = "PBWH_" + (x + y * WH_SLOTS_IN_LINE + 1).ToString()
                    };
                    pb.SendToBack();
                    pb.MouseMove += PbWarehouseItem_MouseMove;
                    pb.MouseDown += PbWarehouse_MouseDown;
                    pb.MouseUp += PbWarehouse_MouseUp;

                    SlotsWarehouse.Add(pb);
                }
        }

        private void PbWarehouse_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (playerItemDragged != null))
            {
                int fromSlot = (int)pbDragged.Tag;
                int nSlot = SlotWarehouseUnderCursor(e.Location);

                playerItemDragged = null;
                pbForDragDrop.Hide();
                pbDragged = null;

                if (nSlot >= 0)
                {
                    if (lobby.CurrentPlayer.Warehouse[nSlot] == null)
                    {
                        if (nSlot != fromSlot)
                        {
                            lobby.CurrentPlayer.MoveItem(fromSlot, nSlot);

                            ShowWarehouse();
                        }
                    }
                }
                else
                {
                    if (ModifierKeys.HasFlag(Keys.Control) == true)
                    {
                        lobby.CurrentPlayer.SellItem(fromSlot);

                        ShowWarehouse();
                    }
                    else
                    {
/*                        if (MessageBox.Show("Продать " + lobby.CurrentPlayer.Warehouse[fromSlot].Item.Name + "?", "FKB", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            lobby.CurrentPlayer.SellItem(fromSlot);

                            ShowWarehouse();
                        }*/
                    }
                }
                //                Control c = tabPageHeroes.GetChildAtPoint(RealCoordCursorDrag());
                //StatusLabelDay.Text = "Свободен";
                StatusLabelDay.Text = "Свободен";
            }
        }

        private void PbWarehouse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Debug.Assert(playerItemDragged == null);
                Debug.Assert(pbForDragDrop.Visible == false);
                Debug.Assert(pbDragged == null);
                Debug.Assert((sender as PictureBox).Tag != null);

                playerItemDragged = lobby.CurrentPlayer.Warehouse[(int)(sender as PictureBox).Tag];
                if (playerItemDragged != null)
                {
                    pbDragged = (sender as PictureBox);
                    shiftDrag = e.Location;
                    StatusLabelDay.Text = "Взял " + (sender as PictureBox).Tag.ToString();
                }
            }
        }

        private void PbWarehouseItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (playerItemDragged != null)
            {
                Debug.Assert(pbDragged != null);
                if (pbForDragDrop.Visible == false)
                {
                    pbForDragDrop.Image = (sender as PictureBox).Image;
                    pbForDragDrop.Show();
                }

                //StatusLabelDay.Text = "PB Понес " + (sender as PictureBox).Tag.ToString() + ", " + e.X.ToString() + ":" + e.Y.ToString();
                pbForDragDrop.Location = RealCoordCursorDrag(e.Location);
                
                PictureBox pb = GetPicBoxSlotOfWarehouse(RealCoordCursorDragForCursor(e.Location));
                if (pb != null)
                {
                    if (pb.Tag != null)
                    {
                        PlayerItem pi = lobby.CurrentPlayer.Warehouse[(int)pb.Tag];
                        if (pi != null)
                            StatusLabelDay.Text = "Подо мной " + pi.Item.ID;
                        else
                            StatusLabelDay.Text = "Подо мной " + pb.Name + " нет предмета";
                    }
                    else
                    {
                        StatusLabelDay.Text = "Подо мной " + pb.Name + " без Tag";
                    }
                }
                else
                    StatusLabelDay.Text = "Подо мной контрола нет";
            }
            else
                StatusLabelDay.Text = "PB " + (sender as Control).Name + " Нет пред., " + e.X.ToString() + ":" + e.Y.ToString();
        }

        private int SlotWarehouseUnderCursor(Point locationMouse)
        {
            PictureBox pb = GetPicBoxSlotOfWarehouse(RealCoordCursorDragForCursor(locationMouse));
            if (pb == null)
                return -1;

            return (int)pb.Tag;
        }

        private PictureBox GetPicBoxSlotOfWarehouse(Point p)
        {
            foreach (PictureBox pb in SlotsWarehouse)
            {
                if ((p.Y >= pb.Top) && (p.Y <= pb.Top + pb.Height) && (p.X >= pb.Left) && (p.X <= pb.Left + pb.Width))
                    return pb;
            }

            return null;
        }

        private Point RealCoordCursorDrag(Point locationMouse)
        {
            return new Point(pbDragged.Left + locationMouse.X - shiftDrag.X, pbDragged.Top + locationMouse.Y - shiftDrag.Y);
        }
        private Point RealCoordCursorDragForCursor(Point locationMouse)
        {
            return new Point(pbDragged.Left + locationMouse.X, pbDragged.Top + locationMouse.Y);
        }

        private void TabPageHeroes_MouseMove(object sender, MouseEventArgs e)
        {
            if (playerItemDragged != null)
            {
                pbForDragDrop.Left = e.X;
                pbForDragDrop.Top = e.Y;
                StatusLabelDay.Text = "Понес " + (sender as PictureBox).Tag.ToString() + ", " + e.X.ToString() + ":" + e.Y.ToString();
            }
            else
                StatusLabelDay.Text = "Нет пред., " + e.X.ToString() + ":" + e.Y.ToString();
        }
    }
}
