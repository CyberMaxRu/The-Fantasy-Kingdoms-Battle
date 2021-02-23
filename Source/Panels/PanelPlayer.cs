using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VisualControl
    {
        private Player player;
        private readonly VCCell panelAvatar;

        public PanelPlayer(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            panelAvatar = new VCCell(this, 0, 0);
            panelAvatar.HighlightUnderMouse = true;

            Width = panelAvatar.Width;
            Height = panelAvatar.Height;
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
            panelAvatar.Selected = Program.formMain.CurrentLobby.CurrentPlayer == player;
            panelAvatar.ImageFilter = panelAvatar.Selected ? ImageFilter.Active : ImageFilter.Press;

            base.Draw(g);
        }
    }
}
