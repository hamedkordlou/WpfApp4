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
    /// Interaction logic for TopLosersWindow.xaml
    /// </summary>
    public partial class TopLosersWindow : Window
    {
        public SeriesCollection TopLosersValues { get; set; }

        public TopLosersWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadChartDataAsync();
        }

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private async Task LoadChartDataAsync()
        {
            var service = new TopGainersService();
            var topLosers = await service.GetTopLosers();

            var values = new ChartValues<double>();
            Labels = new List<string>();

            foreach (var loser in topLosers)
            {
                values.Add(loser.usd_24h_change);
                Labels.Add(loser.name);
            }

            Formatter = value => value.ToString("N");

            topGainersChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "24h Change",
                    Values = values,
                    Fill = Brushes.Red
                }
            };

            DataContext = this;
        }
    }

}
