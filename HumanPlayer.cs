using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс игрока-человека
    internal sealed class HumanPlayer: Player
    {
        public HumanPlayer(XmlNode n) : base(n, TypePlayer.Human)
        {
            Debug.Assert(Name.Length <= 31);

            foreach (HumanPlayer hp in FormMain.Config.HumanPlayers)
            {
                Debug.Assert(ID != hp.ID);
                Debug.Assert(Name != hp.Name);
                Debug.Assert(ImageIndex != hp.ImageIndex);
            }

            foreach (ComputerPlayer cp in FormMain.Config.ComputerPlayers)
            {
                Debug.Assert(ID != cp.ID);
                Debug.Assert(Name != cp.Name);
            }

            DisableComputerPlayerByAvatar();
        }

        public HumanPlayer(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex, TypePlayer.Human)
        {
            Debug.Assert(Name.Length <= 31);

            DisableComputerPlayerByAvatar();
        }

        private void DisableComputerPlayerByAvatar()
        {
            foreach (ComputerPlayer cp in FormMain.Config.ComputerPlayers)
            {
                if (cp.ImageIndex == ImageIndex)
                {
                    cp.Active = false;
                    break;
                }
            }
        }
    }
}
