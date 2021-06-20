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

                    ShowSplashVideo = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Game/ShowSplashVideo"), ShowSplashVideo);
                    FullScreenMode = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Game/FullScreenMode"), FullScreenMode);
                    CheckUpdateOnStartup = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Game/CheckUpdatesOnStartup"), CheckUpdateOnStartup);

                    BattlefieldShowPath = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Battlefield/ShowPath"), BattlefieldShowPath);
                    BattlefieldShowGrid = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Battlefield/ShowGrid"), BattlefieldShowGrid);

                    IndexInternalAvatar = XmlUtils.GetInteger(doc.SelectSingleNode("Settings/Player/IndexAvatar"));
                    if (IndexInternalAvatar < -1)
                        IndexInternalAvatar = 0;
                    DirectoryAvatar = XmlUtils.GetString(doc.SelectSingleNode("Settings/Player/DirectoryAvatar"));
                }
                catch (Exception e)
                {
                    GuiUtils.ShowException(e);

                   SetDefault();
                }
            }
            //if (IndexAvatar >= Program.formMain.ilPlayerAvatars.Images.Count)
            //    IndexAvatar = Program.formMain.ilPlayerAvatars.Images.Count - 1;
        }

        internal bool ShowSplashVideo { get; set; }
        internal bool FullScreenMode { get; set; }
        internal bool CheckUpdateOnStartup { get; set; }
        internal bool BattlefieldShowPath { get; set; }
        internal bool BattlefieldShowGrid { get; set; }
        internal int IndexInternalAvatar { get; set; }
        internal string DirectoryAvatar { get; set; } = "";
        internal Bitmap Avatar { get; private set; }

        internal void SetDefault()
        {
            ShowSplashVideo = true;
            FullScreenMode = true;
            CheckUpdateOnStartup = true;
            BattlefieldShowPath = false;
            BattlefieldShowGrid = false;
        }

        internal void LoadAvatar()
        {
/*            Avatar?.Dispose();
            if (FileNameAvatar.Length == 0)
            {
                Avatar = null;
                Program.formMain.imListObjectsBig.ReplaceImage(Program.formMain.blInternalAvatars.GetImage(0, true, false), Program.formMain.ImageIndexFirstAvatar);
                Program.formMain.imListObjectsCell?.ReplaceImageWithResize(Program.formMain.imListObjectsBig, Program.formMain.ImageIndexFirstAvatar, 1, Program.formMain.bmpMaskSmall);
            }
            else
            {
                Avatar = GuiUtils.PrepareAvatar(FileNameAvatar);
                if (Avatar != null)
                {
                    Program.formMain.imListObjectsBig.ReplaceImage(Avatar, Program.formMain.ImageIndexFirstAvatar);
                    Program.formMain.imListObjectsCell?.ReplaceImageWithResize(Program.formMain.imListObjectsBig, Program.formMain.ImageIndexFirstAvatar, 1, Program.formMain.bmpMaskSmall);
                }
                else
                    FileNameAvatar = "";
            }*/
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
