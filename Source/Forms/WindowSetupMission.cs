using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Reflection;

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

        protected override void BeforeClose(DialogAction da)
        {
            base.BeforeClose(da);

            if (da == DialogAction.OK)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    mission.Players[i].SetTypeTraditions(lines[i].btnTypeTradition1.SelectedTradition, lines[i].btnTypeTradition2.SelectedTradition, lines[i].btnTypeTradition3.SelectedTradition);
                }
            }
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
        internal readonly VCButtonSelectTradition btnTypeTradition1;
        internal readonly VCButtonSelectTradition btnTypeTradition2;
        internal readonly VCButtonSelectTradition btnTypeTradition3;

        private LobbySettingsPlayer setting;

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
            btnTypeTradition1 = new VCButtonSelectTradition(this, btnStartBonus.NextLeft(), 0);
            btnTypeTradition1.Level = "1";
            btnTypeTradition2 = new VCButtonSelectTradition(this, btnTypeTradition1.NextLeft(), 0);
            btnTypeTradition2.Level = "2";
            btnTypeTradition3 = new VCButtonSelectTradition(this, btnTypeTradition2.NextLeft(), 0);
            btnTypeTradition3.Level = "3";

            Height = btnTypePlayer.Height;
            Width = btnTypeTradition3.EndLeft();

            UpdateData();
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
            btnTypeTradition1.SelectedTradition = setting.TypeTradition1;
            btnTypeTradition2.SelectedTradition = setting.TypeTradition2;
            btnTypeTradition3.SelectedTradition = setting.TypeTradition3;

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
        }
    }

    internal sealed class VCButtonSelectTradition : VCIconButton48
    {
        private DescriptorTypeTradition selectedTradition;
        private PanelDropDown panelTypeTraditions;

        public VCButtonSelectTradition(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
            Click += VCCellSelectTradition_Click;
        }


        internal DescriptorTypeTradition SelectedTradition
        {
            get => selectedTradition;
            set
            {
                selectedTradition = value; 
                ImageIndex = selectedTradition != null ? selectedTradition.ImageIndex : FormMain.Config.Gui48_RandomSelect;
                Hint = selectedTradition != null ? selectedTradition.Name : "Случайная традиция";
            }
        }

        internal override void ResultFromDropDown(DialogAction da)
        {
            base.ResultFromDropDown(da);

            if (selectedTradition != null)
            {
                VCLineSetupPlayerMission line = Parent as VCLineSetupPlayerMission;
                if ((line.btnTypeTradition1 != this) && (line.btnTypeTradition1.SelectedTradition == SelectedTradition))
                    line.btnTypeTradition1.SelectedTradition = null;
                if ((line.btnTypeTradition2 != this) && (line.btnTypeTradition2.SelectedTradition == SelectedTradition))
                    line.btnTypeTradition2.SelectedTradition = null;
                if ((line.btnTypeTradition3 != this) && (line.btnTypeTradition3.SelectedTradition == SelectedTradition))
                    line.btnTypeTradition3.SelectedTradition = null;
            }
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            SelectedTradition = ((VCCellSimple)sender).Descriptor as DescriptorTypeTradition;
            panelTypeTraditions.CloseForm(DialogAction.OK);
        }

        private void VCCellSelectTradition_Click(object sender, EventArgs e)
        {
            if (panelTypeTraditions is null)
            {
                int nextLeft = FormMain.Config.GridSize;
                int nextTop = FormMain.Config.GridSize;
                int count = 0;
                panelTypeTraditions = new PanelDropDown();

                foreach (DescriptorTypeTradition tt in FormMain.Descriptors.TypeTraditions)
                {
                    CreateCell(ref count, ref nextLeft, ref nextTop, tt);
                }

                CreateCell(ref count, ref nextLeft, ref nextTop, null);


                panelTypeTraditions.ApplyMaxSize();
                panelTypeTraditions.Width += FormMain.Config.GridSize;
                panelTypeTraditions.Height += FormMain.Config.GridSize;
            }

            panelTypeTraditions.ShowDropDown(Left, Top + Height);

            void  CreateCell(ref int count, ref int nextLeft, ref int nextTop, DescriptorTypeTradition tt)
            {
                VCCellSimple cell = new VCCellSimple(panelTypeTraditions, nextLeft, nextTop);
                cell.HighlightUnderMouse = true;

                if (tt != null)
                {
                    cell.ImageIndex = tt.ImageIndex;
                    cell.Hint = tt.Description;
                    cell.Descriptor = tt;
                }
                else
                {
                    cell.ImageIndex = FormMain.Config.Gui48_RandomSelect;
                    cell.Hint = "Случайная традиция";
                    cell.Descriptor = null;
                }
                cell.Click += Cell_Click;
                cell.PlaySoundOnClick = true;
                cell.PlaySoundOnEnter = true;

                if ((count + 1) % 3 > 0)
                {
                    nextLeft = cell.NextLeft();
                }
                else
                {
                    nextLeft = FormMain.Config.GridSize;
                    nextTop = cell.NextTop();
                }

                count++;
            }
        }
    }
}
