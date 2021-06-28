using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Окно выбора стартового бонуса
    internal sealed class WindowSelectStartBonus : VCForm
    {
        private readonly VCButton btnOk;

        public WindowSelectStartBonus() : base()
        {
            windowCaption.Caption = "Выбор стартового бонуса";

            btnOk = new VCButton(ClientControl, 0, 100, "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;

            AcceptButton = btnOk;
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }
    }
}
