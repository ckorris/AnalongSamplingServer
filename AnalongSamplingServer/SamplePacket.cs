
using System;
using System.IO;
using System.Text;

namespace AnalongSamplingServer
{
    public class SamplePacket
    {
        public int DeviceID;
        public int SampleID;

        public int SamplingDurationUs;
        public int AnalongInPins;//Number of AnalogIn pins being read

        public int SampleCount;
        public ushort[] Samples;

        public static SamplePacket ReadFromStream(BinaryReader reader)
        {
            var packet = new SamplePacket();
            try { 
            packet.DeviceID = reader.ReadInt32();
            packet.SampleID = reader.ReadInt32();

            packet.SamplingDurationUs = reader.ReadInt32();
            packet.AnalongInPins = reader.ReadInt32();

            packet.SampleCount = reader.ReadInt32();
            packet.Samples = new ushort[packet.SampleCount];

            for(int i = 0; i < packet.SampleCount; i++)
            {
                packet.Samples[i] = reader.ReadUInt16();
            }
            }
            catch(Exception e)
            {
                Console.Write(e);
            }
            return packet;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"Device ID: {DeviceID}\n");
            builder.Append($"Sample ID: {SampleID}\n");
            builder.Append($"Sampling Duration uS: {SamplingDurationUs}\n");
            builder.Append($"Analong in pins: {AnalongInPins}\n\n");
            builder.Append($"Samples: {SampleCount}\n");
            if (Samples.Length > 100)
            {
           //     builder.Append($"Samples: {SampleCount} (Truncating to 100)\n");
            }
            else
            {
            //    builder.Append($"Samples: {SampleCount}\n");
            }

            var count = Math.Min(100, Samples.Length);
            for(int x = 0; x < count; x++)
            {
               // builder.AppendLine($"{Samples[x]}");
            }

            return builder.ToString();
        }
    }
}
