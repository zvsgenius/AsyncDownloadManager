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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net.Http;

namespace AsyncDownloadManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartBatton_Click(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Clear();

            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsync(cts.Token);
                resultsTextBox.Text += "\nDownloads complete.\n";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\nDownloads canceled.\n";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\nDownloads failed.\n";
            }

            cts = null;
        }

        private async Task AccessTheWebAsync(CancellationToken ct)
        {
            var client = new HttpClient();

            IEnumerable<string> urlList = SetUpURLList();

            IEnumerable<Task<int>> downloadTasksQuery =
                from url in urlList select ProcessURL(url, client, ct);

            List<Task<int>> downloadTasks = downloadTasksQuery.ToList();

            while(downloadTasks.Count > 0)
            {
                Task<int> firstFinishedTask = await Task.WhenAny(downloadTasks);

                downloadTasks.Remove(firstFinishedTask);

                int length = firstFinishedTask.Result;

                resultsTextBox.Text += String.Format("\nLength of the download: {0}", length);
            }
        }

        async Task<int> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            HttpResponseMessage response = await client.GetAsync(url, ct);

            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            return urlContents.Length;
        }

        private IEnumerable<string> SetUpURLList()
        {
            var urls = new List<string>
            {
                "http://azinnov.com/",
                "http://azinnov.com/news/",
                "http://azinnov.com/news/7178/",
                "http://azinnov.com/news/7177/",
                "http://azinnov.com/news/7176/",
                "http://azinnov.com/news/7175/",
                "http://azinnov.com/news/7174/",
                "http://azinnov.com/news/7173/"
            };
            return urls;
        }

        private void CancelBatton_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

    }

}
