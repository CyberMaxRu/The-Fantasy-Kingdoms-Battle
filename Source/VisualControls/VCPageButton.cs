using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс кнопки для PageControl'а
    internal sealed class VCPageButton : VCIconButton48
    {
        public VCPageButton(VisualControl parent, int shiftX, int shiftY, int imageIndex, string caption, string advice) : base(parent, shiftX, shiftY, imageIndex)
        {
            HighlightUnderMouse = true;
            ShowBorder = true;
            Caption = caption;
            Advice = advice;

            Page = new VisualControl(parent, 0, NextTop());
            Page.Visible = false;
            Page.Click += Page_Click;
        }

        private void Page_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(null);
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        internal Bitmap BackgroundImage { get; set; }
        internal string Advice { get; }
        internal BigEntity SelectedPlayerObject { get; set; }
    }

}
