using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VCCell
    {
        public PanelPlayer(VisualControl parent, int shiftX) : base(parent, shiftX, 0)
        {
            
        }

        protected override bool Selected() => Program.formMain.CurrentLobby?.CurrentPlayer == Entity;
    }
}
