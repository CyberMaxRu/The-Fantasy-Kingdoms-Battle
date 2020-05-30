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
        private PictureBox pbBuilding;
        private PlayerBuilding building;
        private Button btnInhabitants;
        private Button btnProducts;

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
            Show();
            pbBuilding.Image = Program.formMain.ilBuildings.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0)];
        }
    }
}
