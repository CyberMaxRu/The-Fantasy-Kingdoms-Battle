using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCFormPage : VCIconButton
    {
        private List<VCFormPage> listPages;
        private int indexInList;

        public VCFormPage(VisualControl parent, int shiftX, int shiftY, List<VCFormPage> list, BitmapList bitmapList, int imageIndex, string caption, EventHandler onClick) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            UseFilter = false;
            HighlightUnderMouse = true;
            Caption = caption;

            Page = new VisualControl()
            {
                Visible = false
            };
            Page.Click += Page_Click;
            Click += onClick;

            listPages = list;
            indexInList = listPages.Count;
            listPages.Add(this);

            ArrangeControls();
        }

        private void Page_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(null);
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
        protected override bool Selected()
        {
            return Page.Visible;
        }

        internal override VisualControl GetControl(int x, int y)
        {
            VisualControl vc = base.GetControl(x, y);
            if (vc != null)
                return vc;

            return Page.GetControl(x, y);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if (Page.Visible)
                Page.DrawBackground(g);
        }

        internal override void Draw(Graphics g)
        {
            ImageFilter = Page.Visible ? ImageFilter.Press : ImageFilter.None;

            base.Draw(g);

            if (Page.Visible)
                Page.Draw(g);
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            Page.ShiftX = listPages[0].ShiftX - listPages[indexInList].ShiftX;
            Page.ShiftY = Height + FormMain.Config.GridSize;
            Page.ApplyMaxSize();
            ArrangeControl(Page, false);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddHeader(Caption);
            return true;
        }
    }
}
