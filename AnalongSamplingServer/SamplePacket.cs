
namespace AnalongSamplingServer
{
    public class SamplePacket
    {
        public int SampleCount;
        public int SamplerCount;//Number of AnalogIn pins being read

        public ushort[] Samples;
    }
}
