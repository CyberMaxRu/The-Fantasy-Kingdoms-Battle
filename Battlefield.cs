using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс поля битвы
    internal sealed class Battlefield
    {
        // Основные параметры
        // Поиск пути
        HashSet<BattlefieldTile> closedSet = new HashSet< BattlefieldTile>();
        HashSet<BattlefieldTile> openSet = new HashSet<BattlefieldTile>();

        public Battlefield(int width, int height)
        {
            Width = width;
            Height = height;

            Tiles = new BattlefieldTile[height, width];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Tiles[y, x] = new BattlefieldTile(x, y);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Tiles[y, x].AddNeighboring(this);
        }

        internal int Width { get; }
        internal int Height { get; }
        internal BattlefieldTile[,] Tiles;

        public List<BattlefieldTile> _path;

        // Поиск пути
        public bool Pathfind(BattlefieldTile fromTile, BattlefieldTile toTile, Player throughPlayer)
        {
            Debug.Assert(fromTile != null);
            Debug.Assert(toTile != null);

            // Если клетка зарезервирована, но она не соседняя, то идти на нее можно
            // Когда сделаем шаг, возможно, она уже освободится
            if ((toTile.ReservedForMove != null) && fromTile.IsNeighbourTile(toTile))
                return false;

            // А* с учетом направления
            _path = new List<BattlefieldTile>();// Наиболее легкий найденный путь

            foreach (BattlefieldTile t in closedSet)
            {
                t.PathLengthFromStart = 0;
                t.HeuristicEstimatePathLength = 0;
                t.EstimateFullPathLength = 0;
                t.Closed = false;
                t.Opened = false;
                t.PriorTile = null;
            }
            closedSet.Clear();

            foreach (BattlefieldTile t in openSet)
            {
                t.PathLengthFromStart = 0;
                t.HeuristicEstimatePathLength = 0;
                t.EstimateFullPathLength = 0;
                t.Closed = false;
                t.Opened = false;
                t.PriorTile = null;
            }
            openSet.Clear();

            FindPath(fromTile, toTile, throughPlayer);

            Debug.Assert((_path.Count() == 0) || (Utils.PointsIsNeighbor(fromTile.Coord, _path.First().Coord) == true));
            Debug.Assert((_path.Count() == 0) || (_path.Last() == toTile));

            return _path.Count() > 0;
        }

        public void FindPath(BattlefieldTile sourceTile, BattlefieldTile destTile, Player throughPlayer)
        {
            Debug.Assert(sourceTile.Unit != null);

            BattlefieldTile currentNode;
            int lengthFromStart;
            //List<BattlefieldTile> path = new List<BattlefieldTile>();
            BattlefieldTile bestTile;

            // Стартуем с начальной ячейки
            openSet.Add(sourceTile);
            sourceTile.Opened = true;

            while (openSet.Count > 0)
            {
                // Текущая ячейка - с наименьшим числом отставшегося пути
                bestTile = openSet[0].First();

                foreach (BattlefieldTile t in openSet)
                {
                    if (t.EstimateFullPathLength < bestTile.EstimateFullPathLength)
                        currentNode = t;
                }
                Debug.Assert(bestTile != null);
                currentNode = bestTile;

                // Если пришли к конечной точке - строим путь и выходим
                if (currentNode == destTile)
                {
                    BuildPathForNode(currentNode);
                    return;
                }

                // Убираем ячейку из необработанных и добавляем в обработанные
                openSet.Remove(bestTile);
                closedSet.Add(currentNode);
                currentNode.Opened = false;
                currentNode.Closed = true;
                // Шаг 6.
                // Переделать на foreach
                foreach (BattlefieldTile neighbourNode in currentNode.TilesAround)
                {
                    // Если зарезервирована соседняя от начала пути ячейки, то обходим её
                    // Когда сделаем шаг, она может быть уже не зарезервирована, поэтому продолжим тот же путь
                    if (neighbourNode.ReservedForMove != null)
                        if (neighbourNode.IsNeighbourTile(sourceTile) == true)
                            continue;

                    if (neighbourNode.Unit != null)
                        if (neighbourNode != destTile)
                            if (!((throughPlayer != null) && (neighbourNode.Unit.Player == throughPlayer)))
                                //if (neighbourNode.IsNeighbourTile(sourceTile) == true)
                                continue;

                    //
                    lengthFromStart = currentNode.PathLengthFromStart + currentNode.GetDistanceToTile(neighbourNode);
                    if (((neighbourNode.PathLengthFromStart == 0) || (lengthFromStart < neighbourNode.PathLengthFromStart)) && (neighbourNode != sourceTile))
                    {
                        neighbourNode.PathLengthFromStart = lengthFromStart;
                        neighbourNode.HeuristicEstimatePathLength = neighbourNode.GetHeuristicPathLength(destTile);
                        neighbourNode.EstimateFullPathLength = neighbourNode.PathLengthFromStart + neighbourNode.HeuristicEstimatePathLength;
                        neighbourNode.PriorTile = currentNode;
                    }

                    // Шаг 7.
                    if ((neighbourNode.Closed == false) && (neighbourNode.Opened == false))
                    {
                        openSet.Add(neighbourNode);
                        neighbourNode.Opened = true;

                    }
                }
            }

            return;
        }

        private void BuildPathForNode(BattlefieldTile pathNode)
        {
            BattlefieldTile currentNode = pathNode;
            while (currentNode != null)
            {
                Debug.Assert(_path.IndexOf(currentNode) == -1);
                //Debug.Assert(currentNode.ReservedForMove == null);
                Debug.Assert((_path.Count == 0) || (Utils.PointsIsNeighbor(new Point(currentNode.X, currentNode.Y), new Point(_path.Last().X, _path.Last().Y)) == true));

                _path.Add(currentNode);
                currentNode = currentNode.PriorTile;
            }
            _path.Reverse();
            _path.RemoveAt(0);
        }
    }
}
