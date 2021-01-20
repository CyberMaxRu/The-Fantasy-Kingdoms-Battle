using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal sealed class VCLabel : VisualControl
    {
        private Label label;

        public VCLabel(VisualControl parent, Point shift, Font font, Color foreColor, int height, string text) : base(parent, shift)
        {
            Height = height;

            label = new Label();
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ForeColor = foreColor;
            label.BackColor = Color.Transparent;
            label.Text = text;
            label.Font = font;
            label.Height = height;
        }

        internal override void Draw(Graphics g)
        {
            //Bitmap bmp = new Bitmap(label.Width, label.Height);
            //bmp.MakeTransparent();
            //g.cle

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            g.DrawString(label.Text, label.Font, FormMain.Config.brushControl, new RectangleF(label.Left, label.Top, label.Width, label.Height), sf);
//            label.DrawToBitmap(b, new Rectangle(label.Left, label.Top, label.Width, label.Height));
        }

        internal override void ArrangeControls()
        {
            label.Left = Left;
            label.Top = Top;
            label.Width = Width;
            //label.Height = Height;
        }
    }
}
