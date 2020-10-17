using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalongSamplingServer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var parsedArguments = new ParsedArguments(args);
            var server = new Server(parsedArguments);
            server.Run();
        }
    }
}
