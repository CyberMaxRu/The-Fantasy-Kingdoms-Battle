using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс настроек
    internal sealed class Settings
    {
        public Settings(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path + "Settings.xml");

            ShowSplashVideo = XmlUtils.GetParamFromXmlBool(doc.SelectSingleNode("Settings/Options/ShowSplashVideo"), true);
            FullScreenMode = XmlUtils.GetParamFromXmlBool(doc.SelectSingleNode("Settings/Options/FullScreenMode"), false);
            CheckUpdateOnStartup = XmlUtils.GetParamFromXmlBool(doc.SelectSingleNode("Settings/Options/CheckUpdatesOnStartup"), true);

            NamePlayer = XmlUtils.GetParamFromXmlString(doc.SelectSingleNode("Settings/Player/Name"));
            if (NamePlayer.Length == 0)
                NamePlayer = "Игрок №1";
            if (NamePlayer.Length > 31)
                throw new Exception("Длина имени игрока более 31 символа.");

            IndexAvatar = XmlUtils.GetParamFromXmlInteger(doc.SelectSingleNode("Settings/Player/IndexAvatar"));
            if (IndexAvatar < -1)
                IndexAvatar = 0;
            //if (IndexAvatar >= Program.formMain.ilPlayerAvatars.Images.Count)
            //    IndexAvatar = Program.formMain.ilPlayerAvatars.Images.Count - 1;
        }

        internal bool ShowSplashVideo { get; private set; }
        internal bool FullScreenMode { get; private set; }
        internal bool CheckUpdateOnStartup { get; private set; }
        internal string NamePlayer { get; private set; }
        internal int IndexAvatar { get; private set; }
    }
}
