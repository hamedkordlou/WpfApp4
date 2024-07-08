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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for barChart.xaml
    /// </summary>
    public partial class barChart : Window
    {

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public barChart()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 10, 50, 39, 50 },
                    DataLabels = true
                },
                new RowSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 20, 30, 25, 45 },
                    DataLabels = true
                }
            };

            Labels = new List<string> { "A", "B", "C", "D" };

            cartesianChart.Series = SeriesCollection;
            cartesianChart.DataContext = this;
        }
    }
}
