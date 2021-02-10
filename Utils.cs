using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс вспомогательных методов
    internal sealed class Utils
    {
        internal static double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        internal static void TrimActions(List<DateTime> list)
        {
            DateTime currentDateTime = DateTime.Now;

            TimeSpan diffTime;
            for (int i = 0; i < list.Count; i++)
            {
                diffTime = currentDateTime - list[i];
                if (diffTime.TotalMilliseconds <= 1_000)
                {
                    list.RemoveRange(0, i);
                    break;
                }
            }
        }

        internal static bool PointInRectagle(int left, int top, int width, int height, int x, int y)
        {
            return (left <= x) && (top <= y) && (left + width >= x) && (top + height >= y);
        }
    }
}
