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
        private int imageIndex;

        public VCButton(ImageList imageList, int imageIndex)
        {
            ImageList = imageList;
            ImageIndex = imageIndex;

            PrepareImage();
        }

        internal ImageList ImageList { get; }
        internal int ImageIndex { get => imageIndex; set { imageIndex = value; PrepareImage(); } }

        internal void PrepareImage()
        {
            Width = ImageList.ImageSize.Width + 4;
            Height = ImageList.ImageSize.Height + 4;

            if ((picture == null) || (picture.Width != Width) || (picture.Height != Height))
            {                
                picture?.Dispose();
                picture = new Bitmap(Width, Height);
            }

            Graphics g = Graphics.FromImage(picture);
            if (ImageIndex == -1)
                g.Clear(Color.Transparent);
            g.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
            if (ImageIndex >= 0)
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), 2, 2);

            g.Dispose();
        }

        internal override void Draw(Graphics g)
        {
            g.DrawImageUnscaled(picture, Left, Top);
        }
    }
}
