using System;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Баозвый класс панели
    internal class BasePanel : Control
    {
        private readonly Pen penBorder = new Pen(Color.Black);
        private Rectangle rectBorder;
        private Bitmap bmpBackground;
        private readonly bool withBackground;

        public BasePanel(bool withBackground) : base()
        {
            this.withBackground = withBackground;
            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);            
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            rectBorder.Width = Width - 1;
            rectBorder.Height = Height - 1;
            
            if (withBackground)
            {
                bmpBackground?.Dispose();

                if ((Width > 2) && (Height > 2))
                    bmpBackground = GuiUtils.MakeBackground(new Size(Width - 2, Height - 2));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем бордюр
            e.Graphics.DrawRectangle(penBorder, rectBorder);

            if (withBackground)
                e.Graphics.DrawImageUnscaled(bmpBackground, 1, 1);
        }
    }
}