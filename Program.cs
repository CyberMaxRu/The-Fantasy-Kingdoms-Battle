using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                // Настройка переменной с папкой ресурсов
                WorkFolder = Environment.CurrentDirectory;

                if (WorkFolder.Contains("Debug"))
                    WorkFolder = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
                else if (WorkFolder.Contains("Release"))
                    WorkFolder = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 11);
                else
                    WorkFolder += @"\";

                FolderResources = Directory.Exists(WorkFolder + @"User_mods\Main") ? WorkFolder + @"User_mods\Main\" : WorkFolder + @"Resources\";
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

        internal static string WorkFolder { get; private set; }
        internal static string FolderResources { get; private set; }
    }
}
