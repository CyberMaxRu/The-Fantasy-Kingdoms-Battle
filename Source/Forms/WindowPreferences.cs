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
        private VCCheckBox chkbFullscreenMode;
        private VCCheckBox chkbStretchControlsInFSMode;
        private VCCheckBox chkbCheckUpdates;

        private VisualControl vcPanelBatttlefield;
        private VCLabel lblCaptionPanelBattlefield;
        private VCCheckBox chkbShowPath;
        private VCCheckBox chkbShowGrid;

        private VisualControl vcPanelSound;
        private VCLabel lblCaptionPanelSound;
        private VCCheckBox chkbPlaySound;
        private VCCheckBox chkbPlayMusic;
        private VCLabel lblVolumeSound;
        private VCLabel lblVolumeMusic;
        private readonly VCHorizTrackBar tbVolumeSound;
        private readonly VCHorizTrackBar tbVolumeMusic;
        private VCCheckBox chkbMusicFromMajesty1;
        private VCCheckBox chkbMusicFromMajesty2;

        private VisualControl vcPanelInterface;
        private VCLabel lblCaptionPanelInterface;
        private VCCheckBox chkbShowShortNames;
        private VCCheckBox chkbShowQuantityDaysForExecuting;
        private VCCheckBox chkbShowTypeCellMenu;
        private VCCheckBox chkbHideFulfilledRequirements;
        private VCCheckBox chkbShowExtraHint;
        private VCCheckBox chkbAllowCheating;

        private Settings settings;

        public WindowPreferences() : base()
        {
            windowCaption.Caption = "Настройки игры";

            vcPanelGame = new VisualControl(ClientControl, 0, 0);
            vcPanelGame.ShowBorder = true;
            lblCaptionPanelGame = new VCLabel(vcPanelGame, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Общие настройки:");
            lblCaptionPanelGame.StringFormat.Alignment = StringAlignment.Near;
            chkbShowSplashVideo = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize, lblCaptionPanelGame.NextTop(), "Показывать видео-заставку");
            chkbFullscreenMode = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize, chkbShowSplashVideo.NextTop(), "Полноэкранный режим");
            chkbStretchControlsInFSMode = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize + 32, chkbFullscreenMode.NextTop(), "Растянуть контролы");
            chkbCheckUpdates = new VCCheckBox(vcPanelGame, FormMain.Config.GridSize, chkbStretchControlsInFSMode.NextTop(), "Проверять обновления при запуске");
            vcPanelGame.ApplyMaxSize();
            vcPanelGame.Height += 8;
            lblCaptionPanelGame.Width = vcPanelGame.Width - (FormMain.Config.GridSize * 2);

            vcPanelBatttlefield = new VisualControl(ClientControl, 0, vcPanelGame.NextTop());
            vcPanelBatttlefield.ShowBorder = true;
            lblCaptionPanelBattlefield = new VCLabel(vcPanelBatttlefield, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Настройки битвы:");
            lblCaptionPanelBattlefield.StringFormat.Alignment = StringAlignment.Near;
            chkbShowPath = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, lblCaptionPanelBattlefield.NextTop(), "Показывать путь юнитов");
            chkbShowGrid = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, chkbShowPath.NextTop(), "Показывать сетку");
            vcPanelBatttlefield.ApplyMaxSize();
            vcPanelBatttlefield.Height += 8;
            lblCaptionPanelBattlefield.Width = vcPanelBatttlefield.Width - (FormMain.Config.GridSize * 2); 

            vcPanelSound = new VisualControl(ClientControl, 0, vcPanelBatttlefield.NextTop());
            vcPanelSound.ShowBorder = true;
            lblCaptionPanelSound = new VCLabel(vcPanelSound, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Настройки звука:");
            lblCaptionPanelSound.StringFormat.Alignment = StringAlignment.Near;
            chkbPlaySound = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize, lblCaptionPanelSound.NextTop(), "Звуки");
            chkbPlaySound.Width = 104;
            chkbPlaySound.Click += ChkbPlaySound_Click;
            chkbPlayMusic = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize, chkbPlaySound.NextTop(), "Музыка");
            chkbPlayMusic.Width = 104;
            chkbPlayMusic.Click += ChkbPlayMusic_Click;
            lblVolumeSound = new VCLabel(vcPanelSound, chkbPlayMusic.NextLeft(), chkbPlaySound.ShiftY, Program.formMain.fontMedCaptionC, Color.White, 24, "");
            lblVolumeSound.StringFormat.Alignment = StringAlignment.Far;
            lblVolumeSound.Width = 44;
            lblVolumeMusic = new VCLabel(vcPanelSound, lblVolumeSound.ShiftX, chkbPlayMusic.ShiftY, Program.formMain.fontMedCaptionC, Color.White, 24, "");
            lblVolumeMusic.StringFormat.Alignment = StringAlignment.Far;
            lblVolumeMusic.Width = 44;
            tbVolumeSound = new VCHorizTrackBar(vcPanelSound, lblVolumeSound.NextLeft(), chkbPlaySound.ShiftY);
            tbVolumeSound.OnPositionChanged += TbVolumeSound_OnPositionChanged;
            tbVolumeMusic = new VCHorizTrackBar(vcPanelSound, lblVolumeSound.NextLeft(), chkbPlayMusic.ShiftY);
            tbVolumeMusic.OnPositionChanged += TbVolumeMusic_OnPositionChanged;
            chkbMusicFromMajesty1 = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize * 5, chkbPlayMusic.NextTop(), "Музыка из Majesty 1");
            chkbMusicFromMajesty1.Width = 200;
            chkbMusicFromMajesty2 = new VCCheckBox(vcPanelSound, chkbMusicFromMajesty1.NextLeft(), chkbMusicFromMajesty1.ShiftY, "Музыка из Majesty 2");
            chkbMusicFromMajesty2.Width = 104;
            vcPanelSound.ApplyMaxSize();
            vcPanelSound.Height += 8;

            vcPanelInterface = new VisualControl(ClientControl, 0, vcPanelSound.NextTop());
            vcPanelInterface.ShowBorder = true;
            lblCaptionPanelInterface = new VCLabel(vcPanelInterface, FormMain.Config.GridSize, 8, Program.formMain.fontMedCaption, Color.MediumTurquoise, 24, "Интерфейс:");
            lblCaptionPanelInterface.StringFormat.Alignment = StringAlignment.Near;
            chkbShowShortNames = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, lblCaptionPanelInterface.NextTop(), "Наименования на иконках умений и предметов");
            chkbShowShortNames.Width = 440;
            chkbShowQuantityDaysForExecuting = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowShortNames.NextTop(), "Показывать количество дней для выполнения действия в меню");
            chkbShowQuantityDaysForExecuting.Width = 440;
            chkbShowTypeCellMenu = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowQuantityDaysForExecuting.NextTop(), "Показывать тип объекта в меню");
            chkbShowTypeCellMenu.Width = 440;
            chkbHideFulfilledRequirements = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowTypeCellMenu.NextTop(), "Скрывать выполненные требования");
            chkbHideFulfilledRequirements.Width = 440;
            chkbShowExtraHint = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbHideFulfilledRequirements.NextTop(), "Показывать дополнительную подсказку");
            chkbShowExtraHint.Width = 440;
            chkbAllowCheating = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowExtraHint.NextTop(), "Разрешить читинг");
            chkbAllowCheating.Width = 440;
            vcPanelInterface.ApplyMaxSize();
            vcPanelInterface.Height += 8;
            lblCaptionPanelInterface.Width = vcPanelInterface.Width - (FormMain.Config.GridSize * 2);

            btnAccept = new VCButton(ClientControl, 24, vcPanelInterface.NextTop() + (FormMain.Config.GridSize * 2), "Принять");
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

            ClientControl.Width = btnCancel.ShiftX + btnCancel.Width + btnCancel.Left + btnAccept.ShiftX;
            ClientControl.Height = btnCancel.NextTop();
            vcPanelGame.Width = ClientControl.Width - (vcPanelGame.ShiftX * 2);
            vcPanelBatttlefield.Width = ClientControl.Width - (vcPanelBatttlefield.ShiftX * 2);
            vcPanelSound.Width = ClientControl.Width - (vcPanelSound.ShiftX * 2);
            vcPanelInterface.Width = ClientControl.Width - (vcPanelInterface.ShiftX * 2);
            lblCaptionPanelSound.Width = vcPanelSound.Width - (FormMain.Config.GridSize * 2);

            tbVolumeSound.Width = ClientControl.Width - tbVolumeSound.ShiftX - FormMain.Config.GridSize;
            tbVolumeMusic.Width = ClientControl.Width - tbVolumeMusic.ShiftX - FormMain.Config.GridSize; 
        }

        private void TbVolumeMusic_OnPositionChanged(object sender, EventArgs e)
        {
            settings.VolumeMusic = tbVolumeMusic.Position;
            lblVolumeMusic.Text = tbVolumeMusic.Position.ToString() + "%";
        }

        private void TbVolumeSound_OnPositionChanged(object sender, EventArgs e)
        {
            settings.VolumeSound = tbVolumeSound.Position;
            lblVolumeSound.Text = tbVolumeSound.Position.ToString() + "%";
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
            chkbFullscreenMode.Checked = settings.FullScreenMode;
            chkbStretchControlsInFSMode.Checked = settings.StretchControlsInFSMode;
            chkbCheckUpdates.Checked = settings.CheckUpdateOnStartup;
            chkbShowPath.Checked = settings.BattlefieldShowPath;
            chkbShowGrid.Checked = settings.BattlefieldShowGrid;
            chkbPlaySound.Checked = settings.PlaySound;
            chkbPlayMusic.Checked = settings.PlayMusic;
            chkbMusicFromMajesty1.Checked = settings.MusicFromMajesty1;
            chkbMusicFromMajesty2.Checked = settings.MusicFromMajesty2;
            tbVolumeSound.Position = settings.VolumeSound;
            tbVolumeMusic.Position = settings.VolumeMusic;
            chkbShowShortNames.Checked = settings.ShowShortNames;
            chkbShowQuantityDaysForExecuting.Checked = settings.ShowQuantityDaysForExecuting;
            chkbShowTypeCellMenu.Checked = settings.ShowTypeCellMenu;
            chkbHideFulfilledRequirements.Checked = settings.HideFulfilledRequirements;
            chkbShowExtraHint.Checked = settings.ShowExtraHint;
            chkbAllowCheating.Checked = settings.AllowCheating;
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            settings.SetDefault();
            ApplySettings(settings);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            settings.LoadSettings();
            ApplySettings(settings);

            CloseForm(DialogAction.None);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            settings.ShowSplashVideo = chkbShowSplashVideo.Checked;
            settings.FullScreenMode = chkbFullscreenMode.Checked;
            settings.StretchControlsInFSMode = chkbStretchControlsInFSMode.Checked;
            settings.CheckUpdateOnStartup = chkbCheckUpdates.Checked;
            settings.BattlefieldShowPath = chkbShowPath.Checked;
            settings.BattlefieldShowGrid = chkbShowGrid.Checked;
            settings.PlaySound = chkbPlaySound.Checked;
            settings.PlayMusic = chkbPlayMusic.Checked;
            settings.VolumeSound = tbVolumeSound.Position;
            settings.VolumeMusic = tbVolumeMusic.Position;
            settings.MusicFromMajesty1 = chkbMusicFromMajesty1.Checked;
            settings.MusicFromMajesty2 = chkbMusicFromMajesty2.Checked;
            settings.ShowShortNames = chkbShowShortNames.Checked;
            settings.ShowQuantityDaysForExecuting = chkbShowQuantityDaysForExecuting.Checked;
            settings.ShowTypeCellMenu = chkbShowTypeCellMenu.Checked;
            settings.HideFulfilledRequirements = chkbHideFulfilledRequirements.Checked;
            settings.ShowExtraHint = chkbShowExtraHint.Checked;
            settings.AllowCheating = chkbAllowCheating.Checked;
            settings.SaveSettings();

            CloseForm(DialogAction.OK);
        }
    }
}
