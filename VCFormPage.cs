using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class VCFormPage : VCButton
    {
        private VCLabel lblCaption;
        private List<VCFormPage> listPages;
        private int indexInList;
        public VCFormPage(VisualControl parent, int shiftX, int shiftY, List<VCFormPage> list, BitmapList bitmapList, int imageIndex, string caption, EventHandler onClick) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            Caption = caption;
            Page = new VisualControl()
            {
                Visible = false
            };
            Click += onClick;

            listPages = list;
            indexInList = listPages.Count;
            listPages.Add(this);
            

            lblCaption = new VCLabel(Page, 0, 0, FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, FormMain.Config.GridSize * 3, caption);
            //lblCaption.StringFormat.LineAlignment = StringAlignment.Center;

            TopForControls = 24;
            ArrangeControls();
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        internal int TopForControls { get; }
        
        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            Page.ShiftX = listPages[0].ShiftX - listPages[indexInList].ShiftX;
            Page.ShiftY = Height + FormMain.Config.GridSize;
            ArrangeControl(Page);

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
