using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private int shiftImage;
        private VCLabel labelLevel;

        public VCImage(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            ImageList = imageList;
            ImageIndex = imageIndex;

            ValidateSize();

            labelLevel = new VCLabel(this, 0, FormMain.Config.GridSize, FormMain.Config.FontBuildingLevel, FormMain.Config.CommonLevel, 16, "");
            labelLevel.Width = Width - FormMain.Config.GridSizeHalf;
            labelLevel.Visible = false;
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
        }

        internal ImageList ImageList { get; }
        internal int ImageIndex { get; set; }
        internal bool NormalImage { get; set; } = true;
        internal int Level { get; set; }
        protected int ShiftImage { get => shiftImage; set { shiftImage = value; ValidateSize(); } }

        private void ValidateSize()
        {
            Width = ImageList.ImageSize.Width + (ShiftImage * 2);
            Height = ImageList.ImageSize.Height + (ShiftImage * 2);
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Level >= 0);

            base.Draw(g);

            if (ImageIndex != -1)
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, NormalImage), Left + ShiftImage, Top + ShiftImage);            
            //else
            //    g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(Left + 1, Top + 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

            if (Level > 0)
            {
                labelLevel.Text = Level.ToString();
                labelLevel.Draw(g);
            }
        }
    }
}
