using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип игрока
    internal abstract class Player : DescriptorEntity
    {
        public Player(XmlNode n, TypePlayer typePlayer) : base(n)
        {
            TypePlayer = typePlayer;
        }

        public Player(string id, string name, string description, int imageIndex, TypePlayer typePlayer) : base(id, name, description, imageIndex)
        {
            TypePlayer = typePlayer;
        }

        internal TypePlayer TypePlayer { get; }

        protected override int ShiftImageIndex() => FormMain.Config.ImageIndexFirstAvatar;
    }
}