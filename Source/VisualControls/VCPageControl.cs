using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс кнопок со страницами
    internal sealed class VCPageControl : VisualControl
    {
        private readonly List<VCFormPage> listFormPage = new List<VCFormPage>();
        private int nextLeft;
        private VCFormPage currentPage;

        public VCPageControl(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
        }

        internal BitmapList BitmapList { get; }

        internal VCFormPage AddPage(int imageIndex, EventHandler onShowHint)
        {
            Debug.Assert(onShowHint != null);

            VCFormPage page = new VCFormPage(this, nextLeft, 0, BitmapList, imageIndex);
            page.Click += Page_Click;
            page.ShowHint += onShowHint;
            nextLeft = page.NextLeft();
            listFormPage.Add(page);

            return page;
        }

        private void Page_Click(object sender, EventArgs e)
        {
            ActivatePage((VCFormPage)sender);
        }

        internal override void ApplyMaxSize()
        {
            foreach (VCFormPage p in listFormPage)
            {
                p.Page.ApplyMaxSize();
            }

            base.ApplyMaxSize();
        }

        internal void ActivatePage(VCFormPage pc)
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
