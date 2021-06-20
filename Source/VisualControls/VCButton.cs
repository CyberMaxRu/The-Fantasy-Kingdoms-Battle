using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка
    internal sealed class VCButton : VCBitmapBand
    {
        private readonly VCLabelM2 labelCaption;
        private Bitmap bmpNormal;
        private Bitmap bmpHot;
        private Bitmap bmpDisabled;
        private Bitmap bmpPressed;
        private bool mouseOver;
        private bool mouseClicked;
        private bool enabled = true;

        public VCButton(VisualControl parent, int shiftX, int shiftY, string caption) : base(parent, shiftX, shiftY)
        {
            Caption = caption;
            Width = 160;

            labelCaption = new VCLabelM2(this, WidthCap(), 1, Program.formMain.fontSmallC, Color.White, GetBitmap().Height, "");
            labelCaption.StringFormat.Alignment = StringAlignment.Center;
            labelCaption.StringFormat.LineAlignment = StringAlignment.Center;
            labelCaption.Visible = false;
            labelCaption.ManualDraw = true;
        }

        internal string Caption { get; set; }
        internal bool Enabled { get => enabled; set { enabled = value; Program.formMain.NeedRedrawFrame(); } }
        protected override int WidthCap() => 31;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandButtonNormal;

        internal override void ArrangeControls()
        {
            bmpNormal = bmpForDraw;
            bmpHot?.Dispose();
            bmpHot = PrepareBand(Program.formMain.bmpBandButtonHot);
            bmpDisabled?.Dispose();
            bmpDisabled = PrepareBand(Program.formMain.bmpBandButtonDisabled);
            bmpPressed?.Dispose();
            bmpPressed = PrepareBand(Program.formMain.bmpBandButtonPressed);

            labelCaption.Width = Width - WidthCap() - WidthCap();

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            //Debug.Assert(bmpNormal != null);
            //Debug.Assert(bmpDisabled != null);
            //Debug.Assert(bmpPressed != null);

            bmpForDraw = !Enabled ? bmpDisabled : mouseOver && mouseClicked ? bmpPressed : mouseOver ? bmpHot : bmpNormal;

            base.Draw(g);

            labelCaption.Text = Caption;
            labelCaption.Color = !enabled ? Color.DarkGray : mouseOver ? Color.Gold : Color.PaleTurquoise;
            labelCaption.Draw(g);
        }

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            if (Enabled)
            {
                mouseOver = true;

                if (!leftButtonDown)
                    mouseClicked = false;

                Program.formMain.SetNeedRedrawFrame();
                Program.formMain.PlaySelectButton();
            }
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            mouseOver = false;
            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void MouseDown()
        {
            base.MouseDown();

            if (Enabled)
            {
                mouseClicked = true;
                Program.formMain.SetNeedRedrawFrame();
                Program.formMain.PlayPushButton();
            }
        }

        internal override void MouseUp()
        {
            base.MouseUp();

            if (mouseClicked)
            {
                mouseClicked = false;
                Program.formMain.SetNeedRedrawFrame();
            }
        }
        protected override bool AllowClick() => Enabled;
    }
}
