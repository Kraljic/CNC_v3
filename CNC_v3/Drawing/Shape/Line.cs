using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CNC_v3.Print;

namespace CNC_v3.Drawing.Shape
{
    [Serializable]
    public class Line : IShape
    {

        private List<Point> _points;
        public override ShapeType Type { get; } = ShapeType.Line;

        public Line(Point p1, Point p2)
        {
            _points = new List<Point> {p1, p2};
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

        public override string ConvertToString(bool dontConnectWithPrevius = true)
        {
            string str = "";
            if (dontConnectWithPrevius)
            {
                str = Print.Config.Prefix + (int) Type + ":" +
                      (GetFirstPoint().X / Config.Scale) + ";" +
                      (GetFirstPoint().Y / Config.Scale) + ";" +
                      (GetLastPoint().X / Config.Scale) + ";" +
                      (GetLastPoint().Y / Config.Scale) + ";";
            }
            else
            {
                str = Print.Config.Prefix + (int) Protocol.Lines + ":" +
                      (GetLastPoint().X / Config.Scale) + ";" +
                      (GetLastPoint().Y / Config.Scale) + ";";
            }
            return str;
        }
    }
}