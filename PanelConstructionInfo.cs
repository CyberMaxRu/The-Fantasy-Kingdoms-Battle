﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс подробной информации о сооружении
    internal sealed class PanelConstructionInfo : PanelBaseInfo
    {
        private VCLabelValue lblGold;
        private readonly PanelWithPanelEntity panelProducts;
        private readonly PanelWithPanelEntity panelInhabitants;
        private readonly PanelWithPanelEntity panelWarehouse;

        public PanelConstructionInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelProducts = new PanelWithPanelEntity(4);
            panelInhabitants = new PanelWithPanelEntity(4);
            panelWarehouse = new PanelWithPanelEntity(4);

            lblGold = new VCLabelValue(this, FormMain.Config.GridSize, TopForControls(), Color.White, true);
            lblGold.Width = imgIcon.Width;
            lblGold.StringFormat.Alignment = StringAlignment.Far;                 
            lblGold.BitmapList = Program.formMain.ilGui16;
            lblGold.ImageIndex = FormMain.GUI_16_GOLD;

            separator.ShiftY = lblGold.NextTop();
            pageControl.ShiftY = separator.NextTop();
            pageControl.AddTab("Товары", FormMain.GUI_GOODS, panelProducts);
            pageControl.AddTab("Склад", FormMain.GUI_INVENTORY, panelWarehouse);
            pageControl.AddTab("Жители", FormMain.GUI_HOME, panelInhabitants);
            pageControl.AddTab("История", FormMain.GUI_BOOK, null);

            pageControl.ApplyMinSize();
            Width = pageControl.Width + FormMain.Config.GridSize * 2;
        }

        internal PlayerConstruction Construction { get => PlayerObject as PlayerConstruction; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            pageControl.Height = Height - pageControl.ShiftY - FormMain.Config.GridSize;
        }

        internal override void Draw(Graphics g)
        {
            imgIcon.Level = (Construction.TypeConstruction.MaxLevel > 1) && (Construction.Level > 0) ? Construction.Level : 0;

            if (Construction.TypeConstruction.HasTreasury)
            {
                lblGold.Visible = true;
                lblGold.Text = Construction.Gold.ToString();
            }
            else
            {
                lblGold.Visible = false;
            }

            //pageControl.SetPageVisible(1, Construction.TypeConstruction.TrainedHero != null);
            //pageControl.SetPageVisible(2, Construction.TypeConstruction.TrainedHero != null);

            panelProducts.ApplyList(Construction.Items);
            panelWarehouse.ApplyList(Construction.Warehouse);
            panelInhabitants.ApplyList(Construction.Heroes);

            base.Draw(g); 
        }

        protected override int GetImageIndex() => Construction.TypeConstruction.ImageIndex;
        protected override bool ImageIsEnabled() => Construction.Level > 0;
        protected override string GetCaption() => Construction.TypeConstruction.Name;
    }
}