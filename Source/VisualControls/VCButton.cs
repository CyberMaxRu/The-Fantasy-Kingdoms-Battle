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
        private readonly VCLabel labelCaption;
        private Bitmap bmpNormal;
        private Bitmap bmpHot;
        private Bitmap bmpDisabled;
        private Bitmap bmpPressed;
        private bool enabled = true;

        public VCButton(VisualControl parent, int shiftX, int shiftY, string caption) : base(parent, shiftX, shiftY, 31)
        {
            Caption = caption;
            Width = 160;
            PlaySoundOnEnter = true;
            PlaySoundOnClick = true;

            labelCaption = new VCLabel(this, WidthCap, 1, Program.formMain.fontSmallC, Color.White, GetBitmap().Height, "");
            labelCaption.StringFormat.LineAlignment = StringAlignment.Center;
            labelCaption.IsActiveControl = false;
        }

        internal string Caption { get; set; }
        internal bool Enabled { get => enabled; set { enabled = value; PlaySoundOnEnter = enabled; } }
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandButtonNormal;
        protected override bool AllowClick() => Enabled;

        internal override void ArrangeControls()
        {
            bmpNormal = bmpForDraw;
            bmpHot?.Dispose();
            bmpHot = PrepareBand(Program.formMain.bmpBandButtonHot);
            bmpDisabled?.Dispose();
            bmpDisabled = PrepareBand(Program.formMain.bmpBandButtonDisabled);
            bmpPressed?.Dispose();
            bmpPressed = PrepareBand(Program.formMain.bmpBandButtonPressed);

            labelCaption.Width = Width - (WidthCap * 2);

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            //Debug.Assert(bmpNormal != null);
            //Debug.Assert(bmpDisabled != null);
            //Debug.Assert(bmpPressed != null);

            bmpForDraw = !Enabled ? bmpDisabled : MouseOver && MouseClicked ? bmpPressed : MouseOver ? bmpHot : bmpNormal;
            labelCaption.Text = Caption;
            labelCaption.Color = !enabled ? Color.DarkGray : MouseOver ? Color.Gold : Color.PaleTurquoise;

            base.Draw(g);
        }
    }
}
