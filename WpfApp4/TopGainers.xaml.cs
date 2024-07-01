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
using System.Windows.Threading;
using System.Reflection.Emit;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TopGainers.xaml
    /// </summary>
    public partial class TopGainers : Window
    {
        private DispatcherTimer _timer;
        private Random _random;

        public TopGainers()
        {
            InitializeComponent();
            DataContext = this;
            //LoadChartDataAsync();
            _random = new Random();
            Labels = new List<string>();
            var values = new ChartValues<double>();
            topGainersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = Brushes.Green
                }
            };

            StartAnimation();
        }

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private async Task LoadChartDataAsync()
        {
            var service = new TopGainersService();
            var topGainers = await service.GetTopGainersAsync();

            var values = new ChartValues<double>();
            Labels = new List<string>();

            foreach (var gainer in topGainers)
            {
                values.Add(gainer.USD24hChange);
                Labels.Add(gainer.Name);
            }

            Formatter = value => value.ToString("N");

            topGainersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = Brushes.Green
                }
            };

            DataContext = this;
        }

        private void StartAnimation()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _timer.Tick += UpdateChart;
            _timer.Start();

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

            //_topLosersValues.First().Values.Add(_random.NextDouble() * 10);
            //labels.Add($"lbl {_random.NextDouble() * 10}");

            var values = new ChartValues<double>();
            

            //foreach (var gainer in topGainers)
            //{
            //    values.Add(gainer.USD24hChange);
            //    Labels.Add(gainer.Name);
            //}

            Formatter = value => value.ToString("N");

            //topGainersChart.Series = new SeriesCollection
            //{
            //    new ColumnSeries
            //    {
            //        Title = "24h Change",
            //        Values = values,
            //        Fill = Brushes.Green
            //    }
            //};

            topGainersChart.Series.First().Values.Add(_random.NextDouble() * 10);
            Labels.Add($"lbl {_random.NextDouble() * 10}");

            DataContext = this;
        }
    }
}
