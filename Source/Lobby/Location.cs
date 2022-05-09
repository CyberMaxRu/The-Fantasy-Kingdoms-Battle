using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс локации
    internal sealed class Location : BigEntity
    {
        public Location(Player player, TypeLobbyLocationSettings settings) : base(player.Descriptor, player.Lobby)
        {
            Player = player;
            Settings = settings;
            Visible = settings.VisibleByDefault;

            // Создание сооружений согласно настройкам
            foreach (TypeLobbyLairSettings ls in settings.LairsSettings)
            {
                Lairs.Add(new Construction(player, ls.TypeLair, this));
            }

            // Создание рандомных логов монстров согласно настроек типа лобби
            // Для этого сначала создаем логова по минимальному списку, а оставшиеся ячейки - из оставшихся по максимуму
            /*int idxCell;
            int idxTypeLair;

            List<DescriptorConstruction> lairs = new List<DescriptorConstruction>();
            lairs.AddRange(Player.Lobby.Lairs[settings.Coord.Y, settings.Coord.X]);
            List<Point> cells = GetCells();
            Debug.Assert(cells.Count <= lairs.Count);

            while (cells.Count > 0)
            {
                // Берем случайную ячейку
                idxCell = Player.Lobby.Rnd.Next(cells.Count);
                // Берем случайное логово
                idxTypeLair = Player.Lobby.Rnd.Next(lairs.Count);

                // Помещаем в нее логово
                Debug.Assert(Lairs[cells[idxCell].Y, cells[idxCell].X] == null);

                Lairs[cells[idxCell].Y, cells[idxCell].X] = new Construction(Player, lairs[idxTypeLair], lairs[idxTypeLair].DefaultLevel, cells[idxCell].X, cells[idxCell].Y, this, TypeNoticeForPlayer.None);

                cells.RemoveAt(idxCell);// Убираем ячейку из списка доступных
                lairs.RemoveAt(idxTypeLair);// Убираем тип логова из списка доступных
            }

            List<Point> GetCells()
            {
                List<Point> l = new List<Point>();
                for (int y = 0; y < Player.Lobby.TypeLobby.LairsHeight; y++)
                    for (int x = 0; x < Player.Lobby.TypeLobby.LairsWidth; x++)
                        l.Add(new Point(x, y));

                return l;
            }*/
        }
        public Location(Player player) : base(player.Descriptor, player.Lobby)
        {
            Player = player;
            Visible = false;
        }

        internal Player Player { get; }
        internal TypeLobbyLocationSettings Settings { get; }
        internal List<Construction> Lairs { get; } = new List<Construction>();
        internal bool Visible { get; set; }

        internal override int GetImageIndex()
        {
            return Settings.TypeLandscape.ImageIndex;
        }

        internal override bool GetNormalImage() => true;

        internal override void MakeMenu(VCMenuCell[,] menu)
        {
            
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Lobby.Layer.panelLocationInfo.Entity = this;
            Lobby.Layer.panelLocationInfo.Visible = true;
        }

        internal override void HideInfo()
        {
            Lobby.Layer.panelLocationInfo.Visible = false;
        }
    }
}
