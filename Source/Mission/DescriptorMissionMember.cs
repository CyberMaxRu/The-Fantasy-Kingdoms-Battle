using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Участники миссии
    internal sealed class DescriptorMissionMember : DescriptorEntity
    {
        public DescriptorMissionMember(XmlNode n) : base(n)
        {
            Title = GetStringNotNull(n, "Title");

            if (Name == "#Lord1#")
                Name = FormMain.Descriptors.HumanPlayers[0].Name;

            if (ImageIndex == -100)
                ImageIndex = FormMain.Descriptors.HumanPlayers[0].ImageIndex;
        }

        internal string Title { get; }// Титул

        internal override string GetTypeEntity() => "Участник";
    }
}
