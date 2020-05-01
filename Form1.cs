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
        private readonly ImageList ilGui;
        private readonly ImageList ilGui16;

        private readonly ToolStripStatusLabel StatusLabelGold;

        internal const int GUI_LOBBY = 0;
        internal const int GUI_GUILDS = 1;
        internal const int GUI_ECONOMY = 2;
        internal const int GUI_DEFENSE = 3;
        internal const int GUI_TEMPLE = 4;
        internal const int GUI_LEVELUP = 5;
        internal const int GUI_BUY = 6;

        internal const int GUI_16_GOLD = 0;

        private readonly Lobby lobby;
        private int curAppliedPlayer = -1;

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
            ilGuilds = PrepareImageList("Guilds.png", 128, 128, true);
            ilBuildings = PrepareImageList("Buildings.png", 126, 126, true);
            ilTemples = PrepareImageList("Temples.png", 126, 126, true);
            ilHeroes = PrepareImageList("Heroes.png", 128, 128, false);
            ilGui = PrepareImageList("Gui.png", 48, 48, false);
            ilGui16 = PrepareImageList("Gui16.png", 16, 16, false);

            //    
            lobby = new Lobby(8);

            // Создаем метку под золото
            StatusStrip.ImageList = ilGui16;

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

            //
            DrawGuilds();
            DrawBuildings();
            DrawTemples();
            ShowDataPlayer();

        }
        internal static Config Config { get; set; }
        internal ImageList ILFractions { get { return ilFractions; } }
        internal ImageList PrepareImageList(string filename, int width, int height, bool convertToGrey)
        {
            ImageList il;
            il = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            il.ImageSize = new Size(width, height);
            Bitmap bmp;
            bmp = new Bitmap(dirResources + "Icons\\" + filename);
            _ = il.Images.AddStrip(bmp);

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
            tstbTurn.Text = "Ход: " + lobby.Turn.ToString();

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayerIndex)
            {
                //DrawExternalBuilding();
                DrawSquad();

                curAppliedPlayer = lobby.CurrentPlayerIndex;
            }

            ShowLobby();
            ShowGuilds();
            ShowBuildings();
            ShowTemples();
            ShowSquad();
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

                        g.Panel = new PanelGuild(left, top, ilGuilds, ilGui);
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

        private void DrawSquad()
        {
            PanelSquad p;
            int top = 0;
            foreach (Squad s in lobby.CurrentPlayer.Squads)
            {
                p = new PanelSquad(s)
                {
                    Parent = tabPageArmy,
                    Top = top,
                    Left = Config.GRID_SIZE
                };
                s.PanelSquad = p;

                top += p.Height + Config.GRID_SIZE;
            }
        }

        private void ShowSquad()
        {
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
    }
}
