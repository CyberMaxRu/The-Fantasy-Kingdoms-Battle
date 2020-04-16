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

            _ = new Config(dirResources);

            // Подготавливаем иконки
            ilResources24 = PrepareImageList("Resources24.png", 24, 24);
            StatusStrip.ImageList = ilResources24;

            ilFractions = PrepareImageList("Fractions.png", 78, 52);

            ilExternalBuildings = PrepareImageList("ExternalBuildings.png", 82, 64);

            ilSkills = PrepareImageList("Skills.png", 82, 94);

            //    
            lobby = new Lobby(8);

            // Создаем метки под ресурсы
            foreach(Resource r in Config.Resources)
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
                pap = new PanelAboutPlayer(p, ilFractions)
                {
                    Parent = tabPage1,
                    Top = top
                };

                top += pap.Height + Config.GRID_SIZE;
            }

            //
            DrawChieftain();
            ShowDataPlayer();

            ImageList PrepareImageList(string filename, int width, int height)
            {
                ImageList il;
                il = new ImageList()
                {
                    ColorDepth = ColorDepth.Depth32Bit
                };
                _ = il.ImageSize = new Size(width, height);
                Bitmap icon;
                icon = new Bitmap(dirResources + "Icons\\" + filename);
                _ = il.Images.AddStrip(icon);

                return il;
            }
        }
        internal static Config Config { get; set; }
        internal ImageList ILFractions { get { return ilFractions; } }
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

                curAppliedPlayer = lobby.CurrentPlayerIndex;
            }

            ShowExternalBuildings();
            ShowChieftain();
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

        private void ShowExternalBuildings()
        {
            foreach (Control c in tabPageExternal.Controls)
                if (c is PanelExternalBuilding)
                    ((PanelExternalBuilding)c).ShowData();
        }

        private void ButtonEndTurn_Click(object sender, EventArgs e)
        {
            lobby.DoEndTurn();

            ShowDataPlayer();
        }
    }
}
