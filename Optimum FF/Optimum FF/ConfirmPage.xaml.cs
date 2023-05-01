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

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for ConfirmPage.xaml
    /// </summary>
    public partial class ConfirmPage : Page
    {
        public ConfirmPage()
        {
            InitializeComponent();
        }

        private void no_button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MainMenu());
        }

        private void yes_button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new ScrapingPage());
        }
    }
}
