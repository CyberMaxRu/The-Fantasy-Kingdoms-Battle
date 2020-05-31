using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Базовый класс панели информации
    internal abstract class PanelBaseInfo : BasePanel
    {
        private readonly PictureBox pbIcon;

        public PanelBaseInfo(int width, int height) : base(true)
        {
            Width = width;
            Height = height;
            DoubleBuffered = true;

            pbIcon = new PictureBox()
            {
                Parent = this,
                Location = new Point(Config.GRID_SIZE, Config.GRID_SIZE),
                Size = GetImageList().ImageSize,
                BackColor = Color.Transparent
            };
        }

        // Используемые потомками методы
        protected int TopForControls() => GuiUtils.NextTop(pbIcon);
        protected int LeftForControls() => pbIcon.Left;

        // Переопределяемые потомками методы
        protected abstract ImageList GetImageList();
        protected abstract int GetImageIndex();

        // Общие для всех панелей методы
        internal virtual void ShowData()
        {
            pbIcon.Image = GetImageList().Images[GetImageIndex()];
        }
    }
}
