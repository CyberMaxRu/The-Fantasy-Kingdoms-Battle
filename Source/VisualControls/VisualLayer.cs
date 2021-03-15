using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
    }
}
