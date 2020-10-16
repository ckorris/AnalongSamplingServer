using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalongSamplingServer
{
    class Server
    {
        private ParsedArguments _arguments;

        private SerialDataSource _tempSerial;

        public Server(ParsedArguments args)
        {
            _arguments = args;
            //Parse into listeners
            _tempSerial = new SerialDataSource(this, "COM3");
            //Start each listener
        }

        public void Run()
        {
            while (true)
            {
                Thread.Sleep(5);
            }
        }
    }
}
