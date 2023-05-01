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
            //Call webscraping script
            Application.Current.MainWindow = this;
            Loaded += OnConfirmMenuLoaded;
        }
        // Navigate to main menu
        private void OnConfirmMenuLoaded(object sender, RoutedEventArgs e)
        {
            ChangeView(new ConfirmPage());
        }
        // Change the page view
        public void ChangeView(Page view)
        {
            mainFrame.NavigationService.Navigate(view);
        }
    }
}
