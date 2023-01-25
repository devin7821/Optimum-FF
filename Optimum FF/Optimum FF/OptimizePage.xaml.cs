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
        public OptimizePage()
        {            
            InitializeComponent();

            //Lineup lineup = new Lineup();
            //lineup.Players = new List<Player>();



            //lineup.Players.Add(masterList.Players[0]);
            //lineup.Players.Add(masterList.Players[40]);
            //lineup.Players.Add(masterList.Players[80]);
            //lineup.Players.Add(masterList.Players[120]);
            //lineup.Players.Add(masterList.Players[121]);
            //lineup.Players.Add(masterList.Players[200]);

            for (int i = 0; i < lineup.Settings.TotalCount; i++)
            {
                ListBoxItem item = new ListBoxItem();
                TextBlock playerBlock = new TextBlock();
                //playerBlock.Text = player.Name;
                //playerBlock.Text += player?.Team?.Name;
                //item.Content = playerBlock;
                //Add to list
                playerList.Items.Add(item);
            }
            var players = new string[masterList.Players.Count()];
            for (int i = 0; i < players.Count(); i++)
            {
                players[i] = masterList.Players[i].Name;
            }
            search.ItemsSource = players;
            //lineup.Display();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MainMenu());
        }

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            bool found = false;
            foreach(var player in masterList.Players)
            {
                if (found == false && player.Name == search.Text)
                {
                    TextBlock playerBlock = new TextBlock();
                    playerBlock.Text = player.Name;
                    playerBlock.Text += " ";
                    playerBlock.Text += player.Team.Name;
                    string addString = player.Name + " " + player.Team.Name;
                    for (int i = 0; i < playerList.Items.Count; i++)
                    {
                        //var content = ((ListBoxItem)playerList.Items[i]).Content;
                        //var name = content.ToString();
                        if (((ListBoxItem)playerList.Items[i]).Content != null)
                        {
                            if (((ListBoxItem)playerList.Items[i]).Content.ToString() == addString)
                            {
                                return;
                            }
                        }
                        if (found == false)
                        {
                            ListBoxItem newItem = new ListBoxItem();
                            newItem.Content = playerBlock;
                            playerList.Items.Insert(i, newItem);
                            found = true;
                        }
                    }
                    lineup.Players.Add(player);
                    //Remove Offensive players from the DP list in python
                    //ListBoxItem item = new ListBoxItem();
                    //TextBlock playerBlock = new TextBlock();
                    //playerBlock.Text = player.Name;
                    //playerBlock.Text += " ";
                    //playerBlock.Text += player.Team.Name;
                    //item.Content = playerBlock;
                    //playerList.Items.Add(item);
                }
            }
        }
    }
}
