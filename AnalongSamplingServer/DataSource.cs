using System;
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
        protected bool IsRunning { get; private set; }

        protected readonly Server Server;

        protected DataSource(Server server)
        {
            IsRunning = true;
            _processingThread = new Thread(ProcessSamplesThread);
            _processingThread.SetApartmentState(ApartmentState.STA); //Set the thread to STA

            _recevingThread = new Thread(ReceiverThread);
            Server = server;

            _processingThread.Start();
            _recevingThread.Start();
        }

        protected abstract void ReceiveLoop();

        private void ReceiverThread()
        {
            while (IsRunning)
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

        [STAThread]
        private void ProcessSamplesThread()
        {
            while (IsRunning)
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

        public virtual void Stop()
        {
            IsRunning = false;
            _processingThread.Join();
            _recevingThread.Join();
        }
    }
}
