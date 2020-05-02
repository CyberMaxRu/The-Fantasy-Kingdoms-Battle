using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс подробной информации о герое
    internal sealed class PanelHeroInfo : Panel
    {
        private readonly PictureBox pbHero;
        private readonly Label lblLevel;
        private readonly Label lblHealth;
        private readonly Label lblMana;
        private readonly Label lblStrength;
        private readonly Label lblDexterity;
        private readonly Label lblWisdom;
        private readonly Label lblStamina;
        private readonly Label lblSpeed;
        private readonly Label lblAttackMelee;
        private readonly Label lblAttackRange;
        private readonly Label lblAttackMagic;
        private readonly Label lblDefenseMelee;
        private readonly Label lblDefenseRange;
        private readonly Label lblDefenseMagic;

        private readonly ImageList imageListHeroes;
        private readonly ImageList imageListParameters;

        public PanelHeroInfo(ImageList ilHeroes, ImageList ilParameters)
        {
            imageListHeroes = ilHeroes;

            BorderStyle = BorderStyle.FixedSingle;

            pbHero = new PictureBox()
            {
                Parent = this,
                Top = Config.GRID_SIZE,
                Left = Config.GRID_SIZE,
                Width = ilHeroes.ImageSize.Width,
                Height = ilHeroes.ImageSize.Height,
            };

            lblLevel = GuiUtils.CreateLabel(this, Config.GRID_SIZE, pbHero.Top + pbHero.Height + Config.GRID_SIZE);
            lblHealth = GuiUtils.CreateLabel(this, Config.GRID_SIZE, lblLevel.Top + lblLevel.Height + Config.GRID_SIZE);
            lblMana = GuiUtils.CreateLabel(this, Config.GRID_SIZE, lblHealth.Top + lblHealth.Height + Config.GRID_SIZE);



            Width = pbHero.Width + (Config.GRID_SIZE * 2);
            Height = lblMana.Top + lblMana.Height + Config.GRID_SIZE;
        }

        internal void ShowHero(PlayerHero ph)
        {
            pbHero.Image = imageListHeroes.Images[ph.Hero.ImageIndex];

            lblLevel.Text = "Уровень: " + ph.Level.ToString();
            lblHealth.Text = "Здоровье: " + ph.CurrentHealth.ToString() + "/" + ph.MaxHealth.ToString();
            lblMana.Text = "Мана: " + ph.CurrentMana.ToString() + "/" + ph.MaxMana.ToString();
        }
    }
}
