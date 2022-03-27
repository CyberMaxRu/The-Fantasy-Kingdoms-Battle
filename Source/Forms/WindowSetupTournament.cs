using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowSetupTournament : WindowOkCancel
    {

        public WindowSetupTournament() : base("Настройка турнира")
        {
            btnOk.Caption = "ОК";
            btnCancel.Caption = "Отмена";
        }
    }
}
