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
        private VCLabel labelCost;
        protected VCLabel labelQuantity;
        int shiftlabelLevel;

        public VCImage48(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, Program.formMain.imListObjects48, imageIndex)
        {
            labelCost = new VCLabel(this, 0, Height - 12, Program.formMain.fontSmallC, FormMain.Config.CommonCost, 16, "");
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
            labelCost.ManualDraw = true;

            shiftlabelLevel = BitmapList.Size >= 128 ? FormMain.Config.GridSize : 6;
            labelLevel = new VCLabel(this, 0, shiftlabelLevel - 4, Program.formMain.fontMedCaptionC, FormMain.Config.CommonLevel, 16, "");
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            labelLevel.Visible = false;
            labelLevel.ManualDraw = true;

            labelQuantity = new VCLabel(this, 0, shiftlabelLevel - 2, Program.formMain.fontMedCaptionC, FormMain.Config.CommonQuantity, 16, "");
            labelQuantity.StringFormat.LineAlignment = StringAlignment.Far;
            labelQuantity.StringFormat.Alignment = StringAlignment.Far;
            labelQuantity.Visible = false;
            labelQuantity.ManualDraw = true;

            labelLevel.Width = Width - shiftlabelLevel;
            labelQuantity.Width = Width - shiftlabelLevel;
            labelCost.Width = Width;

            labelLevel.Width = Width - shiftlabelLevel;
            labelQuantity.Width = Width - shiftlabelLevel;
            labelCost.Width = Width;
        }


        internal string Cost { get; set; }
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
                if ((Cost != null) && (Cost != ""))
                {
                    Debug.Assert(Cost.Length > 0);

                    labelCost.Text = Cost;
                    labelCost.Draw(g);
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

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            labelQuantity.ShiftY = Height - 16;
            labelCost.ShiftY = Height - 16;
        }
    }
}
