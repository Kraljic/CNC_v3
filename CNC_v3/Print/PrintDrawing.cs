using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNC_v3.Drawing.Shape;

namespace CNC_v3.Print
{
    public class PrintDrawing
    {
        public static string GetPrintString(List<Drawing.Shape.IShape> objects)
        {
            var sortedList = SortShapes(objects);
            Drawing.Shape.IShape shapeBefore = null;

            string data = "";
            foreach (var i in sortedList)
            {
                if (shapeBefore != null && shapeBefore.Type != ShapeType.Ellipse)
                {
                    if (shapeBefore.GetLastPoint() == i.GetFirstPoint())
                    {
                        data += i.ConvertToString(false) + "\n";
                        shapeBefore = i;
                        continue;
                    }
                }

                data += i.ConvertToString() + "\n";
                shapeBefore = i;
            }
            
            return data;
        }

        public static List<Drawing.Shape.IShape> SortShapes(List<Drawing.Shape.IShape> objects)
        {
            var list = objects.ToList();
            var sortedList = new List<Drawing.Shape.IShape>();
            
            var lastPoint = new Point(0, 0);

            while (list.Any())
            {
                Drawing.Shape.IShape obj = findClosestShape(list, lastPoint);

                if (obj == null)
                    return sortedList;

                sortedList.Add(obj);
                list.Remove(obj);
                lastPoint = obj.GetLastPoint();
            }

            return sortedList;
        }

        private static Drawing.Shape.IShape findClosestShape(List<Drawing.Shape.IShape> objects, Point p)
        {
            Drawing.Shape.IShape obj = null;
            double distance= 9999d;

            double tmpX, tmpY, distanceTmp;
            foreach (var shape in objects)
            {
                tmpX = Math.Abs(shape.GetFirstPoint().X - p.X);
                tmpY = Math.Abs(shape.GetFirstPoint().Y - p.Y);

                distanceTmp = Math.Sqrt(tmpX * tmpX + tmpY * tmpY);
                if (distanceTmp < distance)
                {
                    obj = shape;
                    distance = distanceTmp;
                    if (distance == 0)
                        return obj;
                }
            }

            return obj;
        }
        public static void SaveShapeList(string path, List<Drawing.Shape.IShape> list)
        {
            //serialize
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, list);
            }

        }

        public static List<Drawing.Shape.IShape> LoadShapeList(string path)
        {
            List<Drawing.Shape.IShape> list;
            //deserialize
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                list = (List<Drawing.Shape.IShape>)bformatter.Deserialize(stream);
            }
            return list;
        }


    }
}