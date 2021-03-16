using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Слой визуальных контролов
    internal sealed class VisualLayer
    {
        internal List<VisualControl> Controls { get; } = new List<VisualControl>();

        internal void Draw(Graphics g)
        {
            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.Draw(g);
            }
        }

        internal void AddControl(VisualControl vc)
        {
            Debug.Assert(vc != null);
            Debug.Assert(Controls.IndexOf(vc) == -1);

            Controls.Add(vc);
        }
    }
}
