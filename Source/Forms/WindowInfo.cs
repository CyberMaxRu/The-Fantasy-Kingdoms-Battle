using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowInfo : VCForm
    {
        private readonly VCButton btnOk;
        private readonly VCText lblText;

        public WindowInfo(string caption, string text) : base()
        {
            windowCaption.Caption = caption;

            lblText = new VCText(ClientControl, 0, 0, Program.formMain.FontParagraph, Color.White, ClientControl.Width);
            lblText.Text = text;
            lblText.Height = lblText.MinHeigth();

            btnOk = new VCButton(ClientControl, 0, 100, "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;

            AcceptButton = btnOk;
            CancelButton = btnOk;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
        }

        internal static void ShowInfo(string caption, string text)
        {
            WindowInfo wc = new WindowInfo(caption, text);
            wc.ShowDialog();
            wc.Dispose();
        }
    }
}
