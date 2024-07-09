using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using WpfApp4.Data;
using WpfApp4.Tools;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MostVolatileCoins.xaml
    /// </summary>
    public partial class MostVolatileCoins : Window
    {
        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public MostVolatileCoins()
        {
            InitializeComponent();

            frames = new List<BitmapSource>();
            stopwatch = new Stopwatch();
            InitializeFrameCaptureTimer();
            videoService = new VideoService(this.Title);

            //InitializeChartAsync();

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
            //await UpdateChart();
            await InitializeChartAsync();
            frameCaptureTimer.Stop();
            stopwatch.Stop();
            SaveVideo();
        }

        private async Task InitializeChartAsync()
        {
            Labels = new List<string>();

            // Initialize SeriesCollection with empty ChartValues and enable DataLabels
            SeriesCollection = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "Price Change in 24h (%)",
                    Values = new ChartValues<double>(),
                    DataLabels = true
                }
            };

            // Set the SeriesCollection and Labels to the chart
            cartesianChart.Series = SeriesCollection;
            cartesianChart.DataContext = this;

            // Retrieve data from your service (assuming GetMostAddedToWatchlistCoins returns a collection of coins)
            var res = MostVolatileCoinsService.GetMostVolatileCoins();

            // Iterate through each coin and add values to the corresponding RowSeries
            foreach (var coin in res)
            {
                // Add value to "Number of Added to Watchlist" series
                //SeriesCollection[0].Values.Add(coin.WatchlistUsers);

                // Add value to "Total Volume (USD)" series
                SeriesCollection[0].Values.Add(coin.PriceChangePercentage24h);

                Labels.Add(coin.Name);
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
