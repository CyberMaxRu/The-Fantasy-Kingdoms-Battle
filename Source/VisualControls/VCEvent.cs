using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - событие игры
    internal abstract class VCEvent : VisualControl
    {
        public VCEvent() : base()
        {
            ShowBorder = true;
        }
    }
}
