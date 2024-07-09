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
using System.Windows.Shapes;
using WpfApp4.Data;
using System.Reflection.Emit;
using Separator = LiveCharts.Wpf.Separator;
using System.Diagnostics;
using System.Windows.Threading;
using WpfApp4.Tools;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TrendingCoin4.xaml
    /// </summary>
    public partial class TrendingCoin4 : Window
    {

        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;

        public TrendingCoin4()
        {
            InitializeComponent();

            frames = new List<BitmapSource>();
            stopwatch = new Stopwatch();
            InitializeFrameCaptureTimer();
            videoService = new VideoService(this.Title);

            cartesianChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Trending",
                    Values = new ChartValues<double> { }
                }
            };

            cartesianChart.AxisX.Add(new Axis
            {
                Title = "Time 24h",
                //Labels = new string[] { },
                ShowLabels = false,
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                }
            });

            cartesianChart.AxisY.Add(new Axis
            {
                Title = "Price 24h (USD)",
                LabelFormatter = value => value.ToString("N")
                //MinValue = 0
            });

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

            var result = TrendingService.GetTrendingCoins();

            var lineSeries = (LineSeries)cartesianChart.Series[0];

            lineSeries.Values.Clear();

            // Filter for hourly prices (every 12th data point for 5-minute intervals)
            for (int i = 0; i < result[0].Count; i += 12)
            {
                lineSeries.Values.Add(result[0][i][1]);
                await Task.Delay(1000);
            }
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
