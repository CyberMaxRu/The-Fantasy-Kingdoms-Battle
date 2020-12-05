using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс вспомогательных методов
    internal sealed class Utils
    {
        internal static bool PointsIsNeighbor(Point p1, Point p2)
        {
            Debug.Assert(!p1.Equals(p2));

            return ((p2.X - 1 == p1.X) && (p2.Y - 1 == p1.Y))
                || ((p2.X == p1.X) && (p2.Y - 1 == p1.Y))
                || ((p2.X + 1 == p1.X) && (p2.Y - 1 == p1.Y))
                || ((p2.X - 1 == p1.X) && (p2.Y == p1.Y))
                || ((p2.X + 1 == p1.X) && (p2.Y == p1.Y))
                || ((p2.X - 1 == p1.X) && (p2.Y + 1 == p1.Y))
                || ((p2.X == p1.X) && (p2.Y + 1 == p1.Y))
                || ((p2.X + 1 == p1.X) && (p2.Y + 1 == p1.Y));
        }
        internal static void LoadRequirements(List<Requirement> list, XmlNode n)
        {
            XmlNode nr = n.SelectSingleNode("Requirements");
            if (nr != null)
            {
                foreach (XmlNode r in nr.SelectNodes("Requirement"))
                    list.Add(new Requirement(r.SelectSingleNode("Building").InnerText, Convert.ToInt32(r.SelectSingleNode("Level").InnerText)));
            }
        }

        internal static string AdaptDescription(string s)
        {
            return s.Replace("/", Environment.NewLine);
        }

        internal static int GetParamFromXml(XmlNode n)
        {
            return n != null ? Convert.ToInt32(n.InnerText) : 0;
        }

        internal static string GetParamFromXmlString(XmlNode n)
        {
            return n != null ? n.InnerText : "";
        }

        internal static double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
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

    }
}
