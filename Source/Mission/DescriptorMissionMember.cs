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
    internal sealed class DescriptorMissionMember
    {
        public DescriptorMissionMember(XmlNode n)
        {
            ID = GetStringNotNull(n, "ID");
            Name = GetStringNotNull(n, "Name");
            Title = GetStringNotNull(n, "Title");
            ImageIndex = GetIntegerNotNull(n, "ImageIndex");


            if (Name == "#Lord1#")
                Name = FormMain.Descriptors.HumanPlayers[0].Name;

            if (ImageIndex == -100)
                ImageIndex = FormMain.Descriptors.HumanPlayers[0].ImageIndex;
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Title { get; }// Титул
        internal int ImageIndex { get; }
    }
}
