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

            plt.Title("Signal Plot Quickstart (5 million points)");
            plt.YLabel("Vertical Units");
            plt.XLabel("Horizontal Units");

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
            private List<Plottable> _last = new List<Plottable>();

            public GraphDataSink (MainWindow window)
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
                    ClearLastPlots();
                }

                PlottableSignal newPlot = plt.PlotSignal(ToDouble(packet.Samples), label: "Device " + packet.DeviceID.ToString());
                plt.Resize();
                _last.Add(newPlot);

                plt.Legend();
                _window.TheGraph.Render();
            }

            private void IngestMainThread(List<SamplePacket> packets)
            {
                var plt = _window.TheGraph.plt;

                ClearLastPlots();

                for (int i = 0; i < packets.Count; i++)
                {
                    SamplePacket packet = packets[i]; //Shorthand.
                    PlottableSignal newPlot = plt.PlotSignal(ToDouble(packet.Samples), yOffset: i * 500, label: "Device " + packet.DeviceID.ToString());
                    plt.Resize();

                    _last.Add(newPlot);
                }

                plt.Legend();
                _window.TheGraph.Render();
            }

            private double[] ToDouble (ushort[] values)
            {
                var result = new double[values.Length];
                for(int x = 0; x < values.Length; x++)
                {
                    result[x] = (double)values[x];
                }
                return result;
            }

            private void ClearLastPlots()
            {
                if (_last != null)
                {
                    Plot plt = _window.TheGraph.plt;

                    for (int i = 0; i < _last.Count; i++)
                    {
                        plt.Remove(_last[i]);
                    }
                }
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _server.TempGetsample();
        }
    }
}
