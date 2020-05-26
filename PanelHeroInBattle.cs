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
        private ImageList ImageListHeroes;
        private Pen penBorder = new Pen(Color.Silver);
        private SolidBrush brushBandHealth = new SolidBrush(Color.Red);
        private SolidBrush brushBandHealthNone = new SolidBrush(Color.LightPink);
        private SolidBrush brushBandMana = new SolidBrush(Color.Blue);
        private SolidBrush brushBandManaNone = new SolidBrush(Color.LightBlue);
        private SolidBrush brushBandStamina = new SolidBrush(Color.Black);
        private SolidBrush brushBandStaminaNone = new SolidBrush(Color.Gray);

        public PanelHeroInBattle(ImageList ilHeroes) : base()
        {
            ImageListHeroes = ilHeroes;

            Width = ilHeroes.ImageSize.Width;
            Height = ilHeroes.ImageSize.Height + 2 + 4 * 3;

            Paint += PanelHeroInBattle_Paint;
        }

        internal HeroInBattle Hero { get; set; }

        private void PanelHeroInBattle_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // Рисуем иконку героя
            if (Hero != null)
            {
                Bitmap bmpIcon = new Bitmap(ImageListHeroes.Images[Hero.PlayerHero.ClassHero.ImageIndex]);
                if (Hero.PlayerHero.Player != Hero.Battle.Player1)
                    bmpIcon.RotateFlip(RotateFlipType.RotateNoneFlipX);
                e.Graphics.DrawImageUnscaled(bmpIcon, 0, 0);

                if (Hero.State != StateHeroInBattle.None)
                {
                    e.Graphics.DrawImageUnscaled(Program.formMain.ilStateHero.Images[(int)Hero.State], 0, 0);
                }

                // Рисуем полоски жизни, маны, бодрости
                DrawBand(0, brushBandHealth, brushBandHealthNone, 75, 100);
                DrawBand(1, brushBandMana, brushBandManaNone, 50, 100);
                DrawBand(2, brushBandStamina, brushBandStaminaNone, 25, 100);
            }

            e.Graphics.DrawRectangle(penBorder, 0, 0, ImageListHeroes.ImageSize.Width - 1, ImageListHeroes.ImageSize.Height - 1);

            void DrawBand(int line, Brush mainColor, Brush backColor, int currentValue, int MaxValue)
            {
                Debug.Assert(currentValue <= MaxValue);

                int widthMain = (int)Math.Round(currentValue / 1.00 / MaxValue * Width);
                int widthNone = Width - widthMain;
                e.Graphics.FillRectangle(mainColor, 0, ImageListHeroes.ImageSize.Height + 4 * line, widthMain, 4);
                if (widthNone > 0)
                    e.Graphics.FillRectangle(backColor, widthMain, ImageListHeroes.ImageSize.Height + 4 * line, widthNone, 4);
            }
        }
    }
}
