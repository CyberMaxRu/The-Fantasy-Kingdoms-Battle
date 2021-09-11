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
        private VCLabel lblCaptionPage;
        private VCTabButton activePage;

        public VCTabControl(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ActivePage = -1;

            lblCaptionPage = new VCLabel(this, 0, Program.formMain.imListObjects48.Size + FormMain.Config.GridSize, Program.formMain.fontSmallBC, FormMain.Config.CommonCaptionPage, 24, "");
            lblCaptionPage.StringFormat.LineAlignment = StringAlignment.Center;
            lblCaptionPage.ShowBorder = true;
            lblCaptionPage.Color = Color.LightGreen;
        }

        internal int ActivePage { get; set; }

        internal override void ArrangeControls()
        {
            lblCaptionPage.Width = Width;

            base.ArrangeControls();

            foreach (VCTabButton tb in btnTabs)
            {
                if (tb.ContextPage != null)
                    tb.ContextPage.ShiftX = (Width - tb.ContextPage.Width) / 2;
            }
        }

        internal VCTabButton AddTab(string nameTab, int imageIndex, VisualControl controlForPage)
        {
            if (controlForPage != null)
                controlForPage.Visible = false;

            VCTabButton btnTab = new VCTabButton(this, leftForNextPage, 0, imageIndex, controlForPage)
            {
                Hint = nameTab,
                IndexPage = btnTabs.Count
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

            return btnTab;
        }

        internal void ActivatePage(int indexPage)
        {
            if (activePage?.IndexPage != indexPage)
            {
                ActivePage = indexPage;

                if ((activePage != null) && (activePage.ContextPage != null))
                    activePage.ContextPage.Visible = false;

                activePage = btnTabs[indexPage];
                lblCaptionPage.Text = activePage.Hint;

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
