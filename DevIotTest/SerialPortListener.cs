using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace DevIotTest
{
    class SerialPortListener
    {
        SerialPort m_serialPort = null;
        StreamWriter m_sw = null;
        bool _continue = true;

        public SerialPortListener(string portName, int baudRate)
        {
            m_serialPort = new SerialPort(portName, baudRate);
            m_sw = new StreamWriter(@"E:\DevKitTest\Test\test.txt", true);

            m_serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }

        public bool IsOpen
        {
            get
            {
                return m_serialPort == null ? false : m_serialPort.IsOpen;
            }
        }

        public bool start()
        {
            try
            {
                m_serialPort.Open();
                _continue = true;

                return true;
            }
            catch
            {
                throw new Exception("Error: Failed to start the serial port");
            }
        }

        public void Stop()
        {
            m_serialPort.Close();

            m_sw.Flush();
            m_sw.Close();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            PrintResult();
        }

        void PrintResult()
        {
            string info = m_serialPort.ReadLine();
            m_sw.WriteLine(info);

            //while (_continue)
            //{
            //    try
            //    {
            //        string info = m_serialPort.ReadLine();
            //        m_sw.WriteLine(info);
            //    }
            //    catch
            //    {

            //    }
            //}
        }
    }
}
