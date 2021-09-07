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
        private VCLabel labelCost;

        public VCImage128(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.imListObjects128, -1)
        {
            ShowBorder = true;

            Width = BitmapList.Size;
            Height = BitmapList.Size;

            labelCost = new VCLabel(this, 0, Height - 12, Program.formMain.fontSmallC, FormMain.Config.CommonCost, 16, "");
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Width = Width;
            //labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
            //labelCost.ManualDraw = true;

            labelLevel = new VCLabel(this, 0, FormMain.Config.GridSizeHalf, Program.formMain.fontMedCaptionC, FormMain.Config.CommonLevel, 16, ""); ;
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            //labelLevel.Visible = false;
            //labelLevel.ManualDraw = true;
            labelLevel.Width = Width - FormMain.Config.GridSizeHalf;
        }

        internal int Level { get; set; }
        internal string Text { get; set; } = "";

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            //labelQuantity.ShiftY -= FormMain.Config.GridSizeHalf;
        }

        internal override void PaintBorder(Graphics g)
        {
            g.DrawImageUnscaled(Program.formMain.bmpBorderBig, Left - 2, Top - 2);
        }

        internal override void Draw(Graphics g)
        {
            labelLevel.Visible = Level > 0;
            labelLevel.Text = Level.ToString();

            labelCost.Visible = Text.Length > 0;
            labelCost.Text = Text;

            base.Draw(g);
        }
    }
}
