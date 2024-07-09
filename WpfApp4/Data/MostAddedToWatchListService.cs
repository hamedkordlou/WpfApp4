using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Data
{
    public class MostAddedToWatchListService
    {
        public static List<MostWatched> mostWatched = new List<MostWatched>();
        public static async Task InitializeDataAsync(int topN = 10)
        {
            mostWatched = await LocalDataBaseService.GetMostAddedToWatchlistCoins(topN);
        }

        public static List<MostWatched> GetMostAddedToWatchlistCoins()
        {
            return mostWatched;
        }
    }
}
