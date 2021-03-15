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
        private readonly VCWindowCaption windowCaption;
        private VisualLayer layer;

        public VCForm()
        {
            ClientControl = new VisualControl(this, 14, 13 + 24);
            ClientControl.Width = 440;
            ClientControl.Height = 200;

            windowCaption = new VCWindowCaption(this, 0, 0);

            layer = Program.formMain.AddLayer(this);
        }

        internal void AdjustSize()
        {
            if ((Width != 14 + ClientControl.Width + 14) || (Height != 13 + 24 + ClientControl.Height + 14))
            {
                Width = 14 + ClientControl.Width + 14;
                Height = 13 + 24 + ClientControl.Height + 14;

                bmpBackground?.Dispose();
                bmpBackground = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(bmpBackground);

                // Рисуем границу
                Bitmap border = Program.formMain.bbBorderWindow.DrawBorder(Width, Height - 13);
                g.DrawImage(border, 0, 0);
                border.Dispose();

                // Рисуем фон
                Bitmap back = GuiUtils.MakeBackground(new Size(Width - 14 - 14, Height - 13 - 14 - 14));
                g.DrawImage(back, 14, 14);
                back.Dispose();

                windowCaption.Width = 400;
                windowCaption.ShiftX = (Width - windowCaption.Width) / 2;
                ClientControl.Visible = false;

                ArrangeControls();
            }
        }

        internal override void Draw(Graphics g)
        {
            AdjustSize();
            g.DrawImage(bmpBackground, Left, Top + 13);

            base.Draw(g);
        }

        internal void CloseForm()
        {
            Program.formMain.RemoveLayer(layer);

        }
        internal void ToCentre()
        {
            SetPos((Program.formMain.sizeGamespace.Width - Width) / 2, (Program.formMain.sizeGamespace.Height - Height) / 2);
            ArrangeControls();
        }
    }
}
