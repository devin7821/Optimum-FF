using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Optimum_FF
{
    public class PlayerMasterList
    {
        List<Player> Players { get; set; }
        List<Team> Teams { get; set; }

        public PlayerMasterList()
        {
            Players = new List<Player>();
            Teams = new List<Team>();
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
                            player.Team = GetTeam(dr["Team"].ToString());

                            lineup.Players.Add(lplayer);

                        }
                    }
                    dr.Close();
                }
            }
        }

        private Team GetTeam(string name)
        {
            Team team = new Team();

            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM Teams";
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
                            player.Team = GetTeam(dr["Team"].ToString());

                            lineup.Players.Add(lplayer);

                        }
                    }
                    dr.Close();
                }
            }
        }

        private void GetQBs(List<Player> players)
        {

        }
    }
}
