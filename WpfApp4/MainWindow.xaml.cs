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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    if (button != null)
        //    {
        //        string buttonText = button.Content.ToString();
        //        OpenNewWindow(buttonText);
        //    }
        //}

        //private void OpenNewWindow(string title)
        //{
        //    Window window = new Window
        //    {
        //        Title = title,
        //        Width = 300,
        //        Height = 200
        //    };
        //    window.Show();
        //}

        private async void FetchData_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.Value = 0;

            await Task.Run(async () =>
            {
                await TopGainersService.InitializeDataAsync();
                UpdateProgressBar(20);
                
                await TrendingService.InitializeDataAsync();
                UpdateProgressBar(40);

                await MostTradedCoinService.InitializeDataAsync();
                UpdateProgressBar(60);

                await MostAddedToWatchListService.InitializeDataAsync();
                UpdateProgressBar(80);

                await MostVolatileCoinsService.InitializeDataAsync();
                UpdateProgressBar(100);
            });

            progressBar.Visibility = Visibility.Collapsed;
        }

        private void UpdateProgressBar(int value)
        {
            Dispatcher.Invoke(() => progressBar.Value = value);
        }

        private void TopGainers_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                
                var topGainers = new TopGainers();
                topGainers.Show();
            }
        }

        private void TopLosers_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                
                var topLosers = new TopLosers();
                topLosers.Show();
            }
        }

        private void TrendingCoin1_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                
                TrendingCoin1 trendingCoin1 = new TrendingCoin1();
                trendingCoin1.Show();
            }
        }

        private void TrendingCoin2_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                TrendingCoin2 trendingCoin2 = new TrendingCoin2();
                trendingCoin2.Show();
            }
        }

        private void TrendingCoin3_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                TrendingCoin3 trendingCoin3 = new TrendingCoin3();
                trendingCoin3.Show();
            }
        }

        private void TrendingCoin4_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                TrendingCoin4 trendingCoin4 = new TrendingCoin4();
                trendingCoin4.Show();
            }
        }

        private void TrendingCoin5_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                TrendingCoin5 trendingCoin5 = new TrendingCoin5();
                trendingCoin5.Show();
            }
        }

        private void MostTradedCoins_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                MostTradedCoins mostTradedCoins = new MostTradedCoins();
                mostTradedCoins.Show();
            }
        }

        private void MostAddedToWatchList_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                MostAddedToWatchList mostAddedToWatchList = new MostAddedToWatchList();
                mostAddedToWatchList.Show();
            }
        }

        private void MostAddedToWatchListTradeInfo_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                MostAddedToWatchListTradeInfo mostAddedToWatchListTradeInfo = new MostAddedToWatchListTradeInfo();
                mostAddedToWatchListTradeInfo.Show();
            }
        }

        private void MostVolatileCoins_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {

                MostVolatileCoins mostVolatileCoins = new MostVolatileCoins();
                mostVolatileCoins.Show();
            }
        }
    }
}
