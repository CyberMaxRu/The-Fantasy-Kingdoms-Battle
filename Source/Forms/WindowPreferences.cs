using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Окно настроек
    internal sealed class WindowPreferences : VCForm
    {
        private VCButton btnAccept;
        private VCButton btnDefault;
        private VCButton btnCancel;

        private VisualControl vcPanelGame;
        private VCLabel lblCaptionPanelGame;
        private VCCheckBox chkbShowSplashVideo;
        private VCCheckBox chkbWindowMode;
        private VCCheckBox chkbCheckUpdates;

        private VisualControl vcPanelBatttlefield;
        private VCLabel lblCaptionPanelBattlefield;
        private VCCheckBox chkbShowPath;
        private VCCheckBox chkbShowGrid;

        private Settings settings;

        public WindowPreferences() : base()
        {
            windowCaption.Caption = "Настройки";

            vcPanelGame = new VisualControl(ClientControl, 0, 0);
            vcPanelGame.ShowBorder = true;
            lblCaptionPanelGame = new VCLabel(vcPanelGame, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.PaleTurquoise, 24, "Общие настройки:");
            lblCaptionPanelGame.StringFormat.Alignment = StringAlignment.Near;
            chkbShowSplashVideo = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize, lblCaptionPanelGame.NextTop(), "Показывать видео-заставку");
            chkbShowSplashVideo.Width = 320;
            chkbWindowMode = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize, chkbShowSplashVideo.NextTop(), "Оконный режим");
            chkbWindowMode.Width = 320;
            chkbCheckUpdates = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize, chkbWindowMode.NextTop(), "Проверять обновления при запуске");
            chkbCheckUpdates.Width = 320;
            vcPanelGame.ApplyMaxSize();
            vcPanelGame.Height += 8;
            lblCaptionPanelGame.Width = vcPanelGame.Width - (FormMain.Config.GridSize * 2);

            vcPanelBatttlefield = new VisualControl(ClientControl, 0, vcPanelGame.NextTop());
            vcPanelBatttlefield.ShowBorder = true;
            lblCaptionPanelBattlefield = new VCLabel(vcPanelBatttlefield, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.PaleTurquoise, 24, "Настройки битвы:");
            lblCaptionPanelBattlefield.StringFormat.Alignment = StringAlignment.Near;
            chkbShowPath = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, lblCaptionPanelBattlefield.NextTop(), "Показывать путь юнитов");
            chkbShowPath.Width = 320;
            chkbShowGrid = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, chkbShowPath.NextTop(), "Показывать сетку");
            chkbShowGrid.Width = 320;
            vcPanelBatttlefield.ApplyMaxSize();
            vcPanelBatttlefield.Height += 8;
            lblCaptionPanelBattlefield.Width = vcPanelBatttlefield.Width - (FormMain.Config.GridSize * 2);

            btnAccept = new VCButton(ClientControl, 0, vcPanelBatttlefield.NextTop() + (FormMain.Config.GridSize * 2), "Принять");
            btnAccept.Width = 160;
            btnAccept.Click += BtnAccept_Click;
            btnDefault = new VCButton(ClientControl, btnAccept.NextLeft(), btnAccept.ShiftY, "Базовые");
            btnDefault.Width = 160;
            btnDefault.Click += BtnDefault_Click;
            btnCancel = new VCButton(ClientControl, btnDefault.NextLeft(), btnAccept.ShiftY, "Отмена");
            btnCancel.Width = 160;
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnAccept;
            CancelButton = btnCancel;

            ClientControl.Width = btnCancel.ShiftX + btnCancel.Width + btnCancel.Left;
            ClientControl.Height = btnCancel.NextTop();
            vcPanelGame.Width = ClientControl.Width - (vcPanelGame.ShiftX * 2);
            vcPanelBatttlefield.Width = ClientControl.Width - (vcPanelBatttlefield.ShiftX * 2);
        }

        internal void ApplySettings(Settings s)
        {
            settings = s;

            chkbShowSplashVideo.Checked = settings.ShowSplashVideo;
            chkbWindowMode.Checked = !settings.FullScreenMode;
            chkbCheckUpdates.Checked = settings.CheckUpdateOnStartup;
            chkbShowPath.Checked = settings.BattlefieldShowPath;
            chkbShowGrid.Checked = settings.BattlefieldShowGrid;

/*            
            filenameAvatar = settings.FileNameAvatar;
            directoryAvatar = settings.DirectoryAvatar;
            ShowAvatar();*/
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            settings.SetDefault();
            ApplySettings(settings);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm(DialogResult.Cancel);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            settings.ShowSplashVideo = chkbShowSplashVideo.Checked;
            settings.FullScreenMode = !chkbWindowMode.Checked;
            settings.CheckUpdateOnStartup = chkbCheckUpdates.Checked;
            settings.BattlefieldShowPath = chkbShowPath.Checked;
            settings.BattlefieldShowGrid = chkbShowGrid.Checked;
            settings.SaveSettings();

            CloseForm(DialogResult.OK);
        }
    }
}
