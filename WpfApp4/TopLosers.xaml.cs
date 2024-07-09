using LiveCharts.Wpf;
using LiveCharts;
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
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using WpfApp4.Data;
using WpfApp4.Tools;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TopLosers.xaml
    /// </summary>
    public partial class TopLosers : Window
    {
        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;

        //private LineSeries _lineSeries;
        public SeriesCollection _topLosersValues { get; set; }
        List<string> labels = new List<string>(); // List to store labels
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }


        public TopLosers()
        {
            InitializeComponent();
            InitializeChart();
            frames = new List<BitmapSource>();
            stopwatch = new Stopwatch();
            InitializeFrameCaptureTimer();
            videoService = new VideoService(this.Title);
            StartAnimation();
        }

        private void InitializeFrameCaptureTimer()
        {
            frameCaptureTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            frameCaptureTimer.Tick += (sender, args) => CaptureFrame();
        }

        private void InitializeChart()
        {
            DataContext = this;
            Labels = new List<string>();
            var values = new ChartValues<double>();
            topLosersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = Brushes.Red,
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:F2}%"
                }
            };
        }




        private void CaptureFrame()
        {
            var renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);
            frames.Add(renderTargetBitmap);
        }

        private async void StartAnimation()
        {
            frameCaptureTimer.Start();
            stopwatch.Start();
            await UpdateChart();
            frameCaptureTimer.Stop();
            stopwatch.Stop();
            SaveVideo();
        }

        private async Task UpdateChart()
        {

            var topLosers = await TopGainersService.GetTopLosers();
            var values = topLosersChart.Series.First().Values;
            foreach (var topLoser in topLosers)
            {
                values.Add(topLoser.usd_24h_change);
                Labels.Add(topLoser.name);
                UpdateLabels();

                await Task.Delay(2000);
            }
        }

        private void UpdateLabels()
        {
            // Update the axis labels
            Axis axis = topLosersChart.AxisX.FirstOrDefault();
            if (axis != null)
            {
                axis.Labels = Labels;
            }

            // Refresh DataContext to update the chart
            DataContext = null;
            DataContext = this;
        }

        private void SaveVideo()
        {
            videoService.SaveFrames(frames);
            videoService.CreateVideo(this.Title);
            MessageBox.Show("Video saved");
            //OpenContainingFolder(_outputFolder);
        }
        
    }
}
