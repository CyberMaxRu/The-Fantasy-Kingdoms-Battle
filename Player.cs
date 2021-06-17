using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс игрока
    internal abstract class Player : TypeObject
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
    }
}