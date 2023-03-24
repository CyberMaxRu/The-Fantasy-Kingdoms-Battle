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

        internal event EventHandler OnClose;

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            if (da == DialogAction.OK)
                 OnClose?.Invoke(this, EventArgs.Empty);
        }

        internal static void ShowConfirm(string caption, string text, EventHandler onClose)
        {
            WindowConfirm wc = new WindowConfirm(caption, text);
            wc.OnClose += onClose;
            wc.ShowDialog();
        }
    }
}
