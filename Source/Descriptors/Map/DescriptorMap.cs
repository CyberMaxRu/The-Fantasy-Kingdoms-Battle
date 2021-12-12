using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorMap
    {
        private VCMap map;

        public DescriptorMap(int width, int height, VCMap map)
        {
            Width = width;
            Height = height;
            PointsMap = new DescriptorPointMap[Height, Width];
            this.map = map;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    PointsMap[y, x] = new DescriptorPointMap(x, y, map.Bitmap.GetPixel(x, y), TypePointMap.Undefined);                    
        }

        internal int Width { get; }
        internal int Height { get; }
        internal DescriptorPointMap[,] PointsMap { get; }

        internal void SearchBorder(Point p)
        {
            if (PointsMap[p.Y, p.X].TypePoint == TypePointMap.Undefined)
            {
                SearchBorderAround(p.X, p.Y, map.Bitmap.GetPixel(p.X, p.Y));
            }

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (PointsMap[y, x].TypePoint == TypePointMap.Border)
                        map.Bitmap.SetPixel(x, y, Color.GreenYellow);
        }

        internal void SearchBorderAround(int x, int y, Color c)
        {
            List<Point> points = new List<Point>();
            List<Point> points2 = new List<Point>();
            TreatPoint(x, y);
            points.AddRange(points2);
            points2.Clear();

            for (; ; )
            {
                foreach (Point p in points)
                {
                    TreatPoint(p.X, p.Y);
                }

                if (points2.Count == 0)
                    break;

                points.Clear();
                points.AddRange(points2);
                points2.Clear();
            }

            void TreatPoint(int ix, int iy)
            {
                if ((ix > 0) && (iy > 0))
                    AddNeighbours(ix - 1, iy - 1);
                if (iy > 0)
                    AddNeighbours(ix, iy - 1);
                if ((ix < Width) && (iy > 0))
                    AddNeighbours(ix + 1, iy - 1);
                if (ix > 0)
                    AddNeighbours(ix - 1, iy);
                if (ix < Width)
                    AddNeighbours(ix + 1, iy);
                if ((ix > 0) && (iy < Height))
                    AddNeighbours(ix - 1, iy + 1);
                if (iy < Height)
                    AddNeighbours(ix, iy + 1);
                if ((ix < Width) && (iy < Height))
                    AddNeighbours(ix + 1, iy + 1);

            }

            void AddNeighbours(int ix, int iy)
            {
                if (PointsMap[iy, ix].TypePoint == TypePointMap.Undefined)
                {
                    Color color = map.Bitmap.GetPixel(ix, iy);
                    if ((color.R <= 15) && (color.G <= 15) && (color.B <= 15))
                    {
                        PointsMap[iy, ix].TypePoint = TypePointMap.Border;
                        points2.Add(new Point(ix, iy));
                    }
                }
            }
        }
    }
}
