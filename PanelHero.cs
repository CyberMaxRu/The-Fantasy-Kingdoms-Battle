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
        private PlayerHero hero;

        public PanelHero(PlayerHero ph, int left, int top, ImageList ilGuiHeroes, ImageList ilGui)
        {
            hero = ph;

            BorderStyle = BorderStyle.FixedSingle;            

            pbHero = new PictureBox()
            {
                Parent = this,
                Top = Config.GRID_SIZE,
                Left = Config.GRID_SIZE,
                Width = ilGuiHeroes.ImageSize.Width,
                Height = ilGuiHeroes.ImageSize.Height,
                Image = ilGuiHeroes.Images[hero.Hero.ImageIndex]
            };

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
        }

        internal void ShowData()
        {
            lblLevel.Text = hero.Level.ToString();
        }
    }
}
