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
        private readonly List<VCPageButton> listFormPage = new List<VCPageButton>();
        private int nextLeft;

        public VCPageControl(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            
        }

        internal VCPageButton CurrentPage { get; private set; }
        internal event EventHandler PageChanged;

        internal VCPageButton AddPage(int imageIndex, string advice, EventHandler onShowHint)
        {
            Debug.Assert(onShowHint != null);

            VCPageButton page = new VCPageButton(this, nextLeft, 0, imageIndex, advice);
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
            // Приводим страницы к единому максимальному размеру
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (VCPageButton p in listFormPage)
            {
                p.Page.ApplyMaxSize();

                maxWidth = Math.Max(maxWidth, p.Page.Width);
                maxHeight = Math.Max(maxHeight, p.Page.Height);
            }

            foreach (VCPageButton p in listFormPage)
            {
                p.Page.Width = maxWidth;
                p.Page.Height = maxHeight;
            }

            base.ApplyMaxSize();
        }

        internal void ActivatePage(VCPageButton pc)
        {
            if (pc != CurrentPage)
            {
                if (CurrentPage != null)
                {
                    CurrentPage.ManualSelected = false;
                    CurrentPage.Page.Visible = false;
                    CurrentPage.SelectedPlayerObject = Program.formMain.selectedPlayerObject;
                }
                CurrentPage = pc;
                CurrentPage.ManualSelected = true;
                CurrentPage.Page.Visible = true;
                Program.formMain.SelectPlayerObject(CurrentPage.SelectedPlayerObject);

                Program.formMain.SetNeedRedrawFrame();

                PageChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
