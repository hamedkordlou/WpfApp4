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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for ChartWindow2.xaml
    /// </summary>
    public partial class ChartWindow2 : Window
    {
        public ChartWindow2()
        {
            InitializeComponent();

            cartesianChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                },
                new ColumnSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 11, 56, 42 }
                }
            };

            cartesianChart.AxisX.Add(new Axis
            {
                Title = "X Axis",
                Labels = new[] { "Apples", "Bananas", "Cherries", "Dates" }
            });

            cartesianChart.AxisY.Add(new Axis
            {
                Title = "Y Axis",
                LabelFormatter = value => value.ToString("N")
            });
        }
    }
}
