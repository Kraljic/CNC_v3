using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace CNC_v3.Print
{
    public class Communication
    {
        public static SerialPort SerialConnection;

        public static String[] GetSerialPorts()
        {
            return SerialPort.GetPortNames();
        }


        public static bool OpenLink(string comPort)
        {
            if (SerialConnection == null || !SerialConnection.IsOpen)
            {
                SerialConnection = new SerialPort(comPort, Config.BaudRate);
                SerialConnection.Open();
                return SerialConnection.IsOpen;
            }
            return false;
        }
        public static void CloseLink()
        {
            if (SerialConnection != null)
                SerialConnection.Close();
            SerialConnection.Dispose();
            SerialConnection = null;
        }
        public static bool IsOpen()
        {
            if (SerialConnection != null)
                return SerialConnection.IsOpen;
            return false;
        }

        public static string ReadResponse(float timeOut = 10.0f)
        {
            if (SerialConnection != null && SerialConnection.IsOpen)
            {
                for (var i = 0; i < timeOut * 100.0; i++)
                {
                    Thread.Sleep(10);
                    if (SerialConnection.BytesToRead <= 0)
                        continue;
                    var str = SerialConnection.ReadLine().Trim();
                    return str;
                }
                throw new TimeoutException("Arduino didn't respond in given time.");
            }
            return null;
        }

        public static bool CheckConnection()
        {
            if (SerialConnection != null && SerialConnection.IsOpen)
            {
                SerialConnection.Write(Config.Prefix + (int)Protocol.CheckConnection + ";\n");

                var response = ReadResponse();
                if (response != null && response == "ready")
                    return true;
            }
            return false;
        }

        public static bool MoveToHome()
        {
            if (SerialConnection != null && SerialConnection.IsOpen)
            {
                SerialConnection.Write(Config.Prefix + (int)Protocol.Home + ";\n");

                var response = ReadResponse(30);
                if (response != null && response == "done")
                    return true;
            }
            return false;
        }

        public static bool StartPrinting()
        {
            if (SerialConnection != null && SerialConnection.IsOpen)
            {
                SerialConnection.DiscardInBuffer();
                SerialConnection.DiscardOutBuffer();
                SerialConnection.Write(Config.Prefix + (int)Protocol.Start + ";\n");

                var response = ReadResponse(50);
                if (response != null && response == "done")
                    return true;
            }
            return false;
        }

        public static bool EndPrinting()
        {
            if (SerialConnection != null && SerialConnection.IsOpen)
            {
                SerialConnection.Write(Config.Prefix + (int)Protocol.End + ";\n");

                var response = ReadResponse(10);
                if (response != null && response == "done")
                    return true;
            }
            return false;
        }

        public static bool SendCommand(string command)
        {
            if (SerialConnection != null && SerialConnection.IsOpen)
            {
                SerialConnection.Write(command + "\n");

                var response = ReadResponse(1000);
                if (response != null && response == "done")
                    return true;
            }
            return false;
        }
    }
}
