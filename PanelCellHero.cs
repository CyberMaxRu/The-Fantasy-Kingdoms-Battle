using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели с ячейкой героя
    internal sealed class PanelCellHero : PictureBox
    {
        private ImageList imageListHeroes;
        private Point pointLevel;

        public PanelCellHero(ImageList ilHeroes)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListHeroes = ilHeroes;
            Size = GuiUtils.SizePictureBoxWithImage(imageListHeroes);
            Paint += PanelCellHero_Paint;

            pointLevel = new Point(2, Height - 20);
        }

        private void PanelCellHero_Paint(object sender, PaintEventArgs e)
        {
            string level = Hero.Level.ToString();
            pointLevel.X = Width - (level.Length * 12) - 6;
            e.Graphics.DrawString(level, Program.formMain.fontQuantity, Program.formMain.brushQuantity, pointLevel);
        }

        internal PlayerHero Hero { get; private set; }

        internal void RefreshHero()
        {
            ShowHero(Hero);
        }

        internal void ShowHero(PlayerHero ph)
        {
            Hero = ph;

            Image = ph != null ? GuiUtils.GetImageFromImageList(imageListHeroes, Hero.ClassHero.ImageIndex, true) : null;
        }
    }
}
