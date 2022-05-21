using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс пути до локации
    sealed internal class PathToLocation : BigEntity
    {
        public PathToLocation(Location l, TypeLobbyLocationPath lp) : base(null, l.Lobby)
        {
            Location = l;
            DescriptorPath = lp;
            Visible = lp.Visible;

            if (!Visible)
                PercentScoutForFound = l.Player.Lobby.Rnd.Next(lp.MinPercentScout, lp.MaxPercentScout + 1);
        }

        internal TypeLobbyLocationPath DescriptorPath { get; }
        internal Location Location { get; set; }// Локация, на которой находится сооружение
        internal Location ToLocation { get; set; }// Локация, в которую ведёт путь
        internal bool Visible { get; private set; }// Объект видим для игрока
        internal int PercentScoutForFound { get; set; }// Процент разведки локации, чтобы найти объект

        internal override int GetImageIndex()
        {
            return ToLocation.GetImageIndex();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }

        internal override void ShowInfo(int selectPage = -1)
        {
        }

        internal void Unhide(bool needNotice)
        {
            Debug.Assert(!Visible);

            Visible = true;

            if (needNotice)
                Location.Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.Explore);
        }

        internal void TuneLinks()
        {
            foreach (Location l in Location.Player.Locations)
            {
                if (DescriptorPath.PathToLocation.ID == l.Settings.ID)
                {
                    ToLocation = l;
                    break;
                }
            }

            Debug.Assert(ToLocation != null);
        }
    }
}
