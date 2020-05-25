using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс вспомогательных методов
    internal sealed class Utils
    {
        internal static bool PointsIsNeighbor(Point p1, Point p2)
        {
            Debug.Assert(!((p1.X == p2.X) && (p1.Y == p2.Y)));

            return ((p2.X - 1 == p1.X) && (p2.Y - 1 == p1.Y))
                || ((p2.X == p1.X) && (p2.Y - 1 == p1.Y))
                || ((p2.X + 1 == p1.X) && (p2.Y - 1 == p1.Y))
                || ((p2.X - 1 == p1.X) && (p2.Y == p1.Y))
                || ((p2.X + 1 == p1.X) && (p2.Y == p1.Y))
                || ((p2.X - 1 == p1.X) && (p2.Y + 1 == p1.Y))
                || ((p2.X == p1.X) && (p2.Y + 1 == p1.Y))
                || ((p2.X + 1 == p1.X) && (p2.Y + 1 == p1.Y));
        }
    }
}
