using CNC_v3.Drawing.Shape;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ellipse = CNC_v3.Drawing.Shape.Ellipse;
using Rectangle = CNC_v3.Drawing.Shape.Rectangle;

namespace CNC_v3.Drawing
{
    public class DrawingCanvas
    {
        private bool _run;

        private readonly PictureBox _pictureBox;
        private Bitmap _imageBitmap;
        private Size _canvaSize;

        private List<IShape> _objects;
        private List<IShape> _objectsUndoList;
        private Template _template;

        private bool _isDrawing;

        public Pen Pen;
        public ShapeType Tool { get; set; }
        public Grid Grid;

        public List<IShape> Objects { get { return _objects; } set { _objects = value; } }

        public DrawingCanvas(PictureBox pictureBox)
        {
            _pictureBox = pictureBox;

            _canvaSize = pictureBox.Size;
            Grid = new Grid(Color.LightGray);
            _objects = new List<IShape>();
            _objectsUndoList = new List<IShape>();
            _isDrawing = false;
            Tool = ShapeType.None;
            Pen = new Pen(Color.Black);

            _pictureBox.MouseMove += _pictureBox_MouseMove;
            _pictureBox.MouseDown += _pictureBox_MouseDown;
            _pictureBox.MouseUp += _pictureBox_MouseUp;
            _pictureBox.SizeChanged += _pictureBox_SizeChanged;
        }

        public void Run()
        {
            _run = true;

            Graphics g;

            while (_run)
            {
                try
                {
                    if (_canvaSize.Width > 0 && _canvaSize.Height > 0)
                    {
                        _imageBitmap = new Bitmap(_canvaSize.Width, _canvaSize.Height);
                        g = Graphics.FromImage(_imageBitmap);

                        RefreshImage(g);
                        if (_isDrawing)
                            RefreshDrawing(g);

                        _pictureBox.Image = _imageBitmap;
                    }
                }
                catch (Exception) { }
                

                GC.Collect();
                Thread.Sleep(1000 / 25);
                Application.DoEvents();
            }
        }

        public void Undo()
        {
            if (_objects.Count() > 0)
            {
                _objectsUndoList.Add(_objects.Last());
                _objects.Remove(_objects.Last());
            }
        }

        public void Redo()
        {
            if (_objectsUndoList.Count() > 0)
            {
                _objects.Add(_objectsUndoList.Last());
                _objectsUndoList.Remove(_objectsUndoList.Last());
            }
        }

        private void RefreshImage(Graphics g)
        {
            g.DrawImage(Grid.GetGridBitmap(_canvaSize), 0, 0);

            foreach (var shape in _objects)
            {
                switch (shape.Type)
                {
                    case ShapeType.Line:
                        g.DrawLine(Pen, shape.GetFirstPoint(), shape.GetLastPoint());
                        break;
                    case ShapeType.Lines:
                        g.DrawLines(Pen, shape.GetPoints());
                        break;
                    case ShapeType.Rectangle:
                        g.DrawRectangle(Pen, ((Rectangle)shape).GetRectangle());
                        break;
                    case ShapeType.Ellipse:
                        g.DrawEllipse(Pen, ((Ellipse)shape).GetEllipse());
                        break;
                    case ShapeType.None:
                    default:
                        continue;
                }
            }
        }

        private void RefreshDrawing(Graphics g)
        {
            try
            {
                var shape = _template.ConvertTo(Tool);

                switch (shape.Type)
                {
                    case ShapeType.Line:
                        g.DrawLine(Pen, shape.GetFirstPoint(), shape.GetLastPoint());
                        break;
                    case ShapeType.Lines:
                        g.DrawLines(Pen, shape.GetPoints());
                        break;
                    case ShapeType.Rectangle:
                        g.DrawRectangle(Pen, ((Rectangle)shape).GetRectangle());
                        break;
                    case ShapeType.Ellipse:
                        g.DrawEllipse(Pen, ((Ellipse)shape).GetEllipse());
                        break;
                }

            }
            catch (Exception) { }

        }
        
        private void _pictureBox_SizeChanged(object sender, System.EventArgs e)
        {
            _canvaSize = ((PictureBox)sender).Size;
        }

        private void _pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDrawing)
                return;
            _isDrawing = false;

            _template.Add(Grid.GrabToGrid(e.Location));

            if (Tool == ShapeType.None)
                return;
            var shape = _template.ConvertTo(Tool);
            if (shape != null)
                _objects.Add(shape);
        }

        private void _pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            _isDrawing = true;
            _template = new Template();
            _template.Add(Grid.GrabToGrid(e.Location));
            _objectsUndoList.Clear();
        }

        private void _pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing)
                return;
            _template.Add(Grid.GrabToGrid(e.Location));
        }
    }
}
