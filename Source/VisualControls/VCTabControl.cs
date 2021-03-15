using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Fantasy_Kingdoms_Battle
{
    // Класс контрола со страницами
    internal sealed class VCTabControl : VisualControl
    {
        private List<VCTabButton> btnTabs = new List<VCTabButton>();
        private int leftForNextPage = 0;
        private VCLabel lblCaptionPage;
        private VCTabButton activePage;

        public VCTabControl(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ActivePage = -1;

            lblCaptionPage = new VCLabel(this, 0, BitmapList.Size + FormMain.Config.GridSize, FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, 24, "");
            lblCaptionPage.StringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
            lblCaptionPage.ShowBorder = true;
        }

        internal BitmapList BitmapList { get; set; }
        internal int ActivePage { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            lblCaptionPage.Width = Width;
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
