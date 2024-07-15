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
using System.Globalization;
using Separator = LiveCharts.Wpf.Separator;
using System.Diagnostics;
using System.Windows.Threading;
using WpfApp4.Tools;

namespace WpfApp4
{
    public partial class MostAddedToWatchList : Window
    {
        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;
        private bool capturing;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }


        public MostAddedToWatchList()
        {
            InitializeComponent();

            frames = new List<BitmapSource>();
            stopwatch = new Stopwatch();
            InitializeFrameCaptureTimer();
            videoService = new VideoService(this.Title);
            capturing = true;

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
            var frameCaptureTask = Task.Run(() =>
            {
                while (capturing)
                {
                    Application.Current.Dispatcher.Invoke(() => CaptureFrame());
                    System.Threading.Thread.Sleep(10); // Ensures the frame capture interval
                }
            });
            stopwatch.Start();
            //await UpdateChart();
            await InitializeChartAsync();
            await Task.Delay(3000);
            capturing = false;
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
                    Title = "Number of Added to Watchlist",
                    Values = new ChartValues<double>(),
                    DataLabels = true,
                    LabelPoint = point => point.X.ToString("N0", new CultureInfo("en-US")),
                    Foreground = Brushes.White,
                    FontSize = 20 // Increase the font size of the data labels
                }
                //new RowSeries
                //{
                //    Title = "Total Volume (USD)",
                //    Values = new ChartValues<decimal>(),
                //    DataLabels = true
                //}
            };

            // Set the SeriesCollection and Labels to the chart
            cartesianChart.Series = SeriesCollection;
            cartesianChart.DataContext = this;

            cartesianChart.AxisX.First().FontSize = 20; // Increase the font size of the X-axis labels
            cartesianChart.AxisY.First().FontSize = 20;

            // Retrieve data from your service (assuming GetMostAddedToWatchlistCoins returns a collection of coins)
            var res = MostAddedToWatchListService.GetMostAddedToWatchlistCoins()?.OrderBy(x => x.WatchlistUsers);

            // Iterate through each coin and add values to the corresponding RowSeries
            foreach (var coin in res)
            {
                // Add value to "Number of Added to Watchlist" series
                SeriesCollection[0].Values.Add(coin.WatchlistUsers);

                // Add value to "Total Volume (USD)" series
                //SeriesCollection[1].Values.Add(coin.TotalVolumeUsd);

                Labels.Add(coin.Name);
                await Task.Delay(2000);
            }

            //// Example: Assigning labels based on the number of coins retrieved
            //Labels = res.Select((coin, index) => $"Coin {index + 1}").ToList();

            
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
