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
        private readonly VCIconButton48 btnQueue;
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
            lblNameMapObject.IsActiveControl = false;

            imgMapObject = new VCImage128(this, FormMain.Config.GridSize, lblNameMapObject.NextTop());
            imgMapObject.HighlightUnderMouse = true;
            imgMapObject.Click += ImgLair_Click;
            imgMapObject.ShowHint += ImgLair_ShowHint;

            btnHeroes = new VCIconButton48(this, imgMapObject.ShiftX, imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnHeroes.Click += BtnHeroes_Click;
            btnHeroes.ShowHint += BtnHeroes_ShowHint;
            btnHeroes.Visible = false;

            btnQueue = new VCIconButton48(this, imgMapObject.NextLeft(), btnHeroes.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, -1);
            btnQueue.ShowHint += BtnQueue_ShowHint;
            btnQueue.Click += BtnQueue_Click;

            btnBuildOrUpgrade = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), FormMain.Config.Gui48_Build);
            btnBuildOrUpgrade.Click += BtnBuildOrUpgrade_Click;
            btnBuildOrUpgrade.ShowHint += BtnBuildOrUpgrade_ShowHint;

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), Color.Green, true);
            lblIncome.Width = imgMapObject.Width;
            lblIncome.Image.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.StringFormat.Alignment = StringAlignment.Near;
            lblIncome.Hint = "Доход в день";

            lblGreatness = new VCLabelValue(this, lblIncome.ShiftX, lblIncome.NextTop() - FormMain.Config.GridSizeHalf, Color.Green, true);
            lblGreatness.Width = lblIncome.Width;
            lblGreatness.Image.ImageIndex = FormMain.GUI_16_GREATNESS;
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
            lblRewardGold.Image.ImageIndex = FormMain.GUI_16_GOLD;
            lblRewardGold.StringFormat.Alignment = StringAlignment.Near;
            lblRewardGold.Hint = "Награда золотом за уничтожение";

            lblRewardGreatness = new VCLabelValue(this, lblRewardGold.ShiftX, lblRewardGold.NextTop() - FormMain.Config.GridSizeHalf, FormMain.Config.HintIncome, true);
            lblRewardGreatness.Width = lblRewardGold.Width;
            lblRewardGreatness.Image.ImageIndex = FormMain.GUI_16_GREATNESS;
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

        private void BtnQueue_Click(object sender, EventArgs e)
        {
            if (Construction.ListQueueProcessing.Count > 0)
            {
                Construction.RemoveEntityFromQueueProcessing(Construction.ListQueueProcessing[0]);
            }
        }

        private void BtnQueue_ShowHint(object sender, EventArgs e)
        {
            Construction.ListQueueProcessing[0].PrepareHint(PanelHint);
        }

        private void BtnHeroes_Click(object sender, EventArgs e)
        {
            SelectThisConstruction();
            Construction.Lobby.Layer.panelConstructionInfo.SelectPageInhabitant();
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

            //g.DrawImageUnscaled(bmpBackground, Left, Top);
        }

        internal override void Draw(Graphics g)
        {
            imgMapObject.ImageIndex = Construction.ImageIndexLair();
            imgMapObject.ImageIsEnabled = Construction.ImageEnabled();
            imgMapObject.Level = Construction.GetLevel();

            lblNameMapObject.Text = Construction.NameLair();
            lblNameMapObject.Color = Construction.GetColorCaption();

            btnHeroes.Visible = false;

            if (Construction.ListQueueProcessing.Count > 0)
            {
                CellMenuConstruction cm = Construction.ListQueueProcessing[0];
                btnQueue.Visible = true;
                btnQueue.ImageIndex = cm.GetImageIndex();
                btnQueue.LowText = cm.DaysLeft.ToString() + " д.";
                btnQueue.Level = Construction.ListQueueProcessing.Count == 1 ? "" : Construction.ListQueueProcessing.Count.ToString();
            }
            else
                btnQueue.Visible = false;

            if (Construction.Visible && (Construction.TypeConstruction.IsOurConstruction || Construction.TypeConstruction.Category == CategoryConstruction.External))
            {
                lblRewardGold.Visible = false;
                lblRewardGreatness.Visible = false;

                int income = Construction.Level > 0 ? Construction.Income() : Construction.IncomeNextLevel();
                if (income > 0)
                {
                    lblIncome.Text = $"+{income}";
                    lblIncome.Color = FormMain.Config.ColorIncome(Construction.Level > 0);
                    lblIncome.Image.ImageIsEnabled = Construction.Level > 0;
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
                    lblGreatness.Image.ImageIsEnabled = Construction.Level > 0;
                }

                if (Construction.TypeConstruction.PlayerCanBuild)
                {
                    if (Construction.ListQueueProcessing.Count > 0)
                    {
                        btnBuildOrUpgrade.Visible = false;
                    }
                    else if (Construction.Level > 0)
                    {
                        if (Construction.CanLevelUp())
                        {
                            Debug.Assert(Construction.CellMenuBuildOrLevelUp != null, $"У {Construction.TypeConstruction.ID} не найдено действие в меню для улучшения.");

                            btnBuildOrUpgrade.Visible = true;
                            btnBuildOrUpgrade.LowText = Construction.CellMenuBuildOrLevelUp.GetCost().ValueGold().ToString();
                            btnBuildOrUpgrade.Level = Construction.CellMenuBuildOrLevelUp.GetLevel().ToString();
                            btnBuildOrUpgrade.ImageIndex = Construction.CellMenuBuildOrLevelUp.GetImageIndex();
                            btnBuildOrUpgrade.ImageIsEnabled = Construction.CellMenuBuildOrLevelUp.GetImageIsEnabled();
                            btnBuildOrUpgrade.Color = Construction.CellMenuBuildOrLevelUp.GetColorText();
                        }
                        else
                        {
                            if (Construction.TypeConstruction.ID == FormMain.Config.IDHolyPlace)
                            {
                                btnBuildOrUpgrade.Visible = true;
                                btnBuildOrUpgrade.LowText = "";
                                btnBuildOrUpgrade.Level = "";
                                btnBuildOrUpgrade.ImageIndex = FormMain.Config.Gui48_Temple;
                                btnBuildOrUpgrade.ImageIsEnabled = true;
                            }
                            else
                                btnBuildOrUpgrade.Visible = false;
                        }
                    }
                    else
                    {
                        if (Construction.CellMenuBuildOrLevelUp != null)
                        {
                            Debug.Assert(Construction.CellMenuBuildOrLevelUp != null, $"У {Construction.TypeConstruction.ID} не найдено действие в меню для постройки.");

                            btnBuildOrUpgrade.Visible = true;
                            btnBuildOrUpgrade.LowText = Construction.CellMenuBuildOrLevelUp.GetCost().ValueGold().ToString();
                            btnBuildOrUpgrade.Level = Construction.CellMenuBuildOrLevelUp.GetLevel().ToString();
                            btnBuildOrUpgrade.ImageIndex = Construction.CellMenuBuildOrLevelUp.GetImageIndex();
                            btnBuildOrUpgrade.ImageIsEnabled = Construction.CellMenuBuildOrLevelUp.GetImageIsEnabled();
                            btnBuildOrUpgrade.Color = Construction.CellMenuBuildOrLevelUp.GetColorText();
                        }
                    }
                }
                else
                    btnBuildOrUpgrade.Visible = false;
            }
            else
            {
                lblIncome.Visible = false;
                lblGreatness.Visible = false;
                btnHeroes.Visible = false;

                btnAction.Visible = !Construction.Visible || (Construction.TypeConstruction.Category == CategoryConstruction.Lair);
                if (btnAction.Visible)
                {
                    btnAction.ImageIsEnabled = Construction.Player.ExistsFreeFlag();
                    int level = (int)(Construction.PriorityFlag + 1);
                    btnAction.Level = level == 0 ? "" : level.ToString();
                    btnAction.LowText = Construction.CheckFlagRequirements() ? Construction.RequiredGold().ValueGold().ToString() : "";
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
                                btnInhabitants.LowText = Construction.Monsters.Count.ToString();
                            break;
                        case TypeFlag.Defense:
                            btnAction.ImageIndex = FormMain.Config.Gui48_FlagDefense;
                            btnInhabitants.Visible = Construction.Monsters.Count > 0;
                            if (btnInhabitants.Visible)
                                btnInhabitants.LowText = Construction.Monsters.Count.ToString();
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
                    btnAttackHeroes.LowText = $"{Construction.listAttackedHero.Count}/{Construction.MaxHeroesForFlag()}";

                lblRewardGold.Visible = Construction.Visible && (Construction.TypeConstruction.Reward != null) && (Construction.TypeConstruction.Reward.Cost.ValueGold() > 0);
                if (lblRewardGold.Visible)
                {
                    lblRewardGold.Text = Construction.TypeConstruction.Reward.Cost.ValueGold().ToString();
                }

                lblRewardGreatness.Visible = Construction.Visible && (Construction.TypeConstruction.Reward != null) && (Construction.TypeConstruction.Reward.Greatness > 0);
                if (lblRewardGreatness.Visible)
                {
                    lblRewardGreatness.Text = Construction.TypeConstruction.Reward.Greatness.ToString();
                }
            }

            base.Draw(g);
        }

        private void ImgLair_ShowHint(object sender, EventArgs e)
        {
            Entity.PrepareHint(PanelHint);
        }

        private void ImgLair_Click(object sender, EventArgs e)
        {
            if (!Selected())
                Construction.PlaySoundSelect();
            SelectThisConstruction();
        }

        private void SelectThisConstruction()
        {
            Debug.Assert(Entity != null);
            Construction.Lobby.Layer.SelectPlayerObject(Entity as BigEntity);
        }

        protected override bool Selected()
        {
            return (Entity != null) && Construction.Lobby.Layer.PlayerObjectIsSelected(Entity);
        }
        private void BtnHeroes_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForInhabitantCreatures(PanelHint);
        }

        private void BtnBuildOrUpgrade_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForBuildOrUpgrade(PanelHint, Construction.Level + 1);
        }

        private void BtnBuildOrUpgrade_Click(object sender, EventArgs e)
        {
            SelectThisConstruction();

            if (Construction.TypeConstruction.ID == FormMain.Config.IDHolyPlace)
                return;

            Construction.CellMenuBuildOrLevelUp.Click();
        }

        protected override void SetEntity(Entity po)
        {
            base.SetEntity(po);

            Construction = po as Construction;
            SwitchStyle();
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
            PanelHint.AddStep2Header("Существа");
            PanelHint.AddStep5Description(Construction.ListMonstersForHint());
        }

        private void BtnAttackHeroes_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Герои, выполняющие флаг");
            PanelHint.AddStep5Description(Construction.ListHeroesForHint());
        }

        private void BtnAttackHeroes_Click(object sender, EventArgs e)
        {
            Construction.Lobby.Layer.SelectPlayerObject(Entity as BigEntity);
            Construction.Lobby.Layer.panelLairInfo.SelectPageHeroes();
        }

        private void BtnCancel_ShowHint(object sender, EventArgs e)
        {
            if (!Construction.Visible)
            {
                if (!Construction.Cashback().ExistsResources())
                {
                    PanelHint.AddSimpleHint("Отмена флага разведки");
                }
                else
                {
                    PanelHint.AddStep2Header("Отмена флага разведки");
                    PanelHint.AddStep5Description("Возврат денег");
                    PanelHint.AddStep6Income(Construction.Cashback().ValueGold());
                }
            }
            else
            {
                if (!Construction.Cashback().ExistsResources())
                {
                    PanelHint.AddSimpleHint("Отмена флага атаки");
                }
                else
                {
                    PanelHint.AddStep2Header("Отмена флага атаки");
                    PanelHint.AddStep5Description("Возврат денег");
                    PanelHint.AddStep6Income(Construction.Cashback().ValueGold());
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
                    {
                        PanelHint.AddStep2Header("Разведка");
                        PanelHint.AddStep5Description("Установить флаг разведки для отправки героев к месту");
                    }
                    else if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                    {
                        PanelHint.AddStep2Header("Разведка");
                        PanelHint.AddStep4Level(Construction.PriorityFlatToText() + " приоритет");
                        PanelHint.AddStep5Description("Повысить приоритет разведки места");
                    }
                    else
                    {
                        PanelHint.AddStep2Header("Разведка");
                        PanelHint.AddStep4Level(Construction.PriorityFlatToText() + " приоритет");
                        PanelHint.AddStep5Description("Установлен максимальный приоритет флага");
                    }
                    break;
                case TypeFlag.Attack:
                    if (Construction.PriorityFlag == PriorityExecution.None)
                    {
                        PanelHint.AddStep2Header("Атака");
                        PanelHint.AddStep5Description("Установить флаг атаки для отправки героев к месту");
                    }
                    else if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                    {
                        PanelHint.AddStep2Header("Атака");
                        PanelHint.AddStep4Level(Construction.PriorityFlatToText() + " приоритет");
                        PanelHint.AddStep5Description("Повысить приоритет атаки логова");
                    }
                    else
                    {
                        PanelHint.AddStep2Header("Атака");
                        PanelHint.AddStep4Level(Construction.PriorityFlatToText() + " приоритет");
                        PanelHint.AddStep5Description("Установлен максимальный приоритет флага");
                    }
                    break;
                case TypeFlag.Defense:
                    if (Construction.PriorityFlag == PriorityExecution.None)
                    {
                        PanelHint.AddStep2Header("Защита");
                        PanelHint.AddStep5Description("Установить флаг защиты для отправки героев к месту");
                    }
                    else if (Construction.PriorityFlag < PriorityExecution.Exclusive)
                    {
                        PanelHint.AddStep2Header("Защита");
                        PanelHint.AddStep4Level(Construction.PriorityFlatToText() + " приоритет");
                        PanelHint.AddStep5Description("Повысить приоритет защиты места");
                    }
                    else
                    {
                        PanelHint.AddStep2Header("Защита");
                        PanelHint.AddStep4Level(Construction.PriorityFlatToText() + " приоритет");
                        PanelHint.AddStep5Description("Установлен максимальный приоритет флага");
                    }
                    break;
                default:
                    throw new Exception($"Неизвестный тип действия: {Construction.TypeAction()}");
            }

            PanelHint.AddStep11Requirement(Construction.GetRequirements());
            PanelHint.AddStep12Gold(Construction.Player.BaseResources, Construction.RequiredGold());
        }

        private void BtnInhabitants_Click(object sender, EventArgs e)
        {
            Construction.Lobby.Layer.SelectPlayerObject(Entity as BigEntity);
            Construction.Lobby.Layer.panelLairInfo.SelectPageInhabitants();
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
    }
}