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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for TopGainer24HourTradingVolume.xaml
    /// </summary>
    public partial class TopGainer24HourTradingVolume : Window
    {
        public TopGainer24HourTradingVolume()
        {
            InitializeComponent();
            DataContext = this;
            LoadVolumeChartDataAsync();
        }

        public List<string> VolumeLabels { get; set; }
        public Func<double, string> VolumeFormatter { get; set; }

        private async Task LoadVolumeChartDataAsync()
        {
            var service = new TopGainersService();
            var topGainers = await service.GetTopGainersAsync();

            var volumeValues = new ChartValues<double>();
            VolumeLabels = new List<string>();

            foreach (var gainer in topGainers)
            {
                volumeValues.Add(gainer.USD24hVol);
                VolumeLabels.Add(gainer.Name);
            }

            VolumeFormatter = value => value.ToString("N");

            tradingVolumeChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Trading Volume",
                    Values = volumeValues
                }
            };

            DataContext = this;
        }
    }
}
