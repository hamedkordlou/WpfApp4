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
    /// Interaction logic for ChartWindow1.xaml
    /// </summary>
    public partial class ChartWindow1 : Window
    {
        public ChartWindow1()
        {
            InitializeComponent();

            cartesianChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2, 7 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4, 6 }
                }
            };

            cartesianChart.AxisX.Add(new Axis
            {
                Title = "X Axis",
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
            });

            cartesianChart.AxisY.Add(new Axis
            {
                Title = "Y Axis",
                LabelFormatter = value => value.ToString("N")
            });
        }
    }
}
