using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AnalongSamplingServer;
using SpinAnalysis;
using SpinAnalysis.DataStructs;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server _server;
        public MainWindow()
        {
            var parsedArguments = new ParsedArguments(new string[] { });
            var server = new Server(parsedArguments);
            _server = server;

            server.TempAddSink(new GraphDataSink(this));

            server.Run();

            InitializeComponent();

            var plt = TheGraph.plt;

            Random rand = new Random(0);
            int pointCount = (int)1e6;
            int lineCount = 5;

            plt.Title("IR Light Received Over Time");
            plt.YLabel("Power");
            plt.XLabel("Time (μs)");

            plt.Resize();
            TheGraph.Render();

        }

        private void WpfPlot_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public class GraphDataSink : IDataSink
        {
            private MainWindow _window;
            //private Plottable _last;
            //private List<Plottable> _last = new List<Plottable>();

            private Dictionary<int, Plottable> devicePlots = new Dictionary<int, Plottable>();

            /*private Dictionary<int, SampleCache> deviceSamples = new Dictionary<int, SampleCache>();

            private struct SampleCache
            {
                public List<ushort> sampleValues;
                public List<double> timeIndexesMS;
            }
            */
            private RawDeviceRegistrar _deviceRegistrar = new RawDeviceRegistrar();
            public GraphDataSink(MainWindow window)
            {
                _window = window;
            }


            public void IngestNewSamples(SamplePacket samplePacket)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    IngestMainThread(samplePacket);
                }));


            }

            private void IngestMainThread(SamplePacket packet)
            {
                var plt = _window.TheGraph.plt;

                if (packet.SampleID == 0) //Could be done better, like a flag.
                {
                    ClearAllDataAndPlots();
                }

                //TODO: This updates all previous samples with the same overall sample rate. They should be equally accurate in theory, but errors will present incorrectly. Fix.
                double sampleDuration = packet.EndTimeUs - packet.StartTimeUs;
                double microSecondsPerSample = sampleDuration / packet.SampleCount;
                double milliSecondsPerSample = microSecondsPerSample / 1000d;
                double sampleRateMS = 1000d / microSecondsPerSample;
                double startOffset = (packet.StartTimeUs == 0) ? 0 : packet.StartTimeUs;

                //Make list of raw samples formatted for SpinAnalysis library.
                List<RawSample> newRawSamples = new List<RawSample>();
                for (int i = 0; i < packet.SampleCount; i++)
                {
                    RawSample newSample = new RawSample()
                    {
                        TimeStampUs = startOffset + i * microSecondsPerSample,
                        Value = packet.Samples[i]
                    };
                    newRawSamples.Add(newSample);
                }

                _deviceRegistrar.AddRawSamples(packet.DeviceID, newRawSamples);

                //Clear the previous plot so that we can extend it with the old and new data combined. 
                if (devicePlots.ContainsKey(packet.DeviceID))
                {
                    _window.TheGraph.plt.Remove(devicePlots[packet.DeviceID]);
                }

                //Make arrays for the time stamps and values.
                double[] timeStamps;
                double[] values;

                SampleSetUtilities.SplitSampleDataIntoArrays(_deviceRegistrar.DevicesByIndex[packet.DeviceID].RawSamples, out timeStamps, out values);

                //Plot all the data up to this point for the given device ID.
                System.Drawing.Color plotColor = ColorUtilities.GetColorByIndex(packet.DeviceID);
                //PlottableSignalXY newPlot = plt.PlotSignalXY(deviceSamples[packet.DeviceID].timeIndexesMS.ToArray(), ToDouble(deviceSamples[packet.DeviceID].sampleValues.ToArray()),
                PlottableSignalXY newPlot = plt.PlotSignalXY(timeStamps, values, label: "Device " + packet.DeviceID.ToString(), color: plotColor);
                plt.Resize();
                devicePlots[packet.DeviceID] = newPlot;

                plt.Legend(fontSize: 8);
                _window.TheGraph.Render();
            }

            private double[] ToDouble(ushort[] values)
            {
                var result = new double[values.Length];
                for (int x = 0; x < values.Length; x++)
                {
                    result[x] = (double)values[x];
                }
                return result;
            }

            private void ClearAllDataAndPlots()
            {
                foreach (Plottable plottable in devicePlots.Values)
                {
                    _window.TheGraph.plt.Remove(plottable);
                }

                devicePlots.Clear();

                _deviceRegistrar.Clear();
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _server.TempGetsample();
        }
    }
}
