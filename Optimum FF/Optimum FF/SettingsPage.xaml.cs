using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
                    settings.DEFCount + settings.DPCount + settings.BenchCount - 1;              
            }
            catch (Exception f)
            {
                MessageBox.Show("Value not selected!");
                return;
            }
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new OptimizePage(settings));
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            Lineup lineup = new Lineup(settings);
            PlayerMasterList masterList = new PlayerMasterList();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "c:\\";
            dialog.Filter = "CSV Spreadsheet|*.csv";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(dialog.FileName);
                    int p = 0;
                    foreach (string line in lines)
                    {
                        string[] columns = line.Split(',');
                        if (columns[0] != "QBCount")
                        {
                            int num = 0;
                            bool isDigit = int.TryParse(columns[0], out num);
                            if (isDigit)
                            {
                                settings.QBCount = int.Parse(columns[0]);
                                settings.WRCount = int.Parse(columns[1]);
                                settings.RBCount = int.Parse(columns[2]);
                                settings.TECount = int.Parse(columns[3]);
                                settings.FlexCount = int.Parse(columns[4]);
                                settings.KCount = int.Parse(columns[5]);
                                settings.DEFCount = int.Parse(columns[6]);
                                settings.DPCount = int.Parse(columns[7]);
                                settings.BenchCount = int.Parse(columns[8]);
                                settings.TotalCount = int.Parse(columns[9]);
                                settings.LeagueType = columns[10];
                                lineup = new Lineup(settings);
                            }
                            else
                            {
                                if (columns[0] == "Bench")
                                {
                                    lineup.Players[p].Position = "Bench";
                                }
                                else
                                {                           
                                    lineup.Players[p].Position = columns[1];
                                    if (columns[0] != "")
                                    {
                                        string[] name = columns[0].Split(" ");
                                        lineup.Players[p].Name = columns[0];
                                        if (columns[1] != "DEF")
                                        {
                                            string reverseName = name[0] + " " + name[1];
                                            Team team = new Team();
                                            foreach (Player player in masterList.Players)
                                            {
                                                if (player.Name == reverseName)
                                                {
                                                    team.Name = player.Team.Name;
                                                    lineup.Players[p].Team = team;
                                                }
                                            }
                                        }
                                    }
                                }
                                p++;
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show("CSV format invalid");
                    return;
                }
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow?.ChangeView(new OptimizePage(lineup));
            }
        }
    }
}
