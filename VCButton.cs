using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCButton : VCImage
    {
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);

        public VCButton(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY, imageList, imageIndex)
        {
        }

        protected override void ValidateSize()
        {
            Width = ImageList.ImageSize.Width + 4;
            Height = ImageList.ImageSize.Height + 4;
        }

        protected override void DrawImage(Graphics g)
        { 
            if (ImageIndex == -1)
                g.Clear(Color.Transparent);
            g.DrawRectangle(penBorder, Left, Top, Width - 1, Height - 1);
            if (ImageIndex >= 0)
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), Left + 2, Top + 2);
        }
    }
}
