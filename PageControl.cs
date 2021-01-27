using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum IconPages { Inventory, History, Inhabitants, Products, Parameters, Abilities }

    // Класс иконки страницы
    internal sealed class PictureBoxPage : VCImage
    {
        private int imageIndex;
        private ImageList imageList;
        private int mouseOver;
        private Pen penBorder = new Pen(FormMain.Config.CommonBorder);

        public PictureBoxPage(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY, imageList, imageIndex)
        {
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

            (Parent as PageControl).ActivatePage(IndexPage);
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
    internal sealed class PageControl : VisualControl
    {
        private List<PictureBoxPage> btnPages = new List<PictureBoxPage>();
        private int leftForNextPage = 0;
        private VCLabel lblCaptionPage;
        private PictureBoxPage activePage;

        public PageControl(VisualControl parent, int shiftX, int shiftY, ImageList imageList) : base(parent, shiftX, shiftY)
        {
            ImageList = imageList;
            ActivePage = -1;

            lblCaptionPage = new VCLabel(this, 0, ImageList.ImageSize.Height + FormMain.Config.GridSize, FormMain.Config.FontCaptionPage, FormMain.Config.CommonCaptionPage, 16, "");
        }

        internal ImageList ImageList { get; set; }
        internal int ActivePage { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            lblCaptionPage.Width = Width;                
        }

        internal void AddPage(string namePage, int imageIndex, VisualControl controlForPage)
        {

            PictureBoxPage page = new PictureBoxPage(this, leftForNextPage, 0, ImageList, imageIndex)
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

            foreach (PictureBoxPage p in btnPages)
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
