using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс логова игрока
    internal sealed class PlayerLair
    {
        public PlayerLair(Player p, Lair l)
        {
            Player = p;
            Lair = l;

            Level = 1;
        }
        internal Player Player { get; }
        internal Lair Lair { get; }
        internal int Level { get; private set; }
        internal void UpdatePanel()
        {
            Debug.Assert(Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            Lair.Panel.ShowData(this);
        }
    }
}
