using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CNC_v3.Print;

namespace CNC_v3.Drawing.Shape
{
    [Serializable]
    public class Lines : IShape
    {
        private List<Point> _points;
        public override ShapeType Type { get; } = ShapeType.Lines;

        public Lines(Point p1, Point p2)
        {
            _points = new List<Point> {p1, p2};
        }

        public Lines(Point[] points)
        {
            _points = new List<Point>();
            _points.AddRange(points);
        }

        public void Add(Point p)
        {
            if (_points.Count > 0 && p.Equals(_points.Last()))
                return;
            _points.Add(p);
        }

        public void Add(Point[] p)
        {
            _points.AddRange(p);
        }

        public override Point GetFirstPoint()
        {
            return _points.First();
        }

        public override Point GetLastPoint()
        {
            return _points.Last();
        }

        public override Point[] GetPoints()
        {
            return _points.ToArray();
        }

        public override string ConvertToString(bool up = true)
        {
            string str = "";
            if (GetPoints().Length <= 1)
                return str;

            str += Print.Config.Prefix + (int) Print.Protocol.MoveTo + ":" +
                   (GetFirstPoint().X / Config.Scale) + ";" +
                   (GetFirstPoint().Y / Config.Scale) + ";\n";

            foreach (var point in GetPoints())
            {
                str += Print.Config.Prefix + (int) Type + ":" +
                       (point.X / Config.Scale) + ";" +
                       (point.Y / Config.Scale) + ";" + "\n";
            }

            str += Print.Config.Prefix + (int)Print.Protocol.MoveTo + ":" +
                   (GetLastPoint().X / Config.Scale) + ";" +
                   (GetLastPoint().Y / Config.Scale) + ";";
            return str;
        }
    }
}