using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowSetupTournament : WindowOkCancel
    {
        private LobbySettings lobbySettings;
        private VCLinePlayerTournament[] lines;

        public WindowSetupTournament(LobbySettings ls) : base("Настройка турнира")
        {
            lobbySettings = ls;

            btnOk.Caption = "ОК";
            btnCancel.Caption = "Отмена";

            int nextTop = 0;
            lines = new VCLinePlayerTournament[ls.TypeLobby.QuantityPlayers];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = new VCLinePlayerTournament(ClientControl, 0, nextTop, ls.Players[i]);

                nextTop = lines[i].NextTop();
            }


            btnOk.ShiftY = nextTop + FormMain.Config.GridSize;
            btnCancel.ShiftY = btnOk.ShiftY;

            //
            ClientControl.Width = lines[0].Width;
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
        }
    }
}
