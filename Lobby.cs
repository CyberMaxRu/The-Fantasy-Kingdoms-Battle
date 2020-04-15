using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс лобби (королевской битвы)
    internal sealed class Lobby
    {
        public Lobby(int quantityPlayers)
        {
            Random r = new Random();
            Players = new Player[quantityPlayers];
            TypePlayer tp;
            for (int i = 0; i < quantityPlayers; i++)
            {
                tp = i == 0 ? TypePlayer.Human : TypePlayer.Computer;
                Players[i] = new Player("Игрок №" + (i + 1).ToString(), FormMain.Config.Fractions[r.Next(0, FormMain.Config.Fractions.Count)], tp);
            }

            ApplyPlayer(0);
        }

        private void ApplyPlayer(int index)
        {
            CurrentPlayerIndex = index;
            CurrentPlayer = Players[CurrentPlayerIndex];
        }

        internal Player[] Players { get; }
        internal int CurrentPlayerIndex { get; private set; }
        internal Player CurrentPlayer { get; private set; }
    }
}
