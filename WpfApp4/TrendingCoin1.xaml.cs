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
    /// Interaction logic for TrendingCoin1.xaml
    /// </summary>
    public partial class TrendingCoin1 : Window
    {
        public TrendingCoin1()
        {
            InitializeComponent();

            StartAnimation();

            

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
                Title = "Coin Name",
                Labels = new string[] { },
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = true
                }
            });

            cartesianChart.AxisY.Add(new Axis
            {
                Title = "Price Change Percentage 24h (%)",
                LabelFormatter = value => value.ToString("N")
            });
        }

        private async void StartAnimation()
        {
            await UpdateChart();
        }

        private async Task UpdateChart()
        {

            var result = await new TrendingService().GetTrendingCoinsAsync();

            var lineSeries = (LineSeries)cartesianChart.Series[0];

            lineSeries.Values.Clear();
            var coinNames = new List<string>();

            foreach (var market in result)
            {
                lineSeries.Values.Add(market.price_change_percentage_24h);
                coinNames.Add(market.name);
            }

            cartesianChart.AxisX[0].Labels = coinNames.ToArray();
        }
    }
}
