using LiveCharts;
using LiveCharts.Wpf;
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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MostVolatileCoins.xaml
    /// </summary>
    public partial class MostVolatileCoins : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public MostVolatileCoins()
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
                new RowSeries
                {
                    Title = "Price Change in 24h (%)",
                    Values = new ChartValues<double>(),
                    DataLabels = true
                }
            };

            // Retrieve data from your service (assuming GetMostAddedToWatchlistCoins returns a collection of coins)
            var res = await LocalDataBaseService.GetMostVolatileCoins(10);

            // Iterate through each coin and add values to the corresponding RowSeries
            foreach (var coin in res)
            {
                // Add value to "Number of Added to Watchlist" series
                //SeriesCollection[0].Values.Add(coin.WatchlistUsers);

                // Add value to "Total Volume (USD)" series
                SeriesCollection[0].Values.Add(coin.PriceChangePercentage24h);

                Labels.Add(coin.Name);
            }


            // Set the SeriesCollection and Labels to the chart
            cartesianChart.Series = SeriesCollection;
            cartesianChart.DataContext = this;
        }
    }
}
