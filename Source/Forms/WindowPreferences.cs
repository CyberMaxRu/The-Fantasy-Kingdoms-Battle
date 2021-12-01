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

        private VisualControl vcPanelSound;
        private VCLabel lblCaptionPanelSound;
        private VCCheckBox chkbPlaySound;
        private VCCheckBox chkbPlayMusic;
        private readonly VCHorizTrackBar tbVolumeSound;
        private readonly VCHorizTrackBar tbVolumeMusic;

        private VisualControl vcPanelInterface;
        private VCLabel lblCaptionPanelInterface;
        private VCCheckBox chkbShowShortNames;
        private VCCheckBox chkbShowTypeCellMenu;
        private VCCheckBox chkbHideFulfilledRequirements;

        private Settings settings;

        private bool playSound;
        private bool playMusic;

        public WindowPreferences() : base()
        {
            windowCaption.Caption = "Настройки игры";

            vcPanelGame = new VisualControl(ClientControl, 0, 0);
            vcPanelGame.ShowBorder = true;
            lblCaptionPanelGame = new VCLabel(vcPanelGame, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Общие настройки:");
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
            lblCaptionPanelBattlefield = new VCLabel(vcPanelBatttlefield, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Настройки битвы:");
            lblCaptionPanelBattlefield.StringFormat.Alignment = StringAlignment.Near;
            chkbShowPath = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, lblCaptionPanelBattlefield.NextTop(), "Показывать путь юнитов");
            chkbShowPath.Width = 320;
            chkbShowGrid = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, chkbShowPath.NextTop(), "Показывать сетку");
            chkbShowGrid.Width = 320;
            vcPanelBatttlefield.ApplyMaxSize();
            vcPanelBatttlefield.Height += 8;
            lblCaptionPanelBattlefield.Width = vcPanelBatttlefield.Width - (FormMain.Config.GridSize * 2); 

            vcPanelSound = new VisualControl(ClientControl, 0, vcPanelBatttlefield.NextTop());
            vcPanelSound.ShowBorder = true;
            lblCaptionPanelSound = new VCLabel(vcPanelSound, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Настройки звука:");
            lblCaptionPanelSound.StringFormat.Alignment = StringAlignment.Near;
            chkbPlaySound = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize, lblCaptionPanelSound.NextTop(), "Звуки");
            chkbPlaySound.Width = 152;
            chkbPlaySound.Click += ChkbPlaySound_Click;
            chkbPlayMusic = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize, chkbPlaySound.NextTop(), "Музыка");
            chkbPlayMusic.Width = 152;
            chkbPlayMusic.Click += ChkbPlayMusic_Click;
            vcPanelSound.ApplyMaxSize();
            vcPanelSound.Height += 8;
            tbVolumeSound = new VCHorizTrackBar(vcPanelSound, chkbPlaySound.NextLeft(), chkbPlaySound.ShiftY);
            tbVolumeMusic = new VCHorizTrackBar(vcPanelSound, chkbPlayMusic.NextLeft(), chkbPlayMusic.ShiftY);
            lblCaptionPanelSound.Width = vcPanelSound.Width - (FormMain.Config.GridSize * 2);

            vcPanelInterface = new VisualControl(ClientControl, 0, vcPanelSound.NextTop());
            vcPanelInterface.ShowBorder = true;
            lblCaptionPanelInterface = new VCLabel(vcPanelInterface, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Интерфейс:");
            lblCaptionPanelInterface.StringFormat.Alignment = StringAlignment.Near;
            chkbShowShortNames = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, lblCaptionPanelInterface.NextTop(), "Наименования на иконках умений и предметов");
            chkbShowShortNames.Width = 400;
            chkbShowTypeCellMenu = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowShortNames.NextTop(), "Показывать тип объекта в меню");
            chkbShowTypeCellMenu.Width = 400;
            chkbHideFulfilledRequirements = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowTypeCellMenu.NextTop(), "Скрывать выполненные требования");
            chkbHideFulfilledRequirements.Width = 400;
            vcPanelInterface.ApplyMaxSize();
            vcPanelInterface.Height += 8;
            lblCaptionPanelInterface.Width = vcPanelInterface.Width - (FormMain.Config.GridSize * 2);

            btnAccept = new VCButton(ClientControl, 0, vcPanelInterface.NextTop() + (FormMain.Config.GridSize * 2), "Принять");
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
            vcPanelSound.Width = ClientControl.Width - (vcPanelSound.ShiftX * 2);
            vcPanelInterface.Width = ClientControl.Width - (vcPanelInterface.ShiftX * 2);

            tbVolumeSound.Width = ClientControl.Width - tbVolumeSound.ShiftX - FormMain.Config.GridSize;
            tbVolumeMusic.Width = ClientControl.Width - tbVolumeMusic.ShiftX - FormMain.Config.GridSize; 
        }

        private void ChkbPlayMusic_Click(object sender, EventArgs e)
        {
            settings.PlayMusic = chkbPlayMusic.Checked;
        }

        private void ChkbPlaySound_Click(object sender, EventArgs e)
        {
            settings.PlaySound = chkbPlaySound.Checked;
        }

        internal void ApplySettings(Settings s)
        {
            settings = s;

            chkbShowSplashVideo.Checked = settings.ShowSplashVideo;
            chkbWindowMode.Checked = !settings.FullScreenMode;
            chkbCheckUpdates.Checked = settings.CheckUpdateOnStartup;
            chkbShowPath.Checked = settings.BattlefieldShowPath;
            chkbShowGrid.Checked = settings.BattlefieldShowGrid;
            chkbPlaySound.Checked = settings.PlaySound;
            chkbPlayMusic.Checked = settings.PlayMusic;
            chkbShowShortNames.Checked = settings.ShowShortNames;
            chkbShowTypeCellMenu.Checked = settings.ShowTypeCellMenu;
            chkbHideFulfilledRequirements.Checked = settings.HideFulfilledRequirements;

            playSound = settings.PlaySound;
            playMusic = settings.PlayMusic;

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
            settings.PlaySound = playSound;
            settings.PlayMusic = playMusic;

            CloseForm(DialogAction.None);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            settings.ShowSplashVideo = chkbShowSplashVideo.Checked;
            settings.FullScreenMode = !chkbWindowMode.Checked;
            settings.CheckUpdateOnStartup = chkbCheckUpdates.Checked;
            settings.BattlefieldShowPath = chkbShowPath.Checked;
            settings.BattlefieldShowGrid = chkbShowGrid.Checked;
            settings.PlaySound = chkbPlaySound.Checked;
            settings.PlayMusic = chkbPlayMusic.Checked;
            settings.ShowShortNames = chkbShowShortNames.Checked;
            settings.ShowTypeCellMenu = chkbShowTypeCellMenu.Checked;
            settings.HideFulfilledRequirements = chkbHideFulfilledRequirements.Checked;
            settings.SaveSettings();

            CloseForm(DialogAction.OK);
        }
    }
}
