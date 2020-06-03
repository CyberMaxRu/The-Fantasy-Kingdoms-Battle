using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal sealed class PanelPage : Label
    {
        public PanelPage()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            TextAlign = ContentAlignment.TopCenter;
            Visible = false;
        }
    }
}
