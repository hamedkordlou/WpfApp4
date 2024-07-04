using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Data
{
    public static class TrendingService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string url = "https://pro-api.coingecko.com/api/v3/search/trending?x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum";

        public static List<CoinTrending> trendingCoins = new List<CoinTrending>();
        public static List<List<List<double>>> prices = new List<List<List<double>>>();

        public static async Task InitializeDataAsync()
        {
            var response = await client.GetStringAsync(url);
            PopulateCoinLists(response);

            var ids = trendingCoins.Select(x => x.item.id).ToList();
            //var marketUrl = $"https://pro-api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={string.Join("%2C", ids)}&x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum";

            foreach (var id in ids)
            {
                var marketUrl = $"https://pro-api.coingecko.com/api/v3/coins/{id}/market_chart?vs_currency=usd&days=1&x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum";
                // get market for ids
                response = await client.GetStringAsync(marketUrl);
                PopulateCoinHistory(response);
            }
        }

        

        public static List<List<List<double>>> GetTrendingCoins()
        {
            return prices;
        }

        public static void PopulateCoinLists(string jsonResponse)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<Root>(jsonResponse);

                if (apiResponse != null)
                {
                    trendingCoins.AddRange(apiResponse.coins);
                    // add nft here
                    // add category here
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deserializing the JSON response: {ex.Message}");
            }
        }

        public static void PopulateCoinHistory(string jsonResponse)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<CoinHistory>(jsonResponse);

                if (apiResponse != null)
                {
                    prices.Add(apiResponse.prices);
                    // add nft here
                    // add category here
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deserializing the JSON response: {ex.Message}");
            }
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public double market_cap_1h_change { get; set; }
        public string slug { get; set; }
        public int coins_count { get; set; }
        public Data data { get; set; }
    }

    public class CoinTrending
    {
        public Item item { get; set; }
    }

    public class Content
    {
        public string title { get; set; }
        public string description { get; set; }
    }

    public class Data
    {
        public double price { get; set; }
        public string price_btc { get; set; }
        public PriceChangePercentage24h price_change_percentage_24h { get; set; }
        public string market_cap { get; set; }
        public string market_cap_btc { get; set; }
        public string total_volume { get; set; }
        public string total_volume_btc { get; set; }
        public string sparkline { get; set; }
        public Content content { get; set; }
        public string floor_price { get; set; }
        public string floor_price_in_usd_24h_percentage_change { get; set; }
        public string h24_volume { get; set; }
        public string h24_average_sale_price { get; set; }
        public MarketCapChangePercentage24h market_cap_change_percentage_24h { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public int coin_id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int market_cap_rank { get; set; }
        public string thumb { get; set; }
        public string small { get; set; }
        public string large { get; set; }
        public string slug { get; set; }
        public double price_btc { get; set; }
        public int score { get; set; }
        public Data data { get; set; }
    }

    public class MarketCapChangePercentage24h
    {
        public double aed { get; set; }
        public double ars { get; set; }
        public double aud { get; set; }
        public double bch { get; set; }
        public double bdt { get; set; }
        public double bhd { get; set; }
        public double bmd { get; set; }
        public double bnb { get; set; }
        public double brl { get; set; }
        public double btc { get; set; }
        public double cad { get; set; }
        public double chf { get; set; }
        public double clp { get; set; }
        public double cny { get; set; }
        public double czk { get; set; }
        public double dkk { get; set; }
        public double dot { get; set; }
        public double eos { get; set; }
        public double eth { get; set; }
        public double eur { get; set; }
        public double gbp { get; set; }
        public double gel { get; set; }
        public double hkd { get; set; }
        public double huf { get; set; }
        public double idr { get; set; }
        public double ils { get; set; }
        public double inr { get; set; }
        public double jpy { get; set; }
        public double krw { get; set; }
        public double kwd { get; set; }
        public double lkr { get; set; }
        public double ltc { get; set; }
        public double mmk { get; set; }
        public double mxn { get; set; }
        public double myr { get; set; }
        public double ngn { get; set; }
        public double nok { get; set; }
        public double nzd { get; set; }
        public double php { get; set; }
        public double pkr { get; set; }
        public double pln { get; set; }
        public double rub { get; set; }
        public double sar { get; set; }
        public double sek { get; set; }
        public double sgd { get; set; }
        public double thb { get; set; }
        public double @try { get; set; }
        public double twd { get; set; }
        public double uah { get; set; }
        public double usd { get; set; }
        public double vef { get; set; }
        public double vnd { get; set; }
        public double xag { get; set; }
        public double xau { get; set; }
        public double xdr { get; set; }
        public double xlm { get; set; }
        public double xrp { get; set; }
        public double yfi { get; set; }
        public double zar { get; set; }
        public double bits { get; set; }
        public double link { get; set; }
        public double sats { get; set; }
    }

    public class Nft
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string thumb { get; set; }
        public int nft_contract_id { get; set; }
        public string native_currency_symbol { get; set; }
        public double floor_price_in_native_currency { get; set; }
        public double floor_price_24h_percentage_change { get; set; }
        public Data data { get; set; }
    }

    public class PriceChangePercentage24h
    {
        public double aed { get; set; }
        public double ars { get; set; }
        public double aud { get; set; }
        public double bch { get; set; }
        public double bdt { get; set; }
        public double bhd { get; set; }
        public double bmd { get; set; }
        public double bnb { get; set; }
        public double brl { get; set; }
        public double btc { get; set; }
        public double cad { get; set; }
        public double chf { get; set; }
        public double clp { get; set; }
        public double cny { get; set; }
        public double czk { get; set; }
        public double dkk { get; set; }
        public double dot { get; set; }
        public double eos { get; set; }
        public double eth { get; set; }
        public double eur { get; set; }
        public double gbp { get; set; }
        public double gel { get; set; }
        public double hkd { get; set; }
        public double huf { get; set; }
        public double idr { get; set; }
        public double ils { get; set; }
        public double inr { get; set; }
        public double jpy { get; set; }
        public double krw { get; set; }
        public double kwd { get; set; }
        public double lkr { get; set; }
        public double ltc { get; set; }
        public double mmk { get; set; }
        public double mxn { get; set; }
        public double myr { get; set; }
        public double ngn { get; set; }
        public double nok { get; set; }
        public double nzd { get; set; }
        public double php { get; set; }
        public double pkr { get; set; }
        public double pln { get; set; }
        public double rub { get; set; }
        public double sar { get; set; }
        public double sek { get; set; }
        public double sgd { get; set; }
        public double thb { get; set; }
        public double @try { get; set; }
        public double twd { get; set; }
        public double uah { get; set; }
        public double usd { get; set; }
        public double vef { get; set; }
        public double vnd { get; set; }
        public double xag { get; set; }
        public double xau { get; set; }
        public double xdr { get; set; }
        public double xlm { get; set; }
        public double xrp { get; set; }
        public double yfi { get; set; }
        public double zar { get; set; }
        public double bits { get; set; }
        public double link { get; set; }
        public double sats { get; set; }
    }

    public class Root
    {
        public List<CoinTrending> coins { get; set; }
        public List<Nft> nfts { get; set; }
        public List<Category> categories { get; set; }
    }

    // CoinHistory myDeserializedClass = JsonConvert.DeserializeObject<CoinHistory>(myJsonResponse);
    public class CoinHistory
    {
        public List<List<double>> prices { get; set; }
        public List<List<double>> market_caps { get; set; }
        public List<List<double>> total_volumes { get; set; }
    }

}
