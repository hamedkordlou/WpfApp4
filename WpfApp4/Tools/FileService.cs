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
        private readonly Action<int> _updateProgress;

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

        public FileService(OutputWindow outputWindow, Action<int> updateProgress)
        {
            _afterEffectsPath = @"C:\Program Files\Adobe\Adobe After Effects 2024\Support Files\aerender.exe";
            _scriptsFolderPath = @"C:\Users\hamed\Documents\Work\Personal\Crypto\Templates\Data Scene";
            _outputWindow = outputWindow;
            _updateProgress = updateProgress;
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
            int totalFiles = scriptFiles.Length;

            for (int i = 0; i < totalFiles; i++)
            {
                string scriptPath = scriptFiles[i];
                _updateProgress?.Invoke((i + 1) * 100 / totalFiles);
                await RenderScriptAsync(scriptPath);
            }

            _updateProgress?.Invoke(100); // Ensure progress bar shows 100% upon completion
            _outputWindow.AppendOutput("All scripts have been rendered.");
        }

        private async Task RenderScriptAsync(string scriptPath)
        {
            var projectName = Path.GetFileNameWithoutExtension(scriptPath);

            _outputWindow.SetFileTitle(projectName);

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
                    // Set result only if the task hasn't been completed already
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.SetResult(true);
                    }
                    process.Dispose();
                };

                

                // Ensure Exited event is subscribed to
                process.EnableRaisingEvents = true;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                //// Optional: Add a timeout to prevent hanging indefinitely
                //var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5)); // Adjust timeout as needed
                //var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                //if (completedTask == timeoutTask)
                //{
                //    // Handle timeout (e.g., process didn't exit in time)
                //    throw new TimeoutException("The process took too long to exit.");
                //}

                //await tcs.Task;
                await process.WaitForExitAsync();

            }
        }
    }
}
