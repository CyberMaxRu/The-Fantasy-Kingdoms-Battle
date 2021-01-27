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
    internal abstract class PanelBaseInfo : VisualControl
    {
        protected enum Page { Products, Warehouse, Inhabitants, Statistics, Inventory, Abilities, Description };

        private readonly VCLabel lblName;
        protected readonly VCImage imgIcon;
        protected PageControl pageControl;

        public PanelBaseInfo(VisualControl parent, int shiftX, int shiftY, int height) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            Height = height;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize, FormMain.Config.FontNamePage, FormMain.Config.BattlefieldPlayerName, FormMain.Config.GridSize * 3, "");
            lblName.StringFormat.LineAlignment = StringAlignment.Near;

            imgIcon = new VCImage(this, FormMain.Config.GridSize, lblName.NextTop(), GetImageList(), -1);

            pageControl = new PageControl(this, FormMain.Config.GridSize, TopForControls(), Program.formMain.ilPages)
            {
                //Parent = this,
                Width = Width - FormMain.Config.GridSize * 2,
                Height = Height - TopForControls() - FormMain.Config.GridSize
            };
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            lblName.Width = Width - FormMain.Config.GridSize * 2;
        }

        // Используемые потомками методы
        protected int TopForControls() => imgIcon.NextTop();
        protected int LeftForControls() => imgIcon.ShiftX;
        protected int TopForIcon() => imgIcon.ShiftY;
        protected int LeftAfterIcon() => imgIcon.NextLeft();

        //protected Point LeftTopPage() => pointPage;

        // Переопределяемые потомками методы
        protected abstract ImageList GetImageList();
        protected abstract int GetImageIndex();
        protected abstract string GetCaption();

        // Общие для всех панелей методы

        internal override void Draw(Graphics g)
        {
            lblName.Text = GetCaption();
            imgIcon.ImageIndex = GetImageIndex();

            base.Draw(g);
        }
    }
}
