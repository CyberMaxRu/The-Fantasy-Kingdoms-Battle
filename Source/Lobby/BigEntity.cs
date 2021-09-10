using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class BigEntity : Entity
    {
        public BigEntity() : base()
        {

        }

        internal abstract void ShowInfo();
        internal abstract void HideInfo();
    }
}
