using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Класс кнопки для меню
    internal sealed class VCButtonForMenu : VCButton
    {
        public VCButtonForMenu(VisualControl parent, int shiftY, string caption, EventHandler onClick) : base(parent, FormMain.Config.ShiftXButtonsInMenu, shiftY, caption)
        {
            Width = parent.Width - (ShiftX * 2);
            Click += onClick;
        }
    }
}
