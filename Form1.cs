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

        private readonly ImageList ilResources24;
        private readonly ImageList ilFractions;
        private readonly ImageList ilExternalBuildings;
        private readonly ImageList ilSkills;
        private readonly ImageList ilResultBattle;
        private readonly ImageList ilTypeBattle;
        private readonly ImageList ilGuilds;
        private readonly ImageList ilHeroes;
        private readonly ImageList ilGui;

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
            ilResources24 = PrepareImageList("Resources24.png", 24, 24, false);
            StatusStrip.ImageList = ilResources24;

            ilFractions = PrepareImageList("Fractions.png", 78, 52, true);
            
            ilExternalBuildings = PrepareImageList("ExternalBuildings.png", 82, 64, false);

            ilSkills = PrepareImageList("Skills.png", 82, 94, false);
            ilResultBattle = PrepareImageList("ResultBattle52.png", 45, 52, false);
            ilTypeBattle = PrepareImageList("TypeBattle52.png", 52, 52, false);
            ilGuilds = PrepareImageList("Guilds.png", 128, 128, true);
            ilHeroes = PrepareImageList("Heroes.png", 128, 128, false);
            ilGui = PrepareImageList("Gui.png", 48, 48, false);

            //    
            lobby = new Lobby(8);

            // Создаем метки под ресурсы
            foreach (Resource r in Config.Resources)
            {
                r.StatusLabel = new ToolStripStatusLabel(r.Name)
                {
                    ImageIndex = r.ImageIndex,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false,
                    Width = 64,
                    ToolTipText = r.Name,
                    AutoToolTip = true
                };
                StatusStrip.Items.Add(r.StatusLabel);
            }

            // Создаем панели игроков
            PanelAboutPlayer pap;
            int top = Config.GRID_SIZE;
            foreach (Player p in lobby.Players)
            {
                pap = new PanelAboutPlayer(p, ilFractions, ilResultBattle, ilTypeBattle)
                {
                    Parent = tabPage1,
                    Top = top
                };

                p.PanelAbout = pap;
                top += pap.Height + Config.GRID_SIZE;
            }

            //
            tabControl1.ImageList = ilGui; 
            tabPageGuilds.ImageIndex = 0;
            tabPageGuilds.Text = "";

            //
            DrawGuilds();
            DrawChieftain();
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

            foreach (Resource r in Config.Resources)
            {
                r.StatusLabel.Text = lobby.CurrentPlayer.Resources[r.Position].ToString();
            }

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayerIndex)
            {
                DrawExternalBuilding();
                DrawSquad();

                curAppliedPlayer = lobby.CurrentPlayerIndex;
            }

            ShowLobby();
            ShowExternalBuildings();
            ShowGuilds();
            ShowChieftain();
            ShowSquad();
            ShowBattle();
        }

        private void ShowLobby()
        {
            foreach(Player p in lobby.Players)
            {
                p.PanelAbout.ShowData();
            }
        }

        private void DrawExternalBuilding()
        {
            tabPageExternal.Controls.Clear();
            int top = 0;
            int left;
            PanelExternalBuilding p = null;

            foreach (Building b in Config.ExternalBuildings)
            {
                left = 0;
                foreach (BuildingOfPlayer bp in lobby.CurrentPlayer.ExternalBuildings[b.Position])
                {
                    p = new PanelExternalBuilding(lobby.CurrentPlayer, bp, ilExternalBuildings, ilResources24)
                    {
                        Parent = tabPageExternal,
                        Top = top,
                        Left = left
                    };

                    left += p.Width + Config.GRID_SIZE;
                }

                if (p != null)
                    top += p.Height;
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

                        g.Panel = new PanelGuild(left, top, ilGuilds);
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

        private void DrawChieftain()
        {
            PictureBox pb;
            int top = 64;
            int stepFromLeft = 0;

            foreach (Skill s in Config.Skills)
            {
                pb = new PictureBox()
                {
                    Parent = tabPageChieftain,
                    BorderStyle = BorderStyle.FixedSingle,
                    Top = top,
                    Left = Config.GRID_SIZE + (stepFromLeft * (ilSkills.ImageSize.Width + Config.GRID_SIZE)),
                    Width = ilSkills.ImageSize.Width,
                    Height = ilSkills.ImageSize.Height
                };

                SlotSkill.Add(pb);

                stepFromLeft++;
                if (stepFromLeft == 4)
                {
                    top = top + ilSkills.ImageSize.Height + Config.GRID_SIZE;
                    stepFromLeft = 0;
                }
            }
        }

        private void ShowChieftain()
        {
            Chieftain c = lobby.CurrentPlayer.Chieftain;
            LabelChieftainLevel.Text = "Уровень: " + c.Level.ToString();
            labelChieftainExp.Text = "Опыт: " + c.Experience.ToString();

            foreach (SkillOfChieftain sc in c.Skills)
            {
                SlotSkill[sc.Position].Image = ilSkills.Images[sc.Skill.Position * Config.MaxLevelSkill + sc.Level - 1];
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

        private void ShowExternalBuildings()
        {
            foreach (Control c in tabPageExternal.Controls)
                if (c is PanelExternalBuilding)
                    ((PanelExternalBuilding)c).ShowData();
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
    }
}
