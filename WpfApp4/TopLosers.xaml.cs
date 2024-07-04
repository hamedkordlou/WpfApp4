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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TopLosers.xaml
    /// </summary>
    public partial class TopLosers : Window
    {
        private DispatcherTimer _timer;
        //private DispatcherTimer _timer2;
        private Random _random;
        //private int _dataCount;
        private int _frameCount;
        private string _outputFolder = @"C:\Users\hamed\Documents\Work\Personal\Crypto\output";

        //private LineSeries _lineSeries;
        public SeriesCollection _topLosersValues { get; set; }
        List<string> labels = new List<string>(); // List to store labels


        public TopLosers()
        {
            InitializeComponent();
            InitializeChart();
            StartAnimation();
        }

        private void InitializeChart()
        {
            DataContext = this;
            _random = new Random();
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

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }


        private async void StartAnimation()
        {
            await UpdateChart();
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Capture screenshot of the WPF window
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)Width, (int)Height, 108, 108, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);

            // Encode bitmap to PNG format
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Save PNG file
            string fileName = $"frame_{_frameCount:D4}.png";
            string filePath = Path.Combine(_outputFolder, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                pngEncoder.Save(fileStream);
            }

            _frameCount++;

            // Example stopping condition (adjust as needed)
            if (_frameCount > 300)
            {
                _timer.Stop();
                CombineFramesIntoVideo();
            }
        }

        private void CombineFramesIntoVideo()
        {
            string outputPath = @"C:\Users\hamed\Documents\Work\Personal\Crypto\output";
            string ffmpegPath = "ffmpeg"; // Assumes FFmpeg is in the system PATH
            string inputPattern = Path.Combine(outputPath, "frame_%04d.png");
            string outputVideo = Path.Combine(outputPath, "animation.mp4");

            string arguments = $"-r 30 -f image2 -i {inputPattern} -vcodec libx264 -crf 18 -pix_fmt yuv420p {outputVideo}";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    // Handle error
                    string error = process.StandardOutput.ReadToEnd();
                    MessageBox.Show($"FFmpeg Error: {error}");
                }
            }
        }
    }
}
