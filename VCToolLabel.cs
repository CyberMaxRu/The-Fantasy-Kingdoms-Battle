using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - иконка 16 * 16 с текстом для тулбара игры
    internal sealed class VCToolLabel : VCLabel
    {
        public VCToolLabel(VisualControl parent, int shiftX, int shiftY, string text, int imageIndex)
            : base(parent, shiftX, shiftY, FormMain.Config.FontToolbar, Color.White, 20, text)
        {
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Near;

            ImageIndex = imageIndex;
            Width = 80;
            LeftMargin = 20;
            TopMargin = 0;
        }

        internal int ImageIndex { get; }

        internal override void Draw(Graphics g)
        {
            g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(Program.formMain.ilGui16, ImageIndex, true), Left, Top);

            base.Draw(g);
        }
    }
}
