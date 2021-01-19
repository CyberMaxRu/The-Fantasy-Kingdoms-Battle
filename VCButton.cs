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
    internal sealed class VCButton : VCImage
    {
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);

        public VCButton(ImageList imageList, int imageIndex) : base(imageList, imageIndex)
        {
        }

        protected override void ValidateSize()
        {
            Width = ImageList.ImageSize.Width + 4;
            Height = ImageList.ImageSize.Height + 4;
        }

        protected override void DrawImage(Graphics g, int x, int y)
        { 
            if (ImageIndex == -1)
                g.Clear(Color.Transparent);
            g.DrawRectangle(penBorder, x, y, Width - 1, Height - 1);
            if (ImageIndex >= 0)
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), x + 2, y + 2);
        }
    }
}
