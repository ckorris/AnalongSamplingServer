using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace AnalongSamplingServer
{
    class SerialDataSource : DataSource
    {


        private int discard = 0;

        public SerialDataSource(Server server, string comPort) : base(server)
        {
            var serialPort = new SerialPort(
                comPort,
                56700,
                Parity.None,
                8
            );

            

            serialPort.DataReceived += (sender, serialDataReceivedEventArgs) =>
            {
                var bytesToRead = serialPort.BytesToRead;
                var byteBuffer = new Byte[bytesToRead];
                var data = serialPort.Read(byteBuffer, 0, bytesToRead);

                //Console.WriteLine($"Read {data} bytes of data from micro");

                var asString = Encoding.UTF8.GetString(byteBuffer);

                    Console.Write(asString);

               
            };

            serialPort.Open();
        }

        protected override void ReceiveLoop()
        {
            Thread.Sleep(1);
        }
    }
}
