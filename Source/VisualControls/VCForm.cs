using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - окно
    internal class VCForm : VisualControl
    {
        protected VisualControl ClientControl;
        private Bitmap bmpBackground;
        private VisualLayer layer;

        public VCForm()
        {
            ClientControl = new VisualControl(this, 16, 32);
            ClientControl.Width = 200;
            ClientControl.Height = 200;

            layer = Program.formMain.AddLayer(this);
        }

        internal void AdjustSize()
        {
            if ((Width != 16 + ClientControl.Width + 16) || (Height != 32 + ClientControl.Height + 16))
            {
                Width = 16 + ClientControl.Width + 16;
                Height = 32 + ClientControl.Height + 16;

                bmpBackground?.Dispose();

                bmpBackground = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(bmpBackground);

                // Рисуем границу
                Bitmap border = Program.formMain.bbBorderWindow.DrawBorder(Width, Height - 16);
                g.DrawImage(border, 0, 16);
                border.Dispose();

                // Рисуем фон
                Bitmap back = GuiUtils.MakeBackground(new Size(Width - 14 - 14, Height - 14 - 14));
                g.DrawImage(back, 14, 16 + 14);
                back.Dispose();
            }
        }

        internal override void Draw(Graphics g)
        {
            AdjustSize();
            g.DrawImage(bmpBackground, Left, Top + 16);

            base.Draw(g);
        }

        internal void CloseForm()
        {
            Program.formMain.RemoveLayer(layer);

        }
    }
}
