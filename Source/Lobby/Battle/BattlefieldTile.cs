using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
	internal enum StateTile { Undefined, Opened, Closed, Unavailable };
    internal sealed class BattlefieldTile
    {
		private Entity entity;

		private static Dictionary<int, int> pathLength = new Dictionary<int, int>();

		public BattlefieldTile(int x, int y)
		{
			X = x;
			Y = y;
			Coord = new Point(x, y);
		}

		internal int X { get; }
		internal int Y { get; }

		internal Point Coord { get; }

		public Entity Entity
		{
			get { return entity; }
			set
			{
				Debug.Assert(((entity == null) && (value != null)) || ((entity != null) && (value == null)),
					"Попытка поставить одно существо на другое.");
				entity = value;
			}
		}

		internal HeroInBattle Unit;
		internal HeroInBattle OutgoingUnit;
		internal HeroInBattle ReservedForMove;

		internal List<BattlefieldTile> TilesAround = new List<BattlefieldTile>();
		internal int PathLengthFromStart;// Длина пути от старта
		internal int HeuristicEstimatePathLength;// Примерное расстояние до цели
		internal int EstimateFullPathLength;
		internal BattlefieldTile PriorTile;
		internal StateTile State;

		// Ожидаемое полное расстояние до цели.

		internal int GetDistanceToTile(BattlefieldTile toTile)
		{
			Debug.Assert(this != toTile);

			return (X == toTile.X) || (Y == toTile.Y) ? 10 : 14;
		}

		internal int GetHeuristicPathLength(BattlefieldTile toTile)
		{
			int deltaX = Math.Abs(X - toTile.X);
			int deltaY = Math.Abs(Y - toTile.Y);
			int len;

			if (pathLength.TryGetValue(deltaX + 1000 + deltaY, out len))
				return len;
			
			len = Convert.ToInt32(Math.Sqrt(deltaX * deltaX + deltaY * deltaY) * 20);
			pathLength.Add(deltaX + 1000 + deltaY, len);
			return len;
		}

		// Добавить соседнюю ячейку
		internal void AddNeighboring(Battlefield bf)
		{
			if (Y > 0)
				TilesAround.Add(bf.Tiles[Y - 1, X]);
			if ((X < bf.Width - 1) && (Y > 0))
				TilesAround.Add(bf.Tiles[Y - 1, X + 1]);
			if (X > 0)
				TilesAround.Add(bf.Tiles[Y, X - 1]);
			if (X < bf.Width - 1)
				TilesAround.Add(bf.Tiles[Y, X + 1]);
			if ((X > 0) && (Y < bf.Height - 1))
				TilesAround.Add(bf.Tiles[Y + 1, X - 1]);
			if (Y < bf.Height - 1)
				TilesAround.Add(bf.Tiles[Y + 1, X]);
			if ((X < bf.Width - 1) && (Y < bf.Height - 1))
				TilesAround.Add(bf.Tiles[Y + 1, X + 1]);
			if ((X > 0) && (Y > 0))
				TilesAround.Add(bf.Tiles[Y - 1, X - 1]);
		}

		internal bool IsNeighbourTile(BattlefieldTile tile)
		{
			Debug.Assert(tile != null);

			return (tile.X >= X - 1) && (tile.X <= X + 1) && (tile.Y >= Y - 1) && (tile.Y <= Y + 1);
		}

		internal void ClearPathData()
        {
			PathLengthFromStart = 0;
			HeuristicEstimatePathLength = 0;
			EstimateFullPathLength = int.MaxValue;
			State = StateTile.Undefined;
			PriorTile = null;
		}
	}
}
