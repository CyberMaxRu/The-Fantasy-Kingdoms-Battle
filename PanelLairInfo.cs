using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс подробной информации о логове
    internal sealed class PanelLairInfo : PanelBaseInfo
    {
        private PlayerLair lair;
        private readonly PanelWithPanelEntity panelInhabitants;

        public PanelLairInfo(VisualControl parent, int shiftX, int shiftY, int height) : base(parent, shiftX, shiftY, height)
        {
            panelInhabitants = new PanelWithPanelEntity(4);

            pageControl.ShiftY = TopForControls();
            pageControl.AddTab("Существа", FormMain.GUI_HOME, panelInhabitants);
            pageControl.AddTab("История", FormMain.GUI_BOOK, null);

            pageControl.ApplyMinWidth();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal PlayerLair Lair
        {
            get { return lair ; }
            set
            {
                lair = value;
                ShowData();
            }
        }

        internal  void ShowData()
        {
            //base.ShowData();

            //lblIcon.Text = (lair.Lair.MaxLevel > 1) && (building.Level > 0) ? building.Level.ToString() : "";

            panelInhabitants.ApplyList(lair.Monsters);

            Visible = true;
            Program.formMain.ShowFrame();
        }

        protected override BitmapList GetBitmapList() => Program.formMain.imListObjectsBig;
        protected override int GetImageIndex() => lair.Lair.ImageIndex;
        protected override ImageState GetImageState() => ImageState.Normal;
        protected override string GetCaption() => lair.Lair.Name;
    }
}
