using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalongSamplingServer
{
    public class ClipboardDataSink : IDataSink
    {
        public void IngestNewSamples(SamplePacket samplePacket)
        {
            var builder = new StringBuilder();
            for(int i = 0; i < samplePacket.Samples.Length; i++)
            {
                builder.Append($"{samplePacket.Samples[i]}\n");
            }

            Clipboard.SetText(builder.ToString());
        }
    }
}
