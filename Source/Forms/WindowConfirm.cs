using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal class WindowConfirm : WindowOkCancel
    {
        private readonly VCText textConfirm;

        public WindowConfirm(string caption, string text) : base(caption)
        {
            textConfirm = new VCText(ClientControl, 0, 0, Program.formMain.FontParagraph, Color.White, ClientControl.Width)
            {
                Text = text
            };
            textConfirm.SetMinHeight();
        }

        internal static bool ShowConfirm(string caption, string text)
        {
            WindowConfirm wc = new WindowConfirm(caption, text);
            bool res = wc.ShowDialog() == DialogAction.OK;
            wc.Dispose();

            return res;
        }
    }
}
