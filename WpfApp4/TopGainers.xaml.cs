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
using WpfApp4.Data;
using System.Windows.Threading;
using System.Reflection.Emit;
using System.Diagnostics;
using System.IO;
using WpfApp4.Tools;

namespace WpfApp4
{
    public partial class TopGainers : Window
    {
        private List<BitmapSource> frames;
        private Stopwatch stopwatch;
        private DispatcherTimer frameCaptureTimer;
        private VideoService videoService;
        private bool capturing;

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public TopGainers()
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
            topGainersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2CBF12")),
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:F2}%",
                    Foreground = Brushes.White,
                    FontSize = 20, // Increase the font size of the data labels
                    MaxColumnWidth = 70 // Increase the width of the columns
                }
            };

            topGainersChart.AxisX.First().FontSize = 20; // Increase the font size of the X-axis labels
            topGainersChart.AxisY.First().FontSize = 20;
        }

        private void CaptureFrame()
        {
            //var renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 108, 108, PixelFormats.Pbgra32);
            var renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);
            frames.Add(renderTargetBitmap);
        }

        //private async void StartAnimation()
        //{
        //    frameCaptureTimer.Start();
        //    stopwatch.Start();
        //    await UpdateChart();
        //    await Task.Delay(2000);
        //    frameCaptureTimer.Stop();
        //    stopwatch.Stop();
        //    SaveVideo();
        //}

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
            var topGainers = await TopGainersService.GetTopGainersAsync();
            topGainers = topGainers.OrderBy(x => x.usd_24h_change).ToList();
            var values = topGainersChart.Series.First().Values;
            await Task.Delay(2000);
            foreach (var topGainer in topGainers)
            {
                values.Add(topGainer.usd_24h_change);
                Labels.Add(topGainer.name);
                UpdateLabels();
                await Task.Delay(2000); // Update every 2 seconds
            }
        }

        private void UpdateLabels()
        {
            // Update the axis labels
            Axis axis = topGainersChart.AxisX.FirstOrDefault();
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
