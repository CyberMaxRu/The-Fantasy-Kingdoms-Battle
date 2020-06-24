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
    internal sealed class PictureBoxPage : PictureBox
    {
        private int imageIndex;
        private ImageList imageList;
        private int mouseOver;
        private Pen penBorder = new Pen(Color.Black);

        public PictureBoxPage()
        {
            Padding = new Padding(1, 1, 1, 1);            
        }

        internal int ImageIndex { get { return imageIndex; } set { imageIndex = value; UpdateImage(); } }
        internal ImageList ImageList
        {
            get { return imageList; }
            set { imageList = value; Size = value.ImageSize; UpdateImage(); }
        }
        internal string NamePage { get; set; }
        internal int IndexPage { get; set; }
        internal Control ContextPage { get; set; }

        private void UpdateImage()
        {
            Image = (ImageList != null) && (imageIndex != -1) ? imageList.Images[imageIndex * 2 + mouseOver] : null;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            mouseOver = 1;
            UpdateImage();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            mouseOver = 0;
            UpdateImage();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left)
            {
                (Parent as PageControl).ActivatePage(IndexPage);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if ((Parent as PageControl).ActivePage == IndexPage)
            {
                //pe.Graphics.DrawLine(penBorder, 0, 0, Width - 1, 0);
                //pe.Graphics.DrawLine(penBorder, 0, 0, 0, Height - 1);
                //pe.Graphics.DrawLine(penBorder, Width - 1, 0, Width - 1, Height - 1);
                pe.Graphics.DrawLine(penBorder, 0, Height - 1, Width - 1, Height - 1);
            }
        }
    }

    // Класс контрола со страницами
    internal sealed class PageControl : Label
    {
        private ImageList imList;
        private List<PictureBoxPage> btnPages = new List<PictureBoxPage>();
        private int leftForNextPage = 0;
        private Label lblCaptionPage;
        private PictureBoxPage activePage;

        public PageControl(ImageList imageList)
        {
            Debug.Assert(imageList != null);

            BackColor = Color.Transparent;

            imList = imageList;
            ActivePage = -1;

            lblCaptionPage = new Label()
            {
                Parent = this,
                Left = 0,
                Width = ClientSize.Width,
                Top = imageList.ImageSize.Height + Config.GRID_SIZE,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 12),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
        }

        internal int ActivePage { get; set; }

        internal void AddPage(string namePage, int imageIndex, Control controlForPage)
        {
            PictureBoxPage page = new PictureBoxPage()
            {
                Parent = this,
                Left = leftForNextPage,
                Top = 0,
                ImageList = imList,
                ImageIndex = imageIndex,
                NamePage = namePage,
                IndexPage = btnPages.Count,
                ContextPage = controlForPage
            };
            btnPages.Add(page);

            if (controlForPage != null)
            {
                controlForPage.Left = 0;
                controlForPage.Top = GuiUtils.NextTop(lblCaptionPage);
                controlForPage.Hide();
                controlForPage.Parent = this;
            }

            leftForNextPage += page.Width + Config.GRID_SIZE_HALF;

            if (ActivePage == -1)
                ActivatePage(0);
        }

        internal void ActivatePage(int indexPage)
        {
            if (activePage?.IndexPage != indexPage)
            {
                ActivePage = indexPage;

                activePage?.Invalidate();
                activePage?.ContextPage?.Hide();
                activePage = btnPages[indexPage];
                lblCaptionPage.Text = activePage.NamePage;
                activePage.ContextPage?.Show();
                activePage.Invalidate();
            }
        }

        internal void ApplyMinWidth()
        {
            int minWidth = 0;

            foreach (PictureBoxPage p in btnPages)
            {
                if (p.ContextPage != null)
                {
                    foreach (Control c in p.ContextPage.Controls)
                    {
                        minWidth = Math.Max(minWidth, c.Left + c.Width);
                    }
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
