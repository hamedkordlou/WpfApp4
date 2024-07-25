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
    /// Interaction logic for TrendingCoin5.xaml
    /// </summary>
    public partial class TrendingCoin5 : Window
    {

        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;
        private FileService fileService;
        private bool capturing;

        public List<string> XLabels { get; set; } // Add this property for X-axis labels

        public TrendingCoin5()
        {
            InitializeComponent();

            frames = new List<BitmapSource>();
            stopwatch = new Stopwatch();
            InitializeFrameCaptureTimer();
            videoService = new VideoService(this.Title);
            fileService = new FileService(this.Title);
            capturing = true;

            XLabels = new List<string>(); // Initialize the XLabels list

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
                //Title = "Time 24h",
                //Labels = new string[] { },
                //ShowLabels = false,
                Labels = XLabels,
                Separator = new Separator
                {
                    Step = 12,
                    IsEnabled = true
                },
                FontSize = 20, // Increase the font size of the X-axis labels
            });

            cartesianChart.AxisY.Add(new Axis
            {
                //Title = "Price 24h (USD)",
                LabelFormatter = value => value.ToString("N6"),
                FontSize = 20, // Increase the font size of the Y-axis labels
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
            var frameCaptureTask = Task.Run(() =>
            {
                while (capturing)
                {
                    Application.Current.Dispatcher.Invoke(() => CaptureFrame());
                    System.Threading.Thread.Sleep(10); // Ensures the frame capture interval
                }
            });

            stopwatch.Start();
            await UpdateChart();
            await Task.Delay(3000);
            capturing = false;
            await frameCaptureTask;
            stopwatch.Stop();
            SaveVideo();
        }

        private async Task UpdateChart()
        {

            var result = TrendingService.GetTrendingCoinsPrice();
            var coin = TrendingService.GetTrendingCoin(4);

            await fileService.SaveCoinDataAsync(coin?.item);

            var lineSeries = (LineSeries)cartesianChart.Series[0];

            lineSeries.Values.Clear();
            XLabels.Clear(); // Clear the XLabels list

            await Task.Delay(2000);

            try
            {
                // Filter for hourly prices (every 12th data point for 5-minute intervals)
                for (int i = 0; i < result[0].Count; i += 1)
                {
                    lineSeries.Values.Add(result[4][i][1]);

                    long timestampMillis = (long)result[4][i][0];

                    // Ensure the timestamp is within valid range
                    if (timestampMillis < DateTimeOffset.MinValue.ToUnixTimeMilliseconds() ||
                        timestampMillis > DateTimeOffset.MaxValue.ToUnixTimeMilliseconds())
                    {
                        throw new ArgumentOutOfRangeException("Timestamp is out of valid DateTime range.");
                    }

                    DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestampMillis).DateTime;
                    timestamp = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, 0, 0, DateTimeKind.Utc);
                    XLabels.Add(timestamp.ToString("HH:mm"));

                    await Task.Delay(50);
                }
            }
            catch (Exception)
            {

            }
        }

        private void SaveVideo()
        {
            videoService.SaveFrames(frames, 27);
            videoService.CreateVideo(this.Title);
            MessageBox.Show("Video saved");
            //OpenContainingFolder(_outputFolder);
        }
    }
}
