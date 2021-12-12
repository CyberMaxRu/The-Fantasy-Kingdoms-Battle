using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Xml;
using System.IO;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorMap
    {
        public DescriptorMap(Bitmap bmp)
        {
            Width = bmp.Width;
            Height = bmp.Height;
            PointsMap = new DescriptorPointMap[Height, Width];
            Bitmap = bmp;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    PointsMap[y, x] = new DescriptorPointMap(x, y, Bitmap.GetPixel(x, y), TypePointMap.Undefined, null);
        }

        public DescriptorMap(string filename)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename + ".xml");

            Width = XmlUtils.GetIntegerNotNull(xmldoc, "Conquest/Map/Width");
            Height = XmlUtils.GetIntegerNotNull(xmldoc, "Conquest/Map/Height");
            int size = XmlUtils.GetIntegerNotNull(xmldoc, "Conquest/Map/SizeFileMap");

            XmlNode ne = xmldoc.SelectSingleNode("Conquest/Regions");
            if (ne != null)
            {
                foreach (XmlNode l in ne.SelectNodes("Region"))
                {
                    Regions.Add(new Region(l));
                }
            }

            FileStream fs = new FileStream(filename + ".map", FileMode.Open);

            byte[] arr = new byte[size];
            int readSize = fs.Read(arr, 0, size);
            Assert(readSize == size);

            PointsMap = new DescriptorPointMap[Height, Width];
            Bitmap = new Bitmap(Width, Height);

            int shift;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    shift = (y * Width + x) * sizeof(int) * 3;
                    Color color = Color.FromArgb(GetInt(shift));
                    int idx = GetInt(shift + (sizeof(int) * 2));
                    PointsMap[y, x] = new DescriptorPointMap(x, y, color, (TypePointMap)GetInt(shift + sizeof(int)), idx == -1 ? null : Regions[idx]);
                    Bitmap.SetPixel(x, y, color);
                }

            fs.Close();

            MakeMiniMap();

            int GetInt(int offset)
            {
                return BitConverter.ToInt32(arr, offset);
            }
        }

        internal int Width { get; }
        internal int Height { get; }
        internal DescriptorPointMap[,] PointsMap { get; }
        internal Bitmap Bitmap { get; }
        internal Bitmap MiniMap { get; private set; }
        internal int ScaleMiniMap { get; } = 12;
        internal List<Region> Regions = new List<Region>();

        internal void SearchBorder(Point p)
        {
            if (PointsMap[p.Y, p.X].TypePoint == TypePointMap.Undefined)
            {
                SearchBorderAround(p.X, p.Y, Bitmap.GetPixel(p.X, p.Y));
            }

            /*for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (PointsMap[y, x].TypePoint == TypePointMap.Border)
                        Bitmap.SetPixel(x, y, Color.GreenYellow);*/
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
                if ((ix < Width - 1) && (iy > 0))
                    AddNeighbours(ix + 1, iy - 1);
                if (ix > 0)
                    AddNeighbours(ix - 1, iy);
                if (ix < Width - 1)
                    AddNeighbours(ix + 1, iy);
                if ((ix > 0) && (iy < Height - 1))
                    AddNeighbours(ix - 1, iy + 1);
                if (iy < Height)
                    AddNeighbours(ix, iy + 1);
                if ((ix < Width - 1) && (iy < Height - 1))
                    AddNeighbours(ix + 1, iy + 1);
            }

            void AddNeighbours(int ix, int iy)
            {
                if (PointsMap[iy, ix].TypePoint == TypePointMap.Undefined)
                {
                    Color color = Bitmap.GetPixel(ix, iy);
                    if ((color.R <= 15) && (color.G <= 15) && (color.B <= 15))
                    {
                        PointsMap[iy, ix].TypePoint = TypePointMap.Border;
                        PointsMap[iy, ix].Color = Color.GreenYellow;
                        Bitmap.SetPixel(ix, iy, Color.GreenYellow);
                        points2.Add(new Point(ix, iy));
                    }
                }
            }
        }

        internal void SaveToFile(string filename)
        {
            XmlTextWriter textWriter = new XmlTextWriter(filename + ".xml", Encoding.UTF8);
            textWriter.WriteStartDocument();
            textWriter.Formatting = Formatting.Indented;

            textWriter.WriteStartElement("Conquest");

            FileStream fs = new FileStream(filename + ".map", FileMode.Create);

            byte[] arr = new byte[Width * Height * sizeof(int) * 3];

            int shift;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    shift = (y * Width + x) * sizeof(int) * 3;

                    //WriteInt(shift, x);
                    //WriteInt(shift, y);
                    WriteInt(shift, PointsMap[y, x].Color.ToArgb());
                    WriteInt(shift + sizeof(int), (int)PointsMap[y, x].TypePoint);
                    WriteInt(shift + sizeof(int) * 2, PointsMap[y, x].Region != null ? Regions.IndexOf(PointsMap[y, x].Region) : -1);
                }

            fs.Write(arr, 0, arr.Length);
            fs.Close();

            // Записываем информацию о настройках карты
            textWriter.WriteStartElement("Map");
            textWriter.WriteElementString("Width", Width.ToString());
            textWriter.WriteElementString("Height", Height.ToString());
            textWriter.WriteElementString("SizeFileMap", arr.Length.ToString());
            textWriter.WriteEndElement();

            //
            textWriter.WriteStartElement("Regions");
            foreach (Region r in Regions)
            {
                textWriter.WriteStartElement("Region");

                textWriter.WriteElementString("ID", r.ID.ToString());
                textWriter.WriteElementString("Name", r.Name);

                textWriter.WriteEndElement();// Region
            }
            textWriter.WriteEndElement();// Regions

            //
            textWriter.WriteEndElement();// Conquest
            textWriter.Close();
            textWriter.Dispose();

            // Массив карты сохраняем в байтовом формате, чтобы уменьшить занимаемое место
            void WriteInt(int offset, int val)
            {
                byte[] a = BitConverter.GetBytes(val);
                for (int i = 0; i < a.Length; i++)
                    arr[offset + i] = a[i];
            }
        }

        internal void MakeMiniMap()
        {
            MiniMap = new Bitmap(Bitmap.Width / ScaleMiniMap, Bitmap.Height / ScaleMiniMap);

            for (int y = 0; y < MiniMap.Height; y++)
                for (int x = 0; x < MiniMap.Width; x++)
                {
                    MiniMap.SetPixel(x, y, DetermineColor(x, y));
                }

            Color DetermineColor(int x, int y)
            {
                for (int iy = 0; iy < ScaleMiniMap; iy++)
                    for (int ix = 0; ix < ScaleMiniMap; ix++)
                    {
                        if (PointsMap[(y * ScaleMiniMap) + ix, (x * ScaleMiniMap) + ix].TypePoint == TypePointMap.Border)
                            return Color.Black;
                    }

                return Color.White;
            }
        }

        internal void DetermineRegions()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (PointsMap[y, x].TypePoint == TypePointMap.Undefined)
                    {
                        PointsMap[y, x].TypePoint = TypePointMap.Region;
                        PointsMap[y, x].Region = new Region();
                        Regions.Add(PointsMap[y, x].Region);
                        TreatRegion(PointsMap[y, x]);
                    }
                }

            void TreatRegion(DescriptorPointMap d)
            {
                List<Point> points = new List<Point>();
                List<Point> points2 = new List<Point>();
                TreatPoint(d.X, d.Y);
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
                    foreach (Point p in GetNeighbours(ix, iy))
                        if (PointsMap[p.Y, p.X].TypePoint == TypePointMap.Undefined)
                        {
                            PointsMap[p.Y, p.X].TypePoint = TypePointMap.Region;
                            PointsMap[p.Y, p.X].Region = d.Region;
                            points2.Add(p);
                        }
                }
            }
        }

        internal void FindCenters()
        {

        }

        private IEnumerable<Point> GetNeighbours(int x, int y)
        {
            if ((x > 0) && (y > 0))
                yield return new Point(x - 1, y - 1);
            if (y > 0)
                yield return new Point(x, y - 1);
            if ((x < Width - 1) && (y > 0))
                yield return new Point(x + 1, y - 1);
            if (x > 0)
                yield return new Point(x - 1, y);
            if (x < Width - 1)
                yield return new Point(x + 1, y);
            if ((x > 0) && (y < Height - 1))
                yield return new Point(x - 1, y + 1);
            if (y < Height - 1)
                yield return new Point(x, y + 1);
            if ((x < Width - 1) && (y < Height - 1))
                yield return new Point(x + 1, y + 1);
        }
    }
}