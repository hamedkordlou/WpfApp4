using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Data
{
    public static class TopGainersService
    {

        private static readonly HttpClient client = new HttpClient();
        private const string url = "https://pro-api.coingecko.com/api/v3/coins/top_gainers_losers?vs_currency=usd&duration=24h&x_cg_pro_api_key=CG-eyrEeYZTJcaC7skRKaAmSwum";

        public static List<Coin> topGainers = new List<Coin>();
        public static List<Coin> topLosers = new List<Coin>();

        public static async Task InitializeDataAsync()
        {
            var response = await client.GetStringAsync(url);
            PopulateCoinLists(response);
        }


        public static async Task<List<Coin>> GetTopGainersAsync()
        {
            //var response = await client.GetStringAsync(url);
            //PopulateCoinLists(response);

            return topGainers.Take(10).ToList();
        }

        public static async Task<List<Coin>> GetTopLosers()
        {
            //var response = await client.GetStringAsync(url);
            //PopulateCoinLists(response);

            return topLosers.Take(10).ToList(); //.OrderBy(c => c.usd_24h_change);
        }

        public static void PopulateCoinLists(string jsonResponse)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                if (apiResponse != null)
                {
                    topGainers.AddRange(apiResponse.TopGainers);
                    topLosers.AddRange(apiResponse.TopLosers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deserializing the JSON response: {ex.Message}");
            }
        }

        public static List<Coin> GetMockTopGainers()
        {
            return new List<Coin>
            {
                //new Coin { Id = "beta-finance", Symbol = "beta", Name = "Beta Finance", MarketCapRank = 591, USD = 0.06975703264898371, USD24hVol = 30995137.27481198, USD24hChange = 63.33458331225851, Image = "C:\\Users\\hamed\\Documents\\Work\\Personal\\Crypto\\beta_finance.jpg" },
                //new Coin { Id = "radicle", Symbol = "rad", Name = "Radworks", MarketCapRank = 496, USD = 1.4963851342927228, USD24hVol = 151502161.506981, USD24hChange = 26.514084492790722, Image = "https://coin-images.coingecko.com/coins/images/14013/original/radicle.png?1696513741" },
                //new Coin { Id = "weatherxm-network", Symbol = "wxm", Name = "WeatherXM Network", MarketCapRank = 651, USD = 0.8276386643573167, USD24hVol = 908676.7147907694, USD24hChange = 25.408773222562886, Image = "https://coin-images.coingecko.com/coins/images/38154/original/weatherxm-network-logo.png?1716668976" },
                //new Coin { Id = "retardio", Symbol = "retardio", Name = "RETARDIO", MarketCapRank = 942, USD = 0.02270744710479678, USD24hVol = 1179174.3182424996, USD24hChange = 22.967131253001078, Image = "https://coin-images.coingecko.com/coins/images/35759/original/RETARDIO_LOGO-200x200.png?1709726267" },
                //new Coin { Id = "wexo", Symbol = "wexo", Name = "Wexo", MarketCapRank = 367, USD = 2.2251349728819045, USD24hVol = 168267.87044845574, USD24hChange = 22.073366943676163, Image = "https://coin-images.coingecko.com/coins/images/33801/original/wexo_token_200x200.png?1702991908" },
                //// Add more top gainers as needed
            };
        }

        public static List<Coin> GetMockTopLosers()
        {
            return new List<Coin>
            {
                //new Coin { Id = "pacmoon", Symbol = "pac", Name = "PacMoon", Image = "https://coin-images.coingecko.com/coins/images/36459/original/pacmoon.png?1711501114", MarketCapRank = 813, USD = 0.06537863452944274, USD24hVol = 3973827.5800345107, USD24hChange = -31.053755041995636 },
                //new Coin { Id = "andyerc", Symbol = "andy", Name = "AndyBlast", Image = "https://coin-images.coingecko.com/coins/images/35767/original/IMG_0310.jpeg?1711371918", MarketCapRank = 768, USD = 0.34342084708617526, USD24hVol = 3025256.312597007, USD24hChange = -30.32764634684895 },
                //new Coin { Id = "shark-cat", Symbol = "sc", Name = "Shark Cat", Image = "https://coin-images.coingecko.com/coins/images/36562/original/shark.jpeg?1715148291", MarketCapRank = 738, USD = 0.0360972323293523, USD24hVol = 3713279.745455345, USD24hChange = -22.14773938376627 },
                //// Add other items here...
            };
        }
    }

    public class Coin
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int market_cap_rank { get; set; }
        public double usd { get; set; }
        public double usd_24h_vol { get; set; }
        public double usd_24h_change { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("top_gainers")]
        public List<Coin> TopGainers { get; set; }

        [JsonProperty("top_losers")]
        public List<Coin> TopLosers { get; set; }
    }


}
