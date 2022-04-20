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
        private readonly VCLabel lblNameTypeLobby;
        private LobbySettings lobbySettings;
        private VCLinePlayerTournament[] lines;
        private readonly VisualControl panelPlayers;
        private readonly VisualControl panelSettings;
        private readonly VCIconButton48 btnLocation;
        private readonly VCLabel lblNameLocation;
        private readonly VCButton btnDefault;

        public WindowSetupTournament(LobbySettings ls) : base("Настройка Войны лордов")
        {
            lobbySettings = ls;

            btnOk.Caption = "ОК";
            btnCancel.Caption = "Отмена";


            lblNameTypeLobby = new VCLabel(ClientControl, 0, 0, Program.formMain.fontMedCaption, Color.Turquoise, 24, ls.TypeLobby.Name);
            lblNameTypeLobby.Width = ClientControl.Width;

            panelPlayers = new VisualControl(ClientControl, 0, lblNameTypeLobby.NextTop());
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
            panelSettings = new VisualControl(ClientControl, panelPlayers.NextLeft(), panelPlayers.ShiftY);
            panelSettings.ShowBorder = true;

            btnLocation = new VCIconButton48(panelSettings, FormMain.Config.GridSize, FormMain.Config.GridSize, -1);
            btnLocation.Click += BtnLocation_Click;
            btnLocation.Tag = lobbySettings.TypeLandscape != null ? lobbySettings.TypeLandscape.Index : -1;
            lblNameLocation = new VCLabel(panelSettings, btnLocation.NextLeft(), btnLocation.ShiftY, Program.formMain.fontSmall, Color.White, btnLocation.Height, "");
            lblNameLocation.StringFormat.Alignment = StringAlignment.Near;
            lblNameLocation.StringFormat.LineAlignment = StringAlignment.Center;
            lblNameLocation.Width = 24;
            VCLabel lblGold = new VCLabel(panelSettings, FormMain.Config.GridSize, btnLocation.NextTop(), Program.formMain.fontSmall, Color.White, 16, $"Золота на старте: {ls.TypeLobby.BaseResources.ValueGold()}");
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

            btnDefault = new VCButton(ClientControl, 0, btnOk.ShiftY, "По умолчанию");
            btnDefault.Width = 184;
            btnDefault.Click += BtnDefault_Click;

            //
            ClientControl.Width = panelSettings.EndLeft();
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

            UpdateNameLocation();
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            lobbySettings.SetDefault();

            //
            foreach (VCLinePlayerTournament l in lines)
            {
                l.UpdateData();
            }

            UpdateNameLocation();
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width - FormMain.Config.GridSize - btnCancel.Width) / 2;
            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
            btnCancel.ShiftX = btnOk.NextLeft();
            lblNameLocation.Width = lblNameLocation.Parent.Width - lblNameLocation.ShiftX - FormMain.Config.GridSize;
            lblNameTypeLobby.Width = ClientControl.Width;
        }

        private void BtnLocation_Click(object sender, EventArgs e)
        {
            if (btnLocation.Tag == FormMain.Descriptors.TypeLandscapes.Count - 1)
                btnLocation.Tag = -1;
            else
                btnLocation.Tag++;

            lobbySettings.TypeLandscape = btnLocation.Tag == -1 ? null : FormMain.Descriptors.TypeLandscapes[btnLocation.Tag];

            UpdateNameLocation();
        }

        private void UpdateNameLocation()
        {
            if (lobbySettings.TypeLandscape is null)
            {
                btnLocation.ImageIndex = FormMain.Config.Gui48_RandomSelect;
                lblNameLocation.Text = "Случайная";
            }
            else
            {
                btnLocation.ImageIndex = lobbySettings.TypeLandscape.ImageIndex;
                lblNameLocation.Text = lobbySettings.TypeLandscape.Name;
            }
        }
    }
}
