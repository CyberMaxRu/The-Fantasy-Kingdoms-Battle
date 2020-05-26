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
        private readonly Label lblMagic;
        private readonly Label lblVitality;
        private readonly Label lblStamina;
        private readonly Label lblSpeed;
        private readonly Label lblAttackMelee;
        private readonly Label lblAttackRange;
        private readonly Label lblAttackMagic;
        private readonly Label lblDefenseMelee;
        private readonly Label lblDefenseRange;
        private readonly Label lblDefenseMagic;
        private readonly Button btnDismiss;

        internal PanelItem[] slots = new PanelItem[FormMain.SLOT_IN_INVENTORY];

        private readonly ImageList imageListHeroes;
        private readonly ImageList imageListItems;

        public PanelHeroInfo(ImageList ilHeroes, ImageList ilParameters, ImageList ilItems)
        {
            imageListHeroes = ilHeroes;
            imageListItems = ilItems;

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
                Left = GuiUtils.NextLeft(pbHero),
                Top = pbHero.Top,
                ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.GUI_DISMISS,
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui)
            };
            btnDismiss.Click += BtnDismiss_Click;

            lblLevel = GuiUtils.CreateLabel(this, Config.GRID_SIZE, pbHero.Top + pbHero.Height + Config.GRID_SIZE);
            lblHealth = GuiUtils.CreateLabel(this, Config.GRID_SIZE, lblLevel.Top + lblLevel.Height + Config.GRID_SIZE);
            lblMana = GuiUtils.CreateLabel(this, Config.GRID_SIZE, lblHealth.Top + lblHealth.Height + Config.GRID_SIZE);

            lblStrength = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblMana.Top + lblMana.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_STRENGTH);
            lblDexterity = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblStrength.Top + lblStrength.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_DEXTERITY);
            lblMagic = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblDexterity.Top + lblDexterity.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_MAGIC);
            lblVitality = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblMagic.Top + lblMagic.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_VITALITY);
            lblSpeed = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblVitality.Top + lblVitality.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_SPEED_ATTACK);

//            lblStamina = GuiUtils.CreateLabelParameter(this, Config.GRID_SIZE, lblWisdom.Top + lblWisdom.Height + Config.GRID_SIZE, FormMain.GUI_PARAMETER_STAMINA);
            lblAttackMelee = GuiUtils.CreateLabelParameter(this, lblStrength.Left + lblStrength.Width + Config.GRID_SIZE, lblStrength.Top, FormMain.GUI_PARAMETER_ATTACK_MELEE);
            lblAttackRange = GuiUtils.CreateLabelParameter(this, lblDexterity.Left + lblDexterity.Width + Config.GRID_SIZE, lblDexterity.Top, FormMain.GUI_PARAMETER_ATTACK_RANGE);
            lblAttackMagic = GuiUtils.CreateLabelParameter(this, lblMagic.Left + lblMagic.Width + Config.GRID_SIZE, lblMagic.Top, FormMain.GUI_PARAMETER_ATTACK_MAGIC);

            lblDefenseMelee = GuiUtils.CreateLabelParameter(this, lblAttackMelee.Left + lblAttackMelee.Width + Config.GRID_SIZE, lblAttackMelee.Top, FormMain.GUI_PARAMETER_DEFENSE_MELEE);
            lblDefenseRange = GuiUtils.CreateLabelParameter(this, lblAttackRange.Left + lblAttackRange.Width + Config.GRID_SIZE, lblAttackRange.Top, FormMain.GUI_PARAMETER_DEFENSE_RANGE);
            lblDefenseMagic = GuiUtils.CreateLabelParameter(this, lblAttackMagic.Left + lblAttackMagic.Width + Config.GRID_SIZE, lblAttackMagic.Top, FormMain.GUI_PARAMETER_DEFENSE_MAGIC);

            // Слоты инвентаря
            PanelItem pb;
            for (int y = 0; y < FormMain.SLOTS_LINES; y++)
            {
                for (int x = 0; x < FormMain.SLOTS_IN_LINE; x++)
                {
                    pb = new PanelItem(this, ilItems, x + y * FormMain.SLOTS_IN_LINE);
                    pb.Left = Config.GRID_SIZE + ((pb.Width + Config.GRID_SIZE) * x);
                    pb.Top = GuiUtils.NextTop(lblSpeed) + ((pb.Height + Config.GRID_SIZE) * y);

                    slots[x + y * FormMain.SLOTS_IN_LINE] = pb;
                }
            }

            Width = pbHero.Width + (Config.GRID_SIZE * 2) + 160;
            Height = GuiUtils.NextTop(slots[FormMain.SLOT_IN_INVENTORY - 1]);
        }

        internal PlayerHero Hero { get; private set; }

        private void BtnDismiss_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Уволить героя?", "FKB", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Hero.Dismiss();

                ShowHero(null);
                Program.formMain.ShowAllBuildings(); // Так как количество героев уменьшилось, обновляем здания
                Program.formMain.ShowPageHeroes();
            }
        }

        internal void RefreshHero()
        {
            ShowHero(Hero);
        }

        internal void ShowHero(PlayerHero ph)
        {
            Hero = ph;

            if (Hero != null)
            {
                Visible = true;
                pbHero.Image = imageListHeroes.Images[ph.ClassHero.ImageIndex];

                lblLevel.Text = "Уровень: " + ph.Level.ToString();
                lblHealth.Text = "Здоровье: " + ph.ParametersWithAmmunition.Health.ToString();
                lblMana.Text = "Мана: " + ph.ParametersWithAmmunition.Mana.ToString();

                ShowParameter(lblStrength, ph.ParametersBase.Strength, ph.ParametersWithAmmunition.Strength);
                ShowParameter(lblDexterity, ph.ParametersBase.Dexterity, ph.ParametersWithAmmunition.Dexterity);
                ShowParameter(lblMagic, ph.ParametersBase.Magic, ph.ParametersWithAmmunition.Magic);
                ShowParameter(lblVitality, ph.ParametersBase.Vitality, ph.ParametersWithAmmunition.Vitality);
                //ShowParameter(lblStamina, ph.ParametersBase.Stamina, ph.ParametersWithAmmunition.Stamina);
                ShowParameter(lblSpeed, ph.ParametersBase.TimeAttack, ph.ParametersWithAmmunition.TimeAttack);
                if (Hero.ClassHero.TypeAttack == TypeAttack.Melee)
                {
                    lblAttackMelee.Text = ph.ParametersWithAmmunition.MinMeleeDamage.ToString() + " - " + ph.ParametersWithAmmunition.MaxMeleeDamage.ToString();
                    lblAttackRange.Text = "";
                }
                if (Hero.ClassHero.TypeAttack == TypeAttack.Missile)
                {
                    lblAttackMelee.Text = "";
                    lblAttackRange.Text = ph.ParametersWithAmmunition.MinMissileDamage.ToString() + " - " + ph.ParametersWithAmmunition.MaxMissileDamage.ToString();
                }
                ShowParameter(lblAttackMagic, ph.ParametersBase.MagicDamage, ph.ParametersWithAmmunition.MagicDamage);
                ShowParameter(lblDefenseMelee, ph.ParametersBase.DefenseMelee, ph.ParametersWithAmmunition.DefenseMelee);
                ShowParameter(lblDefenseRange, ph.ParametersBase.DefenseMissile, ph.ParametersWithAmmunition.DefenseMissile);
                ShowParameter(lblDefenseMagic, ph.ParametersBase.DefenseMagic, ph.ParametersWithAmmunition.DefenseMagic);

                for (int i = 0; i < ph.Slots.Length; i++)
                {
                    slots[i].ShowItem(ph.Slots[i]);
                }

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
