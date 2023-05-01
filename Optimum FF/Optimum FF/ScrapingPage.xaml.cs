using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for ScrapingPage.xaml
    /// </summary>
    public partial class ScrapingPage : Page
    {
        public ScrapingPage()
        {
            InitializeComponent();
            Loaded += OnScrapingPageLoaded;
        }

        private void OnScrapingPageLoaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(ScrapeStats);

            thread.Start();
            
        }

        private void ScrapeStats()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "FootballScraper.exe",
                Arguments = ConfigurationManager.ConnectionStrings["connection"].ConnectionString,
                UseShellExecute = false,
                CreateNoWindow = true
            }).WaitForExit(1000 * 60 * 10);
            //Call webscraping script
            this.Dispatcher.Invoke(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new MainMenu());
            });          
        }

        private void LoadMenu()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MainMenu());
        }
    }
}
