using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal sealed class PanelMenu : Control
    {
        internal readonly Bitmap bmpMenu;

        private const int DISTANCE_BETWEEN_CELLS = 3;

        public PanelMenu(Control parent, string dirResources) : base()
        {
            Parent = parent;
            bmpMenu = new Bitmap(dirResources + "Icons\\Menu.png");
            Size = bmpMenu.Size;

            DoubleBuffered = true;

            CellsMenu = new PanelCellMenu[FormMain.PANEL_RESEARCH_SIZE.Height, FormMain.PANEL_RESEARCH_SIZE.Width];
            for (int y = 0; y < FormMain.PANEL_RESEARCH_SIZE.Height; y++)
                for (int x = 0; x < FormMain.PANEL_RESEARCH_SIZE.Width; x++)
                    CellsMenu[y, x] = new PanelCellMenu(this, DISTANCE_BETWEEN_CELLS + (x * (Program.formMain.ilItems.ImageSize.Width + DISTANCE_BETWEEN_CELLS)), DISTANCE_BETWEEN_CELLS + (y * (Program.formMain.ilItems.ImageSize.Height + DISTANCE_BETWEEN_CELLS)));
        }

        internal PanelCellMenu[,] CellsMenu { get; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            // Рисууем подложку
            e.Graphics.DrawImageUnscaled(bmpMenu, 0, 0);
        }
    }
}
