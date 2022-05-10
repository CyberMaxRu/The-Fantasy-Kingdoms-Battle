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

        private Location location;

        public VCLocation(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;

            imgTypeLocation = new VCImage128(this, FormMain.Config.GridSize, FormMain.Config.GridSize);
            nameLocation = new VCText(imgTypeLocation, 4, 8, Program.formMain.fontMedCaptionC, Color.White, imgTypeLocation.Width - 8);

            listCells = new List<VCCell>();

            Width = 200;
            Height = imgTypeLocation.NextTop();
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

            while (listCells.Count < Location.Lairs.Count)
            {
                VCCell cell = new VCCell(this, 0, 0);
                ValidateCoordCell(cell, listCells.Count);
                listCells.Add(cell);
            }

            for (int i = 0; i < location.Lairs.Count; i++)
            {
                listCells[i].Entity = location.Lairs[i];
            }
        }

        private void ValidateCoordCell(VCCell cell, int index)
        {
            int cellsPerLine = (Width - imgTypeLocation.NextLeft() - FormMain.Config.GridSize) / 56;
            int line = index / cellsPerLine;
            int offset = index % cellsPerLine;
            cell.ShiftX = imgTypeLocation.NextLeft() + (offset * 56);
            cell.ShiftY = imgTypeLocation.ShiftY + (line & 56);

            ArrangeControl(cell);
        }
    }
}
