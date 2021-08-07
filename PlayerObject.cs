using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс объекта игрока
    internal abstract class PlayerObject : Entity
    {
        public PlayerObject(XmlNode n) : base(n)
        {
        }
        public PlayerObject() : base()
        {
        }

        internal abstract void PrepareHint();
        internal abstract void HideInfo();
        internal abstract void ShowInfo();
    }
}
