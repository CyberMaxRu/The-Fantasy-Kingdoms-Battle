using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal enum TypeItemSmithy { Weapon, Armour }

    // Класс предмета в кузнице
    internal sealed class ItemSmithy
    {

        internal TypeItemSmithy TypeItem { get; }
        internal CategoryUnit CategoryUnit { get; }
    }
}
