using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class WindowOkCancel : VCForm
    {
        protected readonly VCButton btnOk;
        protected readonly VCButton btnCancel;

        public WindowOkCancel(string caption) : base()
        {
            windowCaption.Caption = caption;

            btnOk = new VCButton(ClientControl, 0, 100, "Да");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            btnCancel = new VCButton(ClientControl, 200, 100, "Нет");
            btnCancel.Width = 160;
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        internal event EventHandler OnClose;

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.OK);
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
            btnCancel.ShiftX = ClientControl.Width - btnCancel.Width;
            btnCancel.ShiftY = btnOk.ShiftY;
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            if (da == DialogAction.OK)
                OnClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
