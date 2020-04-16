using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle.Resources.Config
{
    // Класс отряда
    internal sealed class Squad
    {
        public Squad(TypeUnit typeUnit)
        {
            TypeUnit = typeUnit;
        }

        internal TypeUnit TypeUnit { get; }
    }
}