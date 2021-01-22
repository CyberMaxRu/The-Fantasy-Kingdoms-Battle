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
    internal sealed class VCCell : VCCustomImage
    {
        public VCCell(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY, imageList, imageIndex)
        {
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            // Если картинки нет, 
//            if (ImageIndex == -1)
//                g.Clear(Color.Transparent);
            //else                    
                //g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), Left + 2, Top + 2);

            //g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(1, 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));
        }
    }
}
