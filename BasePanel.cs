using System;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Базовый класс панели
    internal class BasePanel : Control
    {
        private readonly Pen penBorder = new Pen(Color.Black);
        private Rectangle rectBorder = new Rectangle(0, 0, 0, 0);
        private Bitmap bmpBackground;

        public BasePanel()
        {
            DoubleBuffered = true;
        }

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

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            pevent.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            pevent.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            pevent.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // Рисуем бордюр
            pevent.Graphics.DrawRectangle(penBorder, rectBorder);

            // Рисуем подложку
            pevent.Graphics.DrawImageUnscaled(bmpBackground, 1, 1);
        }
    }
}