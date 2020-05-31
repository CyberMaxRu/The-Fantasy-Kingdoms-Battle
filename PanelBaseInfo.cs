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
        private readonly Label lblName;
        private readonly PictureBox pbIcon;

        public PanelBaseInfo(int width, int height) : base(true)
        {
            Width = width;
            Height = height;
            DoubleBuffered = true;

            lblName = new Label()
            {
                Parent = this,
                Width = Width - Config.GRID_SIZE * 2,
                Height = Config.GRID_SIZE * 3,
                MaximumSize = new Size(Width - Config.GRID_SIZE * 2, Config.GRID_SIZE * 3),
                Location = new Point(Config.GRID_SIZE, Config.GRID_SIZE),
                Font = new Font("Microsof Sans Serif", 13),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            pbIcon = new PictureBox()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = GuiUtils.NextTop(lblName),
                Size = GetImageList().ImageSize,
                BackColor = Color.Transparent
            };
        }

        // Используемые потомками методы
        protected int TopForControls() => GuiUtils.NextTop(pbIcon);
        protected int LeftForControls() => pbIcon.Left;
        protected int TopForIcon() => pbIcon.Top;
        protected int LeftAfterIcon() => GuiUtils.NextLeft(pbIcon);

        // Переопределяемые потомками методы
        protected abstract ImageList GetImageList();
        protected abstract int GetImageIndex();
        protected abstract string GetCaption();

        // Общие для всех панелей методы
        internal virtual void ShowData()
        {
            lblName.Text = GetCaption();
            pbIcon.Image = GetImageList().Images[GetImageIndex()];
        }
    }
}
