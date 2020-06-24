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
        private readonly PanelWithPanelEntity panelProducts = new PanelWithPanelEntity(4);
        private readonly PanelWithPanelEntity panelInhabitants = new PanelWithPanelEntity(4);
        private readonly PanelWithPanelEntity panelWarehouse = new PanelWithPanelEntity(4);

        public PanelBuildingInfo(int height) : base(height)
        {
            pageControl.AddPage("Товары", (int)IconPages.Products, panelProducts);
            pageControl.AddPage("Склад", (int)IconPages.Inventory, panelWarehouse);
            pageControl.AddPage("Жители", (int)IconPages.Inhabitants, panelInhabitants);
            pageControl.AddPage("История", (int)IconPages.History, null);

            pageControl.ApplyMinWidth();
            Width = pageControl.Width + Config.GRID_SIZE * 2;
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

            pageControl.SetPageVisible(1, building.Building.TrainedHero != null);
            pageControl.SetPageVisible(2, building.Building.TrainedHero != null);

            panelProducts.ApplyList(building.Items);
            panelWarehouse.ApplyList(building.Warehouse);
            panelInhabitants.ApplyList(building.Heroes);

            Show();    
        }

        protected override ImageList GetImageList() => Program.formMain.ilBuildings;
        protected override int GetImageIndex() => GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0);
        protected override string GetCaption() => building.Building.Name;
    }
}
