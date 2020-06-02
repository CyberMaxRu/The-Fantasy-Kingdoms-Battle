using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели меню
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

            //
            CellsMenu = new PanelCellMenu[FormMain.PANEL_MENU_CELLS.Height, FormMain.PANEL_MENU_CELLS.Width];

            for (int y = 0; y < FormMain.PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < FormMain.PANEL_MENU_CELLS.Width; x++)
                    CellsMenu[y, x] = new PanelCellMenu(this, new Point(DISTANCE_BETWEEN_CELLS + (x * (Program.formMain.ilItems.ImageSize.Width + DISTANCE_BETWEEN_CELLS)), DISTANCE_BETWEEN_CELLS + (y * (Program.formMain.ilItems.ImageSize.Height + DISTANCE_BETWEEN_CELLS))));
        }

        internal PanelCellMenu[,] CellsMenu { get; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем подложку
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.DrawImageUnscaled(bmpMenu, 0, 0);
        }
    }
}
