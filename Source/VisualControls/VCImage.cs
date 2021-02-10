using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private int shiftImage;
        private VCLabel labelCost;
        private VCLabel labelLevel;
        private VCLabel labelPopupQuantity;
        private SolidBrush brushPopupQuantity;

        public VCImage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ImageIndex = imageIndex;

            labelCost = new VCLabel(this, 0, Height - 12, FormMain.Config.FontCost, FormMain.Config.CommonCost, 16, "");
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную

            labelLevel = new VCLabel(this, 0, FormMain.Config.GridSize, FormMain.Config.FontBuildingLevel, FormMain.Config.CommonLevel, 16, "");
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            labelLevel.Visible = false;

            labelPopupQuantity = new VCLabel(this, Width - 7, -3, FormMain.Config.FontPopupQuantity, FormMain.Config.CommonPopupQuantity, 16, "");
            labelPopupQuantity.StringFormat.LineAlignment = StringAlignment.Center;
            labelPopupQuantity.StringFormat.Alignment = StringAlignment.Center;
            labelPopupQuantity.Visible = false;

            brushPopupQuantity = new SolidBrush(FormMain.Config.CommonPopupQuantityBack);

            ValidateSize();
        }

        internal BitmapList BitmapList { get; }
        internal int ImageIndex { get; set; }
        internal ImageState ImageState { get; set; } = ImageState.Normal;
        protected int ShiftImage { get => shiftImage; set { shiftImage = value; ValidateSize(); } }
        internal int Cost { get; set; }
        internal bool ShowCostZero { get; set; }
        internal int Level { get; set; }
        internal int PopupQuantity { get; set; }

        internal override void Draw(Graphics g)
        {
            //Debug.Assert(Cost >= 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(PopupQuantity >= 0);
            Debug.Assert(PopupQuantity <= 9);

            base.Draw(g);

            // Иконка
            if (ImageIndex != -1)
                BitmapList.DrawImage(g, ImageIndex, ImageState, Left + ShiftImage, Top + ShiftImage);

            // Цена
            if ((Cost != 0) || ShowCostZero)
            {
                labelCost.Text = Cost.ToString();
                labelCost.Draw(g);
            }

            // Уровень
            if (Level > 0)
            {
                labelLevel.Text = Level.ToString();
                labelLevel.Draw(g);
            }

            // Всплывающее количество 
            if (PopupQuantity > 0)
            {
                g.FillEllipse(brushPopupQuantity, Left + Width - 13, Top - 5, 18, 18);

                labelPopupQuantity.Text = PopupQuantity.ToString();
                labelPopupQuantity.Draw(g);
            }
        }

        private void ValidateSize()
        {
            Width = BitmapList.Size + (ShiftImage * 2);
            Height = BitmapList.Size + (ShiftImage * 2);

            if (labelCost != null)
                labelCost.Width = Width;

            labelLevel.Width = Width - FormMain.Config.GridSizeHalf;
            labelPopupQuantity.Width = labelPopupQuantity.Height;
        }
    }
}
