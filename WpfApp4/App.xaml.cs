using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp4.Data;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // line chart
            //var chartWindow1 = new ChartWindow1();
            //chartWindow1.Show();


            // bar chart
            //var chartWindow2 = new ChartWindow2();
            //chartWindow2.Show();

            //var topGainer24HourTradingVolumeWindow = new TopGainer24HourTradingVolume();
            //topGainer24HourTradingVolumeWindow.Show();

            //TopLosersWindow topLosersWindow = new TopLosersWindow();
            //topLosersWindow.Show();




            //var topGainers = new TopGainers();
            //topGainers.Show();

            //TopLosers topLosers = new TopLosers();
            //topLosers.Show();

            await FetchData();

            TrendingCoin1 trendingCoin1 = new TrendingCoin1();
            trendingCoin1.Show();

            TrendingCoin2 trendingCoin2 = new TrendingCoin2();
            trendingCoin2.Show();

            TrendingCoin3 trendingCoin3 = new TrendingCoin3();
            trendingCoin3.Show();

            TrendingCoin4 trendingCoin4 = new TrendingCoin4();
            trendingCoin4.Show();

            TrendingCoin5 trendingCoin5 = new TrendingCoin5();
            trendingCoin5.Show();
        }

        private static async Task FetchData()
        {
            await TrendingService.InitializeDataAsync();
        }
    }
}
