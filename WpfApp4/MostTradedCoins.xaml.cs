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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MostTradedCoins.xaml
    /// </summary>
    public partial class MostTradedCoins : Window
    {
        public MostTradedCoins()
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
            mostTradedCoinsChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Volume",
                    Values = values,
                    //Fill = Brushes.Green,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("C", new CultureInfo("en-US"))
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

            var mostTraded = await LocalDataBaseService.GetMostTradedCoins();
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
    }
}
