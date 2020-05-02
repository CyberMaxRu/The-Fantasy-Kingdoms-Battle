using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class GuiUtils
    {

        internal static Label CreateLabel(Control parent, int left, int top)
        {
            Label l = new Label()
            {
                Parent = parent,
                Left = left,
                Top = top
            };

            return l;
        }
    }
}
