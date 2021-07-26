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

        internal override void SelectStartBonus()
        {
            Debug.Assert(VariantsStartBonuses.Count > 0);

            // Выбор стартового бонуса
            WindowSelectStartBonus w = new WindowSelectStartBonus(VariantsStartBonuses);
            w.ShowDialog();

            if (Program.formMain.ProgramState != ProgramState.NeedQuit)
                ApplyStartBonus(w.SelectedBonus);

            w.Dispose();
        }

        internal override void DoTurn()
        {
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
    }
}
