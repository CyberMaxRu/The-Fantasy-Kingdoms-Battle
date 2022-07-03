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
        private readonly VCImage128 imgMapObject;
        private readonly VCProgressBar pbDurability;
        private readonly VCIconButton48 btnHeroes;
        private readonly VCIconButton48 btnBuildOrUpgrade;
        private readonly VCIconButton48 btnQueue;
        private readonly VCLabelValue lblIncome;
        private readonly VCIconButton48 btnQueue1;
        private readonly VCBitmap btnQueue2;
        private readonly VCBitmap btnQueue3;

        private readonly VCIconButton48 btnAction;
        private readonly VCIconButton48 btnInhabitants;
        private readonly VCIconButton48 btnAttackHeroes;
        private readonly VCLabelValue lblRewardGold;
        private readonly VCLabelValue lblRewardGreatness;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            Visible = false;

            imgMapObject = new VCImage128(this, FormMain.Config.GridSize, FormMain.Config.GridSize);
            imgMapObject.HighlightUnderMouse = true;
            imgMapObject.Click += ImgLair_Click;
            imgMapObject.BorderWithoutProgressBar = false;
            imgMapObject.ShowHint += ImgLair_ShowHint;

            pbDurability = new VCProgressBar(this, imgMapObject.ShiftX - 2, imgMapObject.NextTop() - 4);
            pbDurability.Width = imgMapObject.Width + 4;
            pbDurability.Max = 100;

            btnHeroes = new VCIconButton48(this, imgMapObject.ShiftX, imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnHeroes.Click += BtnHeroes_Click;
            btnHeroes.ShowHint += BtnHeroes_ShowHint;
            btnHeroes.Visible = false;

            btnQueue = new VCIconButton48(this, imgMapObject.NextLeft(), btnHeroes.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, -1);
            btnQueue.ShowHint += BtnQueue_ShowHint;
            btnQueue.Click += BtnQueue_Click;

            btnQueue1 = new VCIconButton48(this, imgMapObject.ShiftX, pbDurability.NextTop(), 0);
            btnQueue2 = new VCBitmap(this, btnQueue1.NextLeft() - 1, btnQueue1.ShiftY - 1, Program.formMain.bmpBackgroundEntityInQueue);
            btnQueue3 = new VCBitmap(this, btnQueue2.NextLeft() - 3, btnQueue1.ShiftY - 1, Program.formMain.bmpBackgroundEntityInQueue);

            btnBuildOrUpgrade = new VCIconButton48(this, imgMapObject.NextLeft(), pbDurability.NextTop(), FormMain.Config.Gui48_Build);
            btnBuildOrUpgrade.Click += BtnBuildOrUpgrade_Click;

            lblIncome = new VCLabelValue(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, Color.Green, true);
            lblIncome.Width = 72;
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

            Width = lblIncome.NextLeft();
            Height = btnBuildOrUpgrade.NextTop();

            btnHeroes.ShiftX = Width - btnHeroes.Width - FormMain.Config.GridSize;

            Click += ImgLair_Click;
        }

        private void BtnQueue_Click(object sender, EventArgs e)
        {
            if (Construction.QueueExecuting.Count > 0)
            {
                Construction.RemoveCellMenuFromQueue(Construction.QueueExecuting[0], true, true);
            }
        }

        private void BtnQueue_ShowHint(object sender, EventArgs e)
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
            imgMapObject.ImageIndex = Construction.GetImageIndex();
            imgMapObject.ImageIsEnabled = Construction.GetNormalImage();
            imgMapObject.Level = Construction.GetLevel();
            imgMapObject.TextCaption.Text = Construction.GetName();
            imgMapObject.TextCaption.Color = Construction.GetColorCaption();

            btnBuildOrUpgrade.MenuCell = null;
            pbDurability.Visible = false;
            btnHeroes.Visible = false;

            btnQueue1.Visible = Construction.QueueExecuting.Count >= 1;
            btnQueue1.MenuCell = btnQueue1.Visible ? Construction.QueueExecuting[0] : null;

            if (Construction.QueueExecuting.Count > 0)
            {
                CellMenuConstruction cm = Construction.QueueExecuting[0];
                btnQueue.Visible = true;
                btnQueue.ImageIndex = cm.GetImageIndex();
                btnQueue.LowText = cm.DaysLeft.ToString() + " д.";
                btnQueue.Level = Construction.QueueExecuting.Count == 1 ? "" : Construction.QueueExecuting.Count.ToString();
            }
            else
                btnQueue.Visible = false;

            if (Construction.ComponentObjectOfMap.Visible && (Construction.Descriptor.IsOurConstruction || Construction.Descriptor.Category == CategoryConstruction.External))
            {
                lblRewardGold.Visible = false;
                lblRewardGreatness.Visible = false;

                pbDurability.Visible = true;
                pbDurability.Max = Construction.MaxDurability;
                pbDurability.Position = Construction.CurrentDurability;
                switch (Construction.State)
                {
                    case StateConstruction.Work:
                        pbDurability.Text = Construction.CurrentDurability.ToString();
                        break;
                    case StateConstruction.NotBuild:
                        pbDurability.Text = Construction.Descriptor.Levels[1].Durability.ToString();
                        break;
                    case StateConstruction.PreparedBuild:
                    case StateConstruction.Build:
                    case StateConstruction.PauseBuild:
                    case StateConstruction.InQueueBuild:
                    case StateConstruction.NeedRepair:
                    case StateConstruction.Repair:
                        pbDurability.Text = $"{Construction.CurrentDurability}" +
                            $"{(Construction.AddConstructionPointByDay > 0 ? $"+{Construction.AddConstructionPointByDay}" : "")}/{Construction.MaxDurability}";
                        break;
                    default:
                        throw new Exception($"Неизвестное состояние {Construction.State}");
                }

                pbDurability.PositionPotential = 0;
                switch (Construction.State)
                {
                    case StateConstruction.Work:
                        pbDurability.Color = Color.Lime;
                        break;
                    case StateConstruction.NotBuild:
                        break;
                    case StateConstruction.PreparedBuild:
                    case StateConstruction.Build:
                    case StateConstruction.PauseBuild:
                    case StateConstruction.InQueueBuild:
                        pbDurability.Color = Color.PaleTurquoise;
                        pbDurability.PositionPotential = Construction.CurrentDurability + Construction.AddConstructionPointByDay;
                        pbDurability.PositionPotential = Construction.CurrentDurability + Construction.AddConstructionPointByDay;
                        break;
                    case StateConstruction.NeedRepair:
                    case StateConstruction.Repair:
                        int percent = Construction.CurrentDurability * 100 / Construction.MaxDurability;
                        if (percent >= 60)
                            pbDurability.Color = Color.Lime;
                        else if (percent >= 50)
                            pbDurability.Color = Color.Yellow;
                        else
                            pbDurability.Color = Color.Red;

                        pbDurability.PositionPotential = Construction.CurrentDurability + Construction.AddConstructionPointByDay;
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
                    if (Construction.QueueExecuting.Count > 0)
                    {
                        btnBuildOrUpgrade.Visible = false;
                    }
                    else if (Construction.Level > 0)
                    {
                        if (Construction.CanLevelUp())
                        {
                            Debug.Assert(Construction.MainCellMenu != null, $"У {Construction.Descriptor.ID} не найдено действие в меню для улучшения.");

                            btnBuildOrUpgrade.Visible = true;
                            btnBuildOrUpgrade.MenuCell = Construction.MainCellMenu;
                        }
                        else
                        {
                            if (Construction.Descriptor.ID == FormMain.Config.IDHolyPlace)
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
                        if (Construction.MainCellMenu != null)
                        {
                            Debug.Assert(Construction.MainCellMenu != null, $"У {Construction.Descriptor.ID} не найдено действие в меню для постройки.");

                            btnBuildOrUpgrade.Visible = true;
                            btnBuildOrUpgrade.MenuCell = Construction.MainCellMenu;
                        }
                        else
                            btnBuildOrUpgrade.Visible = false;
                    }
                }
                else
                    btnBuildOrUpgrade.Visible = false;
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

            Construction.MainCellMenu.Click();
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
                btnBuildOrUpgrade.Visible = visible;
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
            switch (Construction.TypeAction())
            {
                case TypeFlag.Scout:
                    PanelHint.AddStep2Header("Разведка");
                    PanelHint.AddStep5Description("Установить флаг разведки для отправки героев к месту");
                    break;
                case TypeFlag.Attack:
                    PanelHint.AddStep2Header("Атака");
                    PanelHint.AddStep5Description("Установить флаг атаки для отправки героев к месту");
                    break;
                case TypeFlag.Defense:
                    PanelHint.AddStep2Header("Защита");
                    PanelHint.AddStep5Description("Установить флаг защиты для отправки героев к месту");
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
                SelectThisConstruction(false);

                Construction.Player.SetTaskForHeroes();
            }
        }
    }
}