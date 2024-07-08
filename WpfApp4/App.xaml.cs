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

            await FetchData();


            //var barchart = new barChart();
            //barchart.Show();


            //var topGainers = new TopGainers();
            //topGainers.Show();

            //TopLosers topLosers = new TopLosers();
            //topLosers.Show();



            //TrendingCoin1 trendingCoin1 = new TrendingCoin1();
            //trendingCoin1.Show();

            //TrendingCoin2 trendingCoin2 = new TrendingCoin2();
            //trendingCoin2.Show();

            //TrendingCoin3 trendingCoin3 = new TrendingCoin3();
            //trendingCoin3.Show();

            //TrendingCoin4 trendingCoin4 = new TrendingCoin4();
            //trendingCoin4.Show();

            //TrendingCoin5 trendingCoin5 = new TrendingCoin5();
            //trendingCoin5.Show();


            //var mostTraded = new MostTradedCoins();
            //mostTraded.Show();

            var mostAddedToWatchList = new MostAddedToWatchList();
            mostAddedToWatchList.Show();

            var mostAddedToWatchListTradeInfo = new MostAddedToWatchListTradeInfo();
            mostAddedToWatchListTradeInfo.Show();
        }

        private static async Task FetchData()
        {
            //await new LocalDataBaseService().initializeLocalDatabaseAsync();
            //await TopGainersService.InitializeDataAsync();
            //await TrendingService.InitializeDataAsync();

            //await LocalDataBaseService.GetMostTradedCoins();
            //await LocalDataBaseService.GetMostAddedToWatchlistCoins();

            //await LocalDataBaseService.PrintTopByROI();
            //await LocalDataBaseService.PrintCountOfCoinsAddedSinceLastNDays(1000);
            //await LocalDataBaseService.PrintMostRecentCoin();
            //await LocalDataBaseService.SelectCoinsWithATHCloseToCurrentPrice(5);
            //await LocalDataBaseService.GetTop10PositiveSentimentCoins();
            //await LocalDataBaseService.GetTop10NegativeSentimentCoins();
            //await LocalDataBaseService.PrintCountOf100PercentSentimentUpVotes();
            //await LocalDataBaseService.PrintCoinsWith100PercentSentimentUpVotes();
            //await LocalDataBaseService.PrintMostAddedToWatchlistCoins();

        }
    }
}
