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
            //playerList = new ListBox();
            string connectionString = @"SERVER=.\SQLEXPRESS;DATABASE=tempdb;User ID=mainuser;Password=123";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;
                var sql = "SELECT * FROM QBs";
                cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    string player = dr["Player"].ToString();
                    string tds = dr["TDs"].ToString();
                    string interceptions = dr["Int"].ToString();
                    if (player != null)
                    {
                        ListBoxItem item = new ListBoxItem();
                        TextBlock playerBlock = new TextBlock();
                        //Players.Text = string.Join(Environment.NewLine, player);
                        playerBlock.Text = player;
                        playerBlock.Text += (" TDs: " + tds);
                        playerBlock.Text += (" Int: " + interceptions);
                        item.Content = playerBlock;
                        playerList.Items.Add(item);
                    }
                }
            }
        }
    }
}
