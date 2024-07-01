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
        private int _counter;

        public TopGainers()
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
            topGainersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = Brushes.Green,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("N")
                }
            };
        }

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }


        private async void StartAnimation()
        {
            _counter = 0;
            await UpdateChart();
        }

        private async Task UpdateChart()
        {
            while (_counter < 10)
            {
                var values = topGainersChart.Series.First().Values;
                values.Add(_random.NextDouble() * 10);
                Labels.Add($"Label {_counter + 1}");
                _counter++;

                // Refresh DataContext to update the chart
                UpdateLabels();

                await Task.Delay(2000); // Wait for 1 second
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
    }
}
