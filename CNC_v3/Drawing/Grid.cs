using System;
using System.Drawing;
using CNC_v3.Print;

namespace CNC_v3.Drawing
{
    public class Grid
    {
        public GridSize Size { get; set; }
        public bool Enabled { get; set; }
        public Pen Color { get; set; }

        public Grid(Color color, GridSize size = GridSize._20Px)
        {
            Color = new Pen(color);
            Size = size;
            Enabled = true;
        }

        public Bitmap GetGridBitmap(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            var g = Graphics.FromImage(bmp);


            if (Enabled && Size != GridSize._0Px)
            {
                for (var i = 0; i <= width / (int)Size; i++)
                    g.DrawLine(Color, i * (int)Size, 0, i * (int)Size, height);

                for (var i = 0; i <= height / (int)Size; i++)
                    g.DrawLine(Color, 0, i * (int)Size, width, i * (int)Size);
            }

            g.DrawLine(Pens.Blue, 0, Config.PaperSize.Height, Config.PaperSize.Width, Config.PaperSize.Height);
            g.DrawLine(Pens.Blue, Config.PaperSize.Width, 0, Config.PaperSize.Width, Config.PaperSize.Height);

            g.DrawLine(Pens.Red, 0, Config.A4Size.Height, Config.A4Size.Width, Config.A4Size.Height);
            g.DrawLine(Pens.Red, Config.A4Size.Width, 0, Config.A4Size.Width, Config.A4Size.Height);

            g.Dispose();
            return bmp;
        }

        public Bitmap GetGridBitmap(Size size)
        {
            return GetGridBitmap(size.Width, size.Height);
        }

        public Point GrabToGrid(Point p)
        {
            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;

            if (!Enabled || Size == GridSize._0Px)
                return p;

            var x = Math.Floor(p.X / (float)Size + 0.5) * (int)Size;
            var y = Math.Floor(p.Y / (float)Size + 0.5) * (int)Size;
            return new Point((int)x, (int)y);
        }
    }
}
