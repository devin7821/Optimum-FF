using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
            //Call webscraping script
            Process.Start(new ProcessStartInfo { FileName = "FootballScraper.exe", 
                Arguments = ConfigurationManager.ConnectionStrings["connection"].ConnectionString, UseShellExecute = false,
            CreateNoWindow = true }).WaitForExit(1000 * 60 * 10);
        }
        //Navigate to the Optimize Page
        private void OptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            Settings settings = new Settings();
            mainWindow?.ChangeView(new OptimizePage(settings));
        }
    }
}
