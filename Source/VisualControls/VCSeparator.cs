﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - разделитель
    internal sealed class VCSeparator : VCBitmapBand
    {
        public VCSeparator(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, 10)
        {
            stepForNextControl = FormMain.Config.GridSizeHalf;

            Width = GetBitmap().Width;
        }

        protected override Bitmap GetBitmap() => Program.formMain.bmpSeparator;
    }
}
