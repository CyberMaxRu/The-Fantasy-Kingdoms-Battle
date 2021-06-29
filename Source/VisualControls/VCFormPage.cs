using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCFormPage : VCIconButton
    {
        public VCFormPage(VisualControl parent, int shiftX, int shiftY, List<VCFormPage> list, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            UseFilter = false;
            HighlightUnderMouse = true;
            
            Page = new VisualControl(parent, 0, NextTop());
            Page.Visible = false;
            Page.Click += Page_Click;
        }

        private void Page_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(null);
        }

        internal VisualControl Page { get; }
        protected override bool Selected()
        {
            return Page.Visible;
        }
    }
}
