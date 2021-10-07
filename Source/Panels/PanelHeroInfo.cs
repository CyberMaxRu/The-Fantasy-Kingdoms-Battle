using System;
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
    // Класс подробной информации о герое
    internal sealed class PanelHeroInfo : PanelCreatureInfo
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
        private readonly VCButtonTargetLair btnTarget;

        private readonly VCIconAndDigitValue idvFood;
        private readonly VCIconAndDigitValue idvEnthusiasm;
        private readonly VCIconAndDigitValue idvLoyalty;

        public PanelHeroInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            btnTarget = new VCButtonTargetLair(this);
            btnTarget.ShiftX = panelSpecialization.ShiftX;
            btnTarget.ShiftY = panelSpecialization.NextTop();
            btnTarget.ShowFlag = false;

            btnDismiss = new Button()
            {
                //Parent = this,
                Left = LeftAfterIcon(),
                Top = TopForIcon(),
                //ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.Config.Gui48_Dismiss
                //Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui)
            };
            btnDismiss.Click += BtnDismiss_Click;

            lvGold.ShowHint += LvGold_ShowHint;

            idvFood = new VCIconAndDigitValue(panelStatistics, 0, 0, 64, FormMain.GUI_16_FOOD);
            idvFood.ShowHint += IdvFood_ShowHint;

            idvEnthusiasm = new VCIconAndDigitValue(panelStatistics, idvFood.NextLeft(), 0, 64, FormMain.GUI_16_ENTHUSIASM);
            idvEnthusiasm.ShowHint += IdvEnthusiasm_ShowHint;

            idvLoyalty = new VCIconAndDigitValue(panelStatistics, idvEnthusiasm.NextLeft(), 0, 64, FormMain.GUI_16_LOYALTY);
            idvLoyalty.ShowHint += IdvLoyalty_ShowHint;

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

        private void IdvLoyalty_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep2Header("Лояльность");
            Program.formMain.formHint.AddStep5Description($"Лояльность: {DecIntegerBy10(Hero.Loyalty)}");
            if ((Hero.TypeCreature.Loyalty != 0) || (Hero.ListSourceLoyalty.Count > 0))
            { 
                List<(DescriptorEntity, string)> list = new List<(DescriptorEntity, string)>();

                if (Hero.TypeCreature.Loyalty != 0)
                    list.Add((Hero.TypeCreature, DecIntegerBy10(Hero.TypeCreature.Loyalty, true)));

                foreach (Perk p in Hero.ListSourceLoyalty)
                {
                    Debug.Assert(p.Descriptor.Loyalty != 0);

                    list.Add((p.Descriptor, DecIntegerBy10(p.Descriptor.Loyalty, true)));
                }

                Program.formMain.formHint.AddStep19Descriptors(list);
            }
        }

        private void IdvEnthusiasm_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep2Header("Энтузиазм");
            Program.formMain.formHint.AddStep5Description($"Энтузиазм: {DecIntegerBy10(Hero.Enthusiasm)}{Environment.NewLine}Уменьшение в день (на перк): -{DecIntegerBy10(Hero.EnthusiasmPerDay)}");
        }

        private void IdvFood_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep2Header("Сытость");
            Program.formMain.formHint.AddStep5Description($"Сытость: {DecIntegerBy10(Hero.CurrentSatiety)}/{DecIntegerBy10(Hero.MaxSatiety)}{Environment.NewLine}"
                + $"Потребление в день: {DecIntegerBy10(Hero.ReductionSatietyPerDay)}");
        }

        private void LvGold_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddSimpleHint(Hero.Gold > 0 ? $"Золота в кошельке: {Hero.Gold}" : "Кошелек пуст");
        }

        internal Hero Hero { get => Entity as Hero; }

        private void BtnDismiss_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Уволить героя?", "FKB", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Hero.Dismiss();
                Entity = null;
                Program.formMain.SelectPlayerObject(null);
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        internal override void Draw(Graphics g)
        {
            btnTarget.Entity = Hero.TargetByFlag;
            lvGold.Text = Hero.Gold.ToString();
            idvFood.Text = DecIntegerBy10(Hero.CurrentSatiety).ToString();
            idvEnthusiasm.Text = DecIntegerBy10(Hero.Enthusiasm).ToString();
            idvLoyalty.Text = DecIntegerBy10(Hero.Loyalty).ToString();

            base.Draw(g);
        }

        internal void ShowData()
        {
            //base.ShowData();

            return;
            /*if (Hero != null)
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

        protected override int GetImageIndex() => Program.formMain.TreatImageIndex(Hero.TypeCreature.ImageIndex, Hero.Player);
        protected override bool ImageIsEnabled() => true;
        protected override string GetCaption() => Hero.GetNameHero();
    }
}
