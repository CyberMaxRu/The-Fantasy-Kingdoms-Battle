﻿using System;
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
        private readonly VCLabel lblName;
        private readonly VCImage128 imgMapObject;
        private readonly VCProgressBar pbDurability;
        private readonly VCProgressBar pbProgressAction;
        private readonly VCIconButton48 btnHeroes;
        private readonly VCIconButton48 btnMainAction;
        private readonly VCLabelValue lblIncome;
        private readonly VCIconButton48 btnQueue1;
        private readonly VCEntityInQueue bmpQueue2;
        private readonly VCEntityInQueue bmpQueue3;

        private readonly VCIconButton48 btnAction;
        private readonly VCIconButton48 btnInhabitants;
        private readonly VCIconButton48 btnAttackHeroes;
        private readonly VCLabelValue lblRewardGold;
        private readonly VCLabelValue lblRewardGreatness;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            Visible = false;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize - 2, Program.formMain.FontSmallC, FormMain.Config.CommonCost, 16, "");

            imgMapObject = new VCImage128(this, FormMain.Config.GridSize, lblName.NextTop());
            imgMapObject.BitmapList = Program.formMain.BmpListObjects96;
            imgMapObject.HighlightUnderMouse = true;
            imgMapObject.Click += ImgLair_Click;
            imgMapObject.BorderWithoutProgressBar = false;
            imgMapObject.ShowHint += ImgLair_ShowHint;

            pbDurability = new VCProgressBar(this, imgMapObject.ShiftX - 2, imgMapObject.NextTop() - 4);
            pbDurability.Width = imgMapObject.Width + 4;
            pbDurability.Max = 100;

            pbProgressAction = new VCProgressBar(this, pbDurability.ShiftX, pbDurability.ShiftY + pbDurability.Height - 1);
            pbProgressAction.Width = pbDurability.Width;
            pbProgressAction.ColorProgress = Color.Fuchsia;
            pbProgressAction.Max = 100;

            btnHeroes = new VCIconButton48(this, imgMapObject.ShiftX, imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnHeroes.Click += BtnHeroes_Click;
            btnHeroes.ShowHint += BtnHeroes_ShowHint;
            btnHeroes.Visible = false;

            btnQueue1 = new VCIconButton48(this, imgMapObject.ShiftX, pbProgressAction.NextTop(), 0);
            btnQueue1.ShowHint += BtnQueue1_ShowHint;
            btnQueue1.ShowBorder = false;
            btnQueue1.Click += BtnQueue1_Click;
            bmpQueue2 = new VCEntityInQueue(this, btnQueue1.NextLeft(), btnQueue1.ShiftY);
            bmpQueue2.StateRestTime = StateRestTime.Pause;
            bmpQueue2.Click += BmpQueue2_Click;
            bmpQueue3 = new VCEntityInQueue(this, bmpQueue2.NextLeft(), btnQueue1.ShiftY);
            bmpQueue3.StateRestTime = StateRestTime.Pause;
            bmpQueue3.Click += BmpQueue3_Click;

            btnMainAction = new VCIconButton48(this, bmpQueue3.NextLeft(), pbProgressAction.NextTop(), FormMain.Config.Gui48_Build);
            btnMainAction.Click += BtnBuildOrUpgrade_Click;

            lblIncome = new VCLabelValue(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, Color.Green, true);
            lblIncome.Width = 104;
            lblIncome.Image.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.StringFormat.Alignment = StringAlignment.Near;
            lblIncome.Hint = "Доход в день";

            btnAction = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), FormMain.Config.Gui48_Battle);
            btnAction.Click += BtnAction_Click;
            btnAction.ShowHint += BtnAction_ShowHint;

            btnInhabitants = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnInhabitants.Click += BtnInhabitants_Click;
            btnInhabitants.ShowHint += BtnInhabitants_ShowHint;

            btnAttackHeroes = new VCIconButton48(this, btnInhabitants.ShiftX, btnInhabitants.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, FormMain.Config.Gui48_Target);
            btnAttackHeroes.Click += BtnAttackHeroes_Click;
            btnAttackHeroes.ShowHint += BtnAttackHeroes_ShowHint;

            lblRewardGold = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), FormMain.Config.HintIncome, true);
            lblRewardGold.Width = btnAction.ShiftX - FormMain.Config.GridSize - lblRewardGold.ShiftX;
            lblRewardGold.Image.ImageIndex = FormMain.GUI_16_GOLD;
            lblRewardGold.StringFormat.Alignment = StringAlignment.Near;
            lblRewardGold.Hint = "Награда золотом за уничтожение";

            lblRewardGreatness = new VCLabelValue(this, lblRewardGold.ShiftX, lblRewardGold.NextTop() - FormMain.Config.GridSizeHalf, FormMain.Config.HintIncome, true);
            lblRewardGreatness.Width = lblRewardGold.Width;
            lblRewardGreatness.Image.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblRewardGreatness.StringFormat.Alignment = StringAlignment.Near;
            lblRewardGreatness.Hint = "Награда величием за уничтожение";

            Width = Math.Max(btnMainAction.NextLeft(), lblIncome.NextLeft());
            Height = btnMainAction.NextTop();

            btnHeroes.ShiftX = Width - btnHeroes.Width - FormMain.Config.GridSize;
            lblName.Width = Width - (FormMain.Config.GridSize * 2);

            Click += ImgLair_Click;
        }

        private void BmpQueue3_Click(object sender, EventArgs e)
        {
            RemoveFromQueue(2);
        }

        private void BmpQueue2_Click(object sender, EventArgs e)
        {
            RemoveFromQueue(1);
        }

        private void BtnQueue1_Click(object sender, EventArgs e)
        {
            RemoveFromQueue(0);
        }

        private void RemoveFromQueue(int index)
        {
            if (Construction.QueueExecuting[index] != null)
                if (Construction.QueueExecuting[index].ProgressExecuting.PassedMilliTicks == 0)
                    Construction.Player.RemoveFromQueueExecuting(Construction.QueueExecuting[index], false);
        }

        private void BtnQueue1_ShowHint(object sender, EventArgs e)
        {
            Construction.QueueExecuting[0].PrepareHint(PanelHint);
        }

        private void BtnHeroes_Click(object sender, EventArgs e)
        {
            SelectThisConstruction(false);
            Construction.Lobby.Layer.panelConstructionInfo.SelectPageInhabitant();
        }

        internal Construction Construction { get; private set; }

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
            lblName.Text = Program.formMain.Settings.ShowNameConstruction ? Construction.GetName() : "";
            lblName.Color = Construction.GetColorCaption();
            imgMapObject.ImageIndex = Construction.GetImageIndex();
            imgMapObject.ImageIsEnabled = Construction.GetNormalImage();
            imgMapObject.Level = Construction.GetLevel();

            btnMainAction.MenuCell = null;
            pbDurability.Visible = false;
            btnHeroes.Visible = false;

            btnQueue1.Visible = Construction.QueueExecuting.Count >= 1;
            if (btnQueue1.Visible)
            {
                btnQueue1.MenuCell = btnQueue1.Visible ? Construction.QueueExecuting[0] : null;
                btnQueue1.HighlightUnderMouse = Construction.QueueExecuting[0].ProgressExecuting.PassedMilliTicks == 0;
                btnQueue1.PlaySoundOnClick = Construction.QueueExecuting[0].ProgressExecuting.State != StateProgress.Active;
                bmpQueue2.Visible = true;
                bmpQueue2.Action = Construction.QueueExecuting.Count >= 2 ? Construction.QueueExecuting[1] : null;
                bmpQueue3.Visible = true;
                bmpQueue3.Action = Construction.QueueExecuting.Count >= 3 ? Construction.QueueExecuting[2] : null;
            }
            else
            {
                btnQueue1.MenuCell = null;
                bmpQueue2.Visible = false;
                bmpQueue2.Action = null;
                bmpQueue3.Visible = false;
                bmpQueue3.Action = null;
            }

            if (Construction.ComponentObjectOfMap.Visible && (Construction.Descriptor.IsOurConstruction || Construction.Descriptor.Category == CategoryConstruction.External))
            {
                lblRewardGold.Visible = false;
                lblRewardGreatness.Visible = false;

                pbDurability.Visible = true;

                if ((Construction.FirstActionInQueue != null) && !(Construction.FirstActionInQueue is CellMenuConstructionLevelUp) && !(Construction.FirstActionInQueue is CellMenuConstructionRepair) && (Construction.FirstActionInQueue.ProgressExecuting.State == StateProgress.Active))
                {
                    int percent = Program.formMain.CalcPercentExecuting(Construction.FirstActionInQueue.ProgressExecuting.PassedMilliTicks, Construction.FirstActionInQueue.ProgressExecuting.TotalMilliTicks);

                    pbProgressAction.Text = percent.ToString() + "%";
                    pbProgressAction.Position = percent;
                }
                else
                {
                    pbProgressAction.Text = "";
                    pbProgressAction.Position = 0;
                }

                switch (Construction.State)
                {
                    case StateConstruction.Work:
                        pbDurability.Text = Construction.CurrentDurability.ToString();
                        pbDurability.Max = Construction.MaxDurability;
                        pbDurability.Position = Construction.CurrentDurability;
                        break;
                    case StateConstruction.NotBuild:
                    case StateConstruction.InQueueBuild:
                        pbDurability.Text = Construction.Descriptor.Levels[1].Durability.ToString();
                        pbDurability.Max = Construction.MaxDurability;
                        pbDurability.Position = 0;
                        break;
                    case StateConstruction.Build:
                    case StateConstruction.NeedRepair:
                    case StateConstruction.Repair:
                        pbDurability.Text = $"{Construction.CurrentDurability}/{Construction.MaxDurability}";
                        pbDurability.Max = Construction.MaxDurability;
                        pbDurability.Position = Construction.CurrentDurability;
                        break;
                    default:
                        throw new Exception($"Неизвестное состояние {Construction.State}");
                }

                switch (Construction.State)
                {
                    case StateConstruction.Work:
                        pbDurability.ColorProgress = Color.Lime;
                        break;
                    case StateConstruction.NotBuild:
                    case StateConstruction.InQueueBuild:
                        break;
                    case StateConstruction.Build:
                        pbDurability.ColorProgress = Color.PaleTurquoise;
                        break;
                    case StateConstruction.NeedRepair:
                    case StateConstruction.Repair:
                        int percent = Construction.CurrentDurability * 100 / Construction.MaxDurability;
                        if (percent >= 60)
                            pbDurability.ColorProgress = Color.Lime;
                        else if (percent >= 50)
                            pbDurability.ColorProgress = Color.Yellow;
                        else
                            pbDurability.ColorProgress = Color.Red;
                        break;
                    default:
                        throw new Exception($"Неизвестное состояние {Construction.State}");
                }
            

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

                if (Construction.Descriptor.PlayerCanBuild)
                {
                    if (Construction.Level > 0)
                    {
                        if (Construction.Level < Construction.Descriptor.MaxLevel)
                        {
                            Debug.Assert(Construction.ActionMain != null, $"У {Construction.Descriptor.ID} не найдено действие в меню для улучшения.");

                            if (!Construction.ActionMain.ProgressExecuting.InQueue)
                            {
                                btnMainAction.Visible = true;
                                btnMainAction.MenuCell = Construction.ActionMain;
                            }
                            else
                                btnMainAction.Visible = false;
                        }
                        else
                        {
                            if (Construction.Descriptor.ID == FormMain.Config.IDHolyPlace)
                            {
                                btnMainAction.Visible = true;
                                btnMainAction.LowText = "";
                                btnMainAction.Level = "";
                                btnMainAction.ImageIndex = FormMain.Config.Gui48_Temple;
                                btnMainAction.ImageIsEnabled = true;
                            }
                            else
                                btnMainAction.Visible = false;
                        }
                    }
                    else
                    {
                        if (Construction.ActionMain != null)
                        {
                            Debug.Assert(Construction.ActionMain != null, $"У {Construction.Descriptor.ID} не найдено действие в меню для постройки.");

                            if (!Construction.ActionMain.ProgressExecuting.InQueue)
                            {
                                btnMainAction.Visible = true;
                                btnMainAction.MenuCell = Construction.ActionMain;
                            }
                            else
                                btnMainAction.Visible = false;
                        }
                        else
                            btnMainAction.Visible = false;
                    }
                }
                else
                    btnMainAction.Visible = false;
            }
            else
            {
                lblIncome.Visible = false;
                btnHeroes.Visible = false;

                btnAction.Visible = !Construction.ComponentObjectOfMap.Visible || (Construction.Descriptor.Category == CategoryConstruction.Lair);
                if (btnAction.Visible)
                {
                    btnAction.ImageIsEnabled = true;// Construction.Player.ExistsFreeFlag();
                    int level = 1;
                    btnAction.Level = level == 0 ? "" : level.ToString();
                    btnAction.LowText = Construction.RequiredGold().Gold.ToString();
                }

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

                btnAttackHeroes.Visible = Construction.ComponentObjectOfMap.ListHeroesForFlag.Count > 0;
                if (btnAttackHeroes.Visible)
                    btnAttackHeroes.LowText = $"{Construction.ComponentObjectOfMap.ListHeroesForFlag.Count}/{Construction.ComponentObjectOfMap.MaxHeroesForFlag()}";

                lblRewardGold.Visible = Construction.ComponentObjectOfMap.Visible && (Construction.Descriptor.Reward != null) && (Construction.Descriptor.Reward.Cost.Gold > 0);
                if (lblRewardGold.Visible)
                {
                    lblRewardGold.Text = Construction.Descriptor.Reward.Cost.Gold.ToString();
                }

                lblRewardGreatness.Visible = Construction.ComponentObjectOfMap.Visible && (Construction.Descriptor.Reward != null) && (Construction.Descriptor.Reward.Greatness > 0);
                if (lblRewardGreatness.Visible)
                {
                    lblRewardGreatness.Text = Construction.Descriptor.Reward.Greatness.ToString();
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
            SelectThisConstruction(true);
        }

        private void SelectThisConstruction(bool playSoundSelect)
        {
            Debug.Assert(Entity != null);
            Construction.Lobby.Layer.SelectPlayerObject(Entity as BigEntity, -1, playSoundSelect);
        }

        protected override bool Selected()
        {
            return (Entity != null) && Construction.Lobby.Layer.PlayerObjectIsSelected(Entity);
        }
        private void BtnHeroes_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForInhabitantCreatures(PanelHint);
        }

        private void BtnBuildOrUpgrade_Click(object sender, EventArgs e)
        {
            SelectThisConstruction(false);

            if (Construction.Descriptor.ID == FormMain.Config.IDHolyPlace)
                return;

            Construction.ActionMain.Click();
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
                VisibleOur(Construction.Descriptor.IsOurConstruction);
                VisibleEnemy(!Construction.Descriptor.IsOurConstruction);
            }

            void VisibleOur(bool visible)
            {
                btnHeroes.Visible = visible;
                btnMainAction.Visible = visible;
                lblIncome.Visible = visible;

            }

            void VisibleEnemy(bool visible)
            {
                btnAction.Visible = visible;
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
            PanelHint.AddStep5Description(Construction.ComponentObjectOfMap.ListHeroesForHint());
        }

        private void BtnAttackHeroes_Click(object sender, EventArgs e)
        {
            Construction.Lobby.Layer.SelectPlayerObject(Entity as BigEntity);
            Construction.Lobby.Layer.panelLairInfo.SelectPageHeroes();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SelectThisConstruction(false);
            Construction.CancelFlag();
            Construction.Player.SetTaskForHeroes();
        }

        private void BtnAction_ShowHint(object sender, EventArgs e)
        {
            string nameAction;

            switch (Construction.TypeAction())
            {
                case TypeFlag.Scout:
                    PanelHint.AddStep2Header("Разведка");
                    PanelHint.AddStep5Description("Установить флаг разведки для отправки героев к месту");
                    nameAction = "Установить флаг разведки";
                    break;
                case TypeFlag.Attack:
                    PanelHint.AddStep2Header("Атака");
                    PanelHint.AddStep5Description("Установить флаг атаки для отправки героев к месту");
                    nameAction = "Установить флаг атаки";
                    break;
                case TypeFlag.Defense:
                    PanelHint.AddStep2Header("Защита");
                    PanelHint.AddStep5Description("Установить флаг защиты для отправки героев к месту");
                    nameAction = "Установить флаг защиты";
                    break;
                default:
                    throw new Exception($"Неизвестный тип действия: {Construction.TypeAction()}");
            }

            PanelHint.AddStep12CostExecuting(nameAction, Construction.RequiredGold(), 0, 0, Construction.GetRequirements());
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
                SelectThisConstruction(false);

                Construction.Player.SetTaskForHeroes();
            }
        }
    }
}