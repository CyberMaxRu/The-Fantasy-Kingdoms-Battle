﻿using System;
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
        public Location(Player player, TypeLobbyLayerSettings settings)
        {
            Player = player;
            Settings = settings;
            Descriptor = settings.Location;
            Ownership = settings.Ownership;
            Lairs = new Construction[Player.Lobby.TypeLobby.LairsHeight, Player.Lobby.TypeLobby.LairsWidth];

            // Создание рандомных логов монстров согласно настроек типа лобби
            // Для этого сначала создаем логова по минимальному списку, а оставшиеся ячейки - из оставшихся по максимуму
            int idxCell;
            int idxTypeLair;

            List<DescriptorConstruction> lairs = new List<DescriptorConstruction>();
            lairs.AddRange(Player.Lobby.Lairs[settings.Number]);
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
                Lairs[cells[idxCell].Y, cells[idxCell].X] = new Construction(Player, lairs[idxTypeLair], lairs[idxTypeLair].DefaultLevel, cells[idxCell].X, cells[idxCell].Y, this);

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
            }
        }

        internal Player Player { get; }
        internal TypeLobbyLayerSettings Settings { get; }
        internal DescriptorLocation Descriptor { get; }
        internal bool Ownership { get; set; }
        internal Construction[,] Lairs { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {
            
        }

        internal override void PrepareHint()
        {
            
        }

        internal override void ShowInfo()
        {
            Program.formMain.panelLocationInfo.Entity = this;
            Program.formMain.panelLocationInfo.Visible = true;
        }

        internal override void HideInfo()
        {
            Program.formMain.panelLocationInfo.Visible = false;
        }
    }
}