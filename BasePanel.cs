using System;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Баозвый класс панели
    internal class BasePanel : Control
    {
        private readonly Pen penBorder = new Pen(Color.Black);
        private Rectangle rectBorder = new Rectangle(0, 0, 0, 0);
        private Bitmap bmpBackground;

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            rectBorder.Width = Width - 1;
            rectBorder.Height = Height - 1;

            bmpBackground?.Dispose();
            bmpBackground = null;

            if ((Width > 2) && (Height > 2))
                bmpBackground = GuiUtils.MakeBackground(new Size(Width - 2, Height - 2));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            // Рисуем бордюр
            e.Graphics.DrawRectangle(penBorder, rectBorder);

            // Рисуем подложку
            e.Graphics.DrawImageUnscaled(bmpBackground, 1, 1);
        }
    }
}