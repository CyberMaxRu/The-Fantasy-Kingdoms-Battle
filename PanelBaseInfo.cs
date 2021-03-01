using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс панели информации
    internal abstract class PanelBaseInfo : VisualControl
    {
        protected enum Page { Products, Warehouse, Inhabitants, Statistics, Inventory, Abilities, Description };

        private readonly VCLabel lblName;
        protected readonly VCImage imgIcon;
        protected VCTabControl pageControl;

        public PanelBaseInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Visible = false;
            ShowBorder = true;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize, FormMain.Config.FontNamePage, FormMain.Config.BattlefieldPlayerName, FormMain.Config.GridSize * 3, "");
            lblName.StringFormat.LineAlignment = StringAlignment.Near;

            imgIcon = new VCImage(this, FormMain.Config.GridSize, lblName.NextTop(), Program.formMain.imListObjectsBig, -1);
            imgIcon.ShowHint += ImgIcon_ShowHint;

            pageControl = new VCTabControl(this, FormMain.Config.GridSize, TopForControls(), Program.formMain.ilGui)
            {
                //Parent = this,
                Width = Width - FormMain.Config.GridSize * 2,
                Height = Height - TopForControls() - FormMain.Config.GridSize
            };
        }

        private void ImgIcon_ShowHint(object sender, EventArgs e)
        {
            GetPlayerObject().PrepareHint();
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
        protected abstract int GetImageIndex();
        protected abstract bool ImageIsEnabled();
        protected abstract string GetCaption();
        protected abstract PlayerObject GetPlayerObject();

        // Общие для всех панелей методы

        internal override void Draw(Graphics g)
        {
            lblName.Text = GetCaption();
            imgIcon.ImageIndex = GetImageIndex();
            imgIcon.ImageIsEnabled = ImageIsEnabled();

            base.Draw(g);

            // Рисуем бордюр
            g.DrawImageUnscaled(Program.formMain.bmpBorderBig, imgIcon.Left - 2, imgIcon.Top - 2);
        }
    }
}
