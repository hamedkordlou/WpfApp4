using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp4.Data
{
    public class LocalCoin
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Platforms { get; set; }
    }
    public class LocalDataBaseService
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly IMongoDatabase database;

        static LocalDataBaseService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("crypto_data");
        }

        private static async Task<List<LocalCoin>> GetAllCoins()
        {
            var response = await client.GetStringAsync("https://pro-api.coingecko.com/api/v3/coins/list?status=active&x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum");
            return JsonConvert.DeserializeObject<List<LocalCoin>>(response);
        }

        private static async Task SaveCoinData(string coinId)
        {

            var response = await client.GetStringAsync($"https://pro-api.coingecko.com/api/v3/coins/{coinId}?x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum");

            Console.WriteLine($"Trying coin: {coinId}");
            var coinData = BsonDocument.Parse(response);

            var collection = database.GetCollection<BsonDocument>("coins");
            await collection.InsertOneAsync(coinData);

            Console.WriteLine($"Saved data for coin: {coinId}");
        }

        private static async Task SaveCoinDataIfNotExists(string coinId)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Check if the coin already exists in the database
            var filter = Builders<BsonDocument>.Filter.Eq("id", coinId);
            var existingCoin = await collection.Find(filter).FirstOrDefaultAsync();

            if (existingCoin == null)
            {
                var response = await client.GetStringAsync($"https://pro-api.coingecko.com/api/v3/coins/{coinId}?x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum");

                Console.WriteLine($"Trying coin: {coinId}");
                var coinData = BsonDocument.Parse(response);

                await collection.InsertOneAsync(coinData);
                Console.WriteLine($"Saved data for coin: {coinId}");
            }
            else
            {
                Console.WriteLine($"Coin data for {coinId} already exists in the database.");
            }
        }


        public async Task initializeLocalDatabaseAsync()
        {
            var coinList = await GetAllCoins();
            var tasks = new List<Task>();

            foreach (var coin in coinList)
            {
                //tasks.Add(SaveCoinData(coin.Id));
                tasks.Add(SaveCoinDataIfNotExists(coin.Id));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("All coin data has been saved to the database.");
        }

        public static async Task PrintMostTradedCoins(int topN = 10)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select coins with valid total trading volume and sort them by trading volume
            var mostTradedCoins = allCoins
                .Where(coin => coin.Contains("market_data")
                               && coin["market_data"].AsBsonDocument.Contains("total_volume")
                               && coin["market_data"]["total_volume"].AsBsonDocument.Contains("usd")
                               && !coin["market_data"]["total_volume"]["usd"].IsBsonNull)
                .Select(coin => new
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    TotalVolumeUsd = coin["market_data"]["total_volume"]["usd"].ToDecimal()
                })
                .OrderByDescending(coin => coin.TotalVolumeUsd)
                .Take(topN)
                .ToList();

            // Print most traded coins
            Console.WriteLine($"Top {topN} Most Traded Coins:");
            foreach (var coin in mostTradedCoins)
            {
                Console.WriteLine($"{coin.Name} ({coin.Id}) - Trading Volume (USD): {coin.TotalVolumeUsd.ToString("C", new CultureInfo("en-US"))}");
            }
        }

    }
}
