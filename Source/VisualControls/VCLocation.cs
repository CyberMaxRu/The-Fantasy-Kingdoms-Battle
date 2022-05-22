using System;
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
        private readonly VCText nameLocation;
        private readonly List<VCCell> listCells;
        private readonly VCIconAndDigitValue lblScouted;
        private readonly VCIconAndDigitValue lblDanger;

        private Location location;

        public VCLocation(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;

            imgTypeLocation = new VCImage128(this, FormMain.Config.GridSize, FormMain.Config.GridSize);
            imgTypeLocation.Click += ImgTypeLocation_Click;
            imgTypeLocation.PlaySoundOnClick = true;
            nameLocation = new VCText(imgTypeLocation, 4, 8, Program.formMain.fontMedCaptionC, Color.White, imgTypeLocation.Width - 8);
            nameLocation.IsActiveControl = false;

            lblScouted = new VCIconAndDigitValue(this, imgTypeLocation.NextLeft(), imgTypeLocation.ShiftY, 80, 42);
            lblScouted.ShowHint += LblScouted_ShowHint;
            lblDanger = new VCIconAndDigitValue(this, imgTypeLocation.NextLeft(), lblScouted.NextTop(), 80, 39);
            lblDanger.ShowHint += LblDanger_ShowHint;

            listCells = new List<VCCell>();

            Width = 200;
            Height = imgTypeLocation.NextTop();
            Click += VCLocation_Click;
        }

        private void VCLocation_Click(object sender, EventArgs e)
        {
            Program.formMain.layerGame.SelectPlayerObject(Location);
        }

        private void ImgTypeLocation_Click(object sender, EventArgs e)
        {
            Program.formMain.layerGame.SelectPlayerObject(Location);
        }

        private void LblDanger_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddSimpleHint("Уровень опасности");
        }

        private void LblScouted_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddSimpleHint("Разведано");
        }

        internal Location Location { get => location; set { location = value; UpdateLocation(); } }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            for (int i = 0; i < listCells.Count; i++)
                ValidateCoordCell(listCells[i], i);
        }

        private void UpdateLocation()
        {
            imgTypeLocation.ImageIndex = location.Settings.TypeLandscape.ImageIndex;
            nameLocation.Text = location.Settings.Name;
            nameLocation.Height = nameLocation.MinHeigth();
            nameLocation.ShiftY = imgTypeLocation.Height - nameLocation.Height;
            imgTypeLocation.ArrangeControl(nameLocation);

            lblScouted.Text = Utils.FormatPercent(location.ScoutedArea);
            lblDanger.Text = Utils.FormatPercent(location.Danger);

            // Не всегда все объекты видны. Тем не менее, создадим заранее под них ячейки - пусть будут, все равно пригодятся
            while (listCells.Count < Location.Lairs.Count)
            {
                VCCell cell = new VCCell(this, 0, 0);
                cell.PlaySoundOnClick = true;
                ValidateCoordCell(cell, listCells.Count);
                listCells.Add(cell);
            }

            // Скрываем все ячейки
            foreach (VCCell c in listCells)
                c.Visible = false;

            int nextCell = 0;
            for (int i = 0; i < location.Lairs.Count; i++)
            {
                if (location.Lairs[i].Visible)
                {
                    listCells[nextCell].Entity = location.Lairs[i];
                    listCells[nextCell].Visible = true;
                    nextCell++;
                }
            }
        }

        private void ValidateCoordCell(VCCell cell, int index)
        {
            int cellsPerLine = (Width - lblScouted.NextLeft() - FormMain.Config.GridSize) / 56;
            int line = index / cellsPerLine;
            int offset = index % cellsPerLine;
            cell.ShiftX = lblScouted.NextLeft() + (offset * 56);
            cell.ShiftY = imgTypeLocation.ShiftY + (line * 56);

            ArrangeControl(cell);
        }

        protected override bool Selected()
        {
            return Program.formMain.layerGame.PlayerObjectIsSelected(location);
        }
    }
}
