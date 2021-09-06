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
            
        }

        internal void LinkToLobby(Player p)
        {
            Debug.Assert(p != null);

            player = p;
            Entity = player;
        }

        protected override bool Selected() => Program.formMain.CurrentLobby?.CurrentPlayer == player;
    }
}
