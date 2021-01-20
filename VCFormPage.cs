using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class VCFormPage : VCButton
    {
        private VCLabel lblCaption;
        public VCFormPage(VisualControl parent, Point shift, List<VCFormPage> list, ImageList imageList, int imageIndex, string caption, EventHandler onClick) : base(parent, shift, imageList, imageIndex)
        {
            Caption = caption;
            Page = new VisualControl(false)
            {
                Visible = false
            };
            Click += onClick;

            list.Add(this);

            lblCaption = new VCLabel(Page, new Point(0, 0), FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, FormMain.Config.GridSize * 3, caption);
            ArrangeControls();
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        
        protected override void ArrangeControls()
        {
            base.ArrangeControls();

            Page.Top = NextTop();
            lblCaption.Width = Page.Width;
        }

        internal override void Draw(Graphics g, int x, int y)
        {
            base.Draw(g, x, y);

            if (Page.Visible)
                Page.Draw(g, Page.Left, Page.Top);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Caption, "", "");
            return true;
        }
    }
}
