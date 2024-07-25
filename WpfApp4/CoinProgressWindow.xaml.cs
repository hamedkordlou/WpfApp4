using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp4.Data;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for CoinProgressWindow.xaml
    /// </summary>
    public partial class CoinProgressWindow : Window
    {
        private readonly LocalDataBaseService _localDatabaseService;

        public CoinProgressWindow()
        {
            InitializeComponent();
            _localDatabaseService = new LocalDataBaseService(LogMessage, UpdateProgress);
        }

        public async Task InitializeLocalDatabaseAsync()
        {
            await _localDatabaseService.InitializeLocalDatabaseAsync();
            Dispatcher.Invoke(() => statusTextBox.AppendText("All coin data has been saved to the database.\n"));
        }

        private void UpdateProgress(int index, int totalCoins, string coinId, int retries)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = ((double)index / totalCoins) * 100;
                statusTextBox.AppendText($"Processing coin: {coinId}, Retry: {retries}\n");
                statusTextBox.ScrollToEnd();
            });
        }

        private void LogMessage(string message)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    statusTextBox.AppendText(message + "\n");
                    statusTextBox.ScrollToEnd();
                });
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
