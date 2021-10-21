using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PanelLocationInfo : PanelBaseInfo
    {
        public PanelLocationInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            pageControl.AddTab("История", FormMain.Config.Gui48_Book, null);

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal Location Location { get => Entity as Location; }

        protected override int GetImageIndex() => Location.Descriptor.ImageIndex;
        protected override bool ImageIsEnabled() => true;// Location.Level > 0;
        protected override string GetCaption() => Location.Descriptor.Name;

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
        }
    }
}
