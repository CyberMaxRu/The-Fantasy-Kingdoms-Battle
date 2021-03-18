using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    public struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }

    internal sealed class CustomCursor
    {
        private static Cursor c;

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        internal static void CreateCursor(string filename)
        {
            Bitmap bmp = new Bitmap(filename);
            /*Icon icon = Icon.FromHandle(img.GetHicon());
            Cursor cur = new Cursor(icon.Handle);
            Point p = cur.HotSpot;*/

            IconInfo tmp = new IconInfo();
            GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.xHotspot = 0;
            tmp.yHotspot = 0;
            tmp.fIcon = false;

            c = new Cursor(CreateIconIndirect(ref tmp));
        }

        internal static Cursor GetCursor()
        {
            return c;
        }
    }
}
