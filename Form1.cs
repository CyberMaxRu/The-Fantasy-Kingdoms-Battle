using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                Width = 100
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
            DrawHeroes();
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

        private void DrawHeroes()
        {
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
                        Size = ilItems.ImageSize
                    };

                    SlotsWarehouse.Add(pb);
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
            for (int i = 0; i < lobby.CurrentPlayer.Warehouse.Count; i++)
            {
                SlotsWarehouse[i].Image = ilItems.Images[lobby.CurrentPlayer.Warehouse[i].Item.ImageIndex];
            }

            for (int i = lobby.CurrentPlayer.Warehouse.Count; i < SlotsWarehouse.Count; i++)
            {
                SlotsWarehouse[i].Image = null;
            }
        }

        private void PanelHero_Click(object sender, EventArgs e)
        {
            if (panelHeroInfo == null)
            {
                panelHeroInfo = new PanelHeroInfo(ilHeroes, ilParameters, ilItems)
                {
                    Left = 400,
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
                    Left = 488,
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
    }
}
