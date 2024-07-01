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

            //var chartWindow1 = new ChartWindow1();
            //chartWindow1.Show();

            //var chartWindow2 = new ChartWindow2();
            //chartWindow2.Show();

            var topGainersWindow = new TopGainers();
            topGainersWindow.Show();

            //var topGainer24HourTradingVolumeWindow = new TopGainer24HourTradingVolume();
            //topGainer24HourTradingVolumeWindow.Show();

            //TopLosersWindow topLosersWindow = new TopLosersWindow();
            //topLosersWindow.Show();

            TopLosers topLosers = new TopLosers();
            topLosers.Show();
        }
    }
}
