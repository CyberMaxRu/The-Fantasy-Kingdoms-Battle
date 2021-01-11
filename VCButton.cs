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
    internal sealed class VCButton : VisualControl
    {
        private Bitmap picture;
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);

        public VCButton(ImageList imageList, int imageIndex)
        {
            ImageList = imageList;
            ImageIndex = imageIndex;

            Width = ImageList.ImageSize.Width + 4;
            Height = ImageList.ImageSize.Height + 4;

            PrepareImage();
        }

        internal ImageList ImageList { get; }
        internal int ImageIndex { get; }

        internal void PrepareImage()
        {
            picture?.Dispose();

            picture = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(picture);
            g.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
            g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), 2, 2);
            g.Dispose();
        }

        internal override void Draw(Graphics g)
        {
            g.DrawImageUnscaled(picture, Left, Top);
        }
    }
}
