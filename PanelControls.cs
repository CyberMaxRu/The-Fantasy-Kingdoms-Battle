using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal sealed class PanelControls
    {
        private List<Control> Controls { get; } = new List<Control>();
        private Control Parent;
        private int Left;
        private int Top;

        public PanelControls(Control parent, int left, int top)
        {
            Parent = parent;
            Left = left;
            Top = top;
        }

        internal void AddControl(Control c)
        {
            Debug.Assert(c != null);

            Controls.Add(c);
            c.Left += Left;
            c.Top += Top;
            c.Parent = Parent;
            c.Visible = false;
        }

        internal void SetVisible(bool visible)
        {
            foreach(Control c in Controls)
            {
                if (c.Tag == null)
                    c.Visible = visible;
            }
        }
    }
}
