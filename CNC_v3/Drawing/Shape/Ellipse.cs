using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CNC_v3.Print;

namespace CNC_v3.Drawing.Shape
{
    [Serializable]
    public class Ellipse : IShape
    {

        private List<Point> _points;
        public override ShapeType Type { get; } = ShapeType.Ellipse;
        public Size Size;


        public Ellipse(Point p1, Point p2)
        {
            var tmpX = p1.X;
            p1.X = tmpX > p2.X ? p2.X : tmpX;
            p2.X = tmpX > p2.X ? tmpX : p2.X;

            var tmpY = p1.Y;
            p1.Y = tmpY > p2.Y ? p2.Y : tmpY;
            p2.Y = tmpY > p2.Y ? tmpY : p2.Y;

            Size = new Size(p2.X - p1.X, p2.Y - p1.Y);

            _points = new List<Point> { p1, p2 };
        }

        public System.Drawing.Rectangle GetEllipse()
        {
            return new System.Drawing.Rectangle(GetFirstPoint(), Size);
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
            string str = Print.Config.Prefix + (int)Type + ":" +
                (GetEllipse().X / Config.Scale) + ";" +
                (GetEllipse().Y / Config.Scale) + ";" +
                (GetEllipse().Width / Config.Scale) + ";" +
                (GetEllipse().Height / Config.Scale) + ";";
            return str;
        }
    }
}
