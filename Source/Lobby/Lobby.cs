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
        private Descriptors descriptors;

        public Lobby(TypeLobby tl, LobbySettings settings, LayerGameSingle layer, Descriptors d, DescriptorMission m)
        {
            BigEntity.ResetNumerate();

            ID = generation++;
            TypeLobby = tl;
            Settings = settings;
            Mission = m;
            descriptors = d;
            Layer = layer;

            //Entities.un

            StateLobby = StateLobby.Start;
            Turn = 0;
            Day = 0;
            Week = 1;
            Month = 1;
            TimeOfDay = descriptors.TimesOfDay[0];

            // Создание игроков
            if ((Mission != null) && false)
            {
                Players = new Player[m.Players.Count];

                foreach (DescriptorMissionPlayer p in m.Players)
                {
                    if (p.TypePlayer == TypePlayer.Human)
                        Players[p.Index] = new PlayerHuman(this, Program.formMain.CurrentHumanPlayer, p.Index);
                    //else
                    //    Players[p.Index] = new PlayerComputer(this, )
                }

                foreach (DescriptorMissionMember dm in m.Members)
                {
                    Members.Add(new MissionMember(this, dm));
                }
            }
            else
            {
                Players = new Player[tl.QuantityPlayers];
                Players[0] = new PlayerHuman(this, Program.formMain.CurrentHumanPlayer, 0);// Живой игрок всегда первый

                // Подбираем компьютерных игроков из пула доступных
                GenerateComputerPlayers();

                foreach (DescriptorMissionMember dm in m.Members)
                {
                    Members.Add(new MissionMember(this, dm));
                }
            }
            CalcDayNextBattleBetweenPlayers();

            SetPlayerAsCurrent(0);
            StateLobby = StateLobby.TurnHuman;
            Turn = 1;
            Day = 1;

            void GenerateComputerPlayers()
            {

                List<ComputerPlayer> listCompPlayers = new List<ComputerPlayer>();
                listCompPlayers.AddRange(descriptors.ComputerPlayers.Where(cp => cp.Active));
                Debug.Assert(listCompPlayers.Count >= TypeLobby.QuantityPlayers - 1);

                int idx;
                for (int i = 1; i < TypeLobby.QuantityPlayers; i++)
                {
                    idx = Rnd.Next(listCompPlayers.Count);
                    Players[i] = new PlayerComputer(this, listCompPlayers[idx], i);
                    listCompPlayers.RemoveAt(idx);
                }
            }
        }

        internal LayerGameSingle Layer { get; }
        internal int ID { get; }// Уникальный код лобби
        internal TypeLobby TypeLobby { get; }// Тип лобби
        internal DescriptorMission Mission { get; }// Описатель миссии
        internal LobbySettings Settings { get; }
        internal Player[] Players { get; }
        internal Player CurrentPlayer { get; private set; }
        internal List<MissionMember> Members { get; } = new List<MissionMember>();
        internal int Turn { get; private set; }// Текущий ход лобби
        private DescriptorTimeOfDay TimeOfDay { get; set; }// Время суток
        internal int Day { get; private set; }// День
        internal int Week { get; private set; }// Неделя
        internal int Month { get; private set; }// Месяц
        internal int CounterDay { get => Turn; }// Текущий день с начала игры
        internal List<Battle> Battles { get; } = new List<Battle>();
        internal bool HumanIsWin { get; private set; }
        internal StateLobby StateLobby { get; set; }
        internal Random Rnd { get; } = new Random();
        internal bool InPrepareTurn { get; private set; }

        internal Dictionary<string, BigEntity> Entities { get; } = new Dictionary<string, BigEntity>();// Все сущности

        internal int DayNextBattleBetweenPlayers { get; private set; }// День следующей битвы между игроками
        internal int DaysLeftForBattle { get; private set; }// Осталось дней до следующей битвы между игроками

        internal List<BattlesPlayers> BattlesPlayers { get; } = new List<BattlesPlayers>();

        // Подбор оппонентов для битвы
        private void MakeOpponents()
        {
            foreach (Player pl in Players)
            {
                pl.SkipBattle = false;
            }

            // Алгоритм простой - случайным образом подбираем пару
            List<Player> opponents = new List<Player>();
            opponents.AddRange(Players.Where(pp => pp.IsLive));

            // Если оппонентов получается нечетное количество, убираем одно из них по алгоритму
            if (opponents.Count % 2 != 0)
            {
                List<Player> candidate = new List<Player>();

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
                    List<Player> candidate2 = new List<Player>();
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
                    throw new Exception("Не смог удалить элемент " + p.Descriptor.Name + " из списка оппонентов");
                if (!opponents.Remove(p.Opponent))
                    throw new Exception("Не смог удалить элемент " + p.Opponent.Descriptor.Name + " из списка оппонентов");

                if (opponents.Count == 0)
                    break;
            }
        }

        private void SetPlayerAsCurrent(int index)
        {
            if (index != -1)
            {
                Debug.Assert(Players[index].IsLive || (Players[index].DayOfEndGame == Turn - 1));

                CurrentPlayer = Players[index];
            }
            else
                CurrentPlayer = null;

            VisualControl.PanelHint.Player = CurrentPlayer;
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

                // Общая подготовка дня
                PrepareDay();

                // Подготавливаем новый день каждого игрока
                // Чтобы при начале хода все были в консистентном состоянии
                if (TimeOfDay == descriptors.TimesOfDay[0])
                {
                    InPrepareTurn = true;

                    for (int i = 0; i < Players.Length; i++)
                    {
                        if (Players[i].IsLive)
                        {
                            Players[i].PrepareNewDay();

                            if (Turn > 1)
                                Players[i].ReceiveResources();
                        }
                    }

                    InPrepareTurn = false;
                }

                // Действие игроков (ход людей и ИИ)
                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i].IsLive || (Players[i].DayOfEndGame == Turn - 1))
                    {
                        SetPlayerAsCurrent(i);
                        InPrepareTurn = true;
                        Players[i].PrepareTurn(TimeOfDay == descriptors.TimesOfDay[0]);
                        InPrepareTurn = false;
                        Layer.ShowCurrentPlayerLobby();

                        if (HumanIsWin)
                        {
                            Debug.Assert(Players[i].GetTypePlayer() == TypePlayer.Human);

                            Players[i].PlayerIsWin();
                            return;
                        }

                        if (Turn == 1)
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

                SetPlayerAsCurrent(-1);

                // Расчет результатов хода игроков
                //if (TimeOfDay == descriptors.TimesOfDay[descriptors.TimesOfDay.Count - 1])
                {
                    foreach (Player p in Players.Where(pl => pl.IsLive || (pl.DayOfEndGame == Turn - 1)))
                        p.CalcDay();
                }

                DoEndTurn();
            }

            bool ExistsHumanPlayer()
            {
                foreach (Player lp in Players)
                    if ((lp.IsLive || (lp.DayOfEndGame == Turn - 1)) && (lp.GetTypePlayer() == TypePlayer.Human))
                        return true;

                return false;
            }

            bool ExistsOtherHumanPlayer(int fromIndex)
            {
                for (int i = 0; i < Players.Length; i++)
                    if (Players[i].GetTypePlayer() == TypePlayer.Human)
                        if (Players[i].IsLive || ((Players[i].DayOfEndGame == Turn - 1)  && (i >= fromIndex)))
                            return true;

                return false;
            }
        }

        internal void PrepareDay()
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
            Layer.ShowCurrentPlayerLobby();
            Layer.ShowNamePlayer("Расчет дня");
            CalcFinalityTurn();

            if (IsDayForBattleBetweenPlayers())
            {
                CalcBattles();

                foreach (Player pl in Players)
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
            Turn++;
            TimeOfDay = descriptors.TimesOfDay[descriptors.TimesOfDay.Count - 1];
            if (TimeOfDay.Index < descriptors.TimesOfDay.Count - 1)
                TimeOfDay = descriptors.TimesOfDay[TimeOfDay.Index + 1];
            else
            {
                TimeOfDay = descriptors.TimesOfDay[0];

                Day++;
                if (Day == 8)
                {
                    Day = 1;
                    Week++;
                }
                if (Week == 5)
                    Month++;
            }

            CurrentPlayer = null;

            int livePlayers = 0;
            foreach (Player p in Players)
            {
                if (p.IsLive)
                    livePlayers++;
            }

            if (livePlayers > 1)
            {

            }
            else
            {
                foreach (Player p in Players)
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
            BattlesPlayers rb = new BattlesPlayers(Turn);
            BattlesPlayers.Add(rb);

            foreach (Player p in Players)
            {
                p.BattleCalced = false;
            }

            int livePlayers = 0;
            foreach (Player p in Players)
            {
                if (p.IsLive && !p.SkipBattle)
                    livePlayers++;
            }
            Debug.Assert(livePlayers % 2 == 0);
            int pairs = (livePlayers / 2) - 1;
            int numberPair = 0;
            int maxSteps = FormMain.Config.MaxDurationBattleWithPlayer * FormMain.Config.StepsInSecond;

            foreach (Player p in Players)
                p.BattleCalced = false;

            foreach (Player p in Players)
            {
                if (p.IsLive && !p.SkipBattle)
                {
                    if (p.BattleCalced == false)
                    {
                        Debug.Assert(!(p.Opponent is null));

                        bool showForPlayer = false;// (p.GetTypePlayer() == TypePlayer.Human) || (p.Opponent.GetTypePlayer() == TypePlayer.Human);
                        b = new Battle(p, p.Opponent, Turn, Rnd.Next(), maxSteps, showForPlayer);

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

                        rb.Players.Add(b.Player1 as Player, b.Winner == b.Player1);
                        rb.Players.Add(b.Player2 as Player, b.Winner == b.Player2);

                        // Добавляем событие всем живым игрокам
                        foreach (Player lp in Players.Where(lpp => lpp.IsLive || (lpp.DayOfEndGame == Turn)))
                            if (lp is PlayerHuman h)
                                h.AddEvent(new VCEventBattle(b));
                    }
                }
            }
        }

        private void CalcBattle(Player player1, Player player2, Random r)
        {
        }

        private void CalcFinalityTurn()
        {
            foreach (Player p in Players)
                if (p.IsLive)
                    p.CalcFinalityTurn();
        }

        private void CalcResultTurn()
        {
            // Делаем расчет итогов дня
            int livePlayers = 0;
            foreach (Player p in Players)
            {
                if (p.IsLive == true)
                {
                    p.CalcResultTurn();
                    livePlayers++;
                }
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

        private void SortPlayers()
        {
            List<Player> sort = new List<Player>();

            // Живых игроков сортируем по начальной позиции
            int pos = 1;
            foreach (Player lp in Players.Where(p => p.IsLive))
                lp.PositionInLobby = pos++;

            // Теперь сортируем игроков, вылетевших на прошлом ходу
            foreach (Player lp in Players.Where(p => p.DayOfEndGame == Turn).OrderBy(p => p.CurrentLoses).OrderByDescending(p => p.GreatnessCollected).OrderByDescending(p => p.BaseResources.Gold))
                lp.PositionInLobby = pos++;

            // Проверяем, что нет ошибки в позициях
            int curPos = 1;
            foreach (Player p in Players.OrderBy(p => p.PositionInLobby))
            {
                if (p.PositionInLobby != curPos)
                    throw new Exception("Позиция игрока должна быть " + curPos.ToString() + " вместо " + p.PositionInLobby.ToString());

                curPos++;
            }
        }

        internal bool CheckUniqueAvatars()
        {
            foreach (Player ph1 in Players)
                foreach (Player ph2 in Players)
                    if (ph1 != ph2)
                        if (ph1.GetImageIndexAvatar() == ph2.GetImageIndexAvatar())
                        {
                            return false;
                        }

            return true;
        }

        internal bool CheckUniqueNamePlayers()
        {
            foreach (Player ph1 in Players)
                foreach (Player ph2 in Players)
                    if (ph1 != ph2)
                        if (ph1.Descriptor.Name == ph2.Descriptor.Name)
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
            if (Turn <= TypeLobby.DayStartBattleBetweenPlayers)
                DayNextBattleBetweenPlayers = TypeLobby.DayStartBattleBetweenPlayers;
            else if (TypeLobby.DaysBeforeNextBattleBetweenPlayers == 0)
                DayNextBattleBetweenPlayers = Turn;
            else
            {
                int delta = Turn - TypeLobby.DayStartBattleBetweenPlayers;
                int rep = (int)Math.Truncate((double)delta / (TypeLobby.DaysBeforeNextBattleBetweenPlayers + 1));
                int days = delta % (TypeLobby.DaysBeforeNextBattleBetweenPlayers + 1);
                if (days > 0)
                    rep++;
                DayNextBattleBetweenPlayers = TypeLobby.DayStartBattleBetweenPlayers + (rep * (TypeLobby.DaysBeforeNextBattleBetweenPlayers + 1));
            }

            Debug.Assert(DayNextBattleBetweenPlayers >= Turn);

            DaysLeftForBattle = DayNextBattleBetweenPlayers - Turn;
        }

        internal bool IsDayForBattleBetweenPlayers()
        {
            return Turn == DayNextBattleBetweenPlayers;
        }

        internal void AddEntity(BigEntity e)
        {
            Debug.Assert(e.IDEntity.Length > 0);
            Debug.Assert(e != null);

            Entities.Add(e.IDEntity, e);
        }

        internal BigEntity FindEntity(string id)
        {
            Entities.TryGetValue(id, out BigEntity v);
            return v;
        }
    }
}
