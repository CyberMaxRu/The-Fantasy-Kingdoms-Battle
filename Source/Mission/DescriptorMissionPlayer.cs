using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorMissionPlayer
    { 
        public DescriptorMissionPlayer(XmlNode n)
        {
            ID = GetStringNotNull(n, "ID");
            Number = GetIntegerNotNull(n, "Number");
            Name = GetStringNotNull(n, "Name");
            Title = GetStringNotNull(n, "Title");
            ImageIndex = GetIntegerNotNull(n, "ImageIndex");
            TypePlayer = (TypePlayer)Enum.Parse(typeof(TypePlayer), GetStringNotNull(n, "TypePlayer"));
        }

        internal string ID { get; }
        internal int Number { get; }
        internal string Name { get; }
        internal string Title { get; }// Титул
        internal int ImageIndex { get;}
        internal TypePlayer TypePlayer { get; }

    }
}
