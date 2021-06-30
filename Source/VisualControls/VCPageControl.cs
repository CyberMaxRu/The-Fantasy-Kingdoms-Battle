using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс кнопки для PageControl'а
    internal sealed class VCPageButton : VCIconButton
    {
        public VCPageButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            UseFilter = false;
            HighlightUnderMouse = true;
            ShowBorder = true;

            Page = new VisualControl(parent, 0, NextTop());
            Page.Visible = false;
            Page.Click += Page_Click;
        }

        private void Page_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(null);
        }

        internal VisualControl Page { get; }
    }

    // Класс кнопок со страницами
    internal sealed class VCPageControl : VisualControl
    {
        private readonly List<VCPageButton> listFormPage = new List<VCPageButton>();
        private int nextLeft;
        private VCPageButton currentPage;

        public VCPageControl(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
        }

        internal BitmapList BitmapList { get; }

        internal VCPageButton AddPage(int imageIndex, EventHandler onShowHint)
        {
            Debug.Assert(onShowHint != null);

            VCPageButton page = new VCPageButton(this, nextLeft, 0, BitmapList, imageIndex);
            page.Click += Page_Click;
            page.ShowHint += onShowHint;
            nextLeft = page.NextLeft();
            listFormPage.Add(page);

            return page;
        }

        private void Page_Click(object sender, EventArgs e)
        {
            ActivatePage((VCPageButton)sender);
        }

        internal override void ApplyMaxSize()
        {
            foreach (VCPageButton p in listFormPage)
            {
                p.Page.ApplyMaxSize();
            }

            base.ApplyMaxSize();
        }

        internal void ActivatePage(VCPageButton pc)
        {
            if (pc != currentPage)
            {
                if (currentPage != null)
                {
                    currentPage.ManualSelected = false;
                    currentPage.Page.Visible = false;
                }
                currentPage = pc;
                currentPage.ManualSelected = true;
                currentPage.Page.Visible = true;

                Program.formMain.SetNeedRedrawFrame();
            }
        }
    }
}
