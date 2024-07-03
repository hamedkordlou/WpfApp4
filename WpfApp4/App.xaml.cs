using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
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

            TrendingCoin1 trendingCoin1 = new TrendingCoin1();
            trendingCoin1.Show();
        }
    }
}
