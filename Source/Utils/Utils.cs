using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

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

        internal static string FormatGreatness(int add, int perDay)
        {
            if ((add == 0) && (perDay == 0))
                return null;

            string text = "";

            if (add > 0)
                text = add.ToString();

            if (perDay > 0)
            {
                if (add > 0)
                    text += ", ";
                text += $"+{perDay}/д.";
            }

            return text;
        }

        internal static string DecIntegerBy10(int value, bool showPlus = false)
        {
            if (value == 0)
                return "0";

            int val10 = Math.Abs(value) / 10;
            int modval = Math.Abs(value) % 10;

            return (value > 0 ? (showPlus ? "+" : "") : "-") + val10.ToString() + (modval > 0 ? "." + modval.ToString() : "");
        }

        internal static string FormatInteger(int value)
        {
            return (value > 0 ? "+" : "") + value.ToString();
        }
    }

    internal class DebugWriter : TextWriterTraceListener
    {
        public DebugWriter(string filename) : base(filename)
        {

        }

        public override void Fail(string message)
        {
            Debug.WriteLine(DateTime.Now.ToString() + ": " + Application.ProductVersion);
            Debug.WriteLine(message);
            WriteStack();
            Debug.WriteLine(Environment.NewLine);
            Debug.Flush();

            base.Fail(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            Debug.WriteLine(DateTime.Now.ToString() + ": " + Application.ProductVersion);
            Debug.WriteLine(message);
            Debug.WriteLine(detailMessage);
            WriteStack();
            Debug.WriteLine(Environment.NewLine);
            Debug.Flush();

            base.Fail(message, detailMessage);
        }

        internal void WriteStack()
        {
            StackTrace st = new StackTrace(5, true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                string line = sf.GetFileLineNumber() > 0 ? $" (Line {sf.GetFileLineNumber()})" : "";
                Debug.WriteLine($"{sf.GetMethod()}: {sf.GetFileName()}{line}");
            }
        }
    }
}
