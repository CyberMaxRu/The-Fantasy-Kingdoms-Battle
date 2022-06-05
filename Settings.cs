using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ScreenMode { FullScreenStretched, FillScreenWindowed, Window };

    // Класс настроек
    internal sealed class Settings
    {
        private bool playMusic;
        private int volumeSound;
        private int volumeMusic;

        public Settings()
        {
            LoadSettings();
        }

        internal bool ShowSplashVideo { get; set; }
        internal bool FullScreenMode { get; set; }
        internal bool StretchControlsInFSMode { get; set; }
        internal bool CheckUpdateOnStartup { get; set; }
        internal bool BattlefieldShowPath { get; set; }
        internal bool BattlefieldShowGrid { get; set; }
        internal string DirectoryAvatar { get; set; } = "";
        internal bool PlaySound { get; set; }
        internal bool PlayMusic
        {
            get => playMusic;
            set
            {
                if (playMusic != value)
                {
                    playMusic = value;
                    PlayMusicChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        internal int VolumeSound
        {
            get => volumeSound;
            set
            {
                if (volumeSound != value)
                {
                    volumeSound = value;
                    VolumeSoundChanged?.Invoke(this, new EventArgs());
                }
}
        }
        internal int VolumeMusic
        {
            get => volumeMusic;
            set
            {
                if (volumeMusic != value)
                {
                    volumeMusic = value;
                    VolumeMusicChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        internal bool ShowShortNames { get; set; }
        internal bool ShowQuantityDaysForExecuting { get; set; }
        internal bool ShowTypeCellMenu { get; set; }
        internal bool HideFulfilledRequirements { get; set; }
        internal bool ShowExtraHint { get; set; }
        internal bool AllowCheating { get; set; }

        internal event EventHandler PlayMusicChanged;
        internal event EventHandler VolumeSoundChanged;
        internal event EventHandler VolumeMusicChanged;

        internal void SetDefault()
        {
            ShowSplashVideo = true;
            FullScreenMode = true;
            StretchControlsInFSMode = true;
            CheckUpdateOnStartup = true;
            BattlefieldShowPath = false;
            BattlefieldShowGrid = false;
            PlaySound = true;
            PlayMusic = true;
            VolumeSound = 50;
            VolumeMusic = 50;
            ShowShortNames = false;
            ShowQuantityDaysForExecuting = true;
            ShowTypeCellMenu = true;
            HideFulfilledRequirements = true;
            ShowExtraHint = true;
            AllowCheating = false;
        }

        internal void LoadSettings()
        {
            SetDefault();

            if (File.Exists(Program.FolderResources + "Settings.xml"))
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(Program.FolderResources + "Settings.xml");

                    ShowSplashVideo = XmlUtils.GetBoolean(doc, "Settings/Game/ShowSplashVideo", ShowSplashVideo);
                    FullScreenMode = XmlUtils.GetBoolean(doc, "Settings/Game/FullScreenMode", FullScreenMode);
                    StretchControlsInFSMode = XmlUtils.GetBoolean(doc, "Settings/Game/StretchControlsInFSMode", StretchControlsInFSMode);
                    CheckUpdateOnStartup = XmlUtils.GetBoolean(doc, "Settings/Game/CheckUpdatesOnStartup", CheckUpdateOnStartup);

                    BattlefieldShowPath = XmlUtils.GetBoolean(doc, "Settings/Battlefield/ShowPath", BattlefieldShowPath);
                    BattlefieldShowGrid = XmlUtils.GetBoolean(doc, "Settings/Battlefield/ShowGrid", BattlefieldShowGrid);

                    PlaySound = XmlUtils.GetBoolean(doc, "Settings/Sound/PlaySound", PlaySound);
                    PlayMusic = XmlUtils.GetBoolean(doc, "Settings/Sound/PlayMusic", PlayMusic);
                    VolumeSound = XmlUtils.GetInteger(doc, "Settings/Sound/VolumeSound");
                    VolumeMusic = XmlUtils.GetInteger(doc, "Settings/Sound/VolumeMusic");
                    Debug.Assert(volumeSound >= 0);
                    Debug.Assert(volumeSound <= 100);
                    Debug.Assert(volumeMusic >= 0);
                    Debug.Assert(volumeMusic >= 0);

                    ShowShortNames = XmlUtils.GetBoolean(doc, "Settings/Interface/ShowShortNames", ShowShortNames);
                    ShowQuantityDaysForExecuting = XmlUtils.GetBoolean(doc, "Settings/Interface/ShowQuantityDaysForExecuting", ShowQuantityDaysForExecuting);
                    ShowTypeCellMenu = XmlUtils.GetBoolean(doc, "Settings/Interface/ShowTypeCellMenu", ShowTypeCellMenu);
                    HideFulfilledRequirements = XmlUtils.GetBoolean(doc, "Settings/Interface/HideFulfilledRequirements", HideFulfilledRequirements);
                    ShowExtraHint = XmlUtils.GetBoolean(doc, "Settings/Interface/ShowExtraHint", ShowExtraHint);
                    AllowCheating = XmlUtils.GetBoolean(doc, "Settings/Interface/AllowCheating", AllowCheating);

                    DirectoryAvatar = XmlUtils.GetString(doc, "Settings/Player/DirectoryAvatar");
                }
                catch (Exception e)
                {
                    GuiUtils.ShowException(e);

                    SetDefault();
                }
            }
        }

        internal void SaveSettings()
        {
            XmlTextWriter textWriter = new XmlTextWriter(Program.FolderResources + "Settings.xml", Encoding.UTF8);
            textWriter.WriteStartDocument();
            textWriter.Formatting = Formatting.Indented;

            textWriter.WriteStartElement("Settings");

            // Записываем информацию о настройках игры
            textWriter.WriteStartElement("Game");
            textWriter.WriteElementString("ShowSplashVideo", ShowSplashVideo.ToString());
            textWriter.WriteElementString("FullScreenMode", FullScreenMode.ToString());
            textWriter.WriteElementString("StretchControlsInFSMode", StretchControlsInFSMode.ToString());
            textWriter.WriteElementString("CheckUpdatesOnStartup", CheckUpdateOnStartup.ToString());
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Battlefield");
            textWriter.WriteElementString("ShowPath", BattlefieldShowPath.ToString());
            textWriter.WriteElementString("ShowGrid", BattlefieldShowGrid.ToString());
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Sound");
            textWriter.WriteElementString("PlaySound", PlaySound.ToString());
            textWriter.WriteElementString("PlayMusic", PlayMusic.ToString());
            textWriter.WriteElementString("VolumeSound", VolumeSound.ToString());
            textWriter.WriteElementString("VolumeMusic", VolumeMusic.ToString());
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Interface");
            textWriter.WriteElementString("ShowShortNames", ShowShortNames.ToString());
            textWriter.WriteElementString("ShowQuantityDaysForExecuting", ShowQuantityDaysForExecuting.ToString());
            textWriter.WriteElementString("ShowTypeCellMenu", ShowTypeCellMenu.ToString());
            textWriter.WriteElementString("HideFulfilledRequirements", HideFulfilledRequirements.ToString());
            textWriter.WriteElementString("ShowExtraHint", ShowExtraHint.ToString());
            textWriter.WriteElementString("AllowCheating", AllowCheating.ToString());

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }

        internal ScreenMode ScreenMode()
        {
            if (FullScreenMode)
                return StretchControlsInFSMode ? Fantasy_Kingdoms_Battle.ScreenMode.FullScreenStretched : Fantasy_Kingdoms_Battle.ScreenMode.FillScreenWindowed;

            return Fantasy_Kingdoms_Battle.ScreenMode.Window;
        }
    }
}
