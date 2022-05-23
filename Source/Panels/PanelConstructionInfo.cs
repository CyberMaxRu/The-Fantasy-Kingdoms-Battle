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
        private VCLabel lblSectionProperty;
        private readonly List<VCCreatureProperty> listProperties;
        private VCLabel lblSectionVisits;
        private VCLabel lblSectionSpells;
        private VCLabel lblSectionExtensions;
        private VCLabel lblSectionImprovements;
        private VCLabel lblSectionBaseResources;
        private VCLabel lblSectionResources;
        private VCLabel lblSectionServices;
        private VCLabel lblSectionGoods;
        private VCLabel lblSectionAbilities;
        private readonly PanelWithPanelEntity panelVisits;
        private readonly PanelWithPanelEntity panelSpells;
        private readonly PanelWithPanelEntity panelExtensions;
        private readonly PanelWithPanelEntity panelImprovements;
        private readonly PanelWithPanelEntity panelBaseResources;
        private readonly PanelWithPanelEntity panelResources;
        private readonly PanelWithPanelEntity panelServices;
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
            btnProducts = pageControl.AddTab("Информация", FormMain.Config.Gui48_Goods, tabProducts);
            btnInhabitants = pageControl.AddTab("Жители", FormMain.Config.Gui48_Home, panelInhabitants);
            btnVisitors = pageControl.AddTab("Посетители", FormMain.Config.Gui48_Exit, panelVisitors);
            pageControl.AddTab("История", FormMain.Config.Gui48_Book, null);

            lblSectionProperty = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Основные характеристики:");
            lblSectionProperty.StringFormat.Alignment = StringAlignment.Near;
            listProperties = new List<VCCreatureProperty>();
            for (int i = 0; i < FormMain.Descriptors.PropertiesCreature.Count; i++)
                listProperties.Add(new VCCreatureProperty(tabProducts, 0, 0, 51));

            lblSectionVisits = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Посещение:");
            lblSectionVisits.StringFormat.Alignment = StringAlignment.Near;
            lblSectionSpells = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Заклинания:");
            lblSectionSpells.StringFormat.Alignment = StringAlignment.Near;
            lblSectionExtensions = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Доп. сооружения:");
            lblSectionExtensions.StringFormat.Alignment = StringAlignment.Near;
            lblSectionImprovements = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Улучшения:");
            lblSectionImprovements.StringFormat.Alignment = StringAlignment.Near;
            lblSectionBaseResources = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Базовые ресурсы:");
            lblSectionBaseResources.StringFormat.Alignment = StringAlignment.Near;
            lblSectionResources = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Ресурсы:");
            lblSectionResources.StringFormat.Alignment = StringAlignment.Near;
            lblSectionServices = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Услуги:");
            lblSectionServices.StringFormat.Alignment = StringAlignment.Near;
            lblSectionGoods = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Товары:");
            lblSectionGoods.StringFormat.Alignment = StringAlignment.Near;
            lblSectionAbilities = new VCLabel(tabProducts, 0, 0, Program.formMain.fontSmallC, Color.White, 16, "Умения:");
            lblSectionAbilities.StringFormat.Alignment = StringAlignment.Near;

            panelVisits = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelVisits);
            panelSpells = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelSpells);
            panelExtensions = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelExtensions);
            panelImprovements = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelImprovements);
            panelBaseResources = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelBaseResources);
            panelResources = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelResources);
            panelServices = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelServices);
            panelGoods = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelGoods);
            panelAbilities = new PanelWithPanelEntity(4, false);
            tabProducts.AddControl(panelAbilities);

            tabProducts.ApplyMaxSize();

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
            lblTypeConstruction.Width = Width - lblTypeConstruction.ShiftX * 2;
            tabProducts.Width = pageControl.Width;
            lblSectionProperty.Width = pageControl.Width;
            lblSectionVisits.Width = pageControl.Width;
            lblSectionSpells.Width = pageControl.Width;
            lblSectionExtensions.Width = pageControl.Width;
            lblSectionImprovements.Width = pageControl.Width;
            lblSectionBaseResources.Width = pageControl.Width;
            lblSectionResources.Width = pageControl.Width;
            lblSectionServices.Width = pageControl.Width;
            lblSectionGoods.Width = pageControl.Width;
            lblSectionAbilities.Width = pageControl.Width;
        }

        private void LblInterest_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Интерес героев к сооружению");
            if (Construction.Level > 0)
                PanelHint.AddStep5Description(Construction.HintDescriptionInterest());
            PanelHint.AddStep21Tooltip("Чем больше интерес у сооружения, тем чаще герои будут посещать его");
        }

        private void LblGold_ShowHint(object sender, EventArgs e)
        {
            if (Construction.TypeConstruction.IsOurConstruction)
                PanelHint.AddSimpleHint(Construction.Gold > 0 ? $"Золота в казне: {Construction.Gold}" : "Казна пуста");
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
            imgIcon.ImageIsEnabled = Construction.ImageEnabled();

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

            ShowChapter(lblSectionProperty, lblSectionVisits, Construction.Properties?.ToList(), listProperties);
            int nextTop = lblSectionVisits.ShiftY;

            DrawList(lblSectionVisits, panelVisits, Construction.Visits);
            DrawList(lblSectionSpells, panelSpells, Construction.Spells);
            DrawList(lblSectionExtensions, panelExtensions, Construction.Extensions);
            DrawList(lblSectionImprovements, panelImprovements, Construction.Improvements);
            DrawList(lblSectionBaseResources, panelBaseResources, Construction.BaseMiningResources);
            DrawList(lblSectionResources, panelResources, Construction.Resources);
            DrawList(lblSectionServices, panelServices, Construction.Services);
            DrawList(lblSectionGoods, panelGoods, Construction.Goods);
            DrawList(lblSectionAbilities, panelAbilities, Construction.Abilities);
            tabProducts.ArrangeControls();

            if (Construction.TypeConstruction.Category != CategoryConstruction.Lair)
            {
                panelInhabitants.ApplyList(Construction.Heroes);

                btnProducts.Quantity = Construction.ListEntities.Count;
                btnInhabitants.Quantity = Construction.Heroes.Count;
            }
            else
            {
                if (Construction.Visible)
                {
                    panelInhabitants.ApplyList(Construction.Monsters);
                    btnInhabitants.Quantity = Construction.Monsters.Count;
                }
                else
                {
                    panelInhabitants.SetUnknownList();
                    btnInhabitants.Quantity = 0;
                }

                panelVisits.ApplyList(Construction.listAttackedHero);
                btnVisitors.Quantity = Construction.listAttackedHero.Count;
            }

            lblInterest.Image.ImageIsEnabled = Construction.Level > 0;
            lblInterest.Text = Construction.GetInterest() > 0 ? Utils.DecIntegerBy10(Construction.GetInterest(), false) : "";

            base.Draw(g);

            void DrawList<T>(VCLabel label, PanelWithPanelEntity panel, List<T> list) where T : EntityForConstruction
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

        protected override int GetImageIndex() => Construction.GetImageIndex();
        protected override bool ImageIsEnabled() => Construction.Level > 0;
        protected override string GetCaption() => Construction.GetName();

        internal void SelectPageInhabitant()
        {
            pageControl.ActivatePage(2);
        }
        internal void SelectPage(int number)
        {
            pageControl.ActivatePage(number);
        }
    }
}
