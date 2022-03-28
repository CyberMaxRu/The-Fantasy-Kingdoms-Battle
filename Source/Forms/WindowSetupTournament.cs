using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;    

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowSetupTournament : WindowOkCancel
    {
        private LobbySettings lobbySettings;
        private VCLinePlayerTournament[] lines;
        private readonly VisualControl panelPlayers;
        private readonly VisualControl panelSettings;

        public WindowSetupTournament(LobbySettings ls) : base("Настройка турнира")
        {
            lobbySettings = ls;

            btnOk.Caption = "ОК";
            btnCancel.Caption = "Отмена";

            panelPlayers = new VisualControl(ClientControl, 0, 0);
            panelPlayers.ShowBorder = true; 

            int nextTop = FormMain.Config.GridSize;
            lines = new VCLinePlayerTournament[ls.TypeLobby.QuantityPlayers];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = new VCLinePlayerTournament(panelPlayers, FormMain.Config.GridSize, nextTop, ls.Players[i]);

                nextTop = lines[i].NextTop();
            }

            panelPlayers.ApplyMaxSize();
            panelPlayers.Width += FormMain.Config.GridSize;
            panelPlayers.Height += FormMain.Config.GridSize;

            btnOk.ShiftY = panelPlayers.NextTop() + FormMain.Config.GridSize;
            btnCancel.ShiftY = btnOk.ShiftY;

            // 
            panelSettings = new VisualControl(ClientControl, panelPlayers.NextLeft(), 0);
            panelSettings.ShowBorder = true;
            VCLabel lblGold = new VCLabel(panelSettings, FormMain.Config.GridSize, FormMain.Config.GridSize, Program.formMain.fontSmall, Color.White, 16, $"Золота на старте: {ls.TypeLobby.BaseResources.ValueGold()}");
            lblGold.SetWidthByText();
            VCLabel lblMaxHeroes = new VCLabel(panelSettings, FormMain.Config.GridSize, lblGold.NextTop(), Program.formMain.fontSmall, Color.White, 16, $"Максимум героев: {ls.TypeLobby.MaxHeroes}");
            lblMaxHeroes.SetWidthByText();
            VCLabel lblDayStartBattles = new VCLabel(panelSettings, FormMain.Config.GridSize, lblMaxHeroes.NextTop(), Program.formMain.fontSmall, Color.White, 16, $"Ход начала битв между игроками: {ls.TypeLobby.DayStartBattleBetweenPlayers}");
            lblDayStartBattles.SetWidthByText();
            VCLabel lblDaysBetweenBattles = new VCLabel(panelSettings, FormMain.Config.GridSize, lblDayStartBattles.NextTop(), Program.formMain.fontSmall, Color.White, 16, $"Ходов перед следующей битвой: {ls.TypeLobby.DaysBeforeNextBattleBetweenPlayers}");
            lblDaysBetweenBattles.SetWidthByText();
            VCLabel lblMaxLoses = new VCLabel(panelSettings, FormMain.Config.GridSize, lblDaysBetweenBattles.NextTop(), Program.formMain.fontSmall, Color.White, 16, $"Максимум поражений: {ls.TypeLobby.MaxLoses}");
            lblMaxLoses.SetWidthByText();

            panelSettings.ApplyMaxSize();
            panelSettings.Width += FormMain.Config.GridSize;
            panelSettings.Height = panelPlayers.Height;

            //
            ClientControl.Width = panelSettings.EndLeft();
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width - FormMain.Config.GridSize - btnCancel.Width) / 2;
            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
            btnCancel.ShiftX = btnOk.NextLeft();
        }
    }
}
