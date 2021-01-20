using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - ячейка
    internal sealed class VCCell : VCImage
    {
        public VCCell(VisualControl parent, Point shift, ImageList imageList, int imageIndex) : base(parent, shift, imageList, imageIndex)
        {
        }

        protected override void ValidateSize()
        {
            Width = ImageList.ImageSize.Width + 4;
            Height = ImageList.ImageSize.Height + 4;
        }

        protected override void DrawImage(Graphics g, int x, int y)
        {
            // Если картинки нет, 
            if (ImageIndex == -1)
                g.Clear(Color.Transparent);
            else                    
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), 2, 2);

            //g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));
        }
    }
}
