using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Базовый класс панели информации
    internal abstract class PanelBaseInfo : BasePanel
    {
        protected enum Page { Products, Warehouse, Inhabitants, Statistics, Inventory, Abilities, Description };

        private readonly Label lblName;
        private readonly PictureBox pbIcon;
        protected PageControl pageControl;

        public PanelBaseInfo(int height) : base()
        {
            Height = height;

            lblName = new Label()
            {
                Parent = this,
                Height = Config.GRID_SIZE * 3,
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

            pageControl = new PageControl(Program.formMain.ilPages)
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Width = ClientSize.Width - Config.GRID_SIZE * 2,
                Top = TopForControls(),
                Height = ClientSize.Height - TopForControls() - Config.GRID_SIZE
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (lblName != null)
            {
                lblName.MaximumSize = new Size(Width - Config.GRID_SIZE * 2, Config.GRID_SIZE * 3);
                lblName.Width = Width - Config.GRID_SIZE * 2;
            }
        }

        // Используемые потомками методы
        protected int TopForControls() => GuiUtils.NextTop(pbIcon);
        protected int LeftForControls() => pbIcon.Left;
        protected int TopForIcon() => pbIcon.Top;
        protected int LeftAfterIcon() => GuiUtils.NextLeft(pbIcon);

        //protected Point LeftTopPage() => pointPage;

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
