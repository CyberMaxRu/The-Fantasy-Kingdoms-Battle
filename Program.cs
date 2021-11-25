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
                new FormMain();
                Application.Run(formMain);
            }
            catch (Exception exc)
            {
                string stackTrace = exc.StackTrace.Replace(@"F:\Projects\C-Sharp\Fantasy King's Battle\", "");

                StreamWriter sw = File.AppendText(Environment.CurrentDirectory + @"\debug.log");
                sw.WriteLine(DateTime.Now.ToString() + ": v" + Application.ProductVersion);
                sw.WriteLine(exc.Message);
                sw.WriteLine(stackTrace);
                sw.Close();
                sw.Dispose();

                MessageBox.Show("Произошло исключение: "
                    + (exc.Message.Length > 0 ? Environment.NewLine + Environment.NewLine + exc.Message : "") + Environment.NewLine + Environment.NewLine + stackTrace, FormMain.NAME_PROJECT + " " + FormMain.VERSION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }
        }
    }
}
