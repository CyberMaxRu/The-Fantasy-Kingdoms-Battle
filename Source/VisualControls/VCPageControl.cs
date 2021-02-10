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
        private int imageIndex;
        private ImageList imageList;
        private int mouseOver;
        private Pen penBorder = new Pen(FormMain.Config.CommonBorder);

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

        private void UpdateImage()
        {
            //Image = (ImageList != null) && (imageIndex != -1) ? imageList.Images[imageIndex * 2 + mouseOver] : null;
        }

        protected void OnMouseEnter(EventArgs e)
        {
            //base.OnMouseEnter(e);

            mouseOver = 1;
            UpdateImage();
        }

        protected void OnMouseLeave(EventArgs e)
        {
            //base.OnMouseLeave(e);

            mouseOver = 0;
            UpdateImage();
        }

        internal override void DoClick()
        {
            base.DoClick();

            (Parent as VCPageControl).ActivatePage(IndexPage);
            Program.formMain.ShowFrame();
        }

        protected void OnPaint(PaintEventArgs pe)
        {
            /*base.OnPaint(pe);

            if ((Parent as PageControl).ActivePage == IndexPage)
            {
                //pe.Graphics.DrawLine(penBorder, 0, 0, Width - 1, 0);
                //pe.Graphics.DrawLine(penBorder, 0, 0, 0, Height - 1);
                //pe.Graphics.DrawLine(penBorder, Width - 1, 0, Width - 1, Height - 1);
                pe.Graphics.DrawLine(penBorder, 0, Height - 1, Width - 1, Height - 1);
            }*/
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
