using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VCCell
    {
        private LobbyPlayer player;

        public PanelPlayer(VisualControl parent, int shiftX) : base(parent, shiftX, 0)
        {
            
        }

        internal void LinkToLobby(LobbyPlayer p)
        {
            Debug.Assert(p != null);

            player = p;
            player.Panel = this;
            Entity = player;
        }

        protected override bool Selected() => Program.formMain.CurrentLobby?.CurrentPlayer == player;
    }
}
