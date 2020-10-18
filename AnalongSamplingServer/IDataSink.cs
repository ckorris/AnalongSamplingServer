using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalongSamplingServer
{
    public interface IDataSink
    {
        void IngestNewSamples(SamplePacket samplePacket);
    }
}
