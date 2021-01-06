using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fantasy_King_s_Battle
{
    // Класс настроек
    internal sealed class Settings
    {
        private string pathToResources;
        public Settings(string path)
        {
            pathToResources = path;

            XmlDocument doc = new XmlDocument();
            doc.Load(path + "Settings.xml");

            ShowSplashVideo = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Game/ShowSplashVideo"), true);
            FullScreenMode = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Game/FullScreenMode"), false);
            CheckUpdateOnStartup = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Game/CheckUpdatesOnStartup"), true);

            BattlefieldShowPath = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Battlefield/ShowPath"), false);
            BattlefieldShowGrid = XmlUtils.GetBool(doc.SelectSingleNode("Settings/Battlefield/ShowGrid"), false);

            NamePlayer = XmlUtils.GetString(doc.SelectSingleNode("Settings/Player/Name"));
            if (NamePlayer.Length == 0)
                NamePlayer = "Игрок №1";
            if (NamePlayer.Length > 31)
                throw new Exception("Длина имени игрока более 31 символа.");

            IndexInternalAvatar = XmlUtils.GetInteger(doc.SelectSingleNode("Settings/Player/IndexAvatar"));
            if (IndexInternalAvatar < -1)
                IndexInternalAvatar = 0;
            FileNameAvatar = XmlUtils.GetString(doc.SelectSingleNode("Settings/Player/FileNameAvatar"));
            DirectoryAvatar = XmlUtils.GetString(doc.SelectSingleNode("Settings/Player/DirectoryAvatar"));
            //if (IndexAvatar >= Program.formMain.ilPlayerAvatars.Images.Count)
            //    IndexAvatar = Program.formMain.ilPlayerAvatars.Images.Count - 1;
        }

        internal bool ShowSplashVideo { get; set; }
        internal bool FullScreenMode { get; set; }
        internal bool CheckUpdateOnStartup { get; set; }
        internal bool BattlefieldShowPath { get; set; }
        internal bool BattlefieldShowGrid { get; set; }
        internal string NamePlayer { get; set; }
        internal int IndexInternalAvatar { get; set; }
        internal string FileNameAvatar { get; set; }
        internal string DirectoryAvatar { get; set; }
        internal Bitmap Avatar { get; private set; }

        internal void LoadAvatar()
        {
            Avatar?.Dispose();
            if (FileNameAvatar.Length == 0)
                Avatar = null;
            else
            {
                Avatar = GuiUtils.PrepareAvatar(FileNameAvatar);
                if (Avatar == null)
                    FileNameAvatar = "";
            }
        }

        internal void SaveSettings()
        {
            XmlTextWriter textWriter = new XmlTextWriter(pathToResources + "Settings.xml", Encoding.UTF8);
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

            textWriter.WriteStartElement("Player");
            textWriter.WriteElementString("Name", NamePlayer);
            textWriter.WriteElementString("IndexAvatar", IndexInternalAvatar.ToString());
            textWriter.WriteElementString("FileNameAvatar", FileNameAvatar);
            textWriter.WriteElementString("DirectoryAvatar", DirectoryAvatar);
            textWriter.WriteEndElement();

            textWriter.WriteEndElement();
            textWriter.Close();
            textWriter.Dispose();
        }

        internal int IndexAvatar()
        {
            return FileNameAvatar.Length > 0 ? Program.formMain.AvatarCount - 1 : IndexInternalAvatar;
        }
    }
}
