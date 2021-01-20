using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VisualControl
    {
        private Player player;
        private readonly PanelEntity panelAvatar;
        private Rectangle rectBorder;
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);

        public PanelPlayer(VisualControl parent, Point shift) : base(parent, shift)
        {
            panelAvatar = new PanelEntity(this, new Point(FormMain.Config.GridSize, FormMain.Config.GridSize));

            Width = panelAvatar.Width + (FormMain.Config.GridSize * 2);
            Height = panelAvatar.Height + (FormMain.Config.GridSize * 2);
        }

        internal Player Player
        {
            get { return player; }
            set { Debug.Assert(value != null); player = value; player.Panel = this; panelAvatar.ShowCell(player); }
        }

        protected override void ArrangeControls()
        {
            if ((rectBorder.Left != Left) || (rectBorder.Top != Top) || (rectBorder.Width != Width - 1) || (rectBorder.Height != Height - 1))
                rectBorder = new Rectangle(Left, Top, Width - 1, Height - 1);
        }

        internal override void Draw(Graphics g, int x, int y)
        {
            // Рамка вокруг панели
            penBorder.Color = FormMain.Config.ColorBorderPlayer(player);
            g.DrawRectangle(penBorder, rectBorder);

            // Аватар игрока
            panelAvatar.Left = x + panelAvatar.ShiftOnParent.X;
            panelAvatar.Top = y + panelAvatar.ShiftOnParent.Y;
            panelAvatar.Draw(g, panelAvatar.Left, panelAvatar.Top);
        }
    }
}
