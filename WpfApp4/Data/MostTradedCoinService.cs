using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Data
{
    public class MostTradedCoinService
    {
        public static List<MostTradedCoin> mostTraded = new List<MostTradedCoin>();
        public static async Task InitializeDataAsync(int topN = 10)
        {
            mostTraded = await LocalDataBaseService.GetMostTradedCoins(topN);
        }

        public static List<MostTradedCoin> GetMostTradedCoins()
        {
            return mostTraded;
        }
    }
}
