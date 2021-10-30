using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

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
            catch(Exception e)
            {
                Debug.WriteLine(DateTime.Now.ToString() + ": " + Application.ProductVersion);
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(Environment.NewLine);
                throw;
            }
        }
    }
}
