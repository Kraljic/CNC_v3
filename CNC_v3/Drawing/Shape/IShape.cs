using System;
using System.Drawing;

namespace CNC_v3.Drawing.Shape
{
    [Serializable]
    public abstract class IShape
    {
        public abstract ShapeType Type { get; }

        public abstract Point GetFirstPoint();
        public abstract Point GetLastPoint();
        public  abstract Point[] GetPoints();
        
        public abstract string ConvertToString(bool up = true);
    }
}