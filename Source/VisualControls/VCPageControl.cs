using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс кнопок со страницами
    internal sealed class VCPageControl : VisualControl
    {
        private readonly List<VCFormPage> listFormPage = new List<VCFormPage>();
        private int nextLeft;

        public VCPageControl(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
        }

        internal BitmapList BitmapList { get; }

        internal VCFormPage AddPage(int imageIndex, string caption, EventHandler onClick)
        {
            VCFormPage page = new VCFormPage(this, nextLeft, 0, listFormPage, BitmapList, imageIndex, caption, onClick);
            nextLeft = page.NextLeft();

            return page;
        }

        internal override void ApplyMaxSize()
        {
            foreach (VCFormPage p in listFormPage)
            {
                p.Page.ApplyMaxSize();
            }

            base.ApplyMaxSize();
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

        }
    }
}
