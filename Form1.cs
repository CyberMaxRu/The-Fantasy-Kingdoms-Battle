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

        private readonly Lobby lobby;

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
                pap = new PanelAboutPlayer(p, ilFractions);
                pap.Parent = tabPage1;
                pap.Top = top;

                top += pap.Height + Config.GRID_SIZE;
            }

            //
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
            foreach (Resource r in Config.Resources)
            {
                r.StatusLabel.Text = lobby.CurrentPlayer.Resources[r.Position].ToString();
            }
        }
    }
}
