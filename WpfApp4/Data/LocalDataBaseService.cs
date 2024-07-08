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
using System.Windows.Documents;

namespace WpfApp4.Data
{
    public class LocalCoin
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Platforms { get; set; }
    }

    public class MostTradedCoin
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal TotalVolumeUsd { get; set; }
    }

    public class MostWatched
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double WatchlistUsers { get; set; }
        public decimal TotalVolumeUsd { get; set; }
        
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

        public static async Task<List<MostTradedCoin>> GetMostTradedCoins(int topN = 10)
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
                .Select(coin => new MostTradedCoin
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    TotalVolumeUsd = coin["market_data"]["total_volume"]["usd"].ToDecimal()
                })
                .OrderByDescending(coin => coin.TotalVolumeUsd)
                .Take(topN)
                .ToList();

            // Print most traded coins
            //Console.WriteLine($"Top {topN} Most Traded Coins:");
            //foreach (var coin in mostTradedCoins)
            //{
            //    Console.WriteLine($"{coin.Name} ({coin.Id}) - Trading Volume (USD): {coin.TotalVolumeUsd.ToString("C", new CultureInfo("en-US"))}");
            //}

            return mostTradedCoins;
        }

        public static async Task<List<MostWatched>> GetMostAddedToWatchlistCoins(int topCount = 30)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select and sort coins by watchlist portfolio users
            var mostAddedToWatchlistCoins = allCoins
                .Where(coin => coin.Contains("watchlist_portfolio_users")
                               && !coin["watchlist_portfolio_users"].IsBsonNull
                               && coin.Contains("market_data")
                               && coin["market_data"].AsBsonDocument.Contains("total_volume")
                               && coin["market_data"]["total_volume"].AsBsonDocument.Contains("usd")
                               && !coin["market_data"]["total_volume"]["usd"].IsBsonNull
                               )
                .Select(coin => new MostWatched
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    WatchlistUsers = coin["watchlist_portfolio_users"].ToInt32(),
                    TotalVolumeUsd = coin["market_data"]["total_volume"]["usd"].ToDecimal()
                })
                .OrderByDescending(coin => coin.WatchlistUsers)
                .Take(topCount)
                .ToList();

            // Print the most added to watchlist coins
            Console.WriteLine($"Top {topCount} Coins Most Added to Watchlist:");

            return mostAddedToWatchlistCoins;
        }




        public static async Task PrintTopByROI(int topN = 10)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select coins with valid current price and ATL, then calculate ROI and sort them by ROI
            var topByROICoins = allCoins
                .Where(coin => coin.Contains("market_data")
                               && coin["market_data"].AsBsonDocument.Contains("current_price")
                               && coin["market_data"]["current_price"].AsBsonDocument.Contains("usd")
                               && !coin["market_data"]["current_price"]["usd"].IsBsonNull
                               && coin["market_data"].AsBsonDocument.Contains("atl")
                               && coin["market_data"]["atl"].AsBsonDocument.Contains("usd")
                               && !coin["market_data"]["atl"]["usd"].IsBsonNull)
                .Select(coin => new
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    CurrentPrice = coin["market_data"]["current_price"]["usd"].ToDecimal(),
                    Atl = coin["market_data"]["atl"]["usd"].ToDecimal(),
                    ROI = ((coin["market_data"]["current_price"]["usd"].ToDecimal() - coin["market_data"]["atl"]["usd"].ToDecimal()) / coin["market_data"]["atl"]["usd"].ToDecimal()) * 100
                })
                .OrderByDescending(coin => coin.ROI)
                .Take(topN)
                .ToList();

            // Print top coins by ROI
            Console.WriteLine($"Top {topN} Coins by ROI:");
            foreach (var coin in topByROICoins)
            {
                Console.WriteLine($"{coin.Name} ({coin.Id}) - ROI: {coin.ROI:F2}% - Current Price: ${coin.CurrentPrice:F2} - ATL: ${coin.Atl:F2}");
            }
        }

        public static async Task PrintCountOfCoinsAddedSinceLastNDays(int N)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Calculate the date 7 days ago from now
            DateTime sevenDaysAgo = DateTime.UtcNow.AddDays(-1 * N);

            // Query the database for coins added since N days ago
            var filter = Builders<BsonDocument>.Filter.Gte("genesis_date", sevenDaysAgo);
            var recentCoinsCount = await collection.CountDocumentsAsync(filter);

            // Print the count of recent coins
            Console.WriteLine($"Number of coins added since {sevenDaysAgo.ToShortDateString()}: {recentCoinsCount}");
        }

        public static async Task PrintMostRecentCoin()
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Query the database for the most recent coin based on genesis_date
            var mostRecentCoin = await collection.Find(new BsonDocument())
                                                 .Sort(Builders<BsonDocument>.Sort.Descending("genesis_date"))
                                                 .FirstOrDefaultAsync();

            if (mostRecentCoin != null)
            {
                Console.WriteLine("Most Recent Coin Based on Genesis Date:");
                Console.WriteLine($"Name: {mostRecentCoin["name"].AsString}");
                Console.WriteLine($"ID: {mostRecentCoin["id"].AsString}");
                Console.WriteLine($"Genesis Date: {mostRecentCoin["genesis_date"].ToUniversalTime():yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("No coins found.");
            }
        }


        public static async Task SelectCoinsWithATHCloseToCurrentPrice(decimal thresholdPercentage = 5)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select coins where ATH is within the threshold percentage of the current price
            var coinsCloseToATH = allCoins
                .Where(coin => coin.Contains("market_data")
                               && coin["market_data"].AsBsonDocument.Contains("ath")
                               && coin["market_data"]["ath"].AsBsonDocument.Contains("usd")
                               && coin["market_data"].AsBsonDocument.Contains("current_price")
                               && coin["market_data"]["current_price"].AsBsonDocument.Contains("usd")
                               && !coin["market_data"]["ath"]["usd"].IsBsonNull
                               && !coin["market_data"]["current_price"]["usd"].IsBsonNull)
                .Select(coin => new
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    ATH = coin["market_data"]["ath"]["usd"].ToDecimal(),
                    CurrentPrice = coin["market_data"]["current_price"]["usd"].ToDecimal(),
                    ATHPercentageDifference = coin["market_data"]["ath"]["usd"].ToDecimal() != 0
                                              ? Math.Abs(((coin["market_data"]["current_price"]["usd"].ToDecimal() - coin["market_data"]["ath"]["usd"].ToDecimal()) / coin["market_data"]["ath"]["usd"].ToDecimal()) * 100)
                                              : 0
                })
                .Where(coin => coin.ATHPercentageDifference <= thresholdPercentage)
                .ToList();

            // Print coins where ATH is close to current price
            Console.WriteLine($"Coins with ATH close to Current Price (within {thresholdPercentage}%):");
            foreach (var coin in coinsCloseToATH)
            {
                Console.WriteLine($"{coin.Name} ({coin.Id}) - Current Price: ${coin.CurrentPrice:F2} - ATH: ${coin.ATH:F2}");
                Console.WriteLine($"Difference: {coin.ATHPercentageDifference:F2}%");
                Console.WriteLine(); // Blank line for separation
            }
        }


        public static async Task GetTop10PositiveSentimentCoins()
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select and sort coins by positive sentiment votes
            var positiveSentimentCoins = allCoins
                .Where(coin => coin.Contains("sentiment_votes_up_percentage")
                               && !coin["sentiment_votes_up_percentage"].IsBsonNull)
                .Select(coin => new
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    PositiveSentiment = coin["sentiment_votes_up_percentage"].ToDecimal()
                })
                .OrderByDescending(coin => coin.PositiveSentiment)
                .Take(10)
                .ToList();

            // Print top 10 positive sentiment coins
            Console.WriteLine("Top 10 Positive Sentiment Coins:");
            foreach (var coin in positiveSentimentCoins)
            {
                Console.WriteLine($"{coin.Name} ({coin.Id}) - Positive Sentiment: {coin.PositiveSentiment:F2}%");
            }
        }

        public static async Task GetTop10NegativeSentimentCoins()
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select and sort coins by negative sentiment votes
            var negativeSentimentCoins = allCoins
                .Where(coin => coin.Contains("sentiment_votes_down_percentage")
                               && !coin["sentiment_votes_down_percentage"].IsBsonNull)
                .Select(coin => new
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    NegativeSentiment = coin["sentiment_votes_down_percentage"].ToDecimal()
                })
                .OrderByDescending(coin => coin.NegativeSentiment)
                .Take(10)
                .ToList();

            // Print top 10 negative sentiment coins
            Console.WriteLine("Top 10 Negative Sentiment Coins:");
            foreach (var coin in negativeSentimentCoins)
            {
                Console.WriteLine($"{coin.Name} ({coin.Id}) - Negative Sentiment: {coin.NegativeSentiment:F2}%");
            }
        }

        public static async Task PrintCountOf100PercentSentimentUpVotes()
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Count coins with 100% sentiment up votes
            var count100PercentSentimentUpVotes = allCoins
                .Count(coin => coin.Contains("sentiment_votes_up_percentage")
                               && !coin["sentiment_votes_up_percentage"].IsBsonNull
                               && coin["sentiment_votes_up_percentage"].ToDecimal() == 100);

            // Print the count
            Console.WriteLine($"Count of coins with 100% sentiment up votes: {count100PercentSentimentUpVotes}");
        }

        public static async Task PrintCoinsWith100PercentSentimentUpVotes()
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select coins with 100% sentiment up votes
            var coinsWith100PercentUpVotes = allCoins
                .Where(coin => coin.Contains("sentiment_votes_up_percentage")
                               && !coin["sentiment_votes_up_percentage"].IsBsonNull
                               && coin["sentiment_votes_up_percentage"].ToDecimal() == 100)
                .ToList();

            // Print the details of coins with 100% sentiment up votes
            Console.WriteLine("Coins with 100% Sentiment Up Votes:");
            
        }

        public static async Task<List<MostWatched>> PrintMostAddedToWatchlistCoins(int topCount = 30)
        {
            var collection = database.GetCollection<BsonDocument>("coins");

            // Retrieve all coins from the database
            var allCoins = await collection.Find(new BsonDocument()).ToListAsync();

            // Select and sort coins by watchlist portfolio users
            var mostAddedToWatchlistCoins = allCoins
                .Where(coin => coin.Contains("watchlist_portfolio_users")
                               && !coin["watchlist_portfolio_users"].IsBsonNull
                               && coin.Contains("market_data")
                               && coin["market_data"].AsBsonDocument.Contains("total_volume")
                               && coin["market_data"]["total_volume"].AsBsonDocument.Contains("usd")
                               && !coin["market_data"]["total_volume"]["usd"].IsBsonNull
                               )
                .Select(coin => new MostWatched
                {
                    Id = coin["id"].AsString,
                    Name = coin["name"].AsString,
                    WatchlistUsers = coin["watchlist_portfolio_users"].ToInt32(),
                    TotalVolumeUsd = coin["market_data"]["total_volume"]["usd"].ToDecimal()
                })
                .OrderByDescending(coin => coin.TotalVolumeUsd)
                .Take(topCount)
                .ToList();

            // Print the most added to watchlist coins
            Console.WriteLine($"Top {topCount} Coins Most Added to Watchlist:");

            return mostAddedToWatchlistCoins;
        }

        



    }
}
