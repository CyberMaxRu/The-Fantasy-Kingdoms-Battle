using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class FormConfirmExit : VCForm
    {
        private readonly VCButton btnOk;
        private readonly VCButton btnCancel;
        private readonly VCTextM2 textConfirm;

        public FormConfirmExit() : base()
        {
            windowCaption.Caption = "Выход из программы";

            textConfirm = new VCTextM2(ClientControl, 0, 0, Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            textConfirm.Text = "Выход приведет к потере текущей игры.\r\nПродолжить?";

            btnOk = new VCButton(ClientControl, 0, 100, "Да");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            btnOk.Default = true;
            btnCancel = new VCButton(ClientControl, 200, 100, "Нет");
            btnCancel.Width = 160;
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm(DialogResult.No);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogResult.Yes);
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
            btnCancel.ShiftX = ClientControl.Width - btnCancel.Width;
            btnCancel.ShiftY = btnOk.ShiftY;
        }
    }
}