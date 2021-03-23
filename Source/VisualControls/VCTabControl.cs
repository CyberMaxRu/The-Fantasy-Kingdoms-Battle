using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс контрола со страницами
    internal sealed class VCTabControl : VisualControl
    {
        private List<VCTabButton> btnTabs = new List<VCTabButton>();
        private int leftForNextPage = 0;
        private VCLabelM2 lblCaptionPage;
        private VCTabButton activePage;

        public VCTabControl(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ActivePage = -1;

            lblCaptionPage = new VCLabelM2(this, 0, BitmapList.Size + FormMain.Config.GridSize, Program.formMain.fontSmallBC, FormMain.Config.CommonCaptionPage, 24, "");
            lblCaptionPage.StringFormat.LineAlignment = StringAlignment.Center;
            lblCaptionPage.ShowBorder = true;
            lblCaptionPage.Color = Color.LightGreen;
        }

        internal BitmapList BitmapList { get; set; }
        internal int ActivePage { get; set; }

        internal override void ArrangeControls()
        {
            lblCaptionPage.Width = Width;

            base.ArrangeControls();
        }

        internal void AddTab(string nameTab, int imageIndex, VisualControl controlForPage)
        {
            if (controlForPage != null)
                controlForPage.Visible = false;

            VCTabButton btnTab = new VCTabButton(this, leftForNextPage, 0, BitmapList, imageIndex)
            {
                NameTab = nameTab,
                IndexPage = btnTabs.Count,
                ContextPage = controlForPage
            };
            btnTabs.Add(btnTab);

            if (controlForPage != null)
            {
                AddControl(controlForPage);
                controlForPage.ShiftX = 0;
                controlForPage.ShiftY = lblCaptionPage.NextTop();
            }

            leftForNextPage += btnTab.Width + FormMain.Config.GridSizeHalf;

            if (ActivePage == -1)
                ActivatePage(0);
        }

        internal void ActivatePage(int indexPage)
        {
            if (activePage?.IndexPage != indexPage)
            {
                ActivePage = indexPage;

                if ((activePage != null) && (activePage.ContextPage != null))
                    activePage.ContextPage.Visible = false;

                activePage = btnTabs[indexPage];
                lblCaptionPage.Text = activePage.NameTab;

                if ((activePage != null) && (activePage.ContextPage != null))
                    activePage.ContextPage.Visible = true;

                Program.formMain.NeedRedrawFrame();
            }
        }

        internal void ApplyMinSize()
        {
            int minWidth = 0;
            int minHeight = 0;

            foreach (VCTabButton b in btnTabs)
            {
                if (b.ContextPage != null)
                {
                    minWidth = Math.Max(b.ContextPage.MaxSize().Width, minWidth);
                    minHeight = Math.Max(b.ContextPage.MaxSize().Height, minHeight);
                }
            }

            Height = lblCaptionPage.NextTop() + minHeight;
            Width = minWidth;
        }

        internal void SetPageVisible(int page, bool visible)
        {
            btnTabs[page].Visible = visible;

            if (!visible)
            {
                Debug.Assert(page != 0);

                ActivatePage(0);
            }
        }
    }
}
