using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class VCFormPage : VCButton
    {
        private VCLabel lblCaption;
        public VCFormPage(List<VCFormPage> list, ImageList imageList, int imageIndex, string caption, EventHandler onClick) : base(imageList, imageIndex)
        {
            Caption = caption;
            Page = new VisualControl
            {
                Visible = false
            };
            Click += onClick;

            list.Add(this);

            lblCaption = new VCLabel(FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, FormMain.Config.GridSize * 3, caption);
            Page.AddControl(lblCaption, new Point(0, 0));
            ArrangeControlsAndContainers();
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        
        protected override void ArrangeControlsAndContainers()
        {
            base.ArrangeControlsAndContainers();

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
