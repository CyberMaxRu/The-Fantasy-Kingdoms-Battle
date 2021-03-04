using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ImageFilter { None, Active, Select, Press, Disabled };

    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private int shiftImageX;
        private int shiftImageY;
        private VCLabelM2 labelCost;
        private VCLabelM2 labelLevel;
        private VCLabelM2 labelQuantity;
        private VCLabel labelPopupQuantity;
        private SolidBrush brushPopupQuantity;

        private bool mouseClicked;

        public VCImage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ImageIndex = imageIndex;

            labelCost = new VCLabelM2(this, 0, Height - 12, Program.formMain.fontCost, FormMain.Config.CommonCost, 16, "");
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную

            labelLevel = new VCLabelM2(this, 0, FormMain.Config.GridSize, Program.formMain.fontLevel, FormMain.Config.CommonLevel, 16, "");
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;
            labelLevel.Visible = false;

            labelQuantity = new VCLabelM2(this, 0, FormMain.Config.GridSize, Program.formMain.fontLevel, FormMain.Config.CommonQuantity, 16, "");
            labelQuantity.StringFormat.LineAlignment = StringAlignment.Far;
            labelQuantity.StringFormat.Alignment = StringAlignment.Far;
            labelQuantity.Visible = false;

            labelPopupQuantity = new VCLabel(this, Width - 7, -3, FormMain.Config.FontPopupQuantity, FormMain.Config.CommonPopupQuantity, 16, "");
            labelPopupQuantity.StringFormat.LineAlignment = StringAlignment.Center;
            labelPopupQuantity.StringFormat.Alignment = StringAlignment.Center;
            labelPopupQuantity.Visible = false;

            brushPopupQuantity = new SolidBrush(FormMain.Config.CommonPopupQuantityBack);

            ValidateSize();
        }

        internal BitmapList BitmapList { get; set; }
        internal int ImageIndex { get; set; }
        internal bool ImageIsEnabled { get; set; } = true;
        internal bool ImageIsOver { get; set; } = false;
        protected int ShiftImageX { get => shiftImageX; set { shiftImageX = value; ValidateSize(); } }
        protected int ShiftImageY { get => shiftImageY; set { shiftImageY = value; ValidateSize(); } }
        internal int Cost { get; set; }
        internal bool ShowCostZero { get; set; }
        internal int Level { get; set; }
        internal int Quantity { get; set; }
        internal int PopupQuantity { get; set; }
        internal bool HighlightUnderMouse { get; set; } = false;
        internal bool ShowAsPressed { get; set; } = false;
        internal bool UseFilter { get; set; } = false;
        internal ImageFilter ImageFilter { get; set; } = ImageFilter.None;
        internal TypeObject TypeObject { get; set; }// Тип объекта, связанный с этим изображением

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            ImageIsOver = true;

            if (!leftButtonDown)
                mouseClicked = false;

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            ImageIsOver = false;
            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void MouseDown()
        {
            base.MouseDown();

            mouseClicked = true;
            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void MouseUp()
        {
            base.MouseUp();

            if (mouseClicked != false)
            {
                mouseClicked = false;
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        internal override void DoClick()
        {
            base.DoClick();

            if ((TypeObject != null) && (TypeObject.SoundSelect.Length > 0))
                Program.formMain.PlaySoundSelect(TypeObject.SoundSelect);
        }

        internal override void Draw(Graphics g)
        {
            //Debug.Assert(Cost >= 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(Quantity >= 0);
            Debug.Assert(PopupQuantity >= 0);
            Debug.Assert(PopupQuantity <= 9);

            if (Visible && (BitmapList.Size == Program.formMain.ilMenuCellFilters.Size) && UseFilter)
            {
                if (ImageIsEnabled)
                {
                    if (ShowAsPressed || (mouseClicked && ImageIsOver))
                        ImageFilter = ImageFilter.Press;
                    else if (ImageIsOver)
                        ImageFilter = ImageFilter.Select;
                    else
                        ImageFilter = ImageFilter.Active;
                }
                else
                    ImageFilter = ImageFilter.Disabled;
            }

            base.Draw(g);

            // Иконка
            if (Visible && (ImageIndex != -1))
            {
                BitmapList.DrawImage(g, ImageIndex, UseFilter || ImageIsEnabled, HighlightUnderMouse && ImageIsOver, Left + ShiftImageX, Top + ShiftImageY);

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

                // Количество
                if (Quantity > 0)
                {
                    labelQuantity.Text = Quantity.ToString();
                    labelQuantity.Draw(g);
                }

                if (ImageFilter != ImageFilter.None)
                {
                    Debug.Assert(BitmapList.Size == Program.formMain.ilMenuCellFilters.Size);
                }

                switch (ImageFilter)
                {
                    case ImageFilter.Active:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(0, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    case ImageFilter.Select:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(1, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    case ImageFilter.Press:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(2, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    case ImageFilter.Disabled:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(3, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    default:
                        break;
                }

                // Всплывающее количество 
                if (PopupQuantity > 0)
                {
                    g.FillEllipse(brushPopupQuantity, Left + Width - 13, Top - 5, 18, 18);

                    labelPopupQuantity.Text = PopupQuantity.ToString();
                    labelPopupQuantity.Draw(g);
                }
            }
        }

        private void ValidateSize()
        {
            Width = BitmapList.Size + (ShiftImageX * 2);
            Height = BitmapList.Size + (ShiftImageY * 2);

            labelCost.Width = Width;
            labelCost.ShiftY = Height - 16;

            if (Width == 48)
            {
                labelLevel.Width = Width - FormMain.Config.GridSizeHalf;
                labelLevel.ShiftY = FormMain.Config.GridSizeHalf;
            }
            else
                labelLevel.Width = Width - FormMain.Config.GridSize;

            labelQuantity.Width = Width - FormMain.Config.GridSizeHalf;
            labelQuantity.ShiftY = Height - 12; 

            labelPopupQuantity.Width = labelPopupQuantity.Height;
            labelPopupQuantity.ShiftX = Width - 11;
        }
    }
}
