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
    internal class VCCustomImage : VisualControl
    {
        private int shiftImage;

        public VCCustomImage(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            ImageList = imageList;
            ImageIndex = imageIndex;

            ValidateSize();
        }

        internal ImageList ImageList { get; }
        internal int ImageIndex { get; set; }
        internal bool NormalImage { get; set; } = true;
        protected int ShiftImage { get => shiftImage; set { shiftImage = value; ValidateSize(); } }

        private void ValidateSize()
        {
            Width = ImageList.ImageSize.Width + (ShiftImage * 2);
            Height = ImageList.ImageSize.Height + (ShiftImage * 2);
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if (ImageIndex != -1)
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, NormalImage), Left + ShiftImage, Top + ShiftImage);
            //else
            //    g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(Left + 1, Top + 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));
        }
    }
}
