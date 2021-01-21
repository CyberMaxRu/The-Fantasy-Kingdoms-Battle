using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VisualControl
    {
        private Player player;
        private readonly PanelEntity panelAvatar;
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private Rectangle rectBorder;

        public PanelPlayer(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelAvatar = new PanelEntity(this, FormMain.Config.GridSize, FormMain.Config.GridSize);

            Width = panelAvatar.Width + (FormMain.Config.GridSize * 2);
            Height = panelAvatar.Height + (FormMain.Config.GridSize * 2);
        }

        internal void LinkToLobby(Player p)
        {
            Debug.Assert(p != null);

            player = p;
            player.Panel = this;
            panelAvatar.ShowCell(player);
        }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            if ((rectBorder.Left != Left) || (rectBorder.Top != Top) || (rectBorder.Width != Width - 1) || (rectBorder.Height != Height - 1))
                rectBorder = new Rectangle(Left, Top, Width - 1, Height - 1);
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(rectBorder.Left == Left);
            Debug.Assert(rectBorder.Top == Top);
            Debug.Assert(rectBorder.Width == Width - 1);
            Debug.Assert(rectBorder.Height == Height - 1);

            // Рамка вокруг панели
            penBorder.Color = FormMain.Config.ColorBorderPlayer(player);
            g.DrawRectangle(penBorder, rectBorder);

            // Аватар игрока
            panelAvatar.Draw(g);
        }
    }
}
