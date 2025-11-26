using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Игрок-человек
    internal sealed class PlayerHuman : Player
    {
        private DispatcherFrame frame;

        public PlayerHuman(Lobby lobby, HumanPlayer player, int playerIndex) : base(lobby, player, playerIndex)
        {
            Debug.Assert(player.TypePlayer == TypePlayer.Human);

            CheatingIgnoreRequirements = player.CheatingIgnoreRequirements;
            CheatingSpeedUpProgressBy10 = player.CheatingSpeedUpProgressBy10;
            CheatingReduceCostBy10 = player.CheatingReduceCostBy10;
            CheatingPointsTraditionMore10Times = player.CheatingPointsTraditionMore10Times;

/*            if (CheatingIgnoreRequirements)
                AddNoticeForPlayer(-1, FormMain.Config.Gui48_Cheating, "Применен читинг:", "Игнорировать требования", Color.Orange);
            if (CheatingSpeedUpProgressBy10)
                AddNoticeForPlayer(-1, FormMain.Config.Gui48_Cheating, "Применен читинг:", "Ускорение прогресса в 10 раз", Color.Orange);
            if (CheatingReduceCostBy10)
                AddNoticeForPlayer(-1, FormMain.Config.Gui48_Cheating, "Применен читинг:", "Стоимость меньше в 10 раз", Color.Orange);
*/
            frame = new DispatcherFrame();
        }

        internal List<VCEvent> ListEvents { get; } = new List<VCEvent>();

        internal override void SelectStartBonus()
        {
            base.SelectStartBonus();

            // Выбор постоянного бонуса
            WindowSelectPersistentBonuses wpb = new WindowSelectPersistentBonuses(this);
            wpb.Show();

            // Выбор стартового бонуса
            WindowSelectStartBonus w = new WindowSelectStartBonus(this, VariantsStartBonuses);
            w.Show();
            ApplyStartBonus(w.SelectedBonus);
        }

        internal override void PrepareTurn(bool beginOfDay)
        {

            base.PrepareTurn(beginOfDay);
        }

        internal void DoTurn()
        {
            Debug.Assert(Descriptor.TypePlayer == TypePlayer.Human);
            Debug.Assert(IsLive || (DayOfEndGame == Lobby.Turn - 1));

            ListEvents.Clear();

            Lobby.StateLobby = StateLobby.TurnHuman;

            // Если игрок вылете из лобби на предыдущем ходу, сообщаем его итоговое место и выходим
            if (DayOfEndGame > 0)
            {
                Program.formMain.PlayerMusic.PlayLossLobbyTheme();
                WindowInfo.ShowInfo("ПОРАЖЕНИЕ", $"Вы заняли {PositionInLobby} место.");
                return;
            }
        }

        internal void AddEvent(VCEvent e)
        {
            ListEvents.Add(e);
        }

        internal override void PlayerIsWin()
        {
            Program.formMain.PlayerMusic.PlayWinLobbyTheme();
            WindowInfo.ShowInfo("ПОБЕДА!", $"Поздравлям, вы победитель!");
        }

        internal override void DoTick(bool startNewDay)
        {
            base.DoTick(startNewDay);

        }

        internal override void ExecuteTurn()
        {
        }
    }
}
