using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_v3.Print
{
    public enum Protocol
    {
        Start = 1,
        End = 0,
        Home = 2,
        MoveTo = 3,
        Line = 4,
        Lines = 5,
        Rectangle = 6,
        Ellipse = 7,

        CheckConnection = 8
    }
}
