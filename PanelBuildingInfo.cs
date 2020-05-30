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
        }

        internal PlayerBuilding Building { get; set; }

        internal void ShowData()
        {
            pbBuilding.Image = Program.formMain.ilBuildings.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilBuildings, Building.Building.ImageIndex, Building.Level > 0)];
        }
    }
}
