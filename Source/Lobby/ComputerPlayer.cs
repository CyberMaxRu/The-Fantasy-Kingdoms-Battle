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
    internal sealed class ComputerPlayer : Player
    {
        public ComputerPlayer(XmlNode n) : base(n, TypePlayer.Computer)
        {
            foreach (ComputerPlayer cp in FormMain.Config.ComputerPlayers)
            {
                Debug.Assert(ID != cp.ID);
                Debug.Assert(Name != cp.Name);
                Debug.Assert(ImageIndex != cp.ImageIndex);
            }
        }

        internal bool Active { get; set; } = true;// Активность игрока. Он неактивен, если его аватарка используется

        protected override void CheckData()
        {
            base.CheckData();

            Debug.Assert(ImageIndex >= FormMain.Config.ImageIndexFirstAvatar);
            Debug.Assert(ImageIndex < FormMain.Config.ImageIndexFirstAvatar + FormMain.Config.QuantityInternalAvatars);
        }
    }
}
