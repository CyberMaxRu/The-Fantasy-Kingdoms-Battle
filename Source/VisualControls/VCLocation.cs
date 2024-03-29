﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс отрисовки локации
    sealed internal class VCLocation : VisualControl
    {
        private readonly VCImage128 imgTypeLocation;
        private readonly VCProgressBar pbScout;
        private readonly List<VCCell> listCells;
        private readonly VCIconAndDigitValue lblDanger;

        private Location location;

        public VCLocation(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;

            imgTypeLocation = new VCImage128(this, FormMain.Config.GridSize, FormMain.Config.GridSize);
            imgTypeLocation.TextCaption.Color = Color.White;
            imgTypeLocation.ShowHint += ImgTypeLocation_ShowHint;
            imgTypeLocation.Click += ImgTypeLocation_Click;
            imgTypeLocation.PlaySoundOnClick = true;

            pbScout = new VCProgressBar(this, imgTypeLocation.ShiftX - 2, imgTypeLocation.NextTop() - 4);
            pbScout.Width = imgTypeLocation.Width + 4;
            pbScout.Max = 100;
            pbScout.ShowHint += PbScout_ShowHint;

            lblDanger = new VCIconAndDigitValue(this, imgTypeLocation.NextLeft(), imgTypeLocation.ShiftY, 72, 39);
            lblDanger.ShowHint += LblDanger_ShowHint;

            listCells = new List<VCCell>();

            Width = 192;
            Width = lblDanger.NextLeft();
            Height = pbScout.NextTop();
            Click += VCLocation_Click;
        }

        private void PbScout_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Entity(location);
            PanelHint.AddStep5Description("Разведано: " + Utils.FormatPercent(location.PercentScoutedArea)
                + (location.PercentScoutedArea == 1000 ? "" :
                    Environment.NewLine + (location.PercentScoutAreaToday > 0 ? "Будет разведано в этот день: +" + Utils.FormatPercent(location.PercentScoutAreaToday) : "Разведки на этом ходу нет")));
        }

        private void ImgTypeLocation_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Entity(location);
        }

        private void VCLocation_Click(object sender, EventArgs e)
        {
            Program.formMain.layerGame.SelectPlayerObject(Location);
        }

        private void ImgTypeLocation_Click(object sender, EventArgs e)
        {
            Program.formMain.layerGame.SelectPlayerObject(Location, -1, true);
        }

        private void LblDanger_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddSimpleHint("Уровень опасности");
        }

        internal Location Location { get => location; set { location = value; UpdateLocation(); } }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            return;
            for (int i = 0; i < listCells.Count; i++)
                ValidateCoordCell(listCells[i], i);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            UpdateLocation();
        }

        private void UpdateLocation()
        {
            imgTypeLocation.ImageIndex = location.GetImageIndex();
            imgTypeLocation.TextCaption.Text = location.Settings.Name;

            pbScout.Position = location.PercentScoutedArea / 10;
            pbScout.Text = Utils.FormatPercent(location.PercentScoutedArea) + (location.PercentScoutAreaToday > 0 ? $"+{Utils.FormatPercent(location.PercentScoutAreaToday)}" : "");
            lblDanger.Text = Utils.FormatPercent(location.Danger);

            return;
            // Не всегда все объекты видны. Тем не менее, создадим заранее под них ячейки - пусть будут, все равно пригодятся
            while (listCells.Count < Location.Lairs.Count)
            {
                VCCell cell = new VCCell(this, 0, 0);
                cell.PlaySoundOnClick = true;
                ValidateCoordCell(cell, listCells.Count);
                listCells.Add(cell);
            }

            int nextCell = 0;
            for (int i = 0; i < location.Lairs.Count; i++)
            {
                if (location.Lairs[i].ComponentObjectOfMap.Visible)
                {
                    listCells[nextCell].Entity = location.Lairs[i];
                    listCells[nextCell].Visible = true;
                    nextCell++;
                }
            }

            // Скрываем оставшиеся
            for (int i = nextCell; i < listCells.Count; i++)
            {
                listCells[i].Visible = false;
                listCells[i].Entity = null;
            }
        }

        private void ValidateCoordCell(VCCell cell, int index)
        {
            int cellsPerLine = (Width - lblDanger.NextLeft() - FormMain.Config.GridSize) / 56;
            int line = index / cellsPerLine;
            int offset = index % cellsPerLine;
            cell.ShiftX = lblDanger.NextLeft() + (offset * 56);
            cell.ShiftY = imgTypeLocation.ShiftY + (line * 56);

            ArrangeControl(cell);
        }

        protected override bool Selected()
        {
            return Program.formMain.layerGame.PlayerObjectIsSelected(location);
        }
    }
}
