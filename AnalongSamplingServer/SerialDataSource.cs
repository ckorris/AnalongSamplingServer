using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AnalongSamplingServer
{
    class SerialDataSource : DataSource
    {
        private readonly AutoResetEvent _serialReady = new AutoResetEvent(false);
        private readonly SerialPort _serialPort;

        public SerialDataSource(Server server, string comPort) : base(server)
        {
            _serialPort = new SerialPort(
                comPort,
                921600,
                Parity.None,
                8
            );

            _serialPort.Open();
            _serialReady.Set();
        }

        public void TempSend(int value)
        {
            var ca = new char[1] { (char)value };

            var bytes = new byte[1];
            bytes[0] = 1;

            _serialPort.Write(bytes, 0, 1);
        }

        protected override void ReceiveLoop()
        {
            _serialReady.WaitOne();

            var reader = new BinaryReader(_serialPort.BaseStream);

            while(_serialPort.BytesToRead > 0)
            {
                _serialPort.ReadByte();
            }


            while (IsRunning)
            {
                if (!ValidateMagicValue(reader))
                {
                    continue;
                }

                Console.WriteLine("Got magic");
                var packet = SamplePacket.ReadFromStream(reader);
                Console.WriteLine("Got Packet");
                Console.Write(packet.ToString());
                Console.WriteLine("-----");

                OnDataReady(packet);
            }
        }

        readonly byte[] expectedBytes = new byte[]
        {
                        0,0,0,0,
                        0xFF,0xFF,0xFF,0xFF,
                        0xBF,0xBF,0x7A,0x7A
        };

        private bool ValidateMagicValue(BinaryReader reader)
        {
            foreach (var value in expectedBytes)
            {
                var serialValue = reader.ReadByte();
                if (value != serialValue)
                {
                    Console.WriteLine($"Magic failed: expected {value} but got {serialValue}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"Magic OK: expected {value} and got {serialValue}");
                }
            }
            return true;
        }
    }
}
