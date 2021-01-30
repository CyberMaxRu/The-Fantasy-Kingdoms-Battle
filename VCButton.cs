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
        public VCButton(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY, imageList, imageIndex)
        {
            ShowBorder = true;
            ShiftImage = 2;
        }

        internal override void MouseEnter()
        {
            base.MouseEnter();

            ImageState = ImageState.Over;
            Program.formMain.NeedRedrawFrame();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            ImageState = ImageState.Normal;
            Program.formMain.NeedRedrawFrame();
        }
    }
}
