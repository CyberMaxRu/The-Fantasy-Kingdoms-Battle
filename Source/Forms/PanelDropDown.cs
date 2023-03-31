using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class PanelDropDown : CustomWindow
    {
        private Bitmap bmpBackground;

        public PanelDropDown() : base(true)
        {
            IsDropDown = true;
            ShowBorder = true;
            Width = 0;
            Height = 0;
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if ((bmpBackground is null) || (bmpBackground.Size.Width != Width) || (bmpBackground.Size.Height != Height))
                bmpBackground = GuiUtils.MakeBackground(new Size(Width, Height));

            DrawImage(g, bmpBackground, Left, Top);
        }

        internal void ShowDropDown(int left, int top)
        {
            SetPos(left, top);
            Program.formMain.AddLayer(this, true);

            PanelHint.HideHint();
        }
    }
}
