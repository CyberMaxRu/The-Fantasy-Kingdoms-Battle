using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VisualControl
    {
        private Player player;
        private readonly PanelEntity panelAvatar;

        public PanelPlayer(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelAvatar = new PanelEntity(this, FormMain.Config.GridSize, FormMain.Config.GridSize);

            Width = panelAvatar.Width + (FormMain.Config.GridSize * 2);
            Height = panelAvatar.Height + (FormMain.Config.GridSize * 2);
            ShowBorder = true;
        }

        internal void LinkToLobby(Player p)
        {
            Debug.Assert(p != null);

            player = p;
            player.Panel = this;
            panelAvatar.ShowCell(player);
        }

        internal override void Draw(Graphics g)
        {
            SetColorBorder(FormMain.Config.ColorBorderPlayer(player));

            base.Draw(g);
        }
    }
}
