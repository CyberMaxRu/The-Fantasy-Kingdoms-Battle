using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс объекта карты игрока
    internal abstract class PlayerMapObject : PlayerObject
    {
        public PlayerMapObject(LobbyPlayer player)
        {
            Player = player;
        }

        internal LobbyPlayer Player { get; }
    }
}
