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

        public VCImage128(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.BmpListObjects128, -1)
        {
            ShowBorder = true;

            labelLevel = new VCLabel(this, 0, FormMain.Config.GridSizeHalf, Program.formMain.FontMedCaptionC, FormMain.Config.CommonLevel, 16, ""); ;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.Width = 32;

            TextCaption = new VCText(this, FormMain.Config.GridSizeHalf, 0, Program.formMain.FontSmallC, FormMain.Config.CommonCost, Width - FormMain.Config.GridSize);
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
            if ((Width == 128) && (Height == 128))
            {
                if (BorderWithoutProgressBar)
                    DrawImage(g, Program.formMain.bmpBorderBig, Left - 2, Top - 2);
                else
                    DrawImage(g, Program.formMain.bmpBorderBigForProgressBar, Left - 2, Top - 2);
            }
            else if ((Width == 96) && (Height == 96))
            {
                if (BorderWithoutProgressBar)
                    Utils.DoException("Бордюр не поддерживается");
                else
                    DrawImage(g, Program.formMain.bmpBorder96ForProgressBar, Left, Top);
            }
            else
                Utils.DoException($"Неизвестный размер: Width = {Width}, Height = {Height}");
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

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            if (labelLevel != null)
            {
                labelLevel.ShiftX = Width - labelLevel.Width - 6;
                TextCaption.Width = Width - FormMain.Config.GridSize;
            }
        }
    }
}
