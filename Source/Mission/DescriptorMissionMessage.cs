using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeActionMessage { None, AddQuest };

    internal sealed class DescriptorMissionMessage : Descriptor
    {
        private TypeActionMessage typeAction = TypeActionMessage.None;
        private string idEntityAction = "";

        public DescriptorMissionMessage(XmlNode n, DescriptorMission dm) : base()
        {
            Mission = dm;
            Turn = GetIntegerNotNull(n, "Turn");
            StartRequirements = new ListDescriptorRequirements(this, n.SelectSingleNode("StartRequirements"));
            StartRequirements.AllowCheating = false;

            // Загружаем части сообщения
            foreach (XmlNode np in n.SelectNodes("Part"))
            {
                Parts.Add(new DescriptorMissionMessagePart(np, dm));
            }

            XmlNode na = n.SelectSingleNode("Action");
            if (na != null)
            {
                typeAction = (TypeActionMessage)Enum.Parse(typeof(TypeActionMessage), GetStringNotNull(na, "Type"));
                idEntityAction = GetStringNotNull(na, "ID");
            }
        }

        internal bool Showed { get; set; }// Флаг  - сообщение показано игроку
        internal DescriptorMission Mission { get; }
        internal int Turn { get; }
        internal ListDescriptorRequirements StartRequirements { get; }// Требования для выдачи квеста
        internal List<DescriptorMissionMessagePart> Parts { get; } = new List<DescriptorMissionMessagePart>();
        internal void DoAction(Player p)
        {
            if (typeAction == TypeActionMessage.None)
                return;

            switch (typeAction)
            {
                case TypeActionMessage.AddQuest:
                    p.AddQuest(Mission.FindQuest(idEntityAction));
                    break;
                default:
                    throw new Exception("Неизвестный тип действия");
            }
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            StartRequirements.TuneLinks();
        }
    }

    internal sealed class DescriptorMissionMessagePart
    {
        public DescriptorMissionMessagePart(XmlNode n, DescriptorMission dm)
        {
            From = GetStringNotNull(n, "From");
            Text = GetDescription(n, "Text");
            foreach (DescriptorMissionPlayer dmp in dm.Players)
                Text = Text.Replace($"#{dmp.ID}#", FormMain.Descriptors.HumanPlayers[0].Name);
        }

        internal string From { get; }
        internal string Text { get; }
    }
}
