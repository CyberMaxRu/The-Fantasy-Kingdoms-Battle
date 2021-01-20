using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class VCFormPage : VCButton
    {
        private VCLabel lblCaption;
        public VCFormPage(VisualControl parent, int shiftX, int shiftY, List<VCFormPage> list, ImageList imageList, int imageIndex, string caption, EventHandler onClick) : base(parent, shiftX, shiftY, imageList, imageIndex)
        {
            Caption = caption;
            Page = new VisualControl()
            {
                Visible = false
            };
            Click += onClick;

            list.Add(this);

            lblCaption = new VCLabel(Page, 0, 0, FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, FormMain.Config.GridSize * 3, caption);
            ArrangeControls();
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        
        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            Page.Top = NextTop();
            lblCaption.Width = Page.Width;
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if (Page.Visible)
                Page.Draw(g);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Caption, "", "");
            return true;
        }
    }
}
