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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TrendingCoin4.xaml
    /// </summary>
    public partial class TrendingCoin4 : Window
    {
        public TrendingCoin4()
        {
            InitializeComponent();

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
                Title = "Time 24h",
                //Labels = new string[] { },
                ShowLabels = false,
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                }
            });

            cartesianChart.AxisY.Add(new Axis
            {
                Title = "Price 24h (USD)",
                LabelFormatter = value => value.ToString("N")
                //MinValue = 0
            });

            StartAnimation();
        }

        private async void StartAnimation()
        {
            await UpdateChart();
        }

        private async Task UpdateChart()
        {

            var result = TrendingService.GetTrendingCoins();

            var lineSeries = (LineSeries)cartesianChart.Series[0];

            lineSeries.Values.Clear();

            // Filter for hourly prices (every 12th data point for 5-minute intervals)
            for (int i = 0; i < result[0].Count; i += 12)
            {
                lineSeries.Values.Add(result[0][i][1]);
            }
        }
    }
}
