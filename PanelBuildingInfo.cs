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
    internal sealed class PanelBuildingInfo : BasePanel
    {
        private enum Page { Products, Inhabitants };

        private PictureBox pbBuilding;
        private PlayerBuilding building;
        private Button btnInhabitants;
        private Button btnProducts;
        private Label lblPage;
        private Point pointPage;

        public PanelBuildingInfo(int width, int height) : base(true)
        {
            Width = width;
            Height = height;
            DoubleBuffered = true;

            pbBuilding = new PictureBox()
            {
                Parent = this,
                Location = new Point(Config.GRID_SIZE, Config.GRID_SIZE),
                Size = Program.formMain.ilBuildings.ImageSize,
                BackColor = Color.Transparent
            };

            btnProducts = new Button()
            {
                Parent = this,
                Left = pbBuilding.Left,
                Top = GuiUtils.NextTop(pbBuilding),
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                BackgroundImage = Program.formMain.bmpForBackground,
                ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.GUI_PRODUCTS
            };
            btnProducts.Click += BtnProducts_Click;

            btnInhabitants = new Button()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(btnProducts),
                Top = GuiUtils.NextTop(pbBuilding),
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                BackgroundImage = Program.formMain.bmpForBackground,
                ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.GUI_INHABITANTS
            };

            btnInhabitants.Click += BtnInhabitants_Click;

            lblPage = new Label()
            {
                Parent = this,
                Left = 0,
                Width = ClientSize.Width,
                Top = GuiUtils.NextTop(btnProducts),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 12),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            pointPage = new Point(Config.GRID_SIZE, GuiUtils.NextTop(lblPage));

            SetPage(Page.Products);
        }

        private void SetPage(Page page)
        {
            switch (page)
            {
                case Page.Products:
                    lblPage.Text = "Товары";
                    break;
                case Page.Inhabitants:
                    lblPage.Text = "Жители";
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

        internal void ShowData()
        {
            pbBuilding.Image = Program.formMain.ilBuildings.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0)];

            Show();    
        }
    }
}
