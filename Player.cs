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

        internal TypePlayer TypePlayer { get; }
    }
}