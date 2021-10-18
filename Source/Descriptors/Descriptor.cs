using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class Descriptor
    {
        internal static Config Config { get; set; }

        internal virtual void TuneDeferredLinks()
        {
        }

        internal virtual void AfterTune()
        {

        }
    }
}
