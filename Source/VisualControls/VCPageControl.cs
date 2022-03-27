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
        private int nextLeft;

        public VCPageControl(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            
        }

        internal VCPageButton CurrentPage { get; private set; }
        internal event EventHandler PageChanged;
        internal List<VCPageButton> Pages { get; } = new List<VCPageButton>();

        internal VCPageButton AddPage(int imageIndex, string caption, string advice, EventHandler onShowHint)
        {
            VCPageButton page = new VCPageButton(this, nextLeft, 0, imageIndex, caption, advice, null);
            page.Click += Page_Click;
            if (onShowHint != null)
                page.ShowHint += onShowHint;
            nextLeft = page.NextLeft();
            Pages.Add(page);

            return page;
        }

        internal VCPageButton AddPage(TypeLobbyLocationSettings layer)
        {
            VCPageButton page = new VCPageButton(this, nextLeft, 0, layer.TypeLandscape.ImageIndex, layer.Name, "", layer);
            page.Click += Page_Click;
            page.Hint = layer.TypeLandscape.Description;
            nextLeft = page.NextLeft();
            Pages.Add(page);

            return page;
        }

        internal void Separate()
        {
            nextLeft += FormMain.Config.GridSize * 3;
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
            foreach (VCPageButton p in Pages)
            {
                p.Page.ApplyMaxSize();

                maxWidth = Math.Max(maxWidth, p.Page.Width);
                maxHeight = Math.Max(maxHeight, p.Page.Height);
            }

            foreach (VCPageButton p in Pages)
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
                    CurrentPage.SelectedPlayerObject = Program.formMain.layerGame.selectedPlayerObject;
                }
                CurrentPage = pc;
                CurrentPage.ManualSelected = true;
                CurrentPage.Page.Visible = true;

                if ((CurrentPage.Location != null) && (CurrentPage.SelectedPlayerObject is null))
                    CurrentPage.SelectedPlayerObject = CurrentPage.Location;

                Program.formMain.layerGame?.SelectPlayerObject(CurrentPage.SelectedPlayerObject);

                Program.formMain.SetNeedRedrawFrame();

                PageChanged?.Invoke(this, new EventArgs());
            }
        }


        internal void ClearSelectedObjects()
        {
            foreach (VCPageButton p in Pages)
            {
                if (p.SelectedPlayerObject != null)
                    p.SelectedPlayerObject = null;
            }
        }
    }
}
