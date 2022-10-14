using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_v3.Print
{
    public class Config
    {
        public const string Prefix = "!";
        public const int BaudRate = 9600;
        public static float Scale = 4f;

        public static readonly Size A4Size = new Size(
            (int)(297 * Scale), 
            (int)(210 * Scale));

        public static Size PaperSize = new Size((int)(A4Size.Width - 20 * Scale), (int)(A4Size.Height - 35 * Scale));
    }
}
