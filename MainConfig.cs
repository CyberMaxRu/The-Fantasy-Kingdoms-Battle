using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    public sealed class MainConfig
    {
        public MainConfig(string dirResources)
        {
            // Открываем файл main.xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dirResources + "Main.xml");

            CurrentVersion = XmlUtils.GetVersionFromXml(xmlDoc.SelectSingleNode("Main"), "Version");
            ScreenMinSize = new Size(XmlUtils.GetIntegerNotNull(xmlDoc.SelectSingleNode("Main/ScreenMinWidth")), XmlUtils.GetIntegerNotNull(xmlDoc.SelectSingleNode("Main/ScreenMinHeight")));

            URLDrive = xmlDoc.SelectSingleNode("Main/AutoUpdate/URLDrive").InnerText;
            if (URLDrive.Length == 0)
                throw new Exception("Не указан путь к drive.google.com.");
            UIDVersion = xmlDoc.SelectSingleNode("Main/AutoUpdate/UIDVersion").InnerText;
            if (UIDVersion.Length == 0)
                throw new Exception("Не указан UID файла с версиями.");
            UIDArchive = xmlDoc.SelectSingleNode("Main/AutoUpdate/UIDArchive").InnerText;
            if (UIDArchive.Length == 0)
                throw new Exception("Не указан UID файла с обновлением.");
        }

        public Version CurrentVersion { get; private set; }
        public Version ActualVersion { get; private set; }
        internal Size ScreenMinSize { get; private set; }// Минимальное разрешение экрана для игры
        public string URLDrive { get; private set; }
        public string UIDVersion { get; private set; }
        public string UIDArchive { get; private set; }

        public bool CheckForNewVersion()
        {
            string filenameVersion = Environment.CurrentDirectory + @"\ActualVersion.xml";
            if (WebUtils.DownloadFile(URLDrive, UIDVersion, filenameVersion))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filenameVersion);

                ActualVersion = XmlUtils.GetVersionFromXml(xmlDoc.SelectSingleNode("Versions"), "ActualVersion");
                return ActualVersion > CurrentVersion;
            }
            else
                return false;
        }
    }
}
