using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Панель объекта карты
    internal abstract class PanelMapObject : VisualControl
    {
        private Bitmap bmpBackground;
        protected readonly VCLabelM2 lblNameMapObject;
        protected readonly VCImage imgMapObject;

        public PanelMapObject(VisualControl parent, int shiftX, int shiftY, TypeMapObject typeMapObject) : base(parent, shiftX, shiftY)
        {
            TypeMapObject = typeMapObject;
            ShowBorder = true;

            lblNameMapObject = new VCLabelM2(this, FormMain.Config.GridSize, FormMain.Config.GridSize - 3, Program.formMain.fontMedCaptionC, Color.Transparent, FormMain.Config.GridSize * 3, "");
            lblNameMapObject.StringFormat.Alignment = StringAlignment.Center;
            lblNameMapObject.ShowBorder = true;
            lblNameMapObject.Click += ImgLair_Click;

            imgMapObject = new VCImage(this, FormMain.Config.GridSize, lblNameMapObject.NextTop(), Program.formMain.imListObjectsBig, TypeMapObject.ImageIndex);
            imgMapObject.ShowBorder = false;
            imgMapObject.HighlightUnderMouse = true;
            imgMapObject.Click += ImgLair_Click;
            imgMapObject.ShowHint += ImgLair_ShowHint;
            imgMapObject.TypeObject = TypeMapObject;
        }

        internal TypeMapObject TypeMapObject { get; }
        internal PlayerObject PlayerObject { get; set; }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if ((bmpBackground == null) || (bmpBackground.Width != Width) || (bmpBackground.Height != Height))
            {
                bmpBackground?.Dispose();
                bmpBackground = GuiUtils.MakeBackground(new Size(Width, Height));
            }

            g.DrawImageUnscaled(bmpBackground, Left, Top);
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            // Рисуем бордюр вокруг иконки
            g.DrawImageUnscaled(Program.formMain.bmpBorderBig, imgMapObject.Left - 2, imgMapObject.Top - 2);
        }

        private void ImgLair_ShowHint(object sender, EventArgs e)
        {
            PlayerObject.PrepareHint();
        }

        private void ImgLair_Click(object sender, EventArgs e)
        {
            SelectThisBuilding();
        }

        protected void SelectThisBuilding()
        {
            if (this is PanelLair pl)
                Program.formMain.SelectLair(pl);
            if (this is PanelConstruction pc)
                Program.formMain.SelectBuilding(pc);
        }
    }
}
