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

        private readonly VCLabel lblCharacters;
        private readonly List<VCCreatureProperty> listProperties;
        private readonly VCLabel lblNeeds;
        private readonly List<VCCreatureNeed> listNeeds;
        private readonly VCLabel lblInterests;
        private readonly VCSeparator separator1;
        private readonly VCSeparator separator2;

        private readonly VCIconAndDigitValue idvInterestAttack;
        private readonly VCIconAndDigitValue idvInterestDefense;
        private readonly VCIconAndDigitValue idvInterestExplore;
        private readonly VCIconAndDigitValue idvInterestOther;

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

            lvGold.ImageIndex = FormMain.GUI_16_PURSE;
            lvGold.ShowHint += LvGold_ShowHint;

            // Основные характеристики
            lblCharacters = new VCLabel(panelStatistics, 0, 0, Program.formMain.fontSmall, Color.White, 16, "Основные характеристики:");
            lblCharacters.StringFormat.Alignment = StringAlignment.Near;

            listProperties = new List<VCCreatureProperty>();

            // Потребности
            separator1 = new VCSeparator(panelStatistics, 0, lblCharacters.NextTop());

            lblNeeds = new VCLabel(panelStatistics, 0, separator1.NextTop() - 8, Program.formMain.fontSmall, Color.White, 16, "Потребности:");
            lblNeeds.StringFormat.Alignment = StringAlignment.Near;
            lblNeeds.SetAsSlaveControl(separator1, 0, true);

            listNeeds = new List<VCCreatureNeed>();

            // Интересы
            separator2 = new VCSeparator(panelStatistics, 0, lblNeeds.NextTop() - 4);
            separator2.SetAsSlaveControl(lblNeeds, FormMain.Config.GridSizeHalf, true);

            lblInterests = new VCLabel(panelStatistics, 0, separator2.NextTop() - 8, Program.formMain.fontSmall, Color.White, 16, "Интересы:");
            lblInterests.StringFormat.Alignment = StringAlignment.Near;
            lblInterests.SetAsSlaveControl(separator2, 0, true);

            idvInterestAttack = new VCIconAndDigitValue(panelStatistics, 0, lblInterests.NextTop() - 4, 104, FormMain.GUI_16_INTEREST_ATTACK);
            idvInterestAttack.ShowHint += IdvInterestAttack_ShowHint;
            idvInterestAttack.SetAsSlaveControl(lblInterests, FormMain.Config.GridSizeHalf, true);

            idvInterestDefense = new VCIconAndDigitValue(panelStatistics, idvInterestAttack.NextLeft(), idvInterestAttack.ShiftY, 104, FormMain.GUI_16_INTEREST_DEFENSE);
            idvInterestDefense.ShowHint += IdvInterestDefense_ShowHint;
            idvInterestDefense.SetAsSlaveControl(idvInterestAttack, FormMain.Config.GridSizeHalf, false);

            idvInterestExplore = new VCIconAndDigitValue(panelStatistics, 0, idvInterestDefense.NextTop() - 4, 104, FormMain.GUI_16_INTEREST_EXPLORE);
            idvInterestExplore.ShowHint += IdvInterestExplore_ShowHint;
            idvInterestExplore.SetAsSlaveControl(idvInterestAttack, FormMain.Config.GridSizeHalf, true);

            idvInterestOther = new VCIconAndDigitValue(panelStatistics, idvInterestExplore.NextLeft(), idvInterestExplore.ShiftY, 104, FormMain.GUI_16_INTEREST_OTHER);
            idvInterestOther.ShowHint += IdvInterestOther_ShowHint;
            idvInterestOther.SetAsSlaveControl(idvInterestExplore, FormMain.Config.GridSizeHalf, false);

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

        private void IdvInterestOther_ShowHint(object sender, EventArgs e)
        {

        }

        private void IdvInterestExplore_ShowHint(object sender, EventArgs e)
        {

        }

        private void IdvInterestDefense_ShowHint(object sender, EventArgs e)
        {

        }

        private void IdvInterestAttack_ShowHint(object sender, EventArgs e)
        {

        }

        private void LvGold_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddSimpleHint(Hero.Gold > 0 ? $"Золота в кошельке: {Hero.Gold}" : "Кошелек пуст");
        }

        internal Hero Hero { get => Entity as Hero; }

        internal override void ArrangeControls()
        {
            lblCharacters.Width = panelStatistics.Width;
            lblNeeds.Width = panelStatistics.Width;
            lblInterests.Width = panelStatistics.Width;
            separator1.Width = panelStatistics.Width;
            separator2.Width = panelStatistics.Width;

            base.ArrangeControls();
        }

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

            // Свойства
            int numberProperty = 0;
            int nextLeft = 0;
            int nextTop = lblCharacters.NextTop() - 4;
            VisualControl masterControl = null;
            for (int i = 0; i < Hero.Properties.Length; i++)
                if (Hero.Properties[i] != null)
                {
                    VCCreatureProperty idv = GetVCProperty(numberProperty);
                    idv.SetProperty(Hero.Properties[i]);
                    idv.ShiftX = nextLeft;
                    idv.ShiftY = nextTop;

                    if (numberProperty % 4 == 3)
                    {
                        nextLeft = 0;
                        nextTop = idv.NextTop() - 4;
                    }
                    else
                    {
                        nextLeft = idv.NextLeft() - 4;
                    }

                    if (numberProperty % 4 == 0)
                    {
                        if (masterControl != null)
                            masterControl.SlaveControls.Remove(separator1);
                        separator1.SetAsSlaveControl(idv, FormMain.Config.GridSizeHalf, true);
                        masterControl = idv;
                    }

                    panelStatistics.ArrangeControl(idv);

                    numberProperty++;
                }

            Assert(numberProperty >= 1);

            for (; numberProperty < Hero.Properties.Length; numberProperty++)
            {
                listProperties[numberProperty].SetProperty(null);
                listProperties[numberProperty].SlaveControls.Clear();
            }

            // Потребности
            int numberNeed = 0;
            nextLeft = 0;
            nextTop = lblNeeds.NextTop() - 4;
            masterControl = null;

            for (int i = 0; i < Hero.Needs.Length; i++)
                if (Hero.Needs[i] != null)
                {
                    VCCreatureNeed idv = GetVCNeed(numberNeed);
                    idv.SetNeed(Hero.Needs[i]);
                    idv.ShiftX = nextLeft;
                    idv.ShiftY = nextTop;

                    if (numberNeed % 4 == 3)
                    {
                        nextLeft = 0;
                        nextTop = idv.NextTop() - 4;
                    }
                    else
                    {
                        nextLeft = idv.NextLeft() - 4;
                    }

                    if (numberNeed % 4 == 0)
                    {
                        if (masterControl != null)
                            masterControl.SlaveControls.Remove(separator2);
                        separator2.SetAsSlaveControl(idv, FormMain.Config.GridSizeHalf, true);
                        masterControl = idv;
                    }

                    panelStatistics.ArrangeControl(idv);

                    numberNeed++;
                }

            Assert(numberNeed >= 1);

            for (; numberNeed < Hero.Needs.Length; numberNeed++)
            {
                listNeeds[numberNeed].SetNeed(null);
                listNeeds[numberNeed].SlaveControls.Clear();
            }

            base.Draw(g);

            VCCreatureProperty GetVCProperty(int number)
            {
                if (listProperties.Count > number)
                {
                    listProperties[number].SlaveControls?.Clear();
                    return listProperties[number];
                }
                else
                {
                    VCCreatureProperty idv = new VCCreatureProperty(panelStatistics, 0, 0, 51);
                    listProperties.Add(idv);

                    return idv;
                }
            }

            VCCreatureNeed GetVCNeed(int number)
            {
                if (listNeeds.Count > number)
                {
                    listNeeds[number].SlaveControls?.Clear();
                    return listNeeds[number];
                }
                else
                {
                    VCCreatureNeed idv = new VCCreatureNeed(panelStatistics, 0, 0, 51);
                    listNeeds.Add(idv);

                    return idv;
                }
            }
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
