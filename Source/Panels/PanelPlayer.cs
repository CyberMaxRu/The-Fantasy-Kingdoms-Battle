using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VCCell
    {
        private Player player;

        public PanelPlayer(VisualControl parent, int shiftX) : base(parent, shiftX, 0)
        {
            HighlightUnderMouse = true;
        }

        internal void LinkToLobby(Player p)
        {
            Debug.Assert(p != null);

            player = p;
            player.Panel = this;
            ShowCell(player);
        }

        internal override void Draw(Graphics g)
        {
            ImageFilter = Program.formMain.CurrentLobby.CurrentPlayer == player ? ImageFilter.Active : ImageFilter.Press;

            base.Draw(g);
        }

        protected override bool Selected() => Program.formMain.CurrentLobby.CurrentPlayer == player;
    }
}
