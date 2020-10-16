using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace AnalongSamplingServer
{
    abstract class DataSource
    {

        protected readonly List<IDataSink> DataSinks = new List<IDataSink>();
        private readonly ConcurrentQueue<SamplePacket> _pendingSamplePackets = new ConcurrentQueue<SamplePacket>();

        private readonly Thread _recevingThread;
        private readonly Thread _processingThread;
        private bool _isRunning;

        protected readonly Server Server;

        protected DataSource(Server server)
        {
            _isRunning = true;
            _processingThread = new Thread(ProcessSamplesThread);
            _recevingThread = new Thread(ReceiverThread);
            Server = server;
        }

        protected abstract void ReceiveLoop();

        private void ReceiverThread()
        {
            while (_isRunning)
            {
                ReceiveLoop();
            }
        }

        public void AddSink(IDataSink dataSink)
        {
            if (!DataSinks.Contains(dataSink))
            {
                DataSinks.Add(dataSink);
            }
        }

        public void RemoveSink(IDataSink dataSink)
        {
            DataSinks.Remove(dataSink);
        }

        protected void OnDataReady(SamplePacket newPacket)
        {
            _pendingSamplePackets.Enqueue(newPacket);
        }

        private void ProcessSamplesThread()
        {
            while (_isRunning)
            {
                SamplePacket samplePacket;
                while (_pendingSamplePackets.TryDequeue(out samplePacket))
                {
                    foreach (var dataSink in DataSinks)
                    {
                        dataSink.IngestNewSamples(samplePacket);
                    }
                }
                Thread.Sleep(5);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _processingThread.Join();
            _recevingThread.Join();
        }
    }
}
