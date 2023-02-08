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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            try
            {
                settings.LeagueType = LeagueTypeSelection.Text;
                settings.QBCount = int.Parse(QBCountSelection.Text);
                settings.WRCount = int.Parse(WRCountSelection.Text);
                settings.RBCount = int.Parse(RBCountSelection.Text);
                settings.TECount = int.Parse(TECountSelection.Text);
                settings.FlexCount = int.Parse(FlexCountSelection.Text);
                settings.KCount = int.Parse(KCountSelection.Text);
                settings.DEFCount = int.Parse(DEFCountSelection.Text);
                settings.DPCount = int.Parse(DPCountSelection.Text);
                settings.BenchCount = int.Parse(BenchCountSelection.Text);
                settings.TotalCount = settings.QBCount + settings.WRCount + settings.RBCount + settings.TECount + settings.FlexCount + settings.KCount +
                    settings.DEFCount + settings.DPCount + settings.BenchCount;

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new OptimizePage(settings));
            }
            catch (Exception f)
            {
                MessageBox.Show("Value not selected!");
            }
        }
    }
}
