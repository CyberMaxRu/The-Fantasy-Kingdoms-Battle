using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Панель объекта карты
    internal sealed class PanelConstruction : VisualControl
    {
        private Bitmap bmpBackground;
        private readonly VCLabel lblNameMapObject;
        private readonly VCImage128 imgMapObject;
        private readonly VCIconButton48 btnHeroes;
        private readonly VCIconButton48 btnBuildOrUpgrade;
        private readonly VCIconButton48 btnHireHero;
        private readonly VCLabelValue lblIncome;
        private readonly VCLabelValue lblGreatness;

        private readonly VCIconButton48 btnAction;
        private readonly VCIconButton48 btnCancel;
        private readonly VCIconButton48 btnInhabitants;
        private readonly VCIconButton48 btnAttackHeroes;
        private readonly VCLabelValue lblRewardGold;
        private readonly VCLabelValue lblRewardGreatness;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            Visible = false;

            lblNameMapObject = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize - 3, Program.formMain.fontMedCaptionC, Color.Transparent, 24, "");
            lblNameMapObject.StringFormat.Alignment = StringAlignment.Center;
            //lblNameMapObject.ShowBorder = true;
            lblNameMapObject.TruncLongText = true;
            lblNameMapObject.Click += ImgLair_Click;

            imgMapObject = new VCImage128(this, FormMain.Config.GridSize, lblNameMapObject.NextTop());
            imgMapObject.HighlightUnderMouse = true;
            imgMapObject.Click += ImgLair_Click;
            imgMapObject.ShowHint += ImgLair_ShowHint;

            btnHeroes = new VCIconButton48(this, imgMapObject.ShiftX, imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnHeroes.Click += BtnHeroes_Click;
            btnHeroes.ShowHint += BtnHeroes_ShowHint;

            btnHireHero = new VCIconButton48(this, imgMapObject.NextLeft(), btnHeroes.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, -1);
            btnHireHero.Click += BtnHireHero_Click;
            btnHireHero.ShowHint += BtnHireHero_ShowHint;

            btnBuildOrUpgrade = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), FormMain.Config.Gui48_Build);
            btnBuildOrUpgrade.Click += BtnBuildOrUpgrade_Click;
            btnBuildOrUpgrade.ShowHint += BtnBuildOrUpgrade_ShowHint;

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), Color.Green, true);
            lblIncome.Width = imgMapObject.Width;
            lblIncome.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.StringFormat.Alignment = StringAlignment.Near;
            lblIncome.Hint = "Доход в день";

            lblGreatness = new VCLabelValue(this, lblIncome.ShiftX, lblIncome.NextTop() - FormMain.Config.GridSizeHalf, Color.Green, true);
            lblGreatness.Width = lblIncome.Width;
            lblGreatness.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblGreatness.StringFormat.Alignment = StringAlignment.Near;
            lblGreatness.Color = FormMain.Config.HintIncome;
            lblGreatness.Hint = "Прибавление величия при строительстве и в день";

            btnAction = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), FormMain.Config.Gui48_Battle);
            btnAction.Click += BtnAction_Click;
            btnAction.ShowHint += BtnAction_ShowHint;

            btnCancel = new VCIconButton48(this, btnAction.ShiftX - btnAction.Width - FormMain.Config.GridSize, btnAction.ShiftY, FormMain.Config.Gui48_FlagCancel);
            btnCancel.Click += BtnCancel_Click;
            btnCancel.ShowHint += BtnCancel_ShowHint;

            btnInhabitants = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnInhabitants.Click += BtnInhabitants_Click;
            btnInhabitants.ShowHint += BtnInhabitants_ShowHint;

            btnAttackHeroes = new VCIconButton48(this, btnInhabitants.ShiftX, btnInhabitants.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, FormMain.Config.Gui48_Target);
            btnAttackHeroes.Click += BtnAttackHeroes_Click;
            btnAttackHeroes.ShowHint += BtnAttackHeroes_ShowHint;

            lblRewardGold = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), FormMain.Config.HintIncome, true);
            lblRewardGold.Width = btnCancel.ShiftX - FormMain.Config.GridSize - lblRewardGold.ShiftX;
            lblRewardGold.ImageIndex = FormMain.GUI_16_GOLD;
            lblRewardGold.StringFormat.Alignment = StringAlignment.Near;
            lblRewardGold.Hint = "Награда золотом за уничтожение";

            lblRewardGreatness = new VCLabelValue(this, lblRewardGold.ShiftX, lblRewardGold.NextTop() - FormMain.Config.GridSizeHalf, FormMain.Config.HintIncome, true);
            lblRewardGreatness.Width = lblRewardGold.Width;
            lblRewardGreatness.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblRewardGreatness.StringFormat.Alignment = StringAlignment.Near;
            lblRewardGreatness.Hint = "Награда величием за уничтожение";

            Height = btnAction.NextTop();
            Width = btnAction.NextLeft();

            Height = btnBuildOrUpgrade.NextTop();
            Width = btnBuildOrUpgrade.NextLeft();
            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);

            btnHeroes.ShiftX = Width - btnHeroes.Width - FormMain.Config.GridSize;

            Click += ImgLair_Click;
        }

        private void BtnHeroes_Click(object sender, EventArgs e)
        {
            SelectThisConstruction();
            Program.formMain.panelConstructionInfo.SelectPageInhabitant();
        }

        internal Construction Construction { get; private set; }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if ((bmpBackground == null) || (bmpBackground.Width != Width) || (bmpBackground.Height != Height))
            {
                bmpBackground?.Dispose();
                bmpBackground = GuiUtils.MakeBackground(new Size(Width, Height));
            }

            g.DrawImageUnscaled(bmpBackground, Left, Top);
        }

        internal override void Draw(Graphics g)
        {

            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            if (Construction.TypeConstruction.IsOurConstruction || Construction.TypeConstruction.Category == CategoryConstruction.External)
            {
                imgMapObject.ImageIndex = Construction.TypeConstruction.ImageIndex;
                imgMapObject.ImageIsEnabled = Construction.Level > 0;

                lblNameMapObject.Text = Construction.TypeConstruction.Name;
                lblNameMapObject.Color = FormMain.Config.ColorMapObjectCaption(Construction.Level > 0);

                int income = Construction.Level > 0 ? Construction.Income() : Construction.IncomeNextLevel();
                if (income > 0)
                {
                    lblIncome.Text = $"+{income}";
                    lblIncome.Color = FormMain.Config.ColorIncome(Construction.Level > 0);
                    lblIncome.ImageIsEnabled = Construction.Level > 0;
                    lblIncome.Visible = true;
                }
                else
                    lblIncome.Visible = false;

                bool needShowGreatness = Construction.Level > 0
                        ? Construction.GreatnessPerDay() > 0
                        : (Construction.GreatnessPerDayNextLevel() > 0) || (Construction.GreatnessAddNextLevel() > 0);
                lblGreatness.Visible = needShowGreatness;
                if (lblGreatness.Visible)
                {
                    if (Construction.Level == 0)
                        lblGreatness.Text = Utils.FormatGreatness(Construction.GreatnessAddNextLevel(), Construction.GreatnessPerDayNextLevel());
                    else
                        lblGreatness.Text = Utils.FormatGreatness(0, Construction.GreatnessPerDay());

                    lblGreatness.Color = FormMain.Config.ColorGreatness(Construction.Level > 0);
                    lblGreatness.ImageIsEnabled = Construction.Level > 0;
                }

                if (Construction.TypeConstruction.PlayerCanBuild)
                {
                    if (Construction.Level > 0)
                    {
                        if (Construction.CanLevelUp())
                        {
                            btnBuildOrUpgrade.Visible = true;
                            btnBuildOrUpgrade.Text = Construction.CostBuyOrUpgrade().ToString();
                            btnBuildOrUpgrade.ImageIndex = FormMain.Config.Gui48_LevelUp;
                            btnBuildOrUpgrade.ImageIsEnabled = Construction.CheckRequirements();
                            btnBuildOrUpgrade.Color = btnBuildOrUpgrade.ImageIsEnabled ? Color.LimeGreen : Color.Gray;
                        }
                        else
                        {
                            btnBuildOrUpgrade.Visible = false;
                        }
                    }
                    else
                    {
                        btnBuildOrUpgrade.Visible = true;
                        btnBuildOrUpgrade.Text = Construction.CostBuyOrUpgrade().ToString();
                        btnBuildOrUpgrade.ImageIndex = Construction.TypeConstruction.ID != FormMain.Config.IDHolyPlace ? FormMain.Config.Gui48_Build : FormMain.Config.Gui48_Temple;
                        btnBuildOrUpgrade.ImageIsEnabled = (Construction.TypeConstruction.MaxLevel > 0) ? Construction.CheckRequirements() : true;
                        btnBuildOrUpgrade.Color = btnBuildOrUpgrade.ImageIsEnabled ? Color.LimeGreen : Color.Gray;
                    }
                }
                else
                    btnBuildOrUpgrade.Visible = false;

                if ((Construction.TypeConstruction.TrainedHero != null) && (Construction.TypeConstruction.Category != CategoryConstruction.Economic))
                {
                    //btnHireHero.ImageIndex = (Construction.Level > 0) && ((Construction.Heroes.Count == Construction.MaxHeroes()) || (Construction.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, c.TrainedHero.ImageIndex, Construction.CanTrainHero());
                    if (Construction.Heroes.Count < Construction.MaxHeroes())
                    {
                        btnHireHero.Visible = true;
                        btnHireHero.ImageIndex = ((Construction.Level > 0) && (Construction.MaxHeroesAtPlayer() == true)) ? -1 : Construction.TypeConstruction.TrainedHero.ImageIndex;
                        btnHireHero.ImageIndex = Program.formMain.TreatImageIndex(Construction.TypeConstruction.TrainedHero.ImageIndex, Construction.Player);
                        btnHireHero.ImageIsEnabled = Construction.CanTrainHero();
                        btnHireHero.Text = (Construction.Level == 0) || (Construction.CanTrainHero() == true) ? Construction.TypeConstruction.TrainedHero.Cost.ToString() : null;
                    }
                    else
                        btnHireHero.Visible = false;
                }
                else
                    btnHireHero.Visible = false;

                imgMapObject.Level = Construction.Level < Construction.TypeConstruction.MaxLevel ? Construction.Level : 0;

                if ((Construction.TypeConstruction.TrainedHero != null) && !(Construction.TypeConstruction.TrainedHero is null) && (Construction.Level > 0) && (Construction.Heroes.Count > 0))
                {
                    btnHeroes.Text = Construction.Heroes.Count.ToString() + (Construction.Heroes.Count != Construction.MaxHeroes() ? "/" + Construction.MaxHeroes() : "");
                    //btnHeroes.ImageIndex = Program.formMain.TreatImageIndex(Construction.TypeConstruction.TrainedHero.ImageIndex, Construction.Player);
                    btnHeroes.Visible = true;
                }
                else
                {
                    btnHeroes.Text = null;
                    btnHeroes.Visible = false;
                }
            }
            else
            {
                btnAction.Visible = Construction.Hidden || (Construction.TypeConstruction.Category == CategoryConstruction.Lair);
                if (btnAction.Visible)
                {
                    btnAction.ImageIsEnabled = Construction.CheckFlagRequirements();
                    btnAction.Level = (int)Construction.PriorityFlag + 1;
                    btnAction.Text = Construction.PriorityFlag < PriorityExecution.High ? Construction.RequiredGold().ToString() : null;
                }

                Debug.Assert(btnAction.Visible || (!btnAction.Visible && (Construction.PriorityFlag == PriorityExecution.None)));
                btnCancel.Visible = Construction.PriorityFlag != PriorityExecution.None;

                if (btnAction.Visible)
                {
                    switch (Construction.TypeAction())
                    {
                        case TypeFlag.Scout:
                            btnAction.ImageIndex = FormMain.Config.Gui48_FlagScout;
                            btnInhabitants.Visible = false;
                            break;
                        case TypeFlag.Attack:
                            btnAction.ImageIndex = FormMain.Config.Gui48_FlagAttack;
                            btnInhabitants.Visible = Construction.Monsters.Count > 0;
                            if (btnInhabitants.Visible)
                                btnInhabitants.Text = Construction.Monsters.Count.ToString();
                            break;
                        case TypeFlag.Defense:
                            btnAction.ImageIndex = FormMain.Config.Gui48_FlagDefense;
                            btnInhabitants.Visible = Construction.Monsters.Count > 0;
                            if (btnInhabitants.Visible)
                                btnInhabitants.Text = Construction.Monsters.Count.ToString();
                            break;
                        default:
                            throw new Exception($"Неизвестный тип действия: {Construction.TypeAction()}");
                    }
                }
                else
                {
                    Debug.Assert(Construction.Monsters.Count == 0);
                    btnInhabitants.Visible = false;
                }

                btnAttackHeroes.Visible = Construction.listAttackedHero.Count > 0;
                if (btnAttackHeroes.Visible)
                    btnAttackHeroes.Text = $"{Construction.listAttackedHero.Count}/{Construction.MaxHeroesForFlag()}";

                imgMapObject.ImageIndex = Construction.ImageIndexLair();
                lblNameMapObject.Text = Construction.NameLair();
                lblNameMapObject.Color = GetColorCaption();

                lblRewardGold.Visible = !Construction.Hidden && (Construction.TypeConstruction.Reward != null) && (Construction.TypeConstruction.Reward.Gold > 0);
                if (lblRewardGold.Visible)
                {
                    lblRewardGold.Text = Construction.TypeConstruction.Reward.Gold.ToString();
                }

                lblRewardGreatness.Visible = !Construction.Hidden && (Construction.TypeConstruction.Reward != null) && (Construction.TypeConstruction.Reward.Greatness > 0);
                if (lblRewardGreatness.Visible)
                {
                    lblRewardGreatness.Text = Construction.TypeConstruction.Reward.Greatness.ToString();
                }
            }

            Debug.Assert(lblNameMapObject.Text.Length > 0);

            base.Draw(g);
        }

        private void ImgLair_ShowHint(object sender, EventArgs e)
        {
            Entity.PrepareHint();
        }

        private void ImgLair_Click(object sender, EventArgs e)
        {
            if (!Selected())
                PlaySelect();
            SelectThisConstruction();
        }

        private void SelectThisConstruction()
        {
            Debug.Assert(Entity != null);
            Program.formMain.SelectPlayerObject(Entity as BigEntity);
        }

        protected override bool Selected()
        {
            return (Entity != null) && Program.formMain.PlayerObjectIsSelected(Entity);
        }
        private void BtnHeroes_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForInhabitantCreatures();
        }

        private void BtnHireHero_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForHireHero();
        }

        private void BtnBuildOrUpgrade_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForBuildOrUpgrade();
        }

        private void BtnHireHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(Construction.Level <= Construction.TypeConstruction.MaxLevel);

            SelectThisConstruction();

            if ((Construction.Level > 0) && (Construction.CanTrainHero() == true))
            {
                Construction.HireHero();
                Program.formMain.UpdateListHeroes();
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        private void BtnBuildOrUpgrade_Click(object sender, EventArgs e)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            SelectThisConstruction();

            if (Construction.Player.Gold >= Construction.CostBuyOrUpgrade())
            {
                if (Construction.Level == 0)
                    Construction.Build();
                else
                    Construction.Upgrade();

                Program.formMain.SetNeedRedrawFrame();
                Program.formMain.PlayConstructionComplete();
            }
        }

        protected override void SetEntity(Entity po)
        {
            base.SetEntity(po);

            Construction = po as Construction;
            SwitchStyle();

            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
        }

        private void PlaySelect()
        {
            if (Construction.Hidden)
                Program.formMain.PlayPushButton();
            else
                Construction.TypeConstruction.PlaySoundSelect();
        }

        private void SwitchStyle()
        {
            Visible = Construction != null;
            if (Visible)
            {
                VisibleOur(Construction.TypeConstruction.IsOurConstruction);
                VisibleEnemy(!Construction.TypeConstruction.IsOurConstruction);
            }

            void VisibleOur(bool visible)
            {
                btnHeroes.Visible = visible;
                btnBuildOrUpgrade.Visible = visible;
                btnHireHero.Visible = visible;
                lblIncome.Visible = visible;
                lblGreatness.Visible = visible;

            }

            void VisibleEnemy(bool visible)
            {
                btnAction.Visible = visible;
                btnCancel.Visible = visible;
                btnInhabitants.Visible = visible;
                btnAttackHeroes.Visible = visible;
                lblRewardGold.Visible = visible;
                lblRewardGreatness.Visible = visible;
            }
        }

        private void BtnInhabitants_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep1Header("Существа", "", Construction.ListMonstersForHint());
        }

        private void BtnAttackHeroes_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep1Header("Герои, выполняющие флаг", "", Construction.ListHeroesForHint());
        }

        private void BtnAttackHeroes_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(Entity as BigEntity);
            Program.formMain.panelLairInfo.SelectPageHeroes();
        }

        private void BtnCancel_ShowHint(object sender, EventArgs e)
        {
            if (Construction.Hidden)
            {
                if (Construction.Cashback() == 0)
                {
                    Program.formMain.formHint.AddSimpleHint("Отмена флага разведки");
                }
                else
                {
                    Program.formMain.formHint.AddStep1Header("Отмена флага разведки", "", "Возврат денег");
                    Program.formMain.formHint.AddStep2Income(Construction.Cashback());
                }
            }
            else
            {
                if (Construction.Cashback() == 0)
                {
                    Program.formMain.formHint.AddSimpleHint("Отмена флага атаки");
                }
                else
                {
                    Program.formMain.formHint.AddStep1Header("Отмена флага атаки", "", "Возврат денег");
                    Program.formMain.formHint.AddStep2Income(Construction.Cashback());
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SelectThisConstruction();
            btnCancel.Visible = false;
            Construction.CancelFlag();
            Construction.Player.SetTaskForHeroes();
        }

        private void BtnAction_ShowHint(object sender, EventArgs e)
        {
            switch (Construction.TypeAction())
            {
                case TypeFlag.Scout:
                    if (Construction.PriorityFlag == PriorityExecution.None)
                        Program.formMain.formHint.AddStep1Header("Разведка", "", "Установить флаг разведки для отправки героев к месту");
                    else if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                        Program.formMain.formHint.AddStep1Header("Разведка", Construction.PriorityFlatToText() + " приоритет", "Повысить приоритет разведки места");
                    else
                        Program.formMain.formHint.AddStep1Header("Разведка", Construction.PriorityFlatToText() + " приоритет", "Установлен максимальный приоритет флага");
                    break;
                case TypeFlag.Attack:
                    if (Construction.PriorityFlag == PriorityExecution.None)
                        Program.formMain.formHint.AddStep1Header("Атака", "", "Установить флаг атаки для отправки героев к месту");
                    else if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                        Program.formMain.formHint.AddStep1Header("Атака", Construction.PriorityFlatToText() + " приоритет", "Повысить приоритет атаки логова");
                    else
                        Program.formMain.formHint.AddStep1Header("Атака", Construction.PriorityFlatToText() + " приоритет", "Установлен максимальный приоритет флага");
                    break;
                case TypeFlag.Defense:
                    if (Construction.PriorityFlag == PriorityExecution.None)
                        Program.formMain.formHint.AddStep1Header("Защита", "", "Установить флаг защиты для отправки героев к месту");
                    else if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                        Program.formMain.formHint.AddStep1Header("Защита", Construction.PriorityFlatToText() + " приоритет", "Повысить приоритет защиты места");
                    else
                        Program.formMain.formHint.AddStep1Header("Защита", Construction.PriorityFlatToText() + " приоритет", "Установлен максимальный приоритет флага");
                    break;
                default:
                    throw new Exception($"Неизвестный тип действия: {Construction.TypeAction()}");
            }

            Program.formMain.formHint.AddStep3Requirement(Construction.GetRequirements());
            Program.formMain.formHint.AddStep4Gold(Construction.RequiredGold(), Construction.Player.Gold >= Construction.RequiredGold());
        }

        private void BtnInhabitants_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(Entity as BigEntity);
            Program.formMain.panelLairInfo.SelectPageInhabitants();
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            if (btnAction.ImageIsEnabled)
            {
                SelectThisConstruction();

                if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                {
                    Construction.IncPriority();
                    Construction.Player.SetTaskForHeroes();
                }
            }
        }

        private Color GetColorCaption()
        {
            if (Construction.PriorityFlag == PriorityExecution.None)
                return Construction.Hidden ? FormMain.Config.ColorMapObjectCaption(false) : Color.MediumAquamarine;

            switch (Construction.TypeAction())
            {
                case TypeFlag.Scout:
                    return Color.LimeGreen;
                case TypeFlag.Attack:
                    return Color.OrangeRed;
                case TypeFlag.Defense:
                    return Color.DodgerBlue;
                default:
                    throw new Exception($"Неизвестный тип действия: {Construction.TypeAction()}");
            }
        }
    }
}