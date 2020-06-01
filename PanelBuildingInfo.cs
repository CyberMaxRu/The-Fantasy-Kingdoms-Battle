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
        private readonly PanelWithPanelEntity panelProducts = new PanelWithPanelEntity(3);

        public PanelBuildingInfo(int width, int height) : base(width, height)
        {
            AddPage(Page.Products);
            AddPage(Page.Warehouse);
            AddPage(Page.Inhabitants);

            panelProducts.Parent = this;
            panelProducts.Left = (Width - panelProducts.Width) / 2;
            panelProducts.Top = LeftTopPage().Y;
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

        protected override void ActivatePage(Page page)
        {
            base.ActivatePage(page);

            switch (page)
            {
                case Page.Products:
                    panelProducts.Show();

                    break;
                case Page.Warehouse:
                    panelProducts.Hide();

                    break;
                case Page.Inhabitants:
                    panelProducts.Hide();

                    break;
                default:
                    throw new Exception("Неизвестная страница.");
            }
        }
        internal override void ShowData()
        {
            base.ShowData();

            SetPageVisible(Page.Warehouse, building.Building.TrainedHero != null);
            SetPageVisible(Page.Inhabitants, building.Building.TrainedHero != null);

            panelProducts.ApplyList(building.Items);

            Show();    
        }

        protected override ImageList GetImageList() => Program.formMain.ilBuildings;
        protected override int GetImageIndex() => GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0);
        protected override string GetCaption() => building.Building.Name;
    }
}
