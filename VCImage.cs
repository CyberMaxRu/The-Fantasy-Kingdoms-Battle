using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private int shiftImage;
        private VCLabel labelCost;
        private VCLabel labelLevel;
        private VCLabel labelPopupQuantity;
        private SolidBrush brushPopupQuantity;

        public VCImage(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            ImageList = imageList;
            ImageIndex = imageIndex;

            ValidateSize();

            labelCost = new VCLabel(this, 0, Height - 16, FormMain.Config.FontCost, FormMain.Config.CommonCost, 16, "");
            labelCost.Width = Width;
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную

            labelLevel = new VCLabel(this, 0, FormMain.Config.GridSize, FormMain.Config.FontBuildingLevel, FormMain.Config.CommonLevel, 16, "");
            labelLevel.Width = Width - FormMain.Config.GridSizeHalf;
            labelLevel.Visible = false;
            labelLevel.StringFormat.LineAlignment = StringAlignment.Near;
            labelLevel.StringFormat.Alignment = StringAlignment.Far;

            labelPopupQuantity = new VCLabel(this, Width - 7, -3, FormMain.Config.FontPopupQuantity, FormMain.Config.CommonPopupQuantity, 16, "");
            labelPopupQuantity.Width = labelPopupQuantity.Height;
            labelPopupQuantity.Visible = false;
            labelPopupQuantity.StringFormat.LineAlignment = StringAlignment.Center;
            labelPopupQuantity.StringFormat.Alignment = StringAlignment.Center;

            brushPopupQuantity = new SolidBrush(FormMain.Config.CommonPopupQuantityBack);
        }

        internal ImageList ImageList { get; }
        internal int ImageIndex { get; set; }
        internal bool NormalImage { get; set; } = true;
        protected int ShiftImage { get => shiftImage; set { shiftImage = value; ValidateSize(); } }
        internal int Cost { get; set; }
        internal bool ShowCostZero { get; set; }
        internal int Level { get; set; }
        internal int PopupQuantity { get; set; }

        private void ValidateSize()
        {
            Width = ImageList.ImageSize.Width + (ShiftImage * 2);
            Height = ImageList.ImageSize.Height + (ShiftImage * 2);
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Cost >= 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(PopupQuantity >= 0);
            Debug.Assert(PopupQuantity <= 9);

            base.Draw(g);

            if (ImageIndex != -1)
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, NormalImage), Left + ShiftImage, Top + ShiftImage);
            //else
            //    g.DrawImage(Program.formMain.bmpEmptyEntity, new Rectangle(Left + 1, Top + 0, Program.formMain.bmpBorderForIcon.Width - 2, Program.formMain.bmpBorderForIcon.Height - 2));

            if ((Cost > 0) || ShowCostZero)
            {
                labelCost.Text = Cost.ToString();
                labelCost.Draw(g);
            }

            if (Level > 0)
            {
                labelLevel.Text = Level.ToString();
                labelLevel.Draw(g);
            }

            if (PopupQuantity > 0)
            {
                g.FillEllipse(brushPopupQuantity, Left + Width - 13, Top - 5, 18, 18);

                labelPopupQuantity.Text = PopupQuantity.ToString();
                labelPopupQuantity.Draw(g);
            }
        }
    }
}
