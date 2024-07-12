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
using System.Diagnostics;
using System.Windows.Threading;
using WpfApp4.Tools;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MostTradedCoins.xaml
    /// </summary>
    public partial class MostTradedCoins : Window
    {
        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;
        private bool capturing;

        public MostTradedCoins()
        {
            InitializeComponent();
            InitializeChart();
            frames = new List<BitmapSource>();
            stopwatch = new Stopwatch();
            InitializeFrameCaptureTimer();
            videoService = new VideoService(this.Title);
            capturing = true;
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
            mostTradedCoinsChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Volume",
                    Values = values,
                    //Fill = Brushes.Green,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("C0", new CultureInfo("en-US"))
                }
            };
        }

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

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
            //await Task.Delay(3000);
            frameCaptureTimer.Stop();
            stopwatch.Stop();
            SaveVideo();
        }

        private async Task UpdateChart()
        {

            var mostTraded = MostTradedCoinService.GetMostTradedCoins()?.OrderBy(x => x.TotalVolumeUsd);
            var values = mostTradedCoinsChart.Series.First().Values;
            foreach (var coin in mostTraded)
            {
                values.Add((double)coin.TotalVolumeUsd);
                Labels.Add(coin.Name);
                UpdateLabels();

                await Task.Delay(2000);
            }
        }

        private void UpdateLabels()
        {
            // Update the axis labels
            Axis axis = mostTradedCoinsChart.AxisX.FirstOrDefault();
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
