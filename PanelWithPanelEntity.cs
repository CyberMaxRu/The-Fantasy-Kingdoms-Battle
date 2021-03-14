using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели с расположенными на ней панелями сущностей
    internal sealed class PanelWithPanelEntity : VisualControl
    {
        private List<VCCell> listCells = new List<VCCell>();
        private int rows;// Сколько сейчас строк подготовлено

        public PanelWithPanelEntity(int entityInRow, bool fixedMode = true, int distanceBetweenCells = 2, int distanceBetweenRows = 2) : base()
        {
            Debug.Assert(entityInRow > 1);

            FixedMode = fixedMode;
            EntityInRow = entityInRow;
            DistanceBetweenCells = distanceBetweenCells;
            DistanceBetweenRows = distanceBetweenRows;

            ValidateRows(FixedMode ? FormMain.Config.MinRowsEntities : 1);

            Width = (listCells[0].Width + DistanceBetweenCells) * (EntityInRow - 1) + listCells[0].Width;
            Height = (listCells[0].Height + DistanceBetweenCells) * (FormMain.Config.MinRowsEntities - 1) + listCells[0].Height;
        }

        private int EntityInRow { get; }
        internal bool FixedMode { get; }
        internal int DistanceBetweenCells { get; }
        internal int DistanceBetweenRows { get; }

        internal void ApplyList<T>(List<T> list) where T: ICell
        {
            ValidateRows(list.Count);

            for (int i = 0; i < listCells.Count; i++)
            {
                listCells[i].Visible = true;

                if (i < list.Count)
                    listCells[i].ShowCell(list[i]);
                else if (FixedMode)
                    listCells[i].ShowCell(null);
                else
                    listCells[i].Visible = false;
            }
        }

        internal void SetUnknownList()
        {
            ValidateRows(0);

            for (int i = 0; i < listCells.Count; i++)
            {
                listCells[i].ShowCell(null);
            }
        }

        private void ValidateRows(int count)
        {
            // Определяем необходимое количество строк. Лишние удаляем, необходимые создаем
            int needRows;
            if (FixedMode)
                needRows = Math.Max(count / EntityInRow + (count % EntityInRow == 0 ? 0 : 1), FormMain.Config.MinRowsEntities);
            else
                needRows = count / EntityInRow + (count % EntityInRow == 0 ? 0 : 1);

            while (rows > needRows)
                RemoveRow();

            while (rows < needRows)
                AddRow();
        }

        private void AddRow()
        {
            Debug.Assert(listCells.Count % EntityInRow == 0);

            VCCell pe;
            Point defPoint = new Point(0, 0);

            for (int x = 0; x < EntityInRow; x++)
            {
                pe = new VCCell(this, 0, 0);
                pe.ShiftX = x * (pe.Width + DistanceBetweenCells);
                pe.ShiftY = rows * (pe.Height + DistanceBetweenRows);
                pe.Visible = FixedMode;
                pe.DrawState = !FixedMode;
                ArrangeControl(pe);
                listCells.Add(pe);
            }

            rows++;
            Height = listCells[listCells.Count - 1].Top + listCells[listCells.Count - 1].Height;
        }

        private void RemoveRow()
        {
            Debug.Assert(listCells.Count % EntityInRow == 0);

            VCCell pe;

            for (int x = 0; x < EntityInRow; x++)
            {
                pe = listCells[listCells.Count - 1];
                listCells.Remove(pe);
                Controls.Remove(pe);
            }

            rows--;
            Height = listCells[listCells.Count - 1].Top + listCells[listCells.Count - 1].Height;
        }
    }
}
