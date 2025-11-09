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
        private readonly VCLabel lblName;
        private readonly VCImage128 imgMapObject;
        private readonly VCProgressBar pbDurability;
        private readonly VCProgressBar pbProgressAction;
        private readonly VCIconButton48 btnHeroes;
        private readonly VCIconButton48 btnMainAction;
        private readonly VCLabelValue lblIncome;

        private readonly VCIconButton48 btnInhabitants;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            Visible = true;

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

            btnMainAction = new VCIconButton48(this, imgMapObject.ShiftX, pbProgressAction.NextTop(), FormMain.Config.Gui48_Build);
            btnMainAction.Click += BtnBuildOrUpgrade_Click;

            lblIncome = new VCLabelValue(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, Color.Green, true);
            lblIncome.Width = 104;
            lblIncome.Image.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.StringFormat.Alignment = StringAlignment.Near;
            lblIncome.Hint = "Доход в день";

            btnInhabitants = new VCIconButton48(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, FormMain.Config.Gui48_Home);
            btnInhabitants.Click += BtnInhabitants_Click;
            btnInhabitants.ShowHint += BtnInhabitants_ShowHint;

            Width = Math.Max(btnMainAction.NextLeft(), lblIncome.NextLeft());
            Height = btnMainAction.NextTop();

            btnHeroes.ShiftX = Width - btnHeroes.Width - FormMain.Config.GridSize;
            lblName.Width = Width - (FormMain.Config.GridSize * 2);

            Click += ImgLair_Click;
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
            //lblName.Color = Construction.GetColorCaption();
            imgMapObject.ImageIndex = Construction.GetImageIndex();
            imgMapObject.ImageIsEnabled = Construction.GetNormalImage();
            imgMapObject.Level = Construction.GetLevel();

            btnMainAction.MenuCell = null;
            pbDurability.Visible = false;
            btnHeroes.Visible = false;

            if (Construction.Descriptor.IsOurConstruction)
            {
                pbDurability.Visible = true;

                pbProgressAction.Text = "";
                pbProgressAction.Position = 0;

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

                            btnMainAction.Visible = true;
                            btnMainAction.MenuCell = Construction.ActionMain;
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

                            btnMainAction.Visible = true;
                            btnMainAction.MenuCell = Construction.ActionMain;
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
        }

        private void BtnInhabitants_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Существа");
        }

        private void BtnInhabitants_Click(object sender, EventArgs e)
        {
            Construction.Lobby.Layer.SelectPlayerObject(Entity as BigEntity);
        }
    }
}