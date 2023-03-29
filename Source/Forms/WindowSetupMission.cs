using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowSetupMission : WindowOkCancel
    {
        private LobbySettings mission;

        public WindowSetupMission(LobbySettings m) : base("Настройка миссии", false)
        {
            mission = m;
        }
    }
}
