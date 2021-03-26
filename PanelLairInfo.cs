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
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelHeroes;

        public PanelLairInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelInhabitants = new PanelWithPanelEntity(4);
            panelHeroes = new PanelWithPanelEntity(4);

            //..separator.ShiftY = TopForControls();
            //pageControl.ShiftY = TopForControls();
            pageControl.AddTab("Существа", FormMain.GUI_HOME, panelInhabitants);
            pageControl.AddTab("Герои", FormMain.GUI_TARGET, panelHeroes);
            pageControl.AddTab("История", FormMain.GUI_BOOK, null);

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal PlayerLair Lair { get => PlayerObject as PlayerLair; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
        }

        internal void SelectPageInhabitants()
        {
            pageControl.ActivatePage(0);
        }

        internal void SelectPageHeroes()
        {
            pageControl.ActivatePage(1);
        }

        internal override void Draw(Graphics g)
        {
            if (Lair.Hidden)
                panelInhabitants.SetUnknownList();
            else
                panelInhabitants.ApplyList(Lair.Monsters);

            base.Draw(g);
        }

        protected override int GetImageIndex() => Lair.ImageIndexLair();
        protected override bool ImageIsEnabled() => true;
        protected override string GetCaption() => Lair.NameLair();
    }
}
