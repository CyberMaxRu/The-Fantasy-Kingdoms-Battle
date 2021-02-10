using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс контрола со страницами
    internal sealed class VCPageControl : VisualControl
    {
        private List<VCTabButton> btnPages = new List<VCTabButton>();
        private int leftForNextPage = 0;
        private VCLabel lblCaptionPage;
        private VCTabButton activePage;

        public VCPageControl(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ActivePage = -1;

            lblCaptionPage = new VCLabel(this, 0, BitmapList.Size + FormMain.Config.GridSize, FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, 16, "");
        }

        internal BitmapList BitmapList { get; set; }
        internal int ActivePage { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            lblCaptionPage.Width = Width;                
        }

        internal void AddPage(string namePage, int imageIndex, VisualControl controlForPage)
        {

            VCTabButton page = new VCTabButton(this, leftForNextPage, 0, BitmapList, imageIndex)
            {
                NamePage = namePage,
                IndexPage = btnPages.Count,
                ContextPage = controlForPage
            };
            btnPages.Add(page);

            if (controlForPage != null)
            {
                AddControl(controlForPage);
                controlForPage.ShiftX = 0;
                controlForPage.ShiftY = lblCaptionPage.NextTop();
                //controlForPage.SetVisible(false);
                //controlForPage.Parent = this;
            }

            leftForNextPage += page.Width + FormMain.Config.GridSizeHalf;

            if (ActivePage == -1)
                ActivatePage(0);
        }

        internal void ActivatePage(int indexPage)
        {
            if (activePage?.IndexPage != indexPage)
            {
                ActivePage = indexPage;

                //activePage?.Invalidate();
                if ((activePage != null) && (activePage.ContextPage != null))
                    activePage.ContextPage.Visible = false;
                activePage = btnPages[indexPage];
                lblCaptionPage.Text = activePage.NamePage;
                if ((activePage != null) && (activePage.ContextPage != null))
                    activePage.ContextPage.Visible = true;

                Program.formMain.NeedRedrawFrame();
            }
        }

        internal void ApplyMinWidth()
        {
            int minWidth = 0;

            foreach (VCTabButton p in btnPages)
            {
                if (p.ContextPage != null)
                {
                    minWidth = p.ContextPage.MaxSize().Width;
                }
            }

            Width = minWidth;
        }

        internal void SetPageVisible(int page, bool visible)
        {
            btnPages[page].Visible = visible;

            if (!visible)
            {
                Debug.Assert(page != 0);

                ActivatePage(0);
            }
        }
    }
}
