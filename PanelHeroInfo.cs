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
    internal sealed class PanelHeroInfo : PanelBaseInfo
    {
        private PlayerHero hero;
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

        private readonly PanelWithPanelEntity panelInventory = new PanelWithPanelEntity(3);
        private readonly PanelWithPanelEntity panelAbilities = new PanelWithPanelEntity(3);

        internal PanelEntity[] slots = new PanelEntity[FormMain.SLOT_IN_INVENTORY];

        public PanelHeroInfo(int width, int height) : base(width, height)
        {
            btnDismiss = new Button()
            {
                Parent = this,
                Left = LeftAfterIcon(),
                Top = TopForIcon(),
                ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.GUI_DISMISS,
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui)
            };
            btnDismiss.Click += BtnDismiss_Click;

            AddPage(Page.Statistics);
            AddPage(Page.Inventory);
            AddPage(Page.Abilities);

            panelInventory.Parent = this;
            panelInventory.Left = (Width - panelAbilities.Width) / 2;
            panelInventory.Top = LeftTopPage().Y;

            panelAbilities.Parent = this;
            panelAbilities.Left = (Width - panelAbilities.Width) / 2;
            panelAbilities.Top = LeftTopPage().Y;

            return;
            lblLevel = GuiUtils.CreateLabel(this, Config.GRID_SIZE, TopForControls());
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
            PanelEntity pb;
            for (int y = 0; y < FormMain.SLOTS_LINES; y++)
            {
                for (int x = 0; x < FormMain.SLOTS_IN_LINE; x++)
                {
                    pb = new PanelEntity(this, Program.formMain.ilItems, x + y * FormMain.SLOTS_IN_LINE);
                    pb.Left = Config.GRID_SIZE + ((pb.Width + Config.GRID_SIZE) * x);
                    pb.Top = GuiUtils.NextTop(lblSpeed) + ((pb.Height + Config.GRID_SIZE) * y);

                    slots[x + y * FormMain.SLOTS_IN_LINE] = pb;
                }
            }
        }

        internal PlayerHero Hero
        {
            get { return hero; }
            set
            {
                hero = value;
                ShowData();
            }
        }

        private void BtnDismiss_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Уволить героя?", "FKB", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Hero.Dismiss();
                Hero = null;

                Program.formMain.ShowAllBuildings(); // Так как количество героев уменьшилось, обновляем здания
                Program.formMain.ShowPageHeroes();
            }
        }

        internal override void ShowData()
        {
            base.ShowData();

            List<Item> items = new List<Item>();
            for (int x = 0; x < hero.Slots.Length; x++)
                if (hero.Slots[x] != null)
                    items.Add(hero.Slots[x].Item);

            panelInventory.ApplyListItem(items);
            panelAbilities.ApplyListAbility(Hero.Abilities);

            return;
            if (Hero != null)
            {
                Visible = true;

                lblLevel.Text = "Уровень: " + hero.Level.ToString();
                lblHealth.Text = "Здоровье: " + hero.ParametersWithAmmunition.Health.ToString();
                lblMana.Text = "Мана: " + hero.ParametersWithAmmunition.Mana.ToString();

                ShowParameter(lblStrength, hero.ParametersBase.Strength, hero.ParametersWithAmmunition.Strength);
                ShowParameter(lblDexterity, hero.ParametersBase.Dexterity, hero.ParametersWithAmmunition.Dexterity);
                ShowParameter(lblMagic, hero.ParametersBase.Magic, hero.ParametersWithAmmunition.Magic);
                ShowParameter(lblVitality, hero.ParametersBase.Vitality, hero.ParametersWithAmmunition.Vitality);
                //ShowParameter(lblStamina, ph.ParametersBase.Stamina, ph.ParametersWithAmmunition.Stamina);
                ShowParameter(lblSpeed, hero.ParametersBase.TimeAttack, hero.ParametersWithAmmunition.TimeAttack);
                if (Hero.ClassHero.KindHero.TypeAttack == TypeAttack.Melee)
                {
                    lblAttackMelee.Text = hero.ParametersWithAmmunition.MinMeleeDamage.ToString() + " - " + hero.ParametersWithAmmunition.MaxMeleeDamage.ToString();
                    lblAttackRange.Text = "";
                }
                if (Hero.ClassHero.KindHero.TypeAttack == TypeAttack.Missile)
                {
                    lblAttackMelee.Text = "";
                    lblAttackRange.Text = hero.ParametersWithAmmunition.MinMissileDamage.ToString() + " - " + hero.ParametersWithAmmunition.MaxMissileDamage.ToString();
                }
                ShowParameter(lblAttackMagic, hero.ParametersBase.MagicDamage, hero.ParametersWithAmmunition.MagicDamage);
                ShowParameter(lblDefenseMelee, hero.ParametersBase.DefenseMelee, hero.ParametersWithAmmunition.DefenseMelee);
                ShowParameter(lblDefenseRange, hero.ParametersBase.DefenseMissile, hero.ParametersWithAmmunition.DefenseMissile);
                ShowParameter(lblDefenseMagic, hero.ParametersBase.DefenseMagic, hero.ParametersWithAmmunition.DefenseMagic);

                for (int i = 0; i < hero.Slots.Length; i++)
                {
                    slots[i].ShowPlayerItem(hero.Slots[i]);
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
        protected override void ActivatePage(Page page)
        {
            switch (page)
            {
                case Page.Statistics:
                    panelInventory.Hide();
                    panelAbilities.Hide();
                    break;
                case Page.Inventory:
                    panelAbilities.Hide();
                    panelInventory.Show();
                    break;
                case Page.Abilities:
                    panelInventory.Hide();
                    panelAbilities.Show();
                    break;
                default:
                    throw new Exception("Неизвестная страница.");
            }

        }

        protected override ImageList GetImageList() => Program.formMain.ilHeroes;
        protected override int GetImageIndex() => hero.ClassHero.ImageIndex;
        protected override string GetCaption() => hero.ClassHero.Name;
    }
}
