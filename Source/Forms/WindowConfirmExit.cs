using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowConfirmExit : WindowConfirm
    {
        public WindowConfirmExit() : base("Выход из программы", "Выход приведет к потере текущей игры.\r\nПродолжить?")
        {
        }
    }
}