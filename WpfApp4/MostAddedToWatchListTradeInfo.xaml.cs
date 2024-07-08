using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MostAddedToWatchListTradeInfo.xaml
    /// </summary>
    public partial class MostAddedToWatchListTradeInfo : Window
    {

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public MostAddedToWatchListTradeInfo()
        {
            InitializeComponent();
            InitializeChartAsync();
        }

        private async Task InitializeChartAsync()
        {
            Labels = new List<string>();

            // Initialize SeriesCollection with empty ChartValues and enable DataLabels
            SeriesCollection = new SeriesCollection
            {
                //new RowSeries
                //{
                //    Title = "Number of Added to Watchlist",
                //    Values = new ChartValues<double>(),
                //    DataLabels = true,
                //    LabelPoint = point => point.X.ToString("N0", new CultureInfo("en-US"))
                //}
                new RowSeries
                {
                    Title = "Total Volume (USD)",
                    Values = new ChartValues<decimal>(),
                    DataLabels = true
                }
            };

            // Retrieve data from your service (assuming GetMostAddedToWatchlistCoins returns a collection of coins)
            var res = await LocalDataBaseService.GetMostAddedToWatchlistCoins(10);

            // Iterate through each coin and add values to the corresponding RowSeries
            foreach (var coin in res)
            {
                // Add value to "Number of Added to Watchlist" series
                //SeriesCollection[0].Values.Add(coin.WatchlistUsers);

                // Add value to "Total Volume (USD)" series
                SeriesCollection[0].Values.Add(coin.TotalVolumeUsd);

                Labels.Add(coin.Name);
            }

            //// Example: Assigning labels based on the number of coins retrieved
            //Labels = res.Select((coin, index) => $"Coin {index + 1}").ToList();

            // Set the SeriesCollection and Labels to the chart
            cartesianChart.Series = SeriesCollection;
            cartesianChart.DataContext = this;
        }
    }
}
