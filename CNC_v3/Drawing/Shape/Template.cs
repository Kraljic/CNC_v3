using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CNC_v3.Drawing.Shape
{
    [Serializable]
    public class Template : IShape
    {
        public static int GetDistance(Point p1, Point p2)
        {
            var x = Math.Abs(p1.X - p2.X);
            var y = Math.Abs(p1.Y - p2.Y);
            var dist = Math.Sqrt(x * x + y * y);
            return (int)dist;
        }
        public static List<IShape> Sort(List<IShape> shapes)
        {
            return null;
        }

        private List<Point> _points;
        public override ShapeType Type { get; } = ShapeType.None;

        public Template()
        {
            _points = new List<Point>();
        }

        public void Add(Point p)
        {
            if (_points.Count > 0 && p.Equals(_points.Last()))
                return;
            _points.Add(p);
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

        public IShape ConvertTo(ShapeType type)
        {
            if (_points.Count < 2)
                return null;

            switch (type)
            {
                case ShapeType.Line:
                    var line = new Line(GetFirstPoint(), GetLastPoint());
                    return line;
                case ShapeType.Lines:
                    var lines = new Lines(GetPoints());
                    return lines;
                case ShapeType.Rectangle:
                    {
                        var rec = new Rectangle(GetFirstPoint(), GetLastPoint());
                        if (rec.Size.Width == 0 || rec.Size.Height == 0)
                            return null;
                        return rec;
                    }
                case ShapeType.Ellipse:
                    {
                        var ellipse = new Ellipse(GetFirstPoint(), GetLastPoint());
                        if (ellipse.Size.Width == 0 || ellipse.Size.Height == 0)
                            return null;
                        return ellipse;
                    }
            }

            return null;
        }

        public override string ConvertToString(bool up = true)
        {
            return string.Empty;
        }
    }
}
