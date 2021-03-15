using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class FormConfirmExit : VCForm
    {
        private readonly VCButton bntOk;
        private readonly VCButton bntCancel;

        public FormConfirmExit() : base()
        {
            windowCaption.Caption = "Выход из программы";

            bntOk = new VCButton(ClientControl, 0, 100, "Да");
            bntOk.Width = 160;
            bntCancel = new VCButton(ClientControl, 200, 100, "Нет");
            bntCancel.Width = 160;
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            bntOk.ShiftY = ClientControl.Height - bntOk.Height;
            bntCancel.ShiftX = ClientControl.Width - bntCancel.Width;
            bntCancel.ShiftY = bntOk.ShiftY;
        }
    }
}