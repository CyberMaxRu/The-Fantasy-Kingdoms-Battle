using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс предмета у игрока (находящегося на герое или на складе)
    internal sealed class PlayerItem
    {
        public PlayerItem(Item i)
        {
            Item = i;
        }

        internal Item Item { get; }
    }
}
