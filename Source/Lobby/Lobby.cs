using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal enum StateLobby { Start, TurnHuman, TurnComputer, CalcTurn };

    // Класс лобби
    internal sealed class Lobby
    {
        private static int generation = 0;
        private bool stopLobby = false;

        public Lobby(TypeLobby tl)
        {
            ID = generation++;
            TypeLobby = tl;
            StateLobby = StateLobby.Start;
            Day = 1;

            // Создаем конфигурацию логов
            Lairs = new List<DescriptorConstruction>[TypeLobby.LairsLayers];
            GenerateConfigLairs();

            // Создание игроков
            Players = new LobbyPlayer[tl.QuantityPlayers];
            Players[0] = new LobbyPlayerHuman(this, Program.formMain.CurrentHumanPlayer, 0);// Живой игрок всегда первый
            // Подбираем компьютерных игроков из пула доступных
            GeneratePlayers();

            CalcDayNextBattleBetweenPlayers();

            SetPlayerAsCurrent(0);
            StateLobby = StateLobby.TurnHuman;


            void GenerateConfigLairs()
            {
                // Создание рандомных логов монстров согласно настроек типа лобби
                // Для этого сначала создаем логова по минимальному списку,
                // а оставшиеся ячейки - из оставшихся по максимуму

                TypeLobbyLayerSettings ls;
                int idx;
                int restLairs;

                for (int layer = 0; layer < TypeLobby.LairsLayers; layer++)
                {
                    Lairs[layer] = new List<DescriptorConstruction>();

                    ls = TypeLobby.LayerSettings[layer];
                    restLairs = TypeLobby.LairsHeight * TypeLobby.LairsWidth;

                    // Сначала заполняем минимальными количествами
                    foreach (TypeLobbyLairSettings l in ls.LairsSettings)
                    {
                        for (int i = 0; i < l.MinQuantity; i++)
                        {
                            Lairs[layer].Add(l.TypeLair);
                            restLairs--;
                        }

                        Debug.Assert(restLairs >= 0);
                    }
                    
                    // Если остались свободные ячейки, генерируем по данным о максимальном количестве
                    if (restLairs > 0)
                    {
                        List<DescriptorConstruction> listTypeLairs = new List<DescriptorConstruction>();
                        int q;
                        
                        // Составляем список из максимального числа доступных типов логов
                        foreach (TypeLobbyLairSettings l in ls.LairsSettings)
                        {
                            q = l.MaxQuantity - l.MinQuantity;
                            for (int j = 0; j < q; j++)
                                listTypeLairs.Add(l.TypeLair);
                        }

                        Debug.Assert(restLairs <= listTypeLairs.Count);
                        
                        // Пока есть места, дергаем случайный тип логова
                        while (restLairs > 0)
                        {
                            idx = Rnd.Next(listTypeLairs.Count);

                            Lairs[layer].Add(listTypeLairs[idx]);

                            listTypeLairs.RemoveAt(idx);
                            restLairs--;
                        }
                    }
                }
            }

            void GeneratePlayers()
            {

                List<ComputerPlayer> listCompPlayers = new List<ComputerPlayer>();
                listCompPlayers.AddRange(FormMain.Config.ComputerPlayers.Where(cp => cp.Active));
                Debug.Assert(listCompPlayers.Count >= TypeLobby.QuantityPlayers - 1);

                int idx;
                for (int i = 1; i < TypeLobby.QuantityPlayers; i++)
                {
                    idx = Rnd.Next(listCompPlayers.Count);
                    Players[i] = new LobbyPlayerComputer(this, listCompPlayers[idx], i);
                    listCompPlayers.RemoveAt(idx);
                }
            }
        }

        internal int ID { get; }
        internal TypeLobby TypeLobby { get; }
        internal LobbyPlayer[] Players { get; }
        internal LobbyPlayer CurrentPlayer { get; private set; }
        internal int Day { get; private set; }        
        internal List<Battle> Battles { get; } = new List<Battle>();
        internal bool HumanIsWin { get; private set; }
        internal StateLobby StateLobby { get; set; }
        internal Random Rnd { get; } = new Random();
        internal List<DescriptorConstruction>[] Lairs { get; }

        internal int DayNextBattleBetweenPlayers { get; private set; }// День следующей битвы между игроками
        internal int DaysLeftForBattle { get; private set; }// Осталось дней до следующей битвы между игроками

        internal List<BattlesPlayers> BattlesPlayers { get; } = new List<BattlesPlayers>();

        // Подбор оппонентов для битвы
        private void MakeOpponents()
        {
            foreach (LobbyPlayer pl in Players)
            {
                pl.SkipBattle = false;
            }

            // Алгоритм простой - случайным образом подбираем пару
            List<LobbyPlayer> opponents = new List<LobbyPlayer>();
            opponents.AddRange(Players.Where(pp => pp.IsLive));

            // Если оппонентов получается нечетное количество, убираем одно из них по алгоритму
            if (opponents.Count % 2 != 0)
            {
                List<LobbyPlayer> candidate = new List<LobbyPlayer>();

                // Сначала оставляем тех игроков, у кого наименьшее число пропусков битв
                int minSkip = opponents.Min(o => o.SkippedBattles);
                candidate.AddRange(opponents.Where(o => o.SkippedBattles == minSkip));
                if (candidate.Count == 1)
                {
                    candidate[0].SkippedBattles++;
                    candidate[0].SkipBattle = true;
                    opponents.Remove(candidate[0]);
                }
                else
                {
                    // Затем оставляем тех игроков, у кого наибольшее число поражений
                    int maxLoses = opponents.Max(o => o.CurrentLoses);
                    List<LobbyPlayer> candidate2 = new List<LobbyPlayer>();
                    candidate2.AddRange(candidate.Where(o => o.CurrentLoses == maxLoses));
                    if (candidate2.Count == 1)
                    {
                        candidate2[0].SkippedBattles++;
                        candidate2[0].SkipBattle = true;
                        opponents.Remove(candidate2[0]);
                    }
                    else
                    {
                        // Выбираем случайного игрока
                        int randomPlayer = Rnd.Next(candidate2.Count);
                        candidate2[randomPlayer].SkippedBattles++;
                        candidate2[0].SkipBattle = true;
                        opponents.Remove(candidate2[randomPlayer]);
                    }
                }
            }

            Debug.Assert(opponents.Count % 2 == 0);
            Random r = new Random();
            LobbyPlayer p;
            LobbyPlayer oppo;
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
                    throw new Exception("Не смог удалить элемент " + p.Player.Name + " из списка оппонентов");
                if (!opponents.Remove(p.Opponent))
                    throw new Exception("Не смог удалить элемент " + p.Opponent.Player.Name + " из списка оппонентов");

                if (opponents.Count == 0)
                    break;
            }
        }

        private void SetPlayerAsCurrent(int index)
        {
            if (index != -1)
            {
                Debug.Assert(Players[index].IsLive || (Players[index].DayOfEndGame == Day - 1));

                CurrentPlayer = Players[index];
            }
            else
                CurrentPlayer = null;
        }

        internal void Start()
        {
            // Реальный игрок должен быть жив
            Debug.Assert(Players[0].GetTypePlayer() == TypePlayer.Human);
            Debug.Assert(Players[0].IsLive);
            Debug.Assert(CheckUniqueNamePlayers());

            while (!stopLobby)
            {
                Debug.Assert(ExistsHumanPlayer());

                // Общая подготовка хода
                DoPrepareTurn();

                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i].IsLive || (Players[i].DayOfEndGame == Day - 1))
                    {
                        SetPlayerAsCurrent(i);
                        Players[i].PrepareTurn();
                        Program.formMain.ShowCurrentPlayerLobby();

                        if (HumanIsWin)
                        {
                            Debug.Assert(Players[i].GetTypePlayer() == TypePlayer.Human);

                            Players[i].PlayerIsWin();
                            return;
                        }

                        if (Day == 1)
                        {
                            if (Players[i].VariantsStartBonuses.Count > 0)
                                Players[i].SelectStartBonus();
                        }

                        Players[i].DoTurn();

                        // Если игрок-человек вылетел и больше нет игроков-людей, выходим из лобби
                        if (Players[i].GetTypePlayer() == TypePlayer.Human)
                            if (!ExistsOtherHumanPlayer(i + 1))
                                return;

                        if (stopLobby)
                            return;
                    }
                }

                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i].IsLive || (Players[i].DayOfEndGame == Day - 1))
                    {
                        Players[i].AfterEndTurn();
                    }
                }

                    DoEndTurn();
            }

            bool ExistsHumanPlayer()
            {
                foreach (LobbyPlayer lp in Players)
                    if ((lp.IsLive || (lp.DayOfEndGame == Day - 1)) && (lp.GetTypePlayer() == TypePlayer.Human))
                        return true;

                return false;
            }

            bool ExistsOtherHumanPlayer(int fromIndex)
            {
                for (int i = 0; i < Players.Length; i++)
                    if (Players[i].GetTypePlayer() == TypePlayer.Human)
                        if (Players[i].IsLive || ((Players[i].DayOfEndGame == Day - 1)  && (i >= fromIndex)))
                            return true;

                return false;
            }
        }

        internal void DoPrepareTurn()
        {
            // Считаем день следующей битвы между игроками
            CalcDayNextBattleBetweenPlayers();

            // Если сегодня - день битвы, составляем оппонентов заранее
            if (IsDayForBattleBetweenPlayers())
                MakeOpponents();
        }

        internal void DoEndTurn()
        {
            // Реальный игрок должен быть жив
            Debug.Assert(Players[0].GetTypePlayer() == TypePlayer.Human);
            Debug.Assert(Players[0].IsLive);

/*            // Делаем ходы, перебирая всех игроков, пока все не совершат ход
            int cpi = CurrentPlayer != null ? CurrentPlayer.PlayerIndex : -1;
            for (int i = cpi + 1; i < Players.Count(); i++)
            {
                if ((Players[i].IsLive == true) || (Players[i].GetTypePlayer() == TypePlayer.Human))
                {
                    if (CurrentPlayer.GetTypePlayer() == TypePlayer.Computer)
                    {
                        StateLobby = StateLobby.TurnComputer;
                        Program.formMain.ShowCurrentPlayerLobby();
                        CurrentPlayer.DoTurn();
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        StateLobby = StateLobby.TurnHuman;
                        Program.formMain.ShowCurrentPlayerLobby();
                        return;
                    }
                }
            }
*/
            SetPlayerAsCurrent(-1);
            StateLobby = StateLobby.CalcTurn;
            Program.formMain.ShowCurrentPlayerLobby();
            Program.formMain.ShowNamePlayer("Расчет дня");
            CalcFinalityTurn();

            if (IsDayForBattleBetweenPlayers())
            {
                CalcBattles();

                foreach (LobbyPlayer pl in Players)
                {
                    if (pl.Opponent != null)
                        pl.Opponent = null;
                }
            }

            CalcResultTurn();

            SortPlayers();

            /*if (!Players[0].IsLive)
            {
                CurrentPlayer = null;
                return;
            }*/

            // Делаем начало хода
            Day++;
            CurrentPlayer = null;

            int livePlayers = 0;
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive)
                    livePlayers++;
            }

            if (livePlayers > 1)
            {

            }
            else
            {
                foreach (LobbyPlayer p in Players)
                {
                    if (p.IsLive)
                    {
                        if (p.GetTypePlayer() == TypePlayer.Human)
                        {
                            HumanIsWin = true;
                        }
                    }
                }

                return;
            }
        }


        private void CalcBattles()
        {
            Battle b;
            WindowBattle formBattle;
            BattlesPlayers rb = new BattlesPlayers(Day);
            BattlesPlayers.Add(rb);

            foreach (LobbyPlayer p in Players)
            {
                p.BattleCalced = false;
            }

            int livePlayers = 0;
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive && !p.SkipBattle)
                    livePlayers++;
            }
            Debug.Assert(livePlayers % 2 == 0);
            int pairs = (livePlayers / 2) - 1;
            int numberPair = 0;
            int maxSteps = FormMain.Config.MaxDurationBattleWithPlayer * FormMain.Config.StepsInSecond;

            foreach (LobbyPlayer p in Players)
                p.BattleCalced = false;

            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive && !p.SkipBattle)
                {
                    if (p.BattleCalced == false)
                    {
                        Debug.Assert(!(p.Opponent is null));

                        bool showForPlayer = false;// (p.GetTypePlayer() == TypePlayer.Human) || (p.Opponent.GetTypePlayer() == TypePlayer.Human);
                        b = new Battle(p, p.Opponent, Day, Rnd.Next(), maxSteps, showForPlayer);

                        if (showForPlayer)
                        {
                            formBattle = new WindowBattle(b);
                            formBattle.ShowBattle();
                            formBattle.Dispose();
                        }
                        else
                        {
                            numberPair++;
                            //if (formProgressBattle == null)
                            //    formProgressBattle = new FormProgressBattle();

                            //formProgressBattle.SetBattle(b, pairs, numberPair);
                            b.CalcWholeBattle();
                        }

                        Battles.Add(b);

                        rb.Players.Add(b.Player1 as LobbyPlayer, b.Winner == b.Player1);
                        rb.Players.Add(b.Player2 as LobbyPlayer, b.Winner == b.Player2);

                        // Добавляем событие всем живым игрокам
                        foreach (LobbyPlayer lp in Players.Where(lpp => lpp.IsLive || (lpp.DayOfEndGame == Day)))
                            if (lp is LobbyPlayerHuman h)
                                h.AddEvent(new VCEventBattle(b));
                    }
                }
            }
        }

        private void CalcBattle(LobbyPlayer player1, LobbyPlayer player2, Random r)
        {
        }

        private void CalcFinalityTurn()
        {
            foreach (LobbyPlayer p in Players)
                if (p.IsLive)
                    p.CalcFinalityTurn();
        }

        private void CalcResultTurn()
        {
            // Делаем расчет итогов дня
            int livePlayers = 0;
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive == true)
                {
                    p.CalcResultTurn();
                    livePlayers++;
                }
            }
        }

        internal Battle GetBattle(LobbyPlayer p, int turn)
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

        private void SortPlayers()
        {
            List<LobbyPlayer> sort = new List<LobbyPlayer>();

            // Живых игроков сортируем по начальной позиции
            int pos = 1;
            foreach (LobbyPlayer lp in Players.Where(p => p.IsLive))
                lp.PositionInLobby = pos++;

            // Теперь сортируем игроков, вылетевших на прошлом ходу
            foreach (LobbyPlayer lp in Players.Where(p => p.DayOfEndGame == Day).OrderBy(p => p.CurrentLoses).OrderByDescending(p => p.GreatnessCollected).OrderByDescending(p => p.GoldCollected))
                lp.PositionInLobby = pos++;

            // Проверяем, что нет ошибки в позициях
            int curPos = 1;
            foreach (LobbyPlayer p in Players.OrderBy(p => p.PositionInLobby))
            {
                if (p.PositionInLobby != curPos)
                    throw new Exception("Позиция игрока должна быть " + curPos.ToString() + " вместо " + p.PositionInLobby.ToString());

                curPos++;
            }
        }

        internal bool CheckUniqueAvatars()
        {
            foreach (LobbyPlayer ph1 in Players)
                foreach (LobbyPlayer ph2 in Players)
                    if (ph1 != ph2)
                        if (ph1.GetImageIndexAvatar() == ph2.GetImageIndexAvatar())
                        {
                            return false;
                        }

            return true;
        }

        internal bool CheckUniqueNamePlayers()
        {
            foreach (LobbyPlayer ph1 in Players)
                foreach (LobbyPlayer ph2 in Players)
                    if (ph1 != ph2)
                        if (ph1.Player.Name == ph2.Player.Name)
                        {
                            return false;
                        }

            return true;
        }

        internal void ExitFromLobby()
        {
            CurrentPlayer.EndTurn();
            stopLobby = true;
        }

        internal void CalcDayNextBattleBetweenPlayers()
        {
            if (Day <= TypeLobby.DayStartBattleBetweenPlayers)
                DayNextBattleBetweenPlayers = TypeLobby.DayStartBattleBetweenPlayers;
            else if (TypeLobby.DaysBeforeNextBattleBetweenPlayers == 0)
                DayNextBattleBetweenPlayers = Day;
            else
            {
                int delta = Day - TypeLobby.DayStartBattleBetweenPlayers;
                int rep = (int)Math.Truncate((double)delta / (TypeLobby.DaysBeforeNextBattleBetweenPlayers + 1));
                int days = delta % (TypeLobby.DaysBeforeNextBattleBetweenPlayers + 1);
                if (days > 0)
                    rep++;
                DayNextBattleBetweenPlayers = TypeLobby.DayStartBattleBetweenPlayers + (rep * (TypeLobby.DaysBeforeNextBattleBetweenPlayers + 1));
            }

            Debug.Assert(DayNextBattleBetweenPlayers >= Day);

            DaysLeftForBattle = DayNextBattleBetweenPlayers - Day;
        }

        internal bool IsDayForBattleBetweenPlayers()
        {
            return Day == DayNextBattleBetweenPlayers;
        }
    }
}
