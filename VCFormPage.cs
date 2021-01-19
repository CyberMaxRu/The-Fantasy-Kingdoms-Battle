using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class VCFormPage : VCButton
    {
        public VCFormPage(List<VCFormPage> list, ImageList imageList, int imageIndex, string caption, EventHandler onClick) : base(imageList, imageIndex)
        {
            Caption = caption;
            Page = new VisualControl
            {
                Visible = false
            };
            Click += onClick;

            list.Add(this);
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        
        protected override void ArrangeControlsAndContainers()
        {
            base.ArrangeControlsAndContainers();

            Page.Top = NextTop();
        }

        internal override void Draw(Bitmap b, Graphics g, int x, int y)
        {
            base.Draw(b, g, x, y);

            if (Page.Visible)
                Page.Draw(b, g, Page.Left, Page.Top);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Caption, "", "");
            return true;
        }
    }
}
