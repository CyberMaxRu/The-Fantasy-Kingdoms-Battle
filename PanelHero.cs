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
    internal sealed class PanelHero : PictureBox
    {
        private readonly ImageList imageListGuiHeroes;
        private readonly ImageList imageListGui;
        private readonly ImageList imageListCategoryHeroes;
        private Point pointLevel;

        public PanelHero(Point p, int left, int top, ImageList ilGuiHeroes, ImageList ilGui, ImageList ilCategoryHeroes)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListGuiHeroes = ilGuiHeroes;
            imageListGui = ilGui;
            imageListCategoryHeroes = ilCategoryHeroes;

            BackColor = Color.Transparent;
            Left = left;
            Top = top;
            Size = GuiUtils.SizePictureBoxWithImage(ilGuiHeroes);
            Click += PanelHero_Click;
            Paint += PanelHero_Paint;

            Point = p;

            pointLevel = new Point(2, Height - 20);
        }

        private void PanelHero_Paint(object sender, PaintEventArgs e)
        {
            if ((Hero != null) && (this != Program.formMain.panelHeroForDrag))
            { 
                string level = Hero.Level.ToString();
                pointLevel.X = Width - (level.Length * 12) - 6;
                e.Graphics.DrawString(level, Program.formMain.fontQuantity, Program.formMain.brushQuantity, pointLevel);
            }
        }

        internal Point Point { get; }
        internal PlayerHero Hero { get; private set; }
        internal CategoryHero CategoryHero { get; private set; }

        internal void RefreshHero()
        {
            ShowData(Hero, CategoryHero);
        }

        internal void ShowData(PlayerHero ph, CategoryHero category)
        {
            Hero = ph;
            CategoryHero = category;

            if (this != Program.formMain?.panelHeroForDrag)
            {
                Image = ph != null ? GuiUtils.GetImageFromImageList(imageListGuiHeroes, Hero.Hero.ImageIndex, true) : GuiUtils.GetImageFromImageList(imageListCategoryHeroes, (int)category, true);
            }
            else
                Image = null;
        }
        private void PanelHero_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowAboutHero(Hero);
        }
    }
}
