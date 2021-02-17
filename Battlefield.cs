using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
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
        public bool Pathfind(BattlefieldTile fromTile, BattlefieldTile toTile, BattleParticipant throughPlayer)
        {
            Debug.Assert(fromTile != null);
            Debug.Assert(toTile != null);
            Debug.Assert(fromTile != toTile);

            _path = new List<BattlefieldTile>();// Наиболее легкий найденный путь

            // Если это соседняя ячейка, то не надо искать путь
            if (fromTile.IsNeighbourTile(toTile))
            {
                // Если клетка зарезервирована или на ней есть юнит, идти на нее нельзя
                if ((toTile.ReservedForMove != null) || (toTile.Unit != null))
                    return false;

                _path.Add(toTile);
                return true;
            }

            // Если режим прохода сквозь юнита, то проверяем, есть ли вокруг ячейки для прохода
            // Если нет, выходиим
            if (throughPlayer != null)
            {
                bool foundedFree = false;
                foreach (BattlefieldTile t in fromTile.TilesAround)
                {
                    if ((t.Unit == null) && (t.ReservedForMove == null))
                    {
                        foundedFree = true;
                        break;
                    }    
                }

                if (!foundedFree)
                    return false;
            }

            // Очистка данных
            foreach (BattlefieldTile t in closedSet)
                t.ClearPathData();
            closedSet.Clear();

            foreach (BattlefieldTile t in openSet)
                t.ClearPathData();
            openSet.Clear();

            // Ищем путь
            FindPath(fromTile, toTile, throughPlayer);

            Debug.Assert((_path.Count() == 0) || fromTile.IsNeighbourTile(_path.First()));
            Debug.Assert((_path.Count() == 0) || (_path.Last() == toTile));

            return _path.Count() > 0;
        }

        public void FindPath(BattlefieldTile sourceTile, BattlefieldTile destTile, BattleParticipant throughPlayer)
        {
            Debug.Assert(sourceTile.Unit != null);

            BattlefieldTile currentNode;
            int lengthFromStart;
            //List<BattlefieldTile> path = new List<BattlefieldTile>();
            BattlefieldTile bestTile = null;

            // Стартуем с начальной ячейки
            openSet.Add(sourceTile);
            sourceTile.State = StateTile.Opened;

            // Цикл, пока есть открытые (необработанные) клетки
            while (openSet.Count > 0)
            {
                // Текущая ячейка - с наименьшим числом отставшегося пути
                bestTile = openSet.First();

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
                if (!openSet.Remove(currentNode)) 
                    throw new Exception("Ячейка не удалена из списка.");
                closedSet.Add(currentNode);// Только для того, чтобы по ней потом очистить данные для поиска пути
                currentNode.State = StateTile.Closed;

                foreach (BattlefieldTile neighbourNode in currentNode.TilesAround)
                {
                    // Если клетка недоступна, просто пропускаем ее
                    if (neighbourNode.State == StateTile.Unavailable)
                        continue;

                    // Если зарезервирована соседняя от начала пути ячейки, то обходим её
                    // Когда сделаем шаг, она может быть уже не зарезервирована, поэтому продолжим тот же путь
                    if ((neighbourNode.ReservedForMove != null) && neighbourNode.IsNeighbourTile(sourceTile))
                    {
                        neighbourNode.State = StateTile.Unavailable;
                        openSet.Remove(neighbourNode);
                        closedSet.Add(neighbourNode);

                        continue;
                    }

                    // Если на клетке есть юнит и это не клетка назначения, то пропускаем её
                    if ((neighbourNode.Unit != null) && (neighbourNode != destTile) && !((throughPlayer != null) && (neighbourNode.Unit.Player == throughPlayer)))
                    {
                        neighbourNode.State = StateTile.Unavailable;
                        openSet.Remove(neighbourNode);
                        closedSet.Add(neighbourNode);

                        continue;
                    }

                    //
                    lengthFromStart = currentNode.PathLengthFromStart + currentNode.GetDistanceToTile(neighbourNode);
                    if (((neighbourNode.PathLengthFromStart == 0) || (lengthFromStart < neighbourNode.PathLengthFromStart)) && (neighbourNode != sourceTile))
                    {
                        neighbourNode.PathLengthFromStart = lengthFromStart;
                        neighbourNode.HeuristicEstimatePathLength = neighbourNode.GetHeuristicPathLength(destTile);
                        neighbourNode.EstimateFullPathLength = neighbourNode.PathLengthFromStart + neighbourNode.HeuristicEstimatePathLength;
                        neighbourNode.PriorTile = currentNode;
                    }

                    if (neighbourNode.State == StateTile.Undefined)
                    {
                        openSet.Add(neighbourNode);
                        neighbourNode.State = StateTile.Opened;
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
                Debug.Assert((_path.Count == 0) || currentNode.IsNeighbourTile(_path.Last()));

                _path.Add(currentNode);
                currentNode = currentNode.PriorTile;
            }
            _path.Reverse();
            _path.RemoveAt(0);
        }
    }
}
