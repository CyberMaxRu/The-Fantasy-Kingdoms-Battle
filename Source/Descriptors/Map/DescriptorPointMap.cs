using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypePointMap { Border, Region, Undefined };

    internal struct DescriptorPointMap
    {
        public DescriptorPointMap(int x, int y, Color color, TypePointMap typePoint, Region r)
        {
            X = x;
            Y = y;
            Color = color;
            TypePoint = typePoint;
            Region = r;
        }

        public int X;
        public int Y;
        public TypePointMap TypePoint;
        public Color Color;
        public Region Region;

        internal void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("Point");
            writer.WriteElementString("X", X.ToString());
            writer.WriteElementString("Y", Y.ToString());
            writer.WriteElementString("Color", Color.ToArgb().ToString());
            writer.WriteElementString("Type", ((int)TypePoint).ToString());
            writer.WriteEndElement();
        }
    }
}