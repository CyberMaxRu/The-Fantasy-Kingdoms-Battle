using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс подробной информации о строении
    internal sealed class PanelBuildingInfo : PanelBaseInfo
    {
        private VCLabel lblGold;
        private readonly PanelWithPanelEntity panelProducts;
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelWarehouse;

        public PanelBuildingInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelProducts = new PanelWithPanelEntity(4);
            panelInhabitants = new PanelWithPanelEntity(4);
            panelWarehouse = new PanelWithPanelEntity(4);

            lblGold = new VCLabel(this, FormMain.Config.GridSize, TopForControls(), FormMain.Config.FontCost, Color.White, 16, "");
            lblGold.Width = 80;
            lblGold.BitmapList = Program.formMain.ilGui16;
            lblGold.ImageIndex = FormMain.GUI_16_GOLD;

            pageControl.ShiftY = lblGold.NextTop();
            pageControl.AddTab("Товары", FormMain.GUI_GOODS, panelProducts);
            pageControl.AddTab("Склад", FormMain.GUI_INVENTORY, panelWarehouse);
            pageControl.AddTab("Жители", FormMain.GUI_HOME, panelInhabitants);
            pageControl.AddTab("История", FormMain.GUI_BOOK, null);

            pageControl.ApplyMinWidth();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal PlayerBuilding Building { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
        }

        protected override PlayerObject GetPlayerObject()
        {
            return Building;
        }

        internal override void Draw(Graphics g)
        {
            imgIcon.Level = (Building.Building.MaxLevel > 1) && (Building.Level > 0) ? Building.Level : 0;

            if (Building.Building.HasTreasury)
            {
                lblGold.Visible = true;
                lblGold.Text = Building.Gold.ToString();
            }
            else
            {
                lblGold.Visible = false;
            }

            //pageControl.SetPageVisible(1, building.Building.TrainedHero != null);
            //pageControl.SetPageVisible(2, building.Building.TrainedHero != null);

            panelProducts.ApplyList(Building.Items);
            panelWarehouse.ApplyList(Building.Warehouse);
            panelInhabitants.ApplyList(Building.Heroes);

            base.Draw(g); 
        }

        protected override int GetImageIndex() => Building.Building.ImageIndex;
        protected override bool ImageIsEnabled() => Building.Level > 0;
        protected override string GetCaption() => Building.Building.Name;
    }
}
