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
                        string player = dr["Player"].ToString();
                        string tds = dr["TDs"].ToString();
                        string interceptions = dr["Int"].ToString();
                        //Check if player is null
                        if (player != null)
                        {
                            //Create and fill data of a new ListBoxItem
                            ListBoxItem item = new ListBoxItem();
                            TextBlock playerBlock = new TextBlock();
                            playerBlock.Text = player;
                            playerBlock.Text += (" TDs: " + tds);
                            playerBlock.Text += (" Int: " + interceptions);
                            item.Content = playerBlock;
                            //Add to list
                            playerList.Items.Add(item);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new MainMenu());
        }
    }
}
