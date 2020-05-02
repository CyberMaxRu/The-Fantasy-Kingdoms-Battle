using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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
        private readonly Button btnDismiss;

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

            btnDismiss = new Button()
            {
                Parent = this,
                Left = pbHero.Left + pbHero.Width + Config.GRID_SIZE,
                Top = pbHero.Top,
                ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.GUI_DISMISS,
                Width = Program.formMain.ilGui.ImageSize.Width + 8,
                Height = Program.formMain.ilGui.ImageSize.Height + 8
            };
            btnDismiss.Click += BtnDismiss_Click;

            lblLevel = GuiUtils.CreateLabel(this, Config.GRID_SIZE, pbHero.Top + pbHero.Height + Config.GRID_SIZE);
            lblHealth = GuiUtils.CreateLabel(this, Config.GRID_SIZE, lblLevel.Top + lblLevel.Height + Config.GRID_SIZE);
            lblMana = GuiUtils.CreateLabel(this, Config.GRID_SIZE, lblHealth.Top + lblHealth.Height + Config.GRID_SIZE);

            lblStrength = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblMana.Top + lblMana.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_STRENGTH);
            lblDexterity = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblStrength.Top + lblStrength.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_DEXTERITY);
            lblWisdom = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblDexterity.Top + lblDexterity.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_WISDOM);
            lblStamina = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblWisdom.Top + lblWisdom.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_STAMINA);
            lblSpeed = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblStamina.Top + lblStamina.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_SPEED);

            lblAttackMelee = GuiUtils.CreateLabelParameter(this, lblStrength.Left + lblStrength.Width + Config.GRID_SIZE, lblStrength.Top, FormMain.GUI_PARAMETER_ATTACK_MELEE);
            lblAttackRange = GuiUtils.CreateLabelParameter(this, lblDexterity.Left + lblDexterity.Width + Config.GRID_SIZE, lblDexterity.Top, FormMain.GUI_PARAMETER_ATTACK_RANGE);
            lblAttackMagic = GuiUtils.CreateLabelParameter(this, lblWisdom.Left + lblWisdom.Width + Config.GRID_SIZE, lblWisdom.Top, FormMain.GUI_PARAMETER_ATTACK_MAGIC);

            lblDefenseMelee = GuiUtils.CreateLabelParameter(this, lblAttackMelee.Left + lblAttackMelee.Width + Config.GRID_SIZE, lblAttackMelee.Top, FormMain.GUI_PARAMETER_DEFENSE_MELEE);
            lblDefenseRange = GuiUtils.CreateLabelParameter(this, lblAttackRange.Left + lblAttackRange.Width + Config.GRID_SIZE, lblAttackRange.Top, FormMain.GUI_PARAMETER_DEFENSE_RANGE);
            lblDefenseMagic = GuiUtils.CreateLabelParameter(this, lblAttackMagic.Left + lblAttackMagic.Width + Config.GRID_SIZE, lblAttackMagic.Top, FormMain.GUI_PARAMETER_DEFENSE_MAGIC);

            Width = pbHero.Width + (Config.GRID_SIZE * 2) + 160;
            Height = lblSpeed.Top + lblSpeed.Height + Config.GRID_SIZE;
        }

        internal PlayerHero Hero { get; private set; }

        private void BtnDismiss_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Уволить героя?", "FKB", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Hero.Dismiss();

                ShowHero(null);
                Program.formMain.ShowAllBuildings(); // Так как количество героев уменьшилось, обновляем здания
                Program.formMain.ShowHeroes();
            }
        }

        internal void ShowHero(PlayerHero ph)
        {
            Hero = ph;

            if (Hero != null)
            {
                Visible = true;
                pbHero.Image = imageListHeroes.Images[ph.Hero.ImageIndex];

                lblLevel.Text = "Уровень: " + ph.Level.ToString();
                lblHealth.Text = "Здоровье: " + ph.CurrentHealth.ToString() + "/" + ph.MaxHealth.ToString();
                lblMana.Text = "Мана: " + ph.CurrentMana.ToString() + "/" + ph.MaxMana.ToString();

                ShowParameter(lblStrength, ph.OurParameters.Strength, ph.ModifiedParameters.Strength);
                ShowParameter(lblDexterity, ph.OurParameters.Dexterity, ph.ModifiedParameters.Dexterity);
                ShowParameter(lblWisdom, ph.OurParameters.Wisdom, ph.ModifiedParameters.Wisdom);
                ShowParameter(lblStamina, ph.OurParameters.Stamina, ph.ModifiedParameters.Stamina);
                ShowParameter(lblSpeed, ph.OurParameters.Speed, ph.ModifiedParameters.Speed);
                ShowParameter(lblAttackMelee, ph.OurParameters.AttackMelee, ph.ModifiedParameters.AttackMelee);
                ShowParameter(lblAttackRange, ph.OurParameters.AttackRange, ph.ModifiedParameters.AttackRange);
                ShowParameter(lblAttackMagic, ph.OurParameters.AttackMagic, ph.ModifiedParameters.AttackMagic);
                ShowParameter(lblDefenseMelee, ph.OurParameters.DefenseMelee, ph.ModifiedParameters.DefenseMelee);
                ShowParameter(lblDefenseRange, ph.OurParameters.DefenseRange, ph.ModifiedParameters.DefenseRange);
                ShowParameter(lblDefenseMagic, ph.OurParameters.DefenseMagic, ph.ModifiedParameters.DefenseMagic);
            }
            else
                Hide();

            void ShowParameter(Label l, int normalParam, int modParam)
            {
                l.Text = modParam.ToString();
                if (normalParam == modParam)
                    l.ForeColor = Color.FromKnownColor(KnownColor.Black);
                else if (normalParam > modParam)
                    l.ForeColor = Color.FromKnownColor(KnownColor.Green);
                else 
                    l.ForeColor = Color.FromKnownColor(KnownColor.Red);
            }
        }
    }
}
