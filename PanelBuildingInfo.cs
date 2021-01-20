using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс подробной информации о строении
    internal sealed class PanelBuildingInfo : PanelBaseInfo
    {
        private PlayerBuilding building;
        private Label lblGold;
        private readonly PanelWithPanelEntity panelProducts;
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelWarehouse;

        public PanelBuildingInfo(VisualControl parent, Point shift, int height) : base(parent, shift, height)
        {
            panelProducts = new PanelWithPanelEntity(this, new Point(0, 0), 4);
            panelInhabitants = new PanelWithPanelEntity(this, new Point(0, 0), 4);
            panelWarehouse = new PanelWithPanelEntity(this, new Point(0, 0), 4);

            lblGold = new Label()
            {
                //Parent = this,
                Top = pageControl.Top,
                Left = pageControl.Left,
                Width = 80,
                Height = 16,
                Font = FormMain.Config.FontCost,
                ImageList = Program.formMain.ilGui16,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            pageControl.Top = GuiUtils.NextTop(lblGold);
            pageControl.AddPage("Товары", (int)IconPages.Products, panelProducts);
            pageControl.AddPage("Склад", (int)IconPages.Inventory, panelWarehouse);
            pageControl.AddPage("Жители", (int)IconPages.Inhabitants, panelInhabitants);
            pageControl.AddPage("История", (int)IconPages.History, null);

            pageControl.ApplyMinWidth();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal PlayerBuilding Building
        {
            get { return building; }
            set
            {
                building = value;
                ShowData();
            }
        }

        internal override void ShowData()
        {
            base.ShowData();

            lblIcon.Text = (building.Building.MaxLevel > 1) && (building.Level > 0) ? building.Level.ToString() : "";

            if (building.Building.HasTreasury)
            {
                lblGold.Show();
                lblGold.Text = building.Gold.ToString();
            }
            else
            {
                lblGold.Hide();
            }

            //pageControl.SetPageVisible(1, building.Building.TrainedHero != null);
            //pageControl.SetPageVisible(2, building.Building.TrainedHero != null);

            panelProducts.ApplyList(building.Items);
            panelWarehouse.ApplyList(building.Warehouse);
            panelInhabitants.ApplyList(building.Heroes);

            Visible = true;
        }

        protected override ImageList GetImageList() => Program.formMain.ilBuildings;
        protected override int GetImageIndex() => GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0);
        protected override string GetCaption() => building.Building.Name;
    }
}
