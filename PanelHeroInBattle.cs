using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс отображения героя в бою
    class PanelHeroInBattle : Control 
    {
        private PlayerHero playerHero;
        private ImageList ImageListHeroes;
        private Pen penBandHealth = new Pen(Color.Red);
        private Pen penBandHealthNone = new Pen(Color.LightPink);
        private Pen penBandMana = new Pen(Color.Blue);
        private Pen penBandManaNone = new Pen(Color.LightBlue);
        private Pen penBandStamina = new Pen(Color.Black);
        private Pen penBandStaminaNone = new Pen(Color.Gray);

        public PanelHeroInBattle(PlayerHero ph, ImageList ilHeroes) : base()
        {
            playerHero = ph;
            ImageListHeroes = ilHeroes;

            Width = ilHeroes.ImageSize.Width;
            Height = ilHeroes.ImageSize.Height + 2 + 4 * 3;

            Paint += PanelHeroInBattle_Paint;
        }

        private void PanelHeroInBattle_Paint(object sender, PaintEventArgs e)
        {
            // Рисуем иконку героя
            e.Graphics.DrawImage(ImageListHeroes.Images[playerHero.Hero.ImageIndex], 0, 0);

            // Рисуем полоски жизни, маны, бодрости
            DrawBand(0, penBandHealth, penBandHealthNone, 100, 75);
            DrawBand(1, penBandMana, penBandManaNone, 100, 75);
            DrawBand(2, penBandStamina, penBandStaminaNone, 100, 75);

            void DrawBand(int line, Pen mainColor, Pen backColor, int MaxValue, int currentValue)
            {
                e.Graphics.DrawRectangle(mainColor, new Rectangle(0, ImageListHeroes.ImageSize.Height + 2 + line * 4, Width, ImageListHeroes.ImageSize.Height + 2 + (line + 1) * 4 - 1));
            }
        }
    }
}
