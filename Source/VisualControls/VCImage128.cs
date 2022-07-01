using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - картинка большого объекта (128 * 128)
    internal sealed class VCImage128 : VCImage
    {
        private VCLabel labelLevel;

        public VCImage128(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.imListObjects128, -1)
        {
            ShowBorder = true;

            labelLevel = new VCLabel(this, 0, FormMain.Config.GridSizeHalf, Program.formMain.fontMedCaptionC, FormMain.Config.CommonLevel, 16, ""); ;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.Width = 32;
            labelLevel.ShiftX = Width - labelLevel.Width - 6;

            TextCaption = new VCText(this, 4, 0, Program.formMain.fontSmallC, FormMain.Config.CommonCost, Width - FormMain.Config.GridSize);
            TextCaption.IsActiveControl = false;
            TextCaption.Visible = false;
        }

        internal string Level { get; set; } = "";
        internal VCText TextCaption { get; }
        internal bool BorderWithoutProgressBar { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            //labelQuantity.ShiftY -= FormMain.Config.GridSizeHalf;
        }

        internal override void PaintBorder(Graphics g)
        {
            if (BorderWithoutProgressBar)
                g.DrawImageUnscaled(Program.formMain.bmpBorderBig, Left - 2, Top - 2);
            else 
                g.DrawImageUnscaled(Program.formMain.bmpBorderBigForProgressBar, Left - 2, Top - 2);
        }

        internal override void Draw(Graphics g)
        {
            labelLevel.Visible = Level.Length > 0;
            if (labelLevel.Visible)
                labelLevel.Text = Level;

            TextCaption.Visible = TextCaption.Text.Length > 0;
            if (TextCaption.Visible)
            {
                TextCaption.Height = TextCaption.MinHeigth();
                TextCaption.ShiftY = Height - TextCaption.Height - 3;
                ArrangeControl(TextCaption);
            }

            base.Draw(g);
        }
    }
}
