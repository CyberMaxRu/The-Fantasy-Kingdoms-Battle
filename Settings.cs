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

        internal void SetDefault()
        {
            ShowSplashVideo = true;
            FullScreenMode = true;
            CheckUpdateOnStartup = true;
            BattlefieldShowPath = false;
            BattlefieldShowGrid = false;
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

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }
    }
}
