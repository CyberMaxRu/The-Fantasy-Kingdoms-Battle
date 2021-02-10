using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Fantasy_Kingdoms_Battle
{
    public class WebUtils
    {
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
                GuiUtils.ShowError(e.Message);
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
