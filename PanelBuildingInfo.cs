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
        private List<PanelEntity> panelProducts = new List<PanelEntity>();
//        private List<PanelEntity> panelProducts = new List<PanelEntity>();

        public PanelBuildingInfo(int width, int height) : base(width, height)
        {
            AddPage(Page.Products);
            AddPage(Page.Warehouse);
            AddPage(Page.Inhabitants);
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
                    foreach (PanelEntity pe in panelProducts)
                        pe.Show();

                    break;
                case Page.Warehouse:
                    foreach (PanelEntity pe in panelProducts)
                        pe.Hide();

                    break;
                case Page.Inhabitants:

                    foreach (PanelEntity pe in panelProducts)
                        pe.Hide();

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

            // Перестраиваем список товаров
            foreach (PanelEntity p in panelProducts)
                p.Dispose();

            panelProducts.Clear();

            PanelEntity pe;
            int column = 0;
            int row = 0;
            foreach (Item i in building.Items)
            {
                pe = new PanelEntity(this, Program.formMain.ilItems, 0);
                pe.Location = new Point(LeftTopPage().X + column * (pe.Width + 1), LeftTopPage().Y + row * (pe.Height + 1));
                pe.ShowItem(i);

                column++;
                if (column == 3)
                {
                    column = 0;
                    row++;
                }

                panelProducts.Add(pe);
            }

            Show();    
        }

        protected override ImageList GetImageList() => Program.formMain.ilBuildings;
        protected override int GetImageIndex() => GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0);
        protected override string GetCaption() => building.Building.Name;
    }
}
