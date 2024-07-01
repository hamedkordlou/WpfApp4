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
            //_lineSeries = new LineSeries
            //{
            //    Values = new ChartValues<double> { 0 }
            //};

            var values = new ChartValues<double>();
            

            // Example data, replace with your actual data
            //labels.Add("Label 1");
            //labels.Add("Label 2");
            //labels.Add("Label 3");

            _topLosersValues = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = Brushes.Green,
                    DataLabels = true, // Show data labels
                    LabelPoint = point => $"{point.Y:F2}%" // Format for the label
                }
            };

            

            cartesianChart.Series = _topLosersValues;

            //cartesianChart.AxisX.Add(new Axis { Title = "Name" });
            cartesianChart.AxisX.Add(new Axis
            {
                Labels = labels,
                LabelsRotation = 90
            });



            
            cartesianChart.AxisY.Add(new Axis { Title = "24h Change (%)" });

            _random = new Random();
            //_dataCount = 0;
            _frameCount = 0;
        }

        private void StartAnimation()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _timer.Tick += UpdateChart;
            _timer.Start();

            //_timer2 = new DispatcherTimer
            //{
            //    Interval = TimeSpan.FromMilliseconds(1)
            //};
            //_timer2.Tick += Timer_Tick;
            //_timer2.Start();
        }

        private void UpdateChart(object sender, EventArgs e)
        {
            //_lineSeries.Values.Add(_random.NextDouble() * 10);
            //_dataCount++;

            ////SaveToPng(cartesianChart, $"frame_{_frameCount:D4}.png");

            //if (_dataCount > 100) // Adjust as needed
            //{
            //    _timer.Stop();
            //    CombineFramesIntoVideo();
            //}

            _topLosersValues.First().Values.Add(_random.NextDouble() * 10);
            labels.Add($"lbl {_random.NextDouble() * 10}");
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
