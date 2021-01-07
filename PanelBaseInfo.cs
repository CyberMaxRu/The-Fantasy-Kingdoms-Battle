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
        protected readonly Label lblIcon;
        protected PageControl pageControl;

        public PanelBaseInfo(int height) : base()
        {
            Height = height;

            lblName = new Label()
            {
                Parent = this,
                AutoSize = false,
                Height = FormMain.Config.GridSize * 3,
                Location = new Point(FormMain.Config.GridSize, FormMain.Config.GridSize),
                Font = FormMain.Config.FontNamePage,
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent
            };

            lblIcon = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = GuiUtils.NextTop(lblName),
                Size = GetImageList().ImageSize,
                TextAlign = ContentAlignment.BottomRight,
                Padding = new Padding(0, 0, 0, 3),
                Font = FormMain.Config.FontQuantity,
                ForeColor = FormMain.Config.CommonQuantity,
                BackColor = Color.Transparent
            };

            pageControl = new PageControl(Program.formMain.ilPages)
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Width = ClientSize.Width - FormMain.Config.GridSize * 2,
                Top = TopForControls(),
                Height = ClientSize.Height - TopForControls() - FormMain.Config.GridSize
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (lblName != null)
            {
                lblName.MaximumSize = new Size(Width - FormMain.Config.GridSize * 2, FormMain.Config.GridSize * 3);
                lblName.Width = Width - FormMain.Config.GridSize * 2;
            }
        }

        // Используемые потомками методы
        protected int TopForControls() => GuiUtils.NextTop(lblIcon);
        protected int LeftForControls() => lblIcon.Left;
        protected int TopForIcon() => lblIcon.Top;
        protected int LeftAfterIcon() => GuiUtils.NextLeft(lblIcon);

        //protected Point LeftTopPage() => pointPage;

        // Переопределяемые потомками методы
        protected abstract ImageList GetImageList();
        protected abstract int GetImageIndex();
        protected abstract string GetCaption();

        // Общие для всех панелей методы
        internal virtual void ShowData()
        {
            lblName.Text = GetCaption();
            lblIcon.Image = GetImageList().Images[GetImageIndex()];
        }
    }
}
