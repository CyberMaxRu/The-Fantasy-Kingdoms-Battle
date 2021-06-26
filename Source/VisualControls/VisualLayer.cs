using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальное окно
    internal class VisualLayer
    {
        public VisualLayer(string name)
        {
            Name = name;
        }

        internal string Name { get; }
        internal List<VisualControl> Controls { get; } = new List<VisualControl>();

        internal void DrawBackground(Graphics g)
        {
            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.DrawBackground(g);
            }
        }

        internal void Draw(Graphics g)
        {
            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.Draw(g);
            }
        }

        internal virtual void KeyUp(KeyEventArgs e)
        {
            foreach (VisualControl vc in Controls)
                vc.KeyUp(e);
        }

        internal virtual void KeyPress(KeyPressEventArgs e)
        {
            foreach (VisualControl vc in Controls)
                vc.KeyPress(e);
        }

        internal void AddControl(VisualControl vc)
        {
            Debug.Assert(vc != null);
            Debug.Assert(Controls.IndexOf(vc) == -1);

            Controls.Add(vc);
        }
    }
}
