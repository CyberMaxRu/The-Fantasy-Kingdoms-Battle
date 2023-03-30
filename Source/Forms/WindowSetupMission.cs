using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowSetupMission : WindowOkCancel
    {
        private LobbySettings mission;
        private readonly VCLabel lblNameTypeLobby;
        private VCLineSetupPlayerMission[] lines;
        private readonly VisualControl panelPlayers;
        private readonly VisualControl panelSettings;
        private readonly VCIconButton48 btnLocation;
        private readonly VCLabel lblNameLocation;

        public WindowSetupMission(LobbySettings ls) : base("Настройка миссии", false)
        {
            mission = ls;

            btnOk.Caption = "ОК";
            btnCancel.Caption = "Отмена";

            lblNameTypeLobby = new VCLabel(ClientControl, 0, 0, Program.formMain.FontMedCaption, Color.Turquoise, 24, ls.TypeLobby.Name);
            lblNameTypeLobby.Width = ClientControl.Width;

            panelPlayers = new VisualControl(ClientControl, 0, lblNameTypeLobby.NextTop());
            panelPlayers.ShowBorder = true;

            int nextTop = FormMain.Config.GridSize;
            lines = new VCLineSetupPlayerMission[ls.TypeLobby.QuantityPlayers];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = new VCLineSetupPlayerMission(panelPlayers, FormMain.Config.GridSize, nextTop, ls.Players[i]);

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
            btnLocation.Tag = mission.TypeLandscape != null ? mission.TypeLandscape.Index : -1;
            lblNameLocation = new VCLabel(panelSettings, btnLocation.NextLeft(), btnLocation.ShiftY, Program.formMain.FontSmall, Color.White, btnLocation.Height, "");
            lblNameLocation.StringFormat.Alignment = StringAlignment.Near;
            lblNameLocation.StringFormat.LineAlignment = StringAlignment.Center;
            lblNameLocation.Width = 24;
            VCLabel lblGold = new VCLabel(panelSettings, FormMain.Config.GridSize, btnLocation.NextTop(), Program.formMain.FontSmall, Color.White, 16, $"Золота на старте: {ls.TypeLobby.BaseResources.Gold}");
            lblGold.SetWidthByText();
            VCLabel lblMaxHeroes = new VCLabel(panelSettings, FormMain.Config.GridSize, lblGold.NextTop(), Program.formMain.FontSmall, Color.White, 16, $"Максимум героев: {ls.TypeLobby.MaxHeroes}");
            lblMaxHeroes.SetWidthByText();
            VCLabel lblDayStartBattles = new VCLabel(panelSettings, FormMain.Config.GridSize, lblMaxHeroes.NextTop(), Program.formMain.FontSmall, Color.White, 16, $"Ход начала битв между игроками: {ls.TypeLobby.DayStartBattleBetweenPlayers}");
            lblDayStartBattles.SetWidthByText();
            VCLabel lblDaysBetweenBattles = new VCLabel(panelSettings, FormMain.Config.GridSize, lblDayStartBattles.NextTop(), Program.formMain.FontSmall, Color.White, 16, $"Ходов перед следующей битвой: {ls.TypeLobby.DaysBeforeNextBattleBetweenPlayers}");
            lblDaysBetweenBattles.SetWidthByText();
            VCLabel lblMaxLoses = new VCLabel(panelSettings, FormMain.Config.GridSize, lblDaysBetweenBattles.NextTop(), Program.formMain.FontSmall, Color.White, 16, $"Максимум поражений: {ls.TypeLobby.MaxLoses}");
            lblMaxLoses.SetWidthByText();

            panelSettings.ApplyMaxSize();
            panelSettings.Width += FormMain.Config.GridSize;
            panelSettings.Height = panelPlayers.Height;

            //
            ClientControl.Width = panelSettings.EndLeft();
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

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

            mission.TypeLandscape = btnLocation.Tag == -1 ? null : FormMain.Descriptors.TypeLandscapes[btnLocation.Tag];

            UpdateNameLocation();
        }

        private void UpdateNameLocation()
        {
            if (mission.TypeLandscape is null)
            {
                btnLocation.ImageIndex = FormMain.Config.Gui48_RandomSelect;
                lblNameLocation.Text = "Случайная";
            }
            else
            {
                btnLocation.ImageIndex = mission.TypeLandscape.ImageIndex;
                lblNameLocation.Text = mission.TypeLandscape.Name;
            }
        }
    }

    internal sealed class VCLineSetupPlayerMission : VisualControl
    {
        private readonly VCIconButton48 btnTypePlayer;
        private readonly VCIconButton48 btnAvatar;
        private readonly VCLabel lblName;
        private readonly VCIconButton48 btnPersistentBonus;
        private readonly VCIconButton48 btnStartBonus;
        private readonly VCIconButton48 btnTypeTradition1;
        private readonly VCIconButton48 btnTypeTradition2;
        private readonly VCIconButton48 btnTypeTradition3;

        private LobbySettingsPlayer setting;

        private PanelDropDown panelTypeTraditions;

        public VCLineSetupPlayerMission(VisualControl parent, int shiftX, int shiftY, LobbySettingsPlayer lsp) : base(parent, shiftX, shiftY)
        {
            setting = lsp;

            btnTypePlayer = new VCIconButton48(this, 0, 0, 1);
            btnTypePlayer.Click += BtnTypePlayer_Click;
            btnAvatar = new VCIconButton48(this, btnTypePlayer.NextLeft(), 0, 2);
            lblName = new VCLabel(this, btnAvatar.NextLeft(), 0, Program.formMain.FontMedCaptionC, Color.White, btnTypePlayer.Height, "");
            lblName.StringFormat.LineAlignment = StringAlignment.Center;
            lblName.Width = 240;
            lblName.ShowBorder = true;
            btnPersistentBonus = new VCIconButton48(this, lblName.NextLeft(), 0, 3);
            btnPersistentBonus.Click += BtnPersistentBonus_Click;
            btnStartBonus = new VCIconButton48(this, btnPersistentBonus.NextLeft(), 0, 4);
            btnStartBonus.Click += BtnStartBonus_Click;
            btnTypeTradition1 = new VCIconButton48(this, btnStartBonus.NextLeft(), 0, 4);
            btnTypeTradition1.Click += BtnTypeTradition1_Click;
            btnTypeTradition2 = new VCIconButton48(this, btnTypeTradition1.NextLeft(), 0, 4);
            btnTypeTradition3 = new VCIconButton48(this, btnTypeTradition2.NextLeft(), 0, 4);

            Height = btnTypePlayer.Height;
            Width = btnTypeTradition3.EndLeft();

            UpdateData();
        }

        private static DescriptorTypeTradition SelectedTradition { get; set; }

        private void BtnTypeTradition1_Click(object sender, EventArgs e)
        {
            if (panelTypeTraditions is null)
            {
                int nextLeft = FormMain.Config.GridSize;
                int nextTop = FormMain.Config.GridSize;
                panelTypeTraditions = new PanelDropDown();
                panelTypeTraditions.ShowBorder = true;

                foreach (DescriptorTypeTradition tt in FormMain.Descriptors.TypeTraditions)
                {
                    VCCellSimple cell = new VCCellSimple(panelTypeTraditions, nextLeft, nextTop);
                    cell.HighlightUnderMouse = true;
                    cell.ImageIndex = tt.ImageIndex;
                    cell.Hint = tt.Description;
                    cell.Descriptor = tt;
                    cell.Click += Cell_Click;

                    if ((tt.Index + 1) % 3 > 0)
                    {
                        nextLeft = cell.NextLeft();
                    }
                    else
                    {
                        nextLeft = FormMain.Config.GridSize;
                        nextTop = cell.NextTop();
                    }
                }

                panelTypeTraditions.ApplyMaxSize();
                panelTypeTraditions.Width += FormMain.Config.GridSize;
                panelTypeTraditions.Height += FormMain.Config.GridSize;
            }

            panelTypeTraditions.ShowDropDown(btnTypeTradition1.Left, btnTypeTradition1.Top + btnTypeTradition1.Height);
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            Assert(SelectedTradition is null);

            SelectedTradition = ((VCCellSimple)sender).Descriptor as DescriptorTypeTradition;
            panelTypeTraditions.CloseForm(DialogAction.OK);
        }

        private void BtnStartBonus_Click(object sender, EventArgs e)
        {
            switch (setting.TypeSelectStartBonus)
            {
                case TypeSelectBonus.Manual:
                    setting.TypeSelectStartBonus = TypeSelectBonus.Random;
                    break;
                case TypeSelectBonus.Random:
                    setting.TypeSelectStartBonus = TypeSelectBonus.Manual;
                    break;
            }

            UpdateData();
        }

        private void BtnPersistentBonus_Click(object sender, EventArgs e)
        {
            switch (setting.TypeSelectPersistentBonus)
            {
                case TypeSelectBonus.Manual:
                    setting.TypeSelectPersistentBonus = TypeSelectBonus.Random;
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается игроком";
                    break;
                case TypeSelectBonus.Random:
                    setting.TypeSelectPersistentBonus = TypeSelectBonus.Manual;
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается случайно";
                    break;
            }

            UpdateData();
        }

        private void BtnTypePlayer_Click(object sender, EventArgs e)
        {
            //setting.TypePlayer = setting.TypePlayer == TypePlayer.Human ? TypePlayer.Computer : TypePlayer.Human;

            //UpdateData();
        }

        internal void UpdateData()
        {
            //
            int imIndexTypePlayer = -1;
            switch (setting.TypePlayer)
            {
                case TypePlayer.Human:
                    imIndexTypePlayer = FormMain.Config.Gui48_HumanPlayer;
                    btnTypePlayer.Hint = "Игрок - человек";
                    break;
                case TypePlayer.Computer:
                    imIndexTypePlayer = FormMain.Config.Gui48_ComputerPlayer;
                    btnTypePlayer.Hint = "Игрок - компьютер";
                    break;
                default:
                    DoException($"Неизвестный тип игрока: {setting.TypePlayer}");
                    break;
            }

            //
            btnTypePlayer.ImageIndex = imIndexTypePlayer;
            btnAvatar.ImageIndex = setting.Player != null ? setting.Player.ImageIndex : FormMain.Config.Gui48_RandomSelect;
            lblName.Text = setting.Player != null ? setting.Player.Name : "Случайный игрок";
            btnPersistentBonus.ImageIndex = GetImageIndexBonus(setting.TypeSelectPersistentBonus);
            btnPersistentBonus.ImageIsEnabled = setting.TypePlayer == TypePlayer.Human;
            btnStartBonus.ImageIndex = GetImageIndexBonus(setting.TypeSelectStartBonus);
            btnStartBonus.ImageIsEnabled = setting.TypePlayer == TypePlayer.Human;
            btnTypeTradition1.ImageIndex = GetImageIndexTypeTradition(setting.TypeTradition1);
            btnTypeTradition2.ImageIndex = GetImageIndexTypeTradition(setting.TypeTradition2);
            btnTypeTradition3.ImageIndex = GetImageIndexTypeTradition(setting.TypeTradition3);

            switch (setting.TypeSelectStartBonus)
            {
                case TypeSelectBonus.Manual:
                    btnStartBonus.Hint = "Стартовый бонус выбирается игроком";
                    break;
                case TypeSelectBonus.Random:
                    btnStartBonus.Hint = "Стартовый бонус выбирается случайно";
                    break;
            }

            switch (setting.TypeSelectPersistentBonus)
            {
                case TypeSelectBonus.Manual:
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается игроком";
                    break;
                case TypeSelectBonus.Random:
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается случайно";
                    break;
            }

            int GetImageIndexBonus(TypeSelectBonus type)
            {
                int imageIndex = -1;
                switch (type)
                {
                    case TypeSelectBonus.Manual:
                        imageIndex = FormMain.Config.Gui48_ManualSelect;
                        break;
                    case TypeSelectBonus.Random:
                        imageIndex = FormMain.Config.Gui48_RandomSelect;
                        break;
                    default:
                        DoException($"Неизвестный выбор бонуса: {type}");
                        break;
                }

                return imageIndex;
            }

            int GetImageIndexTypeTradition(DescriptorTypeTradition tt)
            {
                return tt != null ? tt.ImageIndex : FormMain.Config.Gui48_RandomSelect;
            }
        }
    }
}
