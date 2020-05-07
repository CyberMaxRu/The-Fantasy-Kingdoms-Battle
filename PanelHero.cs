using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели с информацией о герое
    internal sealed class PanelHero : Panel
    {
        private readonly PictureBox pbHero;
        private readonly ImageList imageListGuiHeroes;
        private readonly ImageList imageListGui;
        private readonly Label lblLevel;
        private readonly Button btnDismiss;
        private Point pointLevel;

        public PanelHero(PlayerHero ph, int left, int top, ImageList ilGuiHeroes, ImageList ilGui)
        {
            Hero = ph;

            BorderStyle = BorderStyle.FixedSingle;            

            pbHero = new PictureBox()
            {
                Parent = this,
                Top = Config.GRID_SIZE,
                Left = Config.GRID_SIZE,
                Width = ilGuiHeroes.ImageSize.Width,
                Height = ilGuiHeroes.ImageSize.Height,
                Image = ilGuiHeroes.Images[Hero.Hero.ImageIndex]
            };
            pbHero.Click += PbHero_Click;
            pbHero.Paint += PbHero_Paint;

            lblLevel = new Label()
            {
                Parent = this,
                Top = pbHero.Top,
                Left = pbHero.Left + pbHero.Width + Config.GRID_SIZE,
                Height = 32,
                Width = 32,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromKnownColor(KnownColor.SkyBlue)
            };

            Top = top;
            Left = left;
            Height = pbHero.Height + (Config.GRID_SIZE * 2);
            Width = lblLevel.Left + lblLevel.Width + (Config.GRID_SIZE * 2);

            pointLevel = new Point(2, pbHero.Height - 20);
        }

        private void PbHero_Paint(object sender, PaintEventArgs e)
        {
            string level = Hero.Level.ToString();
            pointLevel.X = pbHero.Width - (level.Length * 12) - 6;
            e.Graphics.DrawString(level, Program.formMain.fontQuantity, Program.formMain.brushQuantity, pointLevel);
        }

        internal PlayerHero Hero { get; private set; }

        internal void ShowData()
        {
            //lblLevel.Text = Hero.Level.ToString();
        }
        private void PbHero_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowAboutHero(Hero);
        }

    }
}
