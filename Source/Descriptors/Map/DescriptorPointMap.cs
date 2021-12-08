using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypePointMap { Border, Province, Sea, Other, Undefined };

    internal struct DescriptorPointMap
    {
        public DescriptorPointMap(int x, int y, TypePointMap typePoint)
        {
            X = x;
            Y = y;
            TypePoint = typePoint;
        }

        public int X;
        public int Y;
        public TypePointMap TypePoint;
    }
}