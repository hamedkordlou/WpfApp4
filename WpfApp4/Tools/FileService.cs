using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfApp4.Data;

namespace WpfApp4.Tools
{
    public class FileService
    {
        private string _outputFolder;

        public FileService(string windowName)
        {
            _outputFolder = Path.Combine(@"C:\output", windowName);
            Directory.CreateDirectory(_outputFolder);
        }

        public async Task SaveCoinDataAsync(Item coin)
        {
            // Ensure the directory exists
            Directory.CreateDirectory(_outputFolder);

            // Define the file path
            string filePath = Path.Combine(_outputFolder, "data.txt");
            string logoFilePath = Path.Combine(_outputFolder, "logo.png");

            // Extract data
            string name = coin.name;
            string marketCap = coin.data.market_cap;
            int marketCapRank = coin.market_cap_rank;
            double priceChangePercentage24hUsd = coin.data.price_change_percentage_24h.usd;
            string totalVolume = coin.data.total_volume;
            string logoUrl = coin.large;

            // Create the content
            StringBuilder content = new StringBuilder();
            content.AppendLine($"{name}");
            content.AppendLine($"{marketCap}");
            content.AppendLine($"{marketCapRank}");
            content.AppendLine($"{priceChangePercentage24hUsd}");
            content.AppendLine($"{totalVolume}");

            // Write to file asynchronously
            await File.WriteAllTextAsync(filePath, content.ToString());

            // Download and save the logo image asynchronously
            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(logoUrl);
                await File.WriteAllBytesAsync(logoFilePath, imageBytes);
            }
        }
    }
}
