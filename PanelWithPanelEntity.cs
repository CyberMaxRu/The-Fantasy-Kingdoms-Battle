using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели с расположенными на ней панелями сущностей
    internal sealed class PanelWithPanelEntity : VisualControl
    {
        private List<PanelEntity> panelEntities = new List<PanelEntity>();
        private int rows;// Сколько сейчас строк подготовлено

        public PanelWithPanelEntity(int entityInRow) : base()
        {
            //DoubleBuffered = true;
            //BackColor = Color.Transparent;
            EntityInRow = entityInRow;

            while (rows < FormMain.Config.MinRowsEntities)
                AddRow();

            Width = (panelEntities[0].Width + 1) * EntityInRow - 1;
            Height = (panelEntities[0].Height + 1) * FormMain.Config.MinRowsEntities - 1;
        }

        private int EntityInRow { get; }

        internal void ApplyList<T>(List<T> list) where T: ICell
        {
            ValidateRows(list.Count);

            for (int i = 0; i < panelEntities.Count; i++)
            {
                if (i < list.Count)
                    panelEntities[i].ShowCell(list[i]);
                else
                    panelEntities[i].ShowCell(null);
            }
        }

        private void ValidateRows(int count)
        {
            // Определяем необходимое количество строк. Лишние удаляем, необходимые создаем
            int needRows = Math.Max(count / EntityInRow + (count % EntityInRow == 0 ? 0 : 1), FormMain.Config.MinRowsEntities);

            while (rows > needRows)
                RemoveRow();

            while (rows < needRows)
                AddRow();
        }

        private void AddRow()
        {
            Debug.Assert(panelEntities.Count % EntityInRow == 0);

            PanelEntity pe;

            for (int x = 0; x < EntityInRow; x++)
            {
                pe = new PanelEntity();
                AddControl(pe, new Point(x * (pe.Width + 1), rows * (pe.Height + 1)));
                //pe.Visible = false;
                panelEntities.Add(pe);
            }

            rows++;
            Height = panelEntities[panelEntities.Count - 1].Top + panelEntities[panelEntities.Count - 1].Height;
        }

        private void RemoveRow()
        {
            Debug.Assert(panelEntities.Count % EntityInRow == 0);

            PanelEntity pe;

            for (int x = 0; x < EntityInRow; x++)
            {
                pe = panelEntities[panelEntities.Count - 1];
                panelEntities.Remove(pe);
                //Controls.Remove(pe);
                //pe.Dispose();
            }

            rows--;
            Height = panelEntities[panelEntities.Count - 1].Top + panelEntities[panelEntities.Count - 1].Height;
        }

        internal override void Draw(Bitmap b, Graphics g, int x, int y)
        {
            foreach (KeyValuePair <VisualControl, Point> vc in Controls)
            {
                vc.Key.Draw(b, g, x + vc.Value.X, y + vc.Value.Y);
            }
        }
    }
}
