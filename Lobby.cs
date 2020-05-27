using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс лобби (королевской битвы)
    internal sealed class Lobby
    {
        public Lobby(TypeLobby tl)
        {
            TypeLobby = tl;

            // Создание игроков
            Players = new Player[tl.QuantityPlayers];
            TypePlayer tp;
            for (int i = 0; i < TypeLobby.QuantityPlayers; i++)
            {
                tp = i == 0 ? TypePlayer.Human : TypePlayer.Computer;
                Players[i] = new Player(this, i + 1, "Игрок №" + (i + 1).ToString(), tp);
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
                    throw new Exception("Не смог удалить элемент " + p.Name + " из списка оппонентов");
                if (!opponents.Remove(p.Opponent))
                    throw new Exception("Не смог удалить элемент " + p.Opponent.Name + " из списка оппонентов");

                if (opponents.Count == 0)
                    break;
            }
        }

        private void ApplyPlayer(int index)
        {
            Debug.Assert(Players[CurrentPlayerIndex].IsLive);

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
            MakeOpponents();

            DoEndTurn();
        }


        private void CalcBattles()
        {
            Battle b;
            FormBattle formBattle;

            foreach (Player p in Players)
                p.BattleCalced = false;

            foreach (Player p in Players)
            {
                if (p.IsLive == true)
                {
                    if (p.BattleCalced == false)
                    {
                        b = new Battle(p, p.Opponent, Turn, FormMain.Rnd);

                        if ((p.TypePlayer == TypePlayer.Human) || (p.Opponent.TypePlayer == TypePlayer.Human))
                        {
                            formBattle = new FormBattle();
                            formBattle.ShowBattle(b);
                        }
                        else
                            b.CalcWholeBattle();

                        Battles.Add(b);
                    }
                }
            }
        }

        private void CalcBattle(Player player1, Player player2, Random r)
        {
        }

        private void CalcEndTurn()
        {
            foreach (Player p in Players)
            {
                p.CalcResultTurn();
            }
        }

        internal Battle GetBattle(Player p, int turn)
        {
            if (turn == 0)
                return null;

            foreach (Battle cb in Battles)
            {
                if (((cb.Player1 == p) || (cb.Player2 == p)) && (cb.Turn == turn))
                    return cb;
            }

            throw new Exception("Бой не найден.");
        }

        internal TypeLobby TypeLobby { get; }
        internal Player[] Players { get; }
        internal int CurrentPlayerIndex { get; private set; }
        internal Player CurrentPlayer { get; private set; }
        internal int Turn { get; private set; }
        internal List<Battle> Battles { get; } = new List<Battle>();
    }
}
