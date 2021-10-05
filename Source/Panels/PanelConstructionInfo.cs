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
        private readonly PanelWithPanelEntity panelProducts;
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelVisitors;
        private readonly VCTabButton btnProducts;
        private readonly VCTabButton btnInhabitants;
        private readonly VCTabButton btnVisitors;

        public PanelConstructionInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelProducts = new PanelWithPanelEntity(4);
            panelInhabitants = new PanelWithPanelEntity(4);
            panelVisitors = new PanelWithPanelEntity(4);

            lblTypeConstruction = new VCLabel(this, FormMain.Config.GridSize, TopForControls(), Program.formMain.fontParagraph, Color.White, 16, "");
            lblTypeConstruction.StringFormat.Alignment = StringAlignment.Near;
            lblTypeConstruction.Hint = "Тип сооружения";

            lblGold = new VCIconAndDigitValue(this, FormMain.Config.GridSize, lblTypeConstruction.NextTop(), imgIcon.Width, FormMain.GUI_16_COFFERS);
            lblGold.ShowHint += LblGold_ShowHint;

            separator.ShiftY = lblGold.NextTop();
            pageControl.ShiftY = separator.NextTop();
            btnProducts = pageControl.AddTab("Товары", FormMain.Config.Gui48_Goods, panelProducts);
            btnInhabitants = pageControl.AddTab("Жители", FormMain.Config.Gui48_Home, panelInhabitants);
            btnVisitors = pageControl.AddTab("Посетители", FormMain.Config.Gui48_Exit, panelVisitors);
            pageControl.AddTab("История", FormMain.Config.Gui48_Book, null);

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
            lblTypeConstruction.Width = Width - lblTypeConstruction.ShiftX * 2;
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

            panelProducts.ApplyList(Construction.Items);
            panelInhabitants.ApplyList(Construction.Heroes);

            btnProducts.Quantity = Construction.Items.Count;
            btnInhabitants.Quantity = Construction.Heroes.Count;

            base.Draw(g); 
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
