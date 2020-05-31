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
        private enum Page { Products, Warehouse, Inhabitants };

        private PlayerBuilding building;
        private PictureBox pbProducts;
        private PictureBox pbWarehouse;
        private PictureBox pbInhabitants;
        private Label lblPage;
        private Point pointPage;
        private List<PanelEntity> panelProducts = new List<PanelEntity>();
//        private List<PanelEntity> panelProducts = new List<PanelEntity>();
        private Page activePage;

        public PanelBuildingInfo(int width, int height) : base(width, height)
        {
            pbProducts = new PictureBox()
            {
                Parent = this,
                Left = LeftForControls(),
                Top = TopForControls(),
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                BackgroundImage = Program.formMain.bmpForBackground,
                Image = Program.formMain.ilGui.Images[FormMain.GUI_PRODUCTS]
            };
            pbProducts.Click += BtnProducts_Click;

            pbWarehouse = new PictureBox()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(pbProducts),
                Top = pbProducts.Top,
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                BackgroundImage = Program.formMain.bmpForBackground,
                Image = Program.formMain.ilGui.Images[FormMain.GUI_INVENTORY]
            };
            pbWarehouse.Click += PbWarehouse_Click;

            pbInhabitants = new PictureBox()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(pbWarehouse),
                Top = pbWarehouse.Top,
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                BackgroundImage = Program.formMain.bmpForBackground,
                Image = Program.formMain.ilGui.Images[FormMain.GUI_INHABITANTS]
            };
            pbInhabitants.Click += BtnInhabitants_Click;

            lblPage = new Label()
            {
                Parent = this,
                Left = 0,
                Width = ClientSize.Width,
                Top = GuiUtils.NextTop(pbProducts),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 12),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            pointPage = new Point(Config.GRID_SIZE, GuiUtils.NextTop(lblPage));

            SetPage(Page.Products);
        }

        private void PbWarehouse_Click(object sender, EventArgs e)
        {
            SetPage(Page.Warehouse);
        }

        private void SetPage(Page page)
        {
            activePage = page;

            switch (page)
            {
                case Page.Products:
                    lblPage.Text = "Товары";
                    foreach (PanelEntity pe in panelProducts)
                        pe.Show();

                    break;
                case Page.Warehouse:
                    lblPage.Text = "Склад";
                    foreach (PanelEntity pe in panelProducts)
                        pe.Hide();

                    break;
                case Page.Inhabitants:
                    lblPage.Text = "Жители";

                    foreach (PanelEntity pe in panelProducts)
                        pe.Hide();

                    break;
                default:
                    throw new Exception("Неизвестная страница");
            }
        }

        private void BtnInhabitants_Click(object sender, EventArgs e)
        {
            SetPage(Page.Inhabitants);
        }

        private void BtnProducts_Click(object sender, EventArgs e)
        {
            SetPage(Page.Products);
        }

        internal PlayerBuilding Building
        {
            get
            {
                return building;
            }
            set
            {
                building = value;
                ShowData();
            }
        }

        internal override void ShowData()
        {
            base.ShowData();

            pbWarehouse.Visible = building.Building.TrainedHero != null;
            pbInhabitants.Visible = building.Building.TrainedHero != null;
            if ((activePage == Page.Warehouse) && (!pbWarehouse.Visible))
                SetPage(Page.Products);
            if ((activePage == Page.Inhabitants) && (!pbInhabitants.Visible))
                SetPage(Page.Products);

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
                pe.Location = new Point(pointPage.X + column * (pe.Width + 1), pointPage.Y + row * (pe.Height + 1));
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
    }
}
