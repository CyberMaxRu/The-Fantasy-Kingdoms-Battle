using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal sealed class GuiUtils
    {

        internal static Label CreateLabel(Control parent, int left, int top)
        {
            Label l = new Label()
            {
                Parent = parent,
                Left = left,
                Top = top
            };

            return l;
        }

        internal static Label CreateLabelParameter(Control parent, int left, int top, int imIndex)
        {
            Label l = new Label()
            {
                Parent = parent,
                Left = left,
                Top = top,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight,
                ImageList = Program.formMain.ilParameters,
                ImageIndex = imIndex,
                ImageAlign = ContentAlignment.MiddleLeft
            };

            return l;
        }

        internal static Size SizeButtonWithImage(ImageList il)
        {
            return new Size(il.ImageSize.Width + 8, il.ImageSize.Height + 8);
        }

        internal static int NextLeft(Control c)
        {
            return c.Left + c.Width + Config.GRID_SIZE;
        }

        internal static int NextTop(Control c)
        {
            return c.Top + c.Height + Config.GRID_SIZE;
        }

        internal static Image GetImageFromImageList(ImageList imageList, int imageIndex, bool gray)
        {
            return imageList.Images[imageIndex + (gray == false ? 0 : imageList.Images.Count / 2)];
        }

        internal static int GetImageIndexWithGray(ImageList imageList, int imageIndex, bool gray)
        {
            return imageIndex + (gray == false ? 0 : imageList.Images.Count / 2);
        }
    }
}
