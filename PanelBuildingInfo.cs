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
    internal sealed class PanelBuildingInfo : Control
    {
        private PictureBox pbBuilding;

        public PanelBuildingInfo(int width, int height) : base()
        {
            Width = width;
            Height = height;
            DoubleBuffered = true;
        }

        internal PlayerBuilding Building { get; set; }

        internal void ShowData()
        {

        }
    }
}
