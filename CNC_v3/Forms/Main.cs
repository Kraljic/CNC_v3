using CNC_v3.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CNC_v3.Drawing.Shape;
using CNC_v3.Print;

namespace CNC_v3.Forms
{
    public partial class Main : Form
    {
        private Thread _drawingThread;
        private Drawing.DrawingCanvas _drawingCanvas;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Maximize with borders
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            Show();
            _drawingCanvas = new DrawingCanvas(pictureBox1);
            _drawingThread = new Thread(_drawingCanvas.Run);
            _drawingThread.Start();

            RefreshCOMPorts();
        }
        
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            _drawingThread.Abort();
        }

        public void RefreshCOMPorts()
        {
            cbSerialPorts.Items.Clear();
            foreach (var port in CNC_v3.Print.Communication.GetSerialPorts())
            {
                cbSerialPorts.Items.Add(port);
            }
        }

        public void SelectTool(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            SelectTool(btn.Text.ToLower());
        }

        public void SelectTool(string tool)
        {
            btnMouse.Checked = btnLine.Checked = btnLines.Checked = btnRectangle.Checked = btnEllipse.Checked = false;
            switch (tool)
            {
                case "line":
                    _drawingCanvas.Tool = Drawing.Shape.ShapeType.Line;
                    btnLine.Checked = true;
                    break;
                case "lines":
                    _drawingCanvas.Tool = Drawing.Shape.ShapeType.Lines;
                    btnLines.Checked = true;
                    break;
                case "rectangle":
                    _drawingCanvas.Tool = Drawing.Shape.ShapeType.Rectangle;
                    btnRectangle.Checked = true;
                    break;
                case "ellipse":
                    _drawingCanvas.Tool = Drawing.Shape.ShapeType.Ellipse;
                    btnEllipse.Checked = true;
                    break;
                default:
                    _drawingCanvas.Tool = Drawing.Shape.ShapeType.None;
                    btnMouse.Checked = true;
                    break;
            }
        }
        
        private void btnUndo_Click(object sender, EventArgs e)
        {
            _drawingCanvas.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            _drawingCanvas.Redo();
        }

        private void ChangeGrid_onItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            ChangeGrid(item.Text.ToLower());
        }

        public void ChangeGrid(string size)
        {
            switch (size)
            {
                case "5px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._5Px;
                    break;
                case "10px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._10Px;
                    break;
                case "15px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._15Px;
                    break;
                case "20px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._20Px;
                    break;
                case "25px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._25Px;
                    break;
                case "30px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._30Px;
                    break;
                case "40px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._40Px;
                    break;
                case "50px":
                    _drawingCanvas.Grid.Size = Drawing.GridSize._50Px;
                    break;
                case "no grid":
                default:
                    _drawingCanvas.Grid.Size = Drawing.GridSize._0Px;
                    break;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cbSerialPorts.Text == "")
            {
                MessageBox.Show("Molim vas označite seriski port.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CNC_v3.Print.Communication.OpenLink(cbSerialPorts.Text))
            {
                if (CNC_v3.Print.Communication.CheckConnection())
                {
                    btnConnect.BackColor = System.Drawing.Color.Green;
                    return;
                }
            }
            CNC_v3.Print.Communication.CloseLink();
            btnConnect.BackColor = System.Drawing.Color.Red;
            MessageBox.Show("Veza nije uspostavljena.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        public void Print()
        {
            if (CNC_v3.Print.Communication.IsOpen())
            {
                var printForm = new Forms.Print(_drawingCanvas.Objects);
                printForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Niste povezani s uređajom.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            Home();
        }

        public void Home()
        {
            if (CNC_v3.Print.Communication.IsOpen())
            {
                try
                {
                    CNC_v3.Print.Communication.MoveToHome();
                }
                catch (TimeoutException)
                {
                    CNC_v3.Print.Communication.CloseLink();
                    btnConnect.BackColor = System.Drawing.Color.Red;
                    MessageBox.Show("Veza je prekinuta, uređaj nije odgovorio na vrijeme.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Niste povezani s uređajom.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnRefreshComPorts_Click(object sender, EventArgs e)
        {
            RefreshCOMPorts();
        }
        
        public void OpenDrawing()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Kraljic CNC v1|*.kcv1";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _drawingCanvas.Objects = PrintDrawing.LoadShapeList(openFileDialog.FileName);
                }
                catch (Exception)
                {

                }
            }
        }

        public void SaveDrawing()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Kraljic CNC v1|*.kcv1";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                PrintDrawing.SaveShapeList(saveFileDialog.FileName, _drawingCanvas.Objects);
            }
        }

        public void NewDrawing()
        {
            if (
                MessageBox.Show(
                    "Jeste li sigurni da želite stvoriti novi crtež? \nSve što nije spremljeno bit će obrisano.",
                    "Stvori novi crtež", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                _drawingCanvas.Objects = new List<IShape>();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDrawing();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDrawing();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDrawing();
        }
        
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && e.Control)
                _drawingCanvas.Undo();
            else if (e.KeyCode == Keys.Y && e.Control)
                _drawingCanvas.Redo();
            else if (e.KeyCode == Keys.P && e.Control)
                Print();
            else if (e.KeyCode == Keys.H && e.Control)
                Home();
            else if (e.KeyCode == Keys.S && e.Control)
                SaveDrawing();
            else if (e.KeyCode == Keys.O && e.Control)
                OpenDrawing();
            else if (e.KeyCode == Keys.N && e.Control)
                NewDrawing();
            else if (e.KeyCode == Keys.D0)
                SelectTool("none");
            else if (e.KeyCode == Keys.D1)
                SelectTool("line");
            else if (e.KeyCode == Keys.D2)
                SelectTool("lines");
            else if (e.KeyCode == Keys.D3)
                SelectTool("ellipse");
            else if (e.KeyCode == Keys.D4)
                SelectTool("rectangle");
            else if (e.KeyCode == Keys.NumPad0)
                ChangeGrid("nogrid");
            else if (e.KeyCode == Keys.NumPad1)
                ChangeGrid("5px");
            else if (e.KeyCode == Keys.NumPad2)
                ChangeGrid("10px");
            else if (e.KeyCode == Keys.NumPad3)
                ChangeGrid("15px");
            else if (e.KeyCode == Keys.NumPad4)
                ChangeGrid("20px");
            else if (e.KeyCode == Keys.NumPad5)
                ChangeGrid("25px");
            else if (e.KeyCode == Keys.NumPad6)
                ChangeGrid("30px");
            else if (e.KeyCode == Keys.NumPad7)
                ChangeGrid("40px");
            else if (e.KeyCode == Keys.NumPad8)
                ChangeGrid("50px");
        }
    }
}
