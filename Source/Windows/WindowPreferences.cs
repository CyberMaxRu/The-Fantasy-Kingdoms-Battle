﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Окно настроек
    internal sealed class WindowPreferences : WindowOkCancel
    {
        private VCButton btnDefault;

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
        private VCCheckBox chkbShowTimeForExecuting;
        private VCCheckBox chkbShowTypeCellMenu;
        private VCCheckBox chkbHideFulfilledRequirements;
        private VCCheckBox chkbShowNameConstruction;
        private VCCheckBox chkbShowExtraHint;
        private VCCheckBox chkbAllowCheating;

        private Settings settings;
        private ScreenMode oldScreenMode;


        public WindowPreferences() : base("Настройки игры")
        {
            vcPanelGame = new VisualControl(ClientControl, 0, 0);
            vcPanelGame.ShowBorder = true;
            lblCaptionPanelGame = new VCLabel(vcPanelGame, FormMain.Config.GridSize, 8, Program.formMain.FontMedCaption, Color.MediumTurquoise, 24, "Общие настройки:");
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
            lblCaptionPanelBattlefield = new VCLabel(vcPanelBatttlefield, FormMain.Config.GridSize, 8, Program.formMain.FontMedCaption, Color.MediumTurquoise, 24, "Настройки битвы:");
            lblCaptionPanelBattlefield.StringFormat.Alignment = StringAlignment.Near;
            chkbShowPath = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, lblCaptionPanelBattlefield.NextTop(), "Показывать путь юнитов");
            chkbShowGrid = new VCCheckBox(vcPanelBatttlefield, FormMain.Config.GridSize, chkbShowPath.NextTop(), "Показывать сетку");
            vcPanelBatttlefield.ApplyMaxSize();
            vcPanelBatttlefield.Height += 8;
            lblCaptionPanelBattlefield.Width = vcPanelBatttlefield.Width - (FormMain.Config.GridSize * 2); 

            vcPanelSound = new VisualControl(ClientControl, 0, vcPanelBatttlefield.NextTop());
            vcPanelSound.ShowBorder = true;
            lblCaptionPanelSound = new VCLabel(vcPanelSound, FormMain.Config.GridSize, 8, Program.formMain.FontMedCaption, Color.MediumTurquoise, 24, "Настройки звука:");
            lblCaptionPanelSound.StringFormat.Alignment = StringAlignment.Near;
            chkbPlaySound = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize, lblCaptionPanelSound.NextTop(), "Звуки");
            chkbPlaySound.Width = 104;
            chkbPlaySound.Click += ChkbPlaySound_Click;
            chkbPlayMusic = new VCCheckBox(vcPanelSound, FormMain.Config.GridSize, chkbPlaySound.NextTop(), "Музыка");
            chkbPlayMusic.Width = 104;
            chkbPlayMusic.Click += ChkbPlayMusic_Click;
            lblVolumeSound = new VCLabel(vcPanelSound, chkbPlayMusic.NextLeft(), chkbPlaySound.ShiftY, Program.formMain.FontMedCaptionC, Color.White, 24, "");
            lblVolumeSound.StringFormat.Alignment = StringAlignment.Far;
            lblVolumeSound.Width = 44;
            lblVolumeMusic = new VCLabel(vcPanelSound, lblVolumeSound.ShiftX, chkbPlayMusic.ShiftY, Program.formMain.FontMedCaptionC, Color.White, 24, "");
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
            lblCaptionPanelInterface = new VCLabel(vcPanelInterface, FormMain.Config.GridSize, 8, Program.formMain.FontMedCaption, Color.MediumTurquoise, 24, "Интерфейс:");
            lblCaptionPanelInterface.StringFormat.Alignment = StringAlignment.Near;
            chkbShowShortNames = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, lblCaptionPanelInterface.NextTop(), "Наименования на иконках умений и предметов");
            chkbShowShortNames.Width = 520;
            chkbShowTimeForExecuting = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowShortNames.NextTop(), "Показывать время для выполнения действия");
            chkbShowTimeForExecuting.Width = 520;
            chkbShowTypeCellMenu = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowTimeForExecuting.NextTop(), "Показывать тип объекта в меню");
            chkbShowTypeCellMenu.Width = 520;
            chkbHideFulfilledRequirements = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowTypeCellMenu.NextTop(), "Скрывать выполненные требования");
            chkbHideFulfilledRequirements.Width = 520;
            chkbShowNameConstruction = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbHideFulfilledRequirements.NextTop(), "Показывать наименование сооружения");
            chkbShowNameConstruction.Width = 520;
            chkbShowExtraHint = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowNameConstruction.NextTop(), "Показывать дополнительную подсказку");
            chkbShowExtraHint.Width = 520;
            chkbAllowCheating = new VCCheckBox(vcPanelInterface, FormMain.Config.GridSize, chkbShowExtraHint.NextTop(), "Разрешить читинг");
            chkbAllowCheating.Width = 520;
            vcPanelInterface.ApplyMaxSize();
            vcPanelInterface.Height += 8;
            lblCaptionPanelInterface.Width = vcPanelInterface.Width - (FormMain.Config.GridSize * 2);

            btnDefault = new VCButton(ClientControl, 0, vcPanelInterface.NextTop(), "По умолчанию");
            btnDefault.Width = 184;
            btnDefault.Click += BtnDefault_Click;

            btnOk.Caption = "Принять";
            btnCancel.Caption = "Отмена";

            ClientControl.Width = vcPanelInterface.Width;
            ClientControl.Height = btnDefault.NextTop() + btnOk.Height;
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
            chkbShowTimeForExecuting.Checked = settings.ShowTimeForExecuting;
            chkbShowTypeCellMenu.Checked = settings.ShowTypeCellMenu;
            chkbHideFulfilledRequirements.Checked = settings.HideFulfilledRequirements;
            chkbShowNameConstruction.Checked = settings.ShowNameConstruction;
            chkbShowExtraHint.Checked = settings.ShowExtraHint;
            chkbAllowCheating.Checked = settings.AllowCheating;
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            settings.SetDefault();
            ApplySettings(settings);
        }

        protected override void BeforeClose(DialogAction da)
        {
            base.BeforeClose(da);

            if (da == DialogAction.None)
            {
                settings.LoadSettings();
                ApplySettings(settings);
            }
            else if(da == DialogAction.OK)
            {
                oldScreenMode = settings.ScreenMode();
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
                settings.ShowTimeForExecuting = chkbShowTimeForExecuting.Checked;
                settings.ShowTypeCellMenu = chkbShowTypeCellMenu.Checked;
                settings.HideFulfilledRequirements = chkbHideFulfilledRequirements.Checked;
                settings.ShowNameConstruction = chkbShowNameConstruction.Checked;
                settings.ShowExtraHint = chkbShowExtraHint.Checked;
                settings.AllowCheating = chkbAllowCheating.Checked;
                settings.SaveSettings();
            }
            else
                Utils.DoException("Неизвестный результат диалога");
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            Program.formMain.PlayerMusic.RefreshPlayList();
            /*if (Settings.NamePlayer != lobby.CurrentPlayer.Name)
            {
                lobby.CurrentPlayer.Name = Settings.NamePlayer;
            }*/

            Program.formMain.ApplyFullScreen(oldScreenMode != settings.ScreenMode());

            foreach (LayerCustom l in Program.formMain.Layers)
            {
                l.PreferencesChanged();
            }

        }
    }
}
