using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - иконка
    internal abstract class VCImage : VisualControl
    {
        private Bitmap picture;
        private int imageIndex;

        public VCImage(ImageList imageList, int imageIndex)
        {
            ImageList = imageList;
            ValidateSize();

            ImageIndex = imageIndex;
            PrepareImage();
        }

        internal ImageList ImageList { get; }
        internal int ImageIndex { get => imageIndex; set { imageIndex = value; PrepareImage(); } }

        private void PrepareImage()
        {
            if ((picture == null) || (picture.Width != Width) || (picture.Height != Height))
            {
                picture?.Dispose();
                picture = new Bitmap(Width, Height);
            }

            Graphics g = Graphics.FromImage(picture);
            DrawImage(g, 0, 0);
            g.Dispose();
        }

        protected abstract void ValidateSize();
        protected abstract void DrawImage(Graphics g, int x, int y);

        internal override void Draw(Graphics g, int x, int y)
        {
            // x == Left, y == Top !
            if (imageIndex != -1)
                g.DrawImageUnscaled(picture, x, y);
            else
                g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(x + 1, y + 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));
        }
    }
}
