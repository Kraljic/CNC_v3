using System;
using CNC_v3.Print;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CNC_v3.Drawing.Shape
{
    [Serializable]
    public class Rectangle : IShape
    {
        private List<Point> _points;
        public override ShapeType Type { get; } = ShapeType.Rectangle;
        public Size Size;

        public Rectangle(Point p1, Point p2)
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

        public System.Drawing.Rectangle GetRectangle()
        {
            return new System.Drawing.Rectangle(GetFirstPoint(), Size);
        }

        public override Point GetFirstPoint()
        {
            return _points.First();
        }

        public override Point GetLastPoint()
        {
            return _points.First();
        }

        public override Point[] GetPoints()
        {
            return _points.ToArray();
        }

        public override string ConvertToString(bool up = true)
        {
            string str = Config.Prefix + (int)Type + ":" +
                (GetRectangle().X / Config.Scale) + ";" +
                (GetRectangle().Y / Config.Scale)+";" +
                (GetRectangle().Width / Config.Scale) + ";" +
                (GetRectangle().Height / Config.Scale) + ";";
            return str;
        }
    }
}