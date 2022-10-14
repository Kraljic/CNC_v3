using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNC_v3.Print;

namespace CNC_v3.Forms
{
    public partial class Print : Form
    {
        private List<Drawing.Shape.IShape> _objects;
        private bool _abortPrinting;
        public Print(List<Drawing.Shape.IShape> objects)
        {
            InitializeComponent();
            _objects = objects;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            _abortPrinting = false;
            var lines = PrintDrawing.GetPrintString(_objects).Trim().Split('\n');

            listPrintData.Items.Add("Start printing.");
            Communication.StartPrinting();

            var printingStarted = DateTime.Now.Ticks;

            foreach (var line in lines)
            {
                if (_abortPrinting)
                {
                    listPrintData.Items.Add("Abort request!");
                    _abortPrinting = false;
                    Communication.EndPrinting();
                    listPrintData.Items.Add("End printing.");
                    return;
                }

                if (line == "")
                    continue;
                listPrintData.Items.Add("Sending line: " + line);
                Application.DoEvents();
                if (!Communication.SendCommand(line.Trim()))
                {
                    listPrintData.Items.Add("Error!");
                    listPrintData.Items.Add("Aborting!");
                    MessageBox.Show("Dogodila se greška.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                Application.DoEvents();
            }

            listPrintData.Items.Add("End printing.");
            Communication.EndPrinting();


            var printingEnded = DateTime.Now.Ticks;

            MessageBox.Show("Ispis završen! \nVrijeme potrebno za ispis: " + ((printingEnded - printingStarted) / 10000000) + " sec.", 
                "Info", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _abortPrinting = true;
        }
    }
}
