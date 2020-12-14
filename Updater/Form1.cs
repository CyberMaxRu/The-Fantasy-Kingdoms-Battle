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
using System.Diagnostics;
using System.Security.AccessControl;
using System.IO;
using System.Net;

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

            labelAction.Text = "";

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
                currentVersion = GetVersionFromXml(xmlDoc.SelectSingleNode("Main/Version"));

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
                ShowError(exc.Message);
                Close();
            }

            if ((currentVersion.Major == 0) && (currentVersion.Minor == 0) && (currentVersion.Build == 0))
            {
                ShowError("Не найден текущий номер версии.");
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

            // Показываем историю изменений текущей версии
            string filenameVersion = Environment.CurrentDirectory + @"\ActualVersion.xml";
            XmlDocument xmlDocHistory = new XmlDocument();
            xmlDocHistory.Load(filenameVersion);
            ShowChanges(xmlDocHistory);
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

            if (DownloadFile(URLDrive, UIDVersion, filenameVersion))
            {
                // Смотрим, какая там последняя версия
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filenameVersion);

                    actualVersion = GetVersionFromXml(xmlDoc.SelectSingleNode("Versions/ActualVersion"));
                    if (actualVersion > currentVersion)
                    {
                        SetState("Найдена новая версия: " + actualVersion.ToString());
                        button1.Text = "Обновить игру";
                        needUpdate = true;
                        ShowChanges(xmlDoc);
                    }
                    else if (actualVersion == currentVersion)
                    {
                        labelAction.Text = "Установлена актуальная версия игры";
                    }
                    else
                    {
                        labelAction.Text = "У вас более новая версия игры, чем должна была быть!";
                    }
                }
                catch (Exception exc)
                {
                    ShowError(exc.Message);
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
                v = GetVersionFromXml(n);
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

        public void AddFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {

            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);
        }

        public void AddDirectorySecurity(string dirName, string account, FileSystemRights rights, AccessControlType controlType)
        {

            // Get a FileSecurity object that represents the
            // current security settings.
            DirectorySecurity dSecurity = Directory.GetAccessControl(dirName);

            // Add the FileSystemAccessRule to the security settings.
            dSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));

            // Set the new access settings.
            Directory.SetAccessControl(dirName, dSecurity);
        }

        private void Update()
        {
            bool b = false;
            //получаем имя компьютора и пользователя
            System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            string user = wi.Name;

            try
            {
                // Add the access control entry to the file
                AddDirectorySecurity(Environment.CurrentDirectory, @user, FileSystemRights.FullControl, AccessControlType.Allow);

                b = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (!b)
                return;

            string pathUpdate = Environment.CurrentDirectory + @"\update";
            // Скачиваем архив
            SetState("Скачиваем архив с обновлением...");

            string filenameZip = Environment.CurrentDirectory + @"\update.zip";
            if (DownloadFile(URLDrive, UIDArchive, filenameZip))
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
                    if (Path.GetFileName(newName) == "Settings.xml")// Настройки пропускаем, не заменяем
                        continue;
                    if (Path.GetFileName(newName) == "Updater.exe")
                        newName += ".new";
                    if (System.IO.File.Exists(newName))
                    {
                        AddFileSecurity(newName, @user, FileSystemRights.FullControl, AccessControlType.Allow);
                        System.IO.File.Delete(newName);
                    }
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
            labelAction.Text = text;
            labelAction.Refresh();
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static Version GetVersionFromXml(XmlNode n)
        {
            return new Version(Convert.ToByte(n.SelectSingleNode("Major").InnerText),
                Convert.ToByte(n.SelectSingleNode("Minor").InnerText),
                Convert.ToByte(n.SelectSingleNode("Build").InnerText));
        }

        public static bool DownloadFile(string urlDrive, string uid, string filename)
        {
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile("https://" + urlDrive + "&id=" + uid, filename);
                return true;
            }
            catch (Exception e)
            {
                ShowError(e.Message);
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
