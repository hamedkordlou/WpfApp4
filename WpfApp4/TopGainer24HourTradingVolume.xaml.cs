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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TopGainer24HourTradingVolume.xaml
    /// </summary>
    public partial class TopGainer24HourTradingVolume : Window
    {
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public TopGainer24HourTradingVolume()
        {
            InitializeComponent();
            InitializeChart();
            StartAnimation();
        }

        private void InitializeChart()
        {
            DataContext = this;
            Labels = new List<string>();
            var values = new ChartValues<double>();
            topLosersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:F2}%"
                }
            };
        }

        private async void StartAnimation()
        {
            await UpdateChart();
        }

        private async Task UpdateChart()
        {

            var topLosers = await TopGainersService.GetTopGainersAsync();
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
    }
}
