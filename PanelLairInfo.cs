using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс подробной информации о логове
    internal sealed class PanelLairInfo : PanelBaseInfo
    {
        private PlayerLair lair;
        private readonly PanelWithPanelEntity panelInhabitants = new PanelWithPanelEntity(4);

        public PanelLairInfo(int height) : base(height)
        {
            pageControl.Top = pageControl.Top;
            pageControl.AddPage("Существа", (int)IconPages.Inhabitants, panelInhabitants);
            pageControl.AddPage("История", (int)IconPages.History, null);

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

        internal override void ShowData()
        {
            base.ShowData();

            //lblIcon.Text = (lair.Lair.MaxLevel > 1) && (building.Level > 0) ? building.Level.ToString() : "";

            panelInhabitants.ApplyList(lair.Monsters);

            Show();
        }

        protected override ImageList GetImageList() => Program.formMain.ilLairs;
        protected override int GetImageIndex() => GuiUtils.GetImageIndexWithGray(Program.formMain.ilLairs, lair.Lair.ImageIndex, true);
        protected override string GetCaption() => lair.Lair.Name;
    }
}
