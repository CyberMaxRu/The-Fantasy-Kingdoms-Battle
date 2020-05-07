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
        private readonly Button btnDismiss;
        private Point pointLevel;

        public PanelHero(int left, int top, ImageList ilGuiHeroes, ImageList ilGui)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListGuiHeroes = ilGuiHeroes;
            imageListGui = ilGui;

            pbHero = new PictureBox()
            {
                Parent = this,
                Top = Config.GRID_SIZE,
                Left = Config.GRID_SIZE,
                Width = ilGuiHeroes.ImageSize.Width,
                Height = ilGuiHeroes.ImageSize.Height
            };
            pbHero.Click += PbHero_Click;
            pbHero.Paint += PbHero_Paint;
            
            Top = top;
            Left = left;
            Height = pbHero.Height + (Config.GRID_SIZE * 2);
            Width = GuiUtils.NextLeft(pbHero);

            pointLevel = new Point(2, pbHero.Height - 20);
        }

        private void PbHero_Paint(object sender, PaintEventArgs e)
        {
            if (Hero != null)
            {
                string level = Hero.Level.ToString();
                pointLevel.X = pbHero.Width - (level.Length * 12) - 6;
                e.Graphics.DrawString(level, Program.formMain.fontQuantity, Program.formMain.brushQuantity, pointLevel);
            }
        }

        internal PlayerHero Hero { get; private set; }

        internal void RefreshHero()
        {
            ShowData(Hero);
        }

        internal void ShowData(PlayerHero ph)
        {
            Hero = ph;

            pbHero.Image = ph != null ? GuiUtils.GetImageFromImageList(imageListGuiHeroes, Hero.Hero.ImageIndex, true) : null;
        }
        private void PbHero_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowAboutHero(Hero);
        }

    }
}
