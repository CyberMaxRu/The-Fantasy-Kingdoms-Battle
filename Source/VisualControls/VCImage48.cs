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
        private VCLabel labelLevel;
        private VCLabel labelText;
        protected VCLabel labelQuantity;

        public VCImage48(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, Program.formMain.imListObjects48, imageIndex)
        {
            labelText = new VCLabel(this, 0, Height - 16, Program.formMain.fontSmallC, Color, 16, "");
            labelText.StringFormat.LineAlignment = StringAlignment.Far;
            labelText.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
            labelText.ManualDraw = true;
            labelText.Width = Width;

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

        internal string Text { get; set; } = "";
        internal Color Color { get; set; } = FormMain.Config.CommonCost;
        internal int Level { get; set; }
        internal int Quantity { get; set; }
        internal override void Draw(Graphics g)
        {
            //Debug.Assert(Cost >= 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(Quantity >= 0);

            base.Draw(g);

            // Иконка
            if (Visible && (ImageIndex != -1))
            {
                // Цена
                if (Text.Length > 0)
                {
                    labelText.Text = Text;
                    labelText.Color = Color;
                    labelText.Draw(g);
                }

                // Уровень
                if (Level > 0)
                {
                    labelLevel.Text = Level.ToString();
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
