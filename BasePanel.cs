using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Баозвый класс панели
    internal class BasePanel : Control
    {
        private readonly Pen penBorder;
        private readonly Rectangle rectBorder;
        private Bitmap bmpBackground;

        public BasePanel(bool withBackground) : base()
        {
            penBorder = new Pen(Color.Black);
            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);

            if (withBackground)
            {
                bmpBackground = new Bitmap(2, 2);
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (bmpBackground != null)
            {
                bmpBackground.Dispose();

                if ((Width > 2) && (Height > 2))
                    bmpBackground = GuiUtils.MakeBackground(new Size(Width - 2, Height - 2));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем бордюр
            e.Graphics.DrawRectangle(penBorder, rectBorder);

            if (bmpBackground != null)
                e.Graphics.DrawImageUnscaled(bmpBackground, 1, 1);
        }
    }
}