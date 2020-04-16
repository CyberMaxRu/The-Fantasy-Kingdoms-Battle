using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс лобби (королевской битвы)
    internal sealed class Lobby
    {
        public Lobby(int quantityPlayers)
        {
            // Создание игроков
            Random r = new Random();
            Players = new Player[quantityPlayers];
            TypePlayer tp;
            for (int i = 0; i < quantityPlayers; i++)
            {
                tp = i == 0 ? TypePlayer.Human : TypePlayer.Computer;
                Players[i] = new Player(this, "Игрок №" + (i + 1).ToString(), FormMain.Config.Fractions[r.Next(0, FormMain.Config.Fractions.Count)], tp);
            }

            ApplyPlayer(0);

            //
            Turn = 1;

            // Определяем противников
            MakeOpponents();
        }

        private void MakeOpponents()
        {
            foreach (Player pl in Players)
                pl.Opponent = null;

            // Алгоритм простой - случайным образом подбираем пару
            List<Player> opponents = new List<Player>();
            opponents.AddRange(Players);
            Random r = new Random();
            Player p;
            Player oppo;
            for (; ; )
            {
                p = opponents[0];
                oppo = opponents[r.Next(1, opponents.Count)];
                Debug.Assert(p != oppo);
                Debug.Assert(p.Opponent == null);
                Debug.Assert(oppo.Opponent == null);
                p.Opponent = oppo;
                oppo.Opponent = p;
                if (!opponents.Remove(p))
                    new Exception("Не смог удалить элемент " + p.Name + " из списка оппонентов");
                if (!opponents.Remove(p.Opponent))
                    new Exception("Не смог удалить элемент " + p.Opponent.Name + " из списка оппонентов");

                if (opponents.Count == 0)
                    break;
            }
        }

        private void ApplyPlayer(int index)
        {
            CurrentPlayerIndex = index;
            CurrentPlayer = Players[CurrentPlayerIndex];
        }

        internal void DoEndTurn()
        {
            // Делаем ходы, перебирая всех игроков, пока все не совершат ход
            for (int i = CurrentPlayerIndex + 1; i < Players.Count(); i++)
            {
                ApplyPlayer(i);

                if (CurrentPlayer.TypePlayer == TypePlayer.Computer)
                    CurrentPlayer.DoTurn();
                else
                    return;
            }


            CalcBattles();

            CalcEndTurn();

            // Делаем начало хода
            Turn++;
            CurrentPlayerIndex = -1;

            DoEndTurn();
        }

        private void CalcBattles()
        {

        }

        private void CalcEndTurn()
        {
            foreach (Player p in Players)
            {
                p.CalcResultTurn();
            }
        }

        internal Player[] Players { get; }
        internal int CurrentPlayerIndex { get; private set; }
        internal Player CurrentPlayer { get; private set; }
        internal int Turn { get; private set; }
    }
}
