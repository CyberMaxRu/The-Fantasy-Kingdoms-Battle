using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCIconButton24 : VCImage24
    {
        public VCIconButton24(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, imageIndex)
        {
            HighlightUnderMouse = true;
            PlaySoundOnClick = true;
        }

        internal static VCIconButton24 CreateButton(VisualControl parent, int shiftX, int shiftY, int imageIndex, EventHandler onClick)
        {
            VCIconButton24 button = new VCIconButton24(parent, shiftX, shiftY, imageIndex);
            button.Click += onClick;

            return button;
        }

        internal override void MouseDown()
        {
            base.MouseDown();

            //if (ImageIsEnabled)
            //    Program.formMain.PlayPushButton();
        }
    }
}
