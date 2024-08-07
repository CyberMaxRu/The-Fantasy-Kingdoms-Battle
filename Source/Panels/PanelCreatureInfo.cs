﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.Utils;    

namespace Fantasy_Kingdoms_Battle
{
    internal class PanelCreatureInfo : PanelBaseInfo
    {
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
        private readonly VCLabel labelNameState;
        private readonly VisualControl panelAbilitiesAndSecSkills;
        protected readonly VisualControl panelStatistics;
        private readonly PanelWithPanelEntity panelInventory;
        private readonly PanelWithPanelEntity panelAbilities;
        private readonly VCSeparator separSecSkills;
        private readonly PanelWithPanelEntity panelSecondarySkills;
        private readonly PanelWithPanelEntity panelPerks;
        private readonly VCTabButton btnStatistics;
        private readonly VCTabButton btnInventory;
        private readonly VCTabButton btnAbilities;
        private readonly VCTabButton btnPerks;
        protected VCCell panelSpecialization;
        private VCCell panelMeleeWeapon;
        private VCCell panelRangeWeapon;
        private VCCell panelArmour;
        protected VCIconAndDigitValue lvGold;

        private readonly VCLabel lblProperties;
        private readonly List<VCCreatureProperty> listProperties;
        private readonly VCLabel lblNeeds;
        private readonly List<VCCreatureNeed> listNeeds;
        private readonly VCLabel lblInterests;
        private readonly List<VCCreatureInterest> listInterests;

        internal List<VCCell> slots { get; } = new List<VCCell>();

        public PanelCreatureInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelAbilitiesAndSecSkills = new VisualControl();
            panelInventory = new PanelWithPanelEntity(4);
            panelAbilities = new PanelWithPanelEntity(4);
            panelSecondarySkills = new PanelWithPanelEntity(4);
            panelPerks = new PanelWithPanelEntity(4);

            bmpStateBackground = new VCBitmap(this, FormMain.Config.GridSize, TopForControls(), Program.formMain.bmpBandStateCreature);
            bmpStateBackground.ShowHint += BmpState_ShowHint;
            bmpState = new VCBitmap(bmpStateBackground, 6, 5, Program.formMain.BmpListStateHero.GetImage(0, true, false));
            bmpState.IsActiveControl = false;
            labelNameState = new VCLabel(bmpStateBackground, 36, 8, Program.formMain.FontSmallC, Color.White, 16, "");
            labelNameState.StringFormat.Alignment = StringAlignment.Near;
            labelNameState.StringFormat.LineAlignment = StringAlignment.Center;
            labelNameState.IsActiveControl = false;

            panelSpecialization = new VCCell(this, imgIcon.NextLeft(), imgIcon.ShiftY);
            panelMeleeWeapon = new VCCell(this, FormMain.Config.GridSize, bmpStateBackground.NextTop());
            panelMeleeWeapon.HintForEmpty = "Нет оружия ближнего боя";
            panelRangeWeapon = new VCCell(this, panelMeleeWeapon.NextLeft(), panelMeleeWeapon.ShiftY);
            panelRangeWeapon.HintForEmpty = "Нет оружия дальнего боя";
            panelArmour = new VCCell(this, panelRangeWeapon.NextLeft(), panelMeleeWeapon.ShiftY);
            panelArmour.HintForEmpty = "Нет доспехов";

            lvGold = new VCIconAndDigitValue(this, FormMain.Config.GridSize, panelMeleeWeapon.NextTop(), imgIcon.Width, FormMain.GUI_16_COFFERS);

            panelStatistics = new VisualControl();
            panelStatistics.Width = panelInventory.Width;

            separator.ShiftY = lvGold.NextTop();
            pageControl.ShiftY = separator.NextTop();
            btnStatistics = pageControl.AddTab("Статистика", FormMain.Config.Gui48_Scroll, panelStatistics);
            btnInventory = pageControl.AddTab("Инвентарь", FormMain.Config.Gui48_Inventory, panelInventory);
            btnAbilities = pageControl.AddTab("Способности и навыки", FormMain.Config.Gui48_Target, panelAbilitiesAndSecSkills);
            btnPerks = pageControl.AddTab("Характеристики", FormMain.Config.Gui48_Book, panelPerks);
            
            panelAbilitiesAndSecSkills.AddControl(panelAbilities);
            panelAbilitiesAndSecSkills.AddControl(panelSecondarySkills);
            panelSecondarySkills.ShiftY = panelAbilities.NextTop();// Это для расчета минимальной высоты
            panelAbilitiesAndSecSkills.ArrangeControl(panelSecondarySkills);
            panelAbilitiesAndSecSkills.Height = panelSecondarySkills.NextTop();
            separSecSkills = new VCSeparator(panelAbilitiesAndSecSkills, 0, 0);

            // Основные характеристики
            lblProperties = new VCLabel(panelStatistics, 0, 0, Program.formMain.FontSmall, Color.White, 16, "Основные характеристики:");
            lblProperties.StringFormat.Alignment = StringAlignment.Near;

            listProperties = new List<VCCreatureProperty>();
            for (int i = 0; i < FormMain.Descriptors.PropertiesCreature.Count; i++)
                listProperties.Add(new VCCreatureProperty(panelStatistics, 0, 0, 51));

            // Потребности
            lblNeeds = new VCLabel(panelStatistics, 0, 0, Program.formMain.FontSmall, Color.White, 16, "Потребности:");
            lblNeeds.StringFormat.Alignment = StringAlignment.Near;

            listNeeds = new List<VCCreatureNeed>();
            for (int i = 0; i < FormMain.Descriptors.NeedsCreature.Count; i++)
                listNeeds.Add(new VCCreatureNeed(panelStatistics, 0, 0, 51));

            // Интересы
            lblInterests = new VCLabel(panelStatistics, 0, 0, Program.formMain.FontSmall, Color.White, 16, "Интересы:");
            lblInterests.StringFormat.Alignment = StringAlignment.Near;

            listInterests = new List<VCCreatureInterest>();
            for (int i = 0; i < FormMain.Descriptors.InterestCreature.Count; i++)
                listInterests.Add(new VCCreatureInterest(panelStatistics, 0, 0, 51));

            pageControl.ApplyMinSize();

            CreateCustomControls();

            Width = pageControl.Width + FormMain.Config.GridSize * 2;
            Height = pageControl.NextTop();

            panelAbilitiesAndSecSkills.Width = pageControl.Width;

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

        protected virtual void CreateCustomControls()
        {

        }

        private void BmpState_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header(Creature.StateCreature.Name);
            PanelHint.AddStep5Description(Creature.StateCreature.Description);
        }

        internal Creature Creature { get => Entity as Creature; }

        internal override void ArrangeControls()
        {
            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
            labelNameState.Width = bmpStateBackground.Width - labelNameState.ShiftX - FormMain.Config.GridSize;
            separSecSkills.Width = panelAbilities.Width;
            panelStatistics.Height = pageControl.Height - panelStatistics.ShiftY - FormMain.Config.GridSize;
            lblProperties.Width = panelStatistics.Width;
            lblNeeds.Width = panelStatistics.Width;
            lblInterests.Width = panelStatistics.Width;

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            imgIcon.Level = Creature.GetLevel();
            bmpState.Bitmap = Program.formMain.BmpListStateHero.GetImage(Creature.StateCreature.ImageIndex, true, false);
            labelNameState.Text = Creature.StateCreature.Name;

            panelSpecialization.Entity = Creature.Specialization;// ImageIndex = creature.Specialization != null ? creature.Specialization.ImageIndex : -1;

            panelMeleeWeapon.Entity = Creature.MeleeWeapon;
            panelRangeWeapon.Entity =  Creature.RangeWeapon;
            panelArmour.Entity = Creature.Armour;

            panelInventory.ApplyList(Creature.Inventory);
            panelAbilities.ApplyList(Creature.Abilities);
            panelSecondarySkills.ApplyList(Creature.SecondarySkills);
            panelPerks.ApplyList(Creature.Perks);

            separSecSkills.ShiftY = panelAbilities.NextTop();
            panelAbilitiesAndSecSkills.ArrangeControl(separSecSkills);
            panelSecondarySkills.ShiftY = separSecSkills.NextTop();
            panelAbilitiesAndSecSkills.ArrangeControl(panelSecondarySkills);

            btnInventory.Quantity = Creature.Inventory.Count;
            btnAbilities.Quantity = Creature.Abilities.Count;
            btnPerks.Quantity = Creature.Perks.Count;

            // 
            ShowChapter(lblProperties, lblNeeds, Creature.Properties.ToList(), listProperties);
            ShowChapter(lblNeeds, lblInterests, Creature.Needs.ToList(), listNeeds);
            ShowChapter(lblInterests, null, Creature.Interests.ToList(), listInterests);

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

        protected override int GetImageIndex() => Creature.TypeCreature.ImageIndex;
        protected override bool ImageIsEnabled() => true;
        protected override string GetCaption() => Creature.TypeCreature.Name;
    }
}
