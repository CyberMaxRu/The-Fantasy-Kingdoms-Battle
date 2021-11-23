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
    // Класс настроек
    internal sealed class Settings
    {
        private bool playMusic;

        public Settings()
        {
            SetDefault();

            if (File.Exists(Program.formMain.dirResources + "Settings.xml"))
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(Program.formMain.dirResources + "Settings.xml");

                    ShowSplashVideo = XmlUtils.GetBoolean(doc, "Settings/Game/ShowSplashVideo", ShowSplashVideo);
                    FullScreenMode = XmlUtils.GetBoolean(doc, "Settings/Game/FullScreenMode", FullScreenMode);
                    CheckUpdateOnStartup = XmlUtils.GetBoolean(doc, "Settings/Game/CheckUpdatesOnStartup", CheckUpdateOnStartup);

                    BattlefieldShowPath = XmlUtils.GetBoolean(doc, "Settings/Battlefield/ShowPath", BattlefieldShowPath);
                    BattlefieldShowGrid = XmlUtils.GetBoolean(doc, "Settings/Battlefield/ShowGrid", BattlefieldShowGrid);

                    PlaySound = XmlUtils.GetBoolean(doc, "Settings/Sound/PlaySound", PlaySound);
                    PlayMusic = XmlUtils.GetBoolean(doc, "Settings/Sound/PlayMusic", PlayMusic);

                    ShowShortNames = XmlUtils.GetBoolean(doc, "Settings/Interface/ShowShortNames", ShowShortNames);
                    ShowTypeCellMenu = XmlUtils.GetBoolean(doc, "Settings/Interface/ShowTypeCellMenu", ShowTypeCellMenu);
                    HideFulfilledRequirements = XmlUtils.GetBoolean(doc, "Settings/Interface/HideFulfilledRequirements", HideFulfilledRequirements);

                    DirectoryAvatar = XmlUtils.GetString(doc, "Settings/Player/DirectoryAvatar");
                }
                catch (Exception e)
                {
                    GuiUtils.ShowException(e);

                   SetDefault();
                }
            }
        }

        internal bool ShowSplashVideo { get; set; }
        internal bool FullScreenMode { get; set; }
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
        internal bool ShowShortNames { get; set; }
        internal bool ShowTypeCellMenu { get; set; }
        internal bool HideFulfilledRequirements { get; set; }

        internal event EventHandler PlayMusicChanged;

        internal void SetDefault()
        {
            ShowSplashVideo = true;
            FullScreenMode = true;
            CheckUpdateOnStartup = true;
            BattlefieldShowPath = false;
            BattlefieldShowGrid = false;
            PlaySound = true;
            PlayMusic = true;
            ShowShortNames = false;
            ShowTypeCellMenu = true;
            HideFulfilledRequirements = true;
        }

        internal void SaveSettings()
        {
            XmlTextWriter textWriter = new XmlTextWriter(Program.formMain.dirResources + "Settings.xml", Encoding.UTF8);
            textWriter.WriteStartDocument();
            textWriter.Formatting = Formatting.Indented;

            textWriter.WriteStartElement("Settings");

            // Записываем информацию о настройках игры
            textWriter.WriteStartElement("Game");
            textWriter.WriteElementString("ShowSplashVideo", ShowSplashVideo.ToString());
            textWriter.WriteElementString("FullScreenMode", FullScreenMode.ToString());
            textWriter.WriteElementString("CheckUpdatesOnStartup", CheckUpdateOnStartup.ToString());
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Battlefield");
            textWriter.WriteElementString("ShowPath", BattlefieldShowPath.ToString());
            textWriter.WriteElementString("ShowGrid", BattlefieldShowGrid.ToString());
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Sound");
            textWriter.WriteElementString("PlaySound", PlaySound.ToString());
            textWriter.WriteElementString("PlayMusic", PlayMusic.ToString());
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Interface");
            textWriter.WriteElementString("ShowShortNames", ShowShortNames.ToString());
            textWriter.WriteElementString("ShowTypeCellMenu", ShowTypeCellMenu.ToString());
            textWriter.WriteElementString("HideFulfilledRequirements", HideFulfilledRequirements.ToString());

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }
    }
}
