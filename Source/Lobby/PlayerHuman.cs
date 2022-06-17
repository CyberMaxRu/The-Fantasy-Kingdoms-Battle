using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Игрок-человек
    internal sealed class PlayerHuman : Player
    {
        private DispatcherFrame frame;

        public PlayerHuman(Lobby lobby, DescriptorPlayer player, int playerIndex) : base(lobby, player, playerIndex)
        {
            Debug.Assert(player.TypePlayer == TypePlayer.Human);

            frame = new DispatcherFrame();
        }

        internal List<VCEvent> ListEvents { get; } = new List<VCEvent>();

        internal override void SelectStartBonus()
        {
            base.SelectStartBonus();

            // Выбор постоянного бонуса
            switch (Lobby.Settings.Players[PlayerIndex].TypeSelectPersistentBonus)
            {
                case TypeSelectBonus.Manual:
                    WindowSelectPersistentBonuses wpb = new WindowSelectPersistentBonuses(this);
                    wpb.ShowDialog();
                    wpb.Dispose();
                    break;
                case TypeSelectBonus.Random:
                    SelectRandomPersistentBonus();
                    break;
                default:
                    DoException("Неизвестный тип бонуса.");
                    break;
            }

            // Выбор стартового бонуса
            switch (Lobby.Settings.Players[PlayerIndex].TypeSelectStartBonus)
            {
                case TypeSelectBonus.Manual:
                    WindowSelectStartBonus w = new WindowSelectStartBonus(this, VariantsStartBonuses);
                    w.ShowDialog();
                    ApplyStartBonus(w.SelectedBonus);
                    w.Dispose();
                    break;
                case TypeSelectBonus.Random:
                    ApplyStartBonus(GetRandomStartBonus());
                    break;
                default:
                    DoException("Неизвестный тип бонуса.");
                    break;
            }

            if (Program.formMain.ProgramState != ProgramState.NeedQuit)
            {
                Program.formMain.ShowFrame(true);// Чтобы обновились данные в тулбаре (золото, величие)
            }
        }

        internal override void PrepareTurn(bool beginOfDay)
        {
            base.PrepareTurn(beginOfDay);

            Lobby.Layer.ActivatePageResultTurn();
        }

        internal override void DoTurn()
        {
            Debug.Assert(Descriptor.TypePlayer == TypePlayer.Human);
            Debug.Assert(IsLive || (DayOfEndGame == Lobby.Turn - 1));

            ListEvents.Clear();

            Lobby.StateLobby = StateLobby.TurnHuman;

            // Если игрок вылете из лобби на предыдущем ходу, сообщаем его итоговое место и выходим
            if (DayOfEndGame > 0)
            {
                Program.formMain.playerMusic.PlayLossLobbyTheme();
                WindowInfo.ShowInfo("ПОРАЖЕНИЕ", $"Вы заняли {PositionInLobby} место.");
                return;
            }

            // Показываем сообщения
            WindowMessage wm = null;
            foreach (DescriptorMissionMessage m in Lobby.Mission.Messages)
            {
                if (m.Turn == Lobby.Turn)
                {
                    if (wm is null)
                        wm = new WindowMessage();

                    wm.SetMessage(m);
                    wm.ShowDialog();
                }

                wm?.Dispose();
            }

            frame.Continue = true;
            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        internal override void EndTurn()
        {
            Debug.Assert(frame.Continue);

            foreach (VCNoticeForPlayer e in ListNoticesForPlayer)
            {
                e.Dispose();
            }
            ListNoticesForPlayer.Clear();

            frame.Continue = false;
        }

        internal void AddEvent(VCEvent e)
        {
            ListEvents.Add(e);
        }

        internal override void PlayerIsWin()
        {
            Program.formMain.playerMusic.PlayWinLobbyTheme();
            WindowInfo.ShowInfo("ПОБЕДА!", $"Поздравлям, вы победитель!");
        }
    }
}
