using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Process proc = new Process();
            //DirectoryInfo dirInfo = Directory.GetParent(Directory.GetCurrentDirectory());
            //string path = @dirInfo.Parent.Parent.ToString();
            //path += @"\dist\FootballScraper\FootballScraper.exe";
            //proc.StartInfo.FileName = path;
            //proc.StartInfo.Arguments = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.CreateNoWindow = true;
            //proc.Start();

            Application.Current.MainWindow = this;
            Loaded += OnMainMenuLoaded;

            //proc.WaitForExit(1000 * 60 * 10);
            //Debug.WriteLine("Script done");
        }

        private void OnMainMenuLoaded(object sender, RoutedEventArgs e)
        {
            ChangeView(new MainMenu());
        }

        public void ChangeView(Page view)
        {
            mainFrame.NavigationService.Navigate(view);
        }
    }
}
