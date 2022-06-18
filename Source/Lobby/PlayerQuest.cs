using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal enum StateQuest { InProgress, Executed, Failed };

    // Класс квеста игрока
    internal sealed class PlayerQuest : Entity
    {
        public PlayerQuest(Player p, DescriptorMissionQuest quest)
        {
            Player = p;
            Quest = quest;
            TurnActivate = p.Lobby.Turn;
            State = StateQuest.InProgress;

            // Определяем, от кого поступил квест
            //FromEntity = Player.Lobby.Mission.FindMember(quest.From);
            //if ()
        }

        internal Player Player { get; }
        internal Entity FromEntity { get; }// От какой сущности поступил квест
        internal DescriptorMissionQuest Quest { get; }
        internal int TurnActivate { get; }// Ход, на котором квест был активирован
        internal StateQuest State { get; private set; }// Состояние квеста

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
