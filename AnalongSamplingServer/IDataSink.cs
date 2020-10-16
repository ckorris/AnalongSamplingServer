using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalongSamplingServer
{
    interface IDataSink
    {
        void IngestNewSamples(SamplePacket samplePacket);
    }
}
