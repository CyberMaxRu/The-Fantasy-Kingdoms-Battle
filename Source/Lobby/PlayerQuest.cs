using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum StateQuest { InProgress, Executed, Failed };

    // Класс квеста игрока
    internal sealed class PlayerQuest : Entity
    {
        private string name;
        private string description;

        public PlayerQuest(Player p, DescriptorMissionQuest quest) : base()
        {
            Player = p;
            Quest = quest;
            TurnActivate = p.Lobby.Turn;
            State = StateQuest.InProgress;

            // Определяем, от кого поступил квест
            FromEntity = Player.FindBigEntityInSelfAndLobby(quest.From);
            name = Player.ReplaceIDEntityToName(quest.Name);
            description = Player.ReplaceIDEntityToName(quest.Description);
        }

        internal Player Player { get; }
        internal BigEntity FromEntity { get; }// От какой сущности поступил квест
        internal DescriptorMissionQuest Quest { get; }
        internal int TurnActivate { get; }// Ход, на котором квест был активирован
        internal StateQuest State { get; private set; }// Состояние квеста

        internal override int GetImageIndex()
        {
            return FormMain.Config.Gui48_Quest;
        }

        internal override string GetName() => name;
        internal string Description => description;

        internal override string GetTypeEntity() => "Задание";

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }
    }
}
