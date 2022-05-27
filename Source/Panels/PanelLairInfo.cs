using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс подробной информации о логове
    internal sealed class PanelLairInfo : PanelBaseInfo
    {
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelHeroes;
        private readonly VCTabButton btnInhabitants;
        private readonly VCTabButton btnHeroes;

        public PanelLairInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelInhabitants = new PanelWithPanelEntity(4);
            panelHeroes = new PanelWithPanelEntity(4);

            //..separator.ShiftY = TopForControls();
            //pageControl.ShiftY = TopForControls();
            btnInhabitants = pageControl.AddTab("Существа", FormMain.Config.Gui48_Home, panelInhabitants);
            btnHeroes = pageControl.AddTab("Герои", FormMain.Config.Gui48_Target, panelHeroes);
            pageControl.AddTab("История", FormMain.Config.Gui48_Book, null);

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal Construction Lair { get => Entity as Construction; }

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
            Debug.Assert(!Lair.Destroyed);

            if (Lair.ComponentObjectOfMap.Visible)
            {
                panelInhabitants.ApplyList(Lair.Monsters);
                btnInhabitants.Quantity = Lair.Monsters.Count;
            }
            else
            {
                panelInhabitants.SetUnknownList();
                btnInhabitants.Quantity = 0;
            }

            panelHeroes.ApplyList(Lair.ComponentObjectOfMap.ListHeroesForFlag);
            btnHeroes.Quantity = Lair.ComponentObjectOfMap.ListHeroesForFlag.Count;

            base.Draw(g);
        }

        protected override int GetImageIndex() => Lair.GetImageIndex();
        protected override bool ImageIsEnabled() => true;
        protected override string GetCaption() => Lair.GetName();
    }
}
