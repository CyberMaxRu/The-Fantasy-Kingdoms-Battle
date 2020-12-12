using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO.Compression;
using Fantasy_King_s_Battle;
using System.Diagnostics;

namespace Updater
{
    public partial class Form1 : Form
    {
        private string dirResources;
        private Version currentVersion;
        private Version actualVersion;
        private string curVersionRelease;
        private string URLDrive;
        private string UIDVersion;
        private string UIDArchive;
        private bool needUpdate = false;
        private bool autoUpdate = false;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Настройка переменной с папкой ресурсов
            dirResources = Environment.CurrentDirectory;

            if (dirResources.Contains("Debug"))
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
            else if (dirResources.Contains("Release"))
                dirResources = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
            else
                dirResources += "\\";

            dirResources += "Resources\\";

            // Открываем файл main.xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dirResources + "Main.xml");
            try
            {
                currentVersion = XmlUtils.GetVersionFromXml(xmlDoc.SelectSingleNode("Main/Version"));

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
            catch (Exception exc)
            {
                GuiUtils.ShowError(exc.Message);
                Close();
            }

            if ((currentVersion.Major == 0) && (currentVersion.Minor == 0) && (currentVersion.Build == 0))
            {
                GuiUtils.ShowError("Не найден текущий номер версии.");
                Close();
            }

            labelVersion.Text = "Текущая версия: " + currentVersion + ".";

            foreach (string s in Environment.GetCommandLineArgs())
            {
                if (s == "-silence")
                {
                    autoUpdate = true;
                    CheckUpdate();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (needUpdate)
                Update();
            else
                CheckUpdate();
        }

        private void CheckUpdate()
        {
            // Скачиваем файл с версиями
            string filenameVersion = Environment.CurrentDirectory + @"\ActualVersion.xml";
            SetState("Скачиваем файл с версиями...");

            if (WebUtils.DownloadFile(URLDrive, UIDVersion, filenameVersion))
            {
                // Смотрим, какая там последняя версия
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filenameVersion);

                    actualVersion = XmlUtils.GetVersionFromXml(xmlDoc.SelectSingleNode("Versions/ActualVersion"));
                    if (actualVersion > currentVersion)
                    {
                        SetState("Найдена новая версия: " + actualVersion.ToString());
                        button1.Text = "Обновить игру";
                        needUpdate = true;
                        ShowChanges(xmlDoc);
                    }
                    else if (actualVersion == currentVersion)
                    {
                        labelActualVersion.Text = "У вас актуальная версия игры";
                    }
                    else
                    {
                        labelActualVersion.Text = "У вас более новая версия игры, чем должна была быть!";
                    }
                }
                catch (Exception exc)
                {
                    GuiUtils.ShowError(exc.Message);
                }
            }
            else
            {
                labelActualVersion.Text = "Произошла ошибка при скачивании файла с версиями.";
            }
        }

        private void ShowChanges(XmlDocument d)
        {
            textBox1.Clear();

            XmlNode nc = d.SelectSingleNode("Versions");

            string changes = "";
            string line;
            Version v;
            foreach(XmlNode n in nc.SelectNodes("Version"))
            {
                v = XmlUtils.GetVersionFromXml(n);
                line = v.ToString() + " от " + n.SelectSingleNode("DateBuild").InnerText + Environment.NewLine;

                foreach (XmlNode ld in n.SelectNodes("Description/Line"))
                {
                    line += ld.InnerText + Environment.NewLine;
                }

                if (v > currentVersion)
                    changes = "---Изменения в установленной версии игры:" + Environment.NewLine + changes;
                changes = line + Environment.NewLine + changes;
            }

            textBox1.Text = changes;
        }

        private void Update()
        {
            string pathUpdate = Environment.CurrentDirectory + @"\update";
            // Скачиваем архив
            SetState("Скачиваем архив с обновлением...");

            string filenameZip = Environment.CurrentDirectory + @"\update.zip";
            if (WebUtils.DownloadFile(URLDrive, UIDArchive, filenameZip))
            {
                // Удаляем папку с обновлениями
                if (System.IO.Directory.Exists(pathUpdate))
                { 
                    SetState("Удаляем папку с обновлением...");
                    System.IO.Directory.Delete(pathUpdate, true);
                }

                // Распаковка архива
                SetState("Распаковываем архив...");
                ZipFile.ExtractToDirectory(filenameZip, pathUpdate);

                // Замена файлов
                SetState("Заменяем файлы...");
                string newName;
                foreach (string file in System.IO.Directory.EnumerateFiles(pathUpdate))
                {
                    newName = Environment.CurrentDirectory + @"\" + System.IO.Path.GetFileName(file);
                    if (System.IO.File.Exists(newName))
                        System.IO.File.Delete(newName);
                    System.IO.File.Move(file, newName);
                }

                string newdir;
                foreach (string dir in System.IO.Directory.EnumerateDirectories(pathUpdate))
                {
                    newdir = Environment.CurrentDirectory + dir.Substring(Environment.CurrentDirectory.Length + 7);
                    if (System.IO.Directory.Exists(newdir))
                        System.IO.Directory.Delete(newdir, true);
                    System.IO.Directory.Move(dir, newdir);
                }

                // Удаляем архив
                SetState("Удаляем папку и архив с обновлением...");
                if (System.IO.Directory.Exists(pathUpdate))
                    System.IO.Directory.Delete(pathUpdate, true);
                System.IO.File.Delete(filenameZip);

                // Обновление завершено
                SetState("Обновление завершено...");
            }
            else
            {
                labelActualVersion.Text = "Произошла ошибка при скачивании файла с обновлением.";
            }
        }

        private void SetState(string text)
        {
            labelActualVersion.Text = text;
            labelActualVersion.Refresh();
        }
    }
}
