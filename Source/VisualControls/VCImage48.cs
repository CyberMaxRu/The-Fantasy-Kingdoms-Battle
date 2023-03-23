using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс для иконок 48 * 48
    internal class VCImage48 : VCImage
    {
        private VCLabel labelHighText;
        private VCLabel labelLowText;
        private VCLabel labelLevel;
        protected VCLabel labelQuantity;

        public VCImage48(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, Program.formMain.BmpListObjects48, imageIndex)
        {
            labelHighText = new VCLabel(this, 0, 2, Program.formMain.fontSmallC, Color, 16, "");
            labelHighText.StringFormat.LineAlignment = StringAlignment.Near;
            labelHighText.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
            labelHighText.ManualDraw = true;
            labelHighText.Width = Width;

            labelLowText = new VCLabel(this, 0, Height - 16, Program.formMain.fontSmallC, Color, 16, "");
            labelLowText.StringFormat.LineAlignment = StringAlignment.Far;
            labelLowText.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
            labelLowText.ManualDraw = true;
            labelLowText.Width = Width;

            labelLevel = new VCLabel(this, 0, 1, Program.formMain.fontSmallC, FormMain.Config.CommonLevel, 16, "");
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            labelLevel.Visible = false;
            labelLevel.ManualDraw = true;
            labelLevel.Width = Width - 4;

            labelQuantity = new VCLabel(this, 0, Height - 16, Program.formMain.fontSmallC, FormMain.Config.CommonQuantity, 16, "");
            labelQuantity.StringFormat.LineAlignment = StringAlignment.Far;
            labelQuantity.StringFormat.Alignment = StringAlignment.Far;
            labelQuantity.Visible = false;
            labelQuantity.ManualDraw = true;
            labelQuantity.Width = Width - 4;
        }

        internal string HighText { get; set; } = "";
        internal string LowText { get; set; } = "";
        internal Color Color { get; set; } = FormMain.Config.CommonCost;
        internal string Level { get; set; } = "";
        internal int Quantity { get; set; }
        internal override void Draw(Graphics g)
        {
            //Debug.Assert(Cost >= 0);
            Debug.Assert(Quantity >= 0);

            base.Draw(g);

            // Иконка
            if (Visible && (ImageIndex != -1))
            {
                // Верхний текст
                if (HighText.Length > 0)
                {
                    labelHighText.Text = HighText;
                    labelHighText.Color = Color;
                    labelHighText.Draw(g);
                }

                // Цена
                if (LowText.Length > 0)
                {
                    labelLowText.Text = LowText;
                    labelLowText.Color = Color;
                    labelLowText.Draw(g);
                }

                // Уровень
                if (Level.Length > 0)
                {
                    labelLevel.Text = Level;
                    labelLevel.Draw(g);
                }

                // Количество
                if (Quantity > 0)
                {
                    labelQuantity.Text = Quantity.ToString();
                    labelQuantity.Draw(g);
                }
            }
        }

        protected override bool AllowClick()
        {
            return ImageIsEnabled && base.AllowClick();
        }
    }
}
