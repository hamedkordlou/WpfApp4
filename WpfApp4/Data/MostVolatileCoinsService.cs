using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Data
{
    public class MostVolatileCoinsService
    {
        public static List<MostVolatile> mostVolatile = new List<MostVolatile>();
        public static async Task InitializeDataAsync(int topN = 10)
        {
            mostVolatile = await LocalDataBaseService.GetMostVolatileCoins(topN);
        }

        public static List<MostVolatile> GetMostVolatileCoins()
        {
            return mostVolatile;
        }
    }
}
