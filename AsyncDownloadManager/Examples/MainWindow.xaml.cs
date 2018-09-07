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
using System.Net;
using System.IO;

namespace Examples
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();

            textBox.Text += "Асинхронное получение заголовков страницы в интернете.\n";
            textBox.Text += "Данный вариант используется, если библиотека реализует асинхронную модель C# 5\n\n";
            textBox.Text += "Starting async download\n\n";

            await DoDownloadAsync();
        }

        private async Task DoDownloadAsync()
        {
            var req = (HttpWebRequest)WebRequest.Create("https://www.microsoft.com");
            req.Method = "GET";
            var task = req.GetResponseAsync();

            var resp = (HttpWebResponse)await task;

            textBox.Text += resp.Headers.ToString();

            textBox.Text += "Async download completed";
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();

            textBox.Text += "Асинхронное получение заголовков страницы в интернете.\n";
            textBox.Text += "Данный вариант используется, если библиотека реализует более ранюю асинхронную модель, чем C# 5\n\n";
            textBox.Text += "Starting async download\n\n";

            await DoDownloadFromAsync();
        }

        private async Task DoDownloadFromAsync()
        {
            var req = (HttpWebRequest)WebRequest.Create("https://www.microsoft.com");
            req.Method = "GET";

            Task<WebResponse> getResponseTask = Task.Factory.FromAsync<WebResponse>(
                req.BeginGetResponse, req.EndGetResponse, null);

            var resp = (HttpWebResponse)await getResponseTask;

            textBox.Text += resp.Headers.ToString();

            textBox.Text += "Async download completed";
        }

        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();

            textBox.Text += "Пример асинхронной загрузки страницы\n\n";
            textBox.Text += "Starting async download\n\n";

            using (var w = new WebClient())
            {
                string txt = await w.DownloadStringTaskAsync("https://www.microsoft.com");
                textBox.Text += txt;
                textBox.Text += "\n\nAsync download completed";
            }
        }

        private async void Button4_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();

            textBox.Text += "Ещё пример асинхронной загрузки страницы\n\n";
            textBox.Text += "Starting async download\n\n";

            byte[] urlContents = await GetURLContentsAsunc("https://www.microsoft.com");

            Encoding enc8 = Encoding.UTF8;
            textBox.Text += enc8.GetString(urlContents);
            textBox.Text += "\n\nAsync download completed";
        }

        private async Task<byte[]> GetURLContentsAsunc(string url)
        {
            var content = new MemoryStream();
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            using (WebResponse response = await webReq.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        await responseStream.CopyToAsync(content);
                }
            }
            return content.ToArray();
        }
    }
}
