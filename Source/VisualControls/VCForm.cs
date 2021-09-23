using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - окно
    internal abstract class VCForm : CustomWindow
    {
        private DispatcherFrame frame;
        protected VisualControl ClientControl;
        private Bitmap bmpBackground;
        protected readonly VCWindowCaption windowCaption;
        private DialogResult dialogResult;

        public VCForm()
        {
            ClientControl = new VisualControl(this, 14, 13 + 24);
            ClientControl.Width = 440;
            ClientControl.Height = 200;

            windowCaption = new VCWindowCaption(this, 0, 0);
            windowCaption.Width = 410;// Такая ширина заголовка в Majesty 2
        }

        internal override void AdjustSize()
        {
            if ((Width != 14 + ClientControl.Width + 14) || (Height != 13 + 24 + ClientControl.Height + 14))
            {
                Width = 14 + ClientControl.Width + 14;
                Height = 13 + 24 + ClientControl.Height + 14;

                bmpBackground?.Dispose();
                bmpBackground = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(bmpBackground);

                // Рисуем границу
                Program.formMain.bbBorderWindow.DrawBorder(g, 0, 0, Width, Height - 13);

                // Рисуем фон
                Bitmap back = GuiUtils.MakeBackground(new Size(Width - 14 - 14, Height - 13 - 14 - 14));
                g.DrawImage(back, 14, 14);
                back.Dispose();

                windowCaption.ShiftX = (Width - windowCaption.Width) / 2;

                ArrangeControls();
            }
        }

        internal override void Draw(Graphics g)
        {
            AdjustSize();
            g.DrawImage(bmpBackground, Left, Top + 13);

            base.Draw(g);
        }
    }
}
