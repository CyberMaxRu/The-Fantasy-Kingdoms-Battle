﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace Fantasy_Kingdoms_Battle
{
    static class Program
    {
        public static FormMain formMain;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool debugMode = false;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Настройка переменной с папкой ресурсов
            WorkFolder = Environment.CurrentDirectory;

            if (WorkFolder.Contains("Debug"))
            {
                WorkFolder = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9 - 15);
                debugMode = true;
            }
            else if (WorkFolder.Contains("Release"))
                WorkFolder = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
            else
                WorkFolder += @"\";

            FolderResources = WorkFolder + @"Resources\";
            if (debugMode)
            {
                new FormMain();
                Application.Run(formMain);
            }
            else
            {
                try
                {
                    new FormMain();
                    Application.Run(formMain);
                }
                catch (Exception exc)
                {
                    string stackTrace = exc.StackTrace.Replace(@"F:\Projects\C-Sharp\Fantasy King's Battle\", "");

                    StreamWriter sw = File.AppendText(Environment.CurrentDirectory + @"\Exceptions.log");
                    sw.WriteLine(DateTime.Now.ToString() + ": v" + Application.ProductVersion);
                    sw.WriteLine(exc.Message);
                    sw.WriteLine(stackTrace);
                    sw.Close();
                    sw.Dispose();

                    MessageBox.Show("Произошло исключение: "
                        + (exc.Message.Length > 0 ? Environment.NewLine + Environment.NewLine + exc.Message : "") + Environment.NewLine + Environment.NewLine + stackTrace, FormMain.NAME_PROJECT + " " + FormMain.VERSION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Debug.Fail(exc.Message);
                    Environment.Exit(-1);
                }
            }
        }

        internal static string WorkFolder { get; private set; }
        internal static string FolderResources { get; private set; }
    }
}
