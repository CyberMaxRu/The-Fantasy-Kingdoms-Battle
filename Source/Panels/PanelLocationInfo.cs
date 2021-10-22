using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PanelLocationInfo : PanelBaseInfo
    {
        private VCCellSimple cellOwner;

        public PanelLocationInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            cellOwner = new VCCellSimple(this, imgIcon.NextLeft(), imgIcon.ShiftY);
            cellOwner.ShowHint += CellOwner_ShowHint;

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

        internal override void Draw(Graphics g)
        {
            cellOwner.ImageIndex = Location.Ownership ? Location.Player.GetImageIndexAvatar() : -1;

            base.Draw(g);
        }

        private void CellOwner_ShowHint(object sender, EventArgs e)
        {
            if (Location.Ownership)
            {
                Program.formMain.formHint.Clear();
                Program.formMain.formHint.AddStep2Header(Location.Player.GetName());
                Program.formMain.formHint.AddStep4Level("Владелец местности");
            }
            else
            {
                Program.formMain.formHint.Clear();
                Program.formMain.formHint.AddSimpleHint("У местности нет владельца");
            }
        }
    }
}
