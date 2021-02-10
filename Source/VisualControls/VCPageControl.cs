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
    // Класс иконки страницы
    internal sealed class VCPictureBoxPage : VCImage
    {
        private Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private int shiftX;

        public VCPictureBoxPage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            HighlightUnderMouse = true;
        }

        internal string NamePage { get; set; }
        internal int IndexPage { get; set; }
        internal VisualControl ContextPage { get; set; }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(NamePage, "", "");
            return true;
        }

        internal override void DoClick()
        {
            base.DoClick();

            (Parent as VCPageControl).ActivatePage(IndexPage);
            Program.formMain.ShowFrame();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            shiftX = IndexPage > 0 ? 4 : 0;

            if ((Parent as VCPageControl).ActivePage == IndexPage)
            {
                g.DrawLine(penBorder, Left, Top, Left + Width, Top);// Верх
                g.DrawLine(penBorder, Left, Top, Left, Top + Height - 1);// Левый край
                g.DrawLine(penBorder, Left + Width, Top, Left + Width, Top + Height - 1);// Правый край
                if (shiftX > 0)
                    g.DrawLine(penBorder, Left - shiftX, Top + Height - 1, Left, Top + Height - 1);
            }
            else
            {
                g.DrawLine(penBorder, Left - shiftX, Top + Height - 1, Left + Width, Top + Height - 1);
            }
        }
    }

    // Класс контрола со страницами
    internal sealed class VCPageControl : VisualControl
    {
        private List<VCPictureBoxPage> btnPages = new List<VCPictureBoxPage>();
        private int leftForNextPage = 0;
        private VCLabel lblCaptionPage;
        private VCPictureBoxPage activePage;

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

            VCPictureBoxPage page = new VCPictureBoxPage(this, leftForNextPage, 0, BitmapList, imageIndex)
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

            foreach (VCPictureBoxPage p in btnPages)
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
