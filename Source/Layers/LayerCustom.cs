using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class LayerCustom : VisualControl
    {
        internal static Config Config { get; set; }
        internal static Descriptors Descriptors { get; set; }
    }
}