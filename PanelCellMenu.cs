using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели исследования
    internal sealed class PanelCellMenu : Control
    {
        private PlayerResearch research;
        public PanelCellMenu(Control parent, int left, int top)
        {
            Parent = parent;
            Left = left;
            Top = top;
            Size = Program.formMain.ilItems.ImageSize;
            DoubleBuffered = true;
        }

        internal PlayerResearch Research { get { return research; }
            set
            {
                research = value;

                Visible = research != null;
            }
        }// Исследование

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Research != null)
            {
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                // Иконка исследования
                e.Graphics.DrawImageUnscaled(Program.formMain.ilItems.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilItems, Research.Research.Item.ImageIndex, true)], 0, 0);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (research != null)
            {
                Program.formMain.formHint.ShowHint(new Point(8 + Parent.Left + Left, Parent.Top + Top + Height + 2),
                    research.Research.Item.Name,
                    "",
                    research.Research.Item.Description,
                    null,
                    research.Cost(),
                    research.Cost() <= research.Building.Player.Gold,
                    0,
                    0, false, null);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Program.formMain.formHint.HideHint();
        }
    }
}
