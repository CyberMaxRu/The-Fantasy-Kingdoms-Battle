using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс игрока-компьютера
    internal sealed class ComputerPlayer : TypeObject 
    {
        public ComputerPlayer(XmlNode n) : base(n)
        {
            foreach (ComputerPlayer cp in FormMain.Config.ComputerPlayers)
            {
                Debug.Assert(ID != cp.ID);
                Debug.Assert(Name != cp.Name);
                Debug.Assert(ImageIndex != cp.ImageIndex);
            }
        }
    }
}
