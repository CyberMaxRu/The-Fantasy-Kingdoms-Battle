using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal class PanelCreatureInfo : PanelBaseInfo
    {
        private Creature creature;
        private readonly VCLabel lblKindHero;
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

        private readonly VCBitmap bmpStateBackground;
        private readonly VCBitmap bmpState;
        private readonly VCLabelM2 labelNameState;
        private readonly VisualControl panelAbilitiesAndSecSkills;
        private readonly PanelWithPanelEntity panelInventory;
        private readonly PanelWithPanelEntity panelAbilities;
        private readonly PanelWithPanelEntity panelSecondarySkills;
        private VCCell panelSpecialization;
        private VCCell panelWeapon;
        private VCCell panelArmour;
        internal List<VCCell> slots { get; } = new List<VCCell>();

        public PanelCreatureInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelAbilitiesAndSecSkills = new VisualControl();
            panelInventory = new PanelWithPanelEntity(4);
            panelAbilities = new PanelWithPanelEntity(4);
            panelSecondarySkills = new PanelWithPanelEntity(4);

            lblKindHero = new VCLabel(this, FormMain.Config.GridSize, TopForControls(), FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, 16, "");
            lblKindHero.StringFormat.Alignment = StringAlignment.Near;

            bmpStateBackground = new VCBitmap(this, FormMain.Config.GridSize, lblKindHero.NextTop(), Program.formMain.bmpBandStateCreature);
            bmpState = new VCBitmap(this, 14, bmpStateBackground.ShiftY + 5, null);
            labelNameState = new VCLabelM2(this, 44, bmpStateBackground.ShiftY + 8, Program.formMain.fontSmallC, Color.White, 16, "");
            labelNameState.StringFormat.Alignment = StringAlignment.Near;
            labelNameState.StringFormat.LineAlignment = StringAlignment.Center;

            panelSpecialization = new VCCell(this, imgIcon.NextLeft(), imgIcon.ShiftY);
            panelWeapon = new VCCell(this, FormMain.Config.GridSize, bmpStateBackground.NextTop());
            panelArmour = new VCCell(this, panelWeapon.NextLeft(), panelWeapon.ShiftY);

            separator.ShiftY = panelWeapon.NextTop();
            pageControl.ShiftY = separator.NextTop();
            pageControl.AddTab("Статистика", FormMain.GUI_SCROLL, null);
            pageControl.AddTab("Инвентарь", FormMain.GUI_INVENTORY, panelInventory);
            pageControl.AddTab("Способности и навыки", FormMain.GUI_TARGET, panelAbilitiesAndSecSkills);
            pageControl.AddTab("История", FormMain.GUI_BOOK, null);

            panelAbilitiesAndSecSkills.AddControl(panelAbilities);
            panelAbilitiesAndSecSkills.AddControl(panelSecondarySkills);
            panelSecondarySkills.ShiftY = panelAbilities.NextTop();// Это для расчета минимальной высоты
            panelAbilitiesAndSecSkills.ArrangeControl(panelSecondarySkills);

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
            Height = pageControl.NextTop();

            lblKindHero.Width = Width - lblKindHero.ShiftX - FormMain.Config.GridSize;

            return;
            /*lblLevel = GuiUtils.CreateLabel(this, Config.GRID_SIZE, TopForControls());
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
                    pb = new PanelEntity();
                    pb.Parent = this;
                    pb.Left = Config.GRID_SIZE + ((pb.Width + Config.GRID_SIZE) * x);
                    pb.Top = GuiUtils.NextTop(lblSpeed) + ((pb.Height + Config.GRID_SIZE) * y);

                    slots[x + y * FormMain.SLOTS_IN_LINE] = pb;
                }
            }*/
        }

        internal Creature Creature
        {
            get { return creature; }
            set
            {
                creature = value;
            }
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
            lblKindHero.Width = Width - (lblKindHero.ShiftX * 2);
            labelNameState.Width = Width - labelNameState.ShiftX - -FormMain.Config.GridSize;
        }

        protected override PlayerObject GetPlayerObject()
        {
            return Creature;
        }

        internal override void Draw(Graphics g)
        {
            lblKindHero.Text = creature.TypeCreature.KindCreature.Name;
            bmpState.Bitmap = Program.formMain.ilStateHero.GetImage(creature.StateCreature.ImageIndex, true, false);
            labelNameState.Text = creature.StateCreature.Name;

            panelSpecialization.ShowCell(Creature.Specialization);// ImageIndex = creature.Specialization != null ? creature.Specialization.ImageIndex : -1;

            panelWeapon.ShowCell(Creature.RangeWeapon != null ? Creature.RangeWeapon : Creature.MeleeWeapon);
            panelArmour.ShowCell(Creature.Armour);

            panelInventory.ApplyList(creature.Inventory);
            panelAbilities.ApplyList(creature.Abilities);
            panelSecondarySkills.ApplyList(creature.SecondarySkills);

            panelSecondarySkills.ShiftY = panelAbilities.NextTop();
            panelAbilitiesAndSecSkills.ArrangeControl(panelSecondarySkills);

            base.Draw(g);
            
/*            if (creature != null)
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
                lblAttackMelee.Text = hero.ParametersWithAmmunition.MinMeleeDamage.ToString() + " - " + hero.ParametersWithAmmunition.MaxMeleeDamage.ToString();
                lblAttackRange.Text = hero.ParametersWithAmmunition.MinArcherDamage.ToString() + " - " + hero.ParametersWithAmmunition.MaxArcherDamage.ToString();
                ShowParameter(lblAttackMagic, hero.ParametersBase.MagicDamage, hero.ParametersWithAmmunition.MagicDamage);
                ShowParameter(lblDefenseMelee, hero.ParametersBase.DefenseMelee, hero.ParametersWithAmmunition.DefenseMelee);
                ShowParameter(lblDefenseRange, hero.ParametersBase.DefenseArcher, hero.ParametersWithAmmunition.DefenseArcher);
                ShowParameter(lblDefenseMagic, hero.ParametersBase.DefenseMagic, hero.ParametersWithAmmunition.DefenseMagic);

            }
            else
                Visible = false;
*/

            void ShowParameter(Label l, int normalParam, int modParam)
            {
                l.Text = modParam.ToString();
                if (normalParam == modParam)
                    l.ForeColor = FormMain.Config.UnitNormalParam;
                else if (normalParam > modParam)
                    l.ForeColor = FormMain.Config.UnitHighNormalParam;
                else
                    l.ForeColor = FormMain.Config.UnitLowNormalParam;
            }
        }

        protected override int GetImageIndex() => creature.TypeCreature.ImageIndex;
        protected override bool ImageIsEnabled() => true;
        protected override string GetCaption() => creature.TypeCreature.Name;
    }
}
