using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс подробной информации о сооружении
    internal sealed class PanelConstructionInfo : PanelBaseInfo
    {
        private VCLabel lblTypeConstruction;
        private VCIconAndDigitValue lblGold;
        private readonly VisualControl tabProducts;
        private VCLabel lblSectionVisits;
        private VCLabel lblSectionExtensions;
        private VCLabel lblSectionGoods;
        private VCLabel lblSectionAbilities;
        private readonly PanelWithPanelEntity panelVisits;
        private readonly PanelWithPanelEntity panelExtensions;
        private readonly PanelWithPanelEntity panelGoods;
        private readonly PanelWithPanelEntity panelAbilities;
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelVisitors;
        private readonly VCTabButton btnProducts;
        private readonly VCTabButton btnInhabitants;
        private readonly VCTabButton btnVisitors;
        private readonly VCIconAndDigitValue lblInterest;

        public PanelConstructionInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            tabProducts = new VisualControl();
            panelInhabitants = new PanelWithPanelEntity(4);
            panelVisitors = new PanelWithPanelEntity(4);

            lblTypeConstruction = new VCLabel(this, FormMain.Config.GridSize, TopForControls(), Program.formMain.fontParagraph, Color.White, 16, "");
            lblTypeConstruction.StringFormat.Alignment = StringAlignment.Near;
            lblTypeConstruction.Hint = "Тип сооружения";

            lblGold = new VCIconAndDigitValue(this, FormMain.Config.GridSize, lblTypeConstruction.NextTop(), imgIcon.Width, FormMain.GUI_16_COFFERS);
            lblGold.ShowHint += LblGold_ShowHint;

            lblInterest = new VCIconAndDigitValue(this, imgIcon.NextLeft(), imgIcon.ShiftY, 16, FormMain.GUI_16_INTEREST_OTHER);
            lblInterest.ShowHint += LblInterest_ShowHint;

            separator.ShiftY = lblGold.NextTop();
            pageControl.ShiftY = separator.NextTop();
            btnProducts = pageControl.AddTab("Сооружение", FormMain.Config.Gui48_Goods, tabProducts);
            btnInhabitants = pageControl.AddTab("Жители", FormMain.Config.Gui48_Home, panelInhabitants);
            btnVisitors = pageControl.AddTab("Посетители", FormMain.Config.Gui48_Exit, panelVisitors);
            pageControl.AddTab("История", FormMain.Config.Gui48_Book, null);

            lblSectionVisits = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Посещение:");
            lblSectionVisits.StringFormat.Alignment = StringAlignment.Near;
            lblSectionExtensions = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Доп. сооружения:");
            lblSectionExtensions.StringFormat.Alignment = StringAlignment.Near;
            lblSectionGoods = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Товары:");
            lblSectionGoods.StringFormat.Alignment = StringAlignment.Near;
            lblSectionAbilities = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Умения:");
            lblSectionAbilities.StringFormat.Alignment = StringAlignment.Near;

            panelVisits = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelVisits);
            panelExtensions = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelExtensions);
            panelGoods = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelGoods);
            panelAbilities = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelAbilities);

            tabProducts.ApplyMaxSize();

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
            lblTypeConstruction.Width = Width - lblTypeConstruction.ShiftX * 2;
            tabProducts.Width = pageControl.Width;
            lblSectionVisits.Width = pageControl.Width;
            lblSectionExtensions.Width = pageControl.Width;
            lblSectionGoods.Width = pageControl.Width;
            lblSectionAbilities.Width = pageControl.Width;
        }

        private void LblInterest_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep2Header("Интерес героев к сооружению");
            if (Construction.Level > 0)
                Program.formMain.formHint.AddStep5Description(Construction.HintDescriptionInterest());
        }

        private void LblGold_ShowHint(object sender, EventArgs e)
        {
            if (Construction.TypeConstruction.IsOurConstruction)
                Program.formMain.formHint.AddSimpleHint(Construction.Gold > 0 ? $"Золота в казне: {Construction.Gold}" : "Казна пуста");
            //Program.formMain.formHint.AddSimpleHint(Hero.Gold > 0 ? $"Золота в кошельке: {Hero.Gold}" : "Кошелек пуст");
        }

        internal Construction Construction { get => Entity as Construction; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
            tabProducts.Height = pageControl.Height - tabProducts.ShiftY - FormMain.Config.GridSize;

            lblInterest.Width = Width - imgIcon.NextLeft() - FormMain.Config.GridSize;
        }

        internal override void Draw(Graphics g)
        {
            lblTypeConstruction.Text = Construction.TypeConstruction.TypeConstruction.Name;
            imgIcon.Level = Construction.GetLevel();

            if (Construction.TypeConstruction.HasTreasury)
            {
                lblGold.Visible = true;
                lblGold.Text = Construction.Gold.ToString();
            }
            else
            {
                lblGold.Visible = false;
            }

            //pageControl.SetPageVisible(1, Construction.TypeConstruction.TrainedHero != null);
            //pageControl.SetPageVisible(2, Construction.TypeConstruction.TrainedHero != null);

            int nextTop = 0;
            DrawList(lblSectionVisits, panelVisits, Construction.Visits);
            DrawList(lblSectionExtensions, panelExtensions, Construction.Extensions);
            DrawList(lblSectionGoods, panelGoods, Construction.Goods);
            DrawList(lblSectionAbilities, panelAbilities, Construction.Abilities);
            tabProducts.ArrangeControls();

            panelInhabitants.ApplyList(Construction.Heroes);

            btnProducts.Quantity = Construction.AllProducts.Count;
            btnInhabitants.Quantity = Construction.Heroes.Count;

            lblInterest.ImageIsEnabled = Construction.Level > 0;
            lblInterest.Text = Construction.GetInterest() > 0 ? Utils.DecIntegerBy10(Construction.GetInterest(), false) : "";

            base.Draw(g);

            void DrawList(VCLabel label, PanelWithPanelEntity panel, List<ConstructionProduct> list)
            {
                if (list.Count > 0)
                {
                    label.Visible = true;
                    label.ShiftY = nextTop;

                    panel.Visible = true;
                    panel.ApplyList(list);
                    panel.ShiftY = label.NextTop();

                    nextTop = panel.NextTop() + FormMain.Config.GridSize;
                }
                else
                {
                    label.Visible = false;
                    panel.Visible = false;
                }

            }
        }

        protected override int GetImageIndex() => Construction.TypeConstruction.ImageIndex;
        protected override bool ImageIsEnabled() => Construction.Level > 0;
        protected override string GetCaption() => Construction.TypeConstruction.Name;

        internal void SelectPageInhabitant()
        {
            pageControl.ActivatePage(2);
        }
    }
}
