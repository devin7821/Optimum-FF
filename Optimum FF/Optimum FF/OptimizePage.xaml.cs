using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Configuration;
using System.Diagnostics;

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for OptimizePage.xaml
    /// </summary>
    public partial class OptimizePage : Page
    {
        PlayerMasterList masterList = new PlayerMasterList();
        Lineup lineup = new Lineup();
        //Add kickers to scrape
        public OptimizePage()
        {
            InitializeComponent();

            this.playerList.ItemsSource = lineup.Players;


            int i = 0;
            while (i < lineup.Settings.TotalCount)
            {
                int j = 0;
                while (j < lineup.Settings.QBCount)
                { 
                    lineup.Players[i + j].Position = "QB";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.WRCount)
                {
                    lineup.Players[i + j].Position = "WR";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.RBCount)
                {
                    lineup.Players[i + j].Position = "RB";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.TECount)
                {
                    lineup.Players[i + j].Position = "TE";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.KCount)
                {
                    lineup.Players[i + j].Position = "K";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.DEFCount)
                {
                    lineup.Players[i + j].Position = "DEF";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.DPCount)
                {
                    lineup.Players[i + j].Position = "DP";
                    j++;
                }
                i += j;
                j = 0;
                while (j < lineup.Settings.BenchCount)
                {
                    lineup.Players[i + j].Position = "Bench";
                    j++;
                }
                i += j;
            }
            playerList.Items.Refresh();

            var players = new string[masterList.Players.Count()];
            for (int j = 0; j < players.Count(); j++)
            {
                players[j] = masterList.Players[j].Name;
            }
            search.ItemsSource = players;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MainMenu());
        }

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            bool found = false;
            foreach (var player in lineup.Players)
            {
                if (player == null)
                {
                    break;
                }    
                if (player.Name == search.Text)
                {
                    found = true;
                }
            }
            if (!found)
            {
                foreach (var player in masterList.Players)
                {
                    if (player.Name == search.Text && player.Team != null)
                    {
                        for (int i = 0; i < lineup.Players.Count; i++)
                        {
                            if (lineup.Players[i].Name == "" && (lineup.Players[i].Position == player.Position || lineup.Players[i].Position == "Bench"))
                            {
                                lineup.Players[i].Name = player.Name;
                                lineup.Players[i].Team = player.Team;
                                if (lineup.Players[i].Position == "Bench")
                                {
                                    lineup.Players[i].Position = "Bench: " + player.Position;
                                }
                                else
                                {
                                    lineup.Players[i].Position = player.Position;
                                }
                                lineup.Players[i].Value = player.Value;
                                lineup.Players[i].Rank = player.Rank;
                                break;
                            }
                        }
                        this.playerList.Items.Refresh();
                        break;
                    }
                }
            }
        }
    }
}
