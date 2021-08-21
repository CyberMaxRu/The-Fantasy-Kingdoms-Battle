using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{

    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private int shiftImageX;
        private int shiftImageY;
        private VCLabel labelCost;
        private VCLabel labelLevel;
        protected VCLabel labelQuantity;
        private const int sizePopupBackground = 18;
        int shiftlabelLevel;

        public VCImage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ImageIndex = imageIndex;

            labelCost = new VCLabel(this, 0, Height - 12, Program.formMain.fontSmallC, FormMain.Config.CommonCost, 16, "");
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
            labelCost.ManualDraw = true;

            shiftlabelLevel = bitmapList.Size >= 128 ? FormMain.Config.GridSize : 6;
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

            ValidateSize();
        }

        internal BitmapList BitmapList { get; set; }
        internal int ImageIndex { get; set; }
        internal bool NormalImage { get; set; } = true;
        internal bool ImageIsEnabled { get; set; } = true;
        protected int ShiftImageX { get => shiftImageX; set { shiftImageX = value; ValidateSize(); } }
        protected int ShiftImageY { get => shiftImageY; set { shiftImageY = value; ValidateSize(); } }
        internal string Cost { get; set; }
        internal int Level { get; set; }
        internal int Quantity { get; set; }
        internal bool HighlightUnderMouse { get; set; } = false;
        internal bool ShowAsPressed { get; set; } = false;
        internal TypeObject TypeObject { get; set; }// Тип объекта, связанный с этим изображением

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            Program.formMain.SetNeedRedrawFrame();
            if (PlaySelectSound())
                Program.formMain.PlaySelectButton();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void MouseDown()
        {
            base.MouseDown();

        }

        protected override bool AllowClick()
        {
            return ImageIsEnabled && base.AllowClick();
        }

        internal override void Draw(Graphics g)
        {
            //Debug.Assert(Cost >= 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(Quantity >= 0);


            base.Draw(g);

            // Иконка
            if (Visible && (ImageIndex != -1))
            {
                BitmapList.DrawImage(g, ImageIndex, (true/*UseFilter*/ || ImageIsEnabled) && NormalImage, HighlightUnderMouse && MouseOver && !MouseClicked, Left + ShiftImageX, Top + ShiftImageY);

                // Цена
                if (Cost != null)
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

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            labelQuantity.ShiftY = Height - 16;
            labelCost.ShiftY = Height - 16;
        }

        private void ValidateSize()
        {
            Width = BitmapList.Size + (ShiftImageX * 2);
            Height = BitmapList.Size + (ShiftImageY * 2);

            labelLevel.Width = Width - shiftlabelLevel;
            labelQuantity.Width = Width - shiftlabelLevel;
            labelCost.Width = Width;
        }

        protected virtual bool PlaySelectSound()
        {
            return true;// ImageIsEnabled && ((UseFilter && MouseOver) || HighlightUnderMouse);
        }
    }
}
