using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс квеста игрока
    internal sealed class PlayerQuest : Entity
    {
        public PlayerQuest(Player p, DescriptorMissionQuest quest)
        {
            Player = p;
            Quest = quest;
            TurnActivate = p.Lobby.Turn;
        }

        internal Player Player { get; }
        internal DescriptorMissionQuest Quest { get; }
        internal int TurnActivate { get; }// Ход, на котором квест был активирован

        internal override int GetImageIndex()
        {
            return FormMain.Config.Gui48_Quest;
        }

        internal override string GetName() => Quest.Name;

        internal override string GetTypeEntity() => "Задание";

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }
    }
}
