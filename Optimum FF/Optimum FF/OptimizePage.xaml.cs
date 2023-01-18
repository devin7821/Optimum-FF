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

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for OptimizePage.xaml
    /// </summary>
    public partial class OptimizePage : Page
    {
        public OptimizePage()
        {
            InitializeComponent();

            Lineup lineup = new Lineup();
            lineup.Players = new List<Player>();

            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM QBs";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    //Read QB Data
                    while (dr.Read())
                    {
                        //Get Player info
                        string name = dr["Player"].ToString();
                        
                        //Check if player is null
                        if (name != null)
                        {
                            Player player = new Player();
                            player.Name = name;
                            lineup.Players.Add(player);
                            
                        }
                    }
                    dr.Close();
                }
            }

            foreach (var player in lineup.Players)
            {
                ListBoxItem item = new ListBoxItem();
                TextBlock playerBlock = new TextBlock();
                playerBlock.Text = player.Name;
                //playerBlock.Text += (" TDs: " + tds);
                //playerBlock.Text += (" Int: " + interceptions);
                item.Content = playerBlock;
                //Add to list
                playerList.Items.Add(item);
            }
            //lineup.Display();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MainMenu());
        }
    }
}
