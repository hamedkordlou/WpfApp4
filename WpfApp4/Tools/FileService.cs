using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfApp4.Data;
using Path = System.IO.Path;

namespace WpfApp4.Tools
{
    public class FileService
    {
        private string _outputFolder;
        private readonly string _scriptsFolderPath;
        private readonly string _afterEffectsPath;
        private OutputWindow _outputWindow;

        public FileService()
        {
            _afterEffectsPath = @"C:\Program Files\Adobe\Adobe After Effects 2024\Support Files\aerender.exe";
            _scriptsFolderPath = @"C:\Users\hamed\Documents\Work\Personal\Crypto\Templates\Data Scene";
        }

        public FileService(string windowName)
        {
            _outputFolder = Path.Combine(@"C:\output", windowName);
            Directory.CreateDirectory(_outputFolder);
        }

        public FileService(OutputWindow outputWindow)
        {
            _afterEffectsPath = @"C:\Program Files\Adobe\Adobe After Effects 2024\Support Files\aerender.exe";
            _scriptsFolderPath = @"C:\Users\hamed\Documents\Work\Personal\Crypto\Templates\Data Scene";
            _outputWindow = outputWindow;
        }


        public async Task SaveCoinDataAsync(Item coin)
        {
            // Ensure the directory exists
            Directory.CreateDirectory(_outputFolder);

            // Define the file path
            string filePath = Path.Combine(_outputFolder, "data.json");
            string logoFilePath = Path.Combine(_outputFolder, "logo.png");

            // Create an anonymous object to hold the data
            var coinData = new
            {
                name = coin.name,
                market_cap = coin.data.market_cap,
                market_cap_rank = coin.market_cap_rank,
                price_change_percentage_24h = coin.data.price_change_percentage_24h.usd,
                total_volume = coin.data.total_volume
            };

            // Serialize the object to JSON
            string jsonContent = JsonSerializer.Serialize(coinData, new JsonSerializerOptions { WriteIndented = true });

            // Write to file asynchronously
            await File.WriteAllTextAsync(filePath, jsonContent);

            // Download and save the logo image asynchronously
            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(coin.large);
                await File.WriteAllBytesAsync(logoFilePath, imageBytes);
            }
        }

        public async Task RenderScriptsInFolderAsync()
        {
            if (!Directory.Exists(_scriptsFolderPath))
            {
                _outputWindow.AppendOutput($"The folder path '{_scriptsFolderPath}' does not exist.");
                return;
            }

            string[] scriptFiles = Directory.GetFiles(_scriptsFolderPath, "*.aep");

            foreach (var scriptPath in scriptFiles)
            {
                await RenderScriptAsync(scriptPath);
            }

            _outputWindow.AppendOutput("All scripts have been rendered.");
        }

        private async Task RenderScriptAsync(string scriptPath)
        {
            var projectName = Path.GetFileNameWithoutExtension(scriptPath);

            var startInfo = new ProcessStartInfo
            {
                FileName = _afterEffectsPath,
                Arguments = $"-project \"{scriptPath}\" -comp \"{projectName}\" -output \"{_scriptsFolderPath}\\{projectName}.mp4\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                var tcs = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        _outputWindow.AppendOutput(args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        _outputWindow.AppendOutput(args.Data);
                    }
                };

                process.Exited += (sender, args) =>
                {
                    tcs.SetResult(true);
                    process.Dispose();
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await tcs.Task;
            }
        }
    }
}
