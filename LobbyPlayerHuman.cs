using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;

namespace Fantasy_Kingdoms_Battle
{
    // Игрок-человек
    internal sealed class LobbyPlayerHuman : LobbyPlayer
    {
        private DispatcherFrame frame;

        public LobbyPlayerHuman(Lobby lobby, Player player, int playerIndex) : base(lobby, player, playerIndex)
        {
            Debug.Assert(player.TypePlayer == TypePlayer.Human);

            frame = new DispatcherFrame();
        }

        internal List<VCEvent> ListEvents { get; } = new List<VCEvent>();

        internal override void SelectStartBonus()
        {
            base.SelectStartBonus();

            // Выбор стартового бонуса
            WindowSelectStartBonus w = new WindowSelectStartBonus(VariantsStartBonuses);
            w.ShowDialog();

            if (Program.formMain.ProgramState != ProgramState.NeedQuit)
                ApplyStartBonus(w.SelectedBonus);

            w.Dispose();
        }

        internal override void DoTurn()
        {
            Debug.Assert(Player.TypePlayer == TypePlayer.Human);
            Debug.Assert(IsLive);

            Lobby.StateLobby = StateLobby.TurnHuman;
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
            frame.Continue = false;
        }

        internal void AddEvent(VCEvent e)
        {
            ListEvents.Add(e);
        }
    }
}
