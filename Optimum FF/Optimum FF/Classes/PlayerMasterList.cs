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
        public List<Player> Players { get; set; }
        public List<Team> Teams { get; set; }

        public PlayerMasterList()
        {
            Players = new List<Player>();
            Teams = GetTeams();
            GetQBs(Players, Teams);
            GetRBs(Players, Teams);
            GetWRs(Players, Teams);
            GetTEs(Players, Teams);
            GetDPs(Players, Teams);
        }

        private List<Team> GetTeams()
        {
            List<Team> teams = new List<Team>();
            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM Teams";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    //Read Opponent Data
                    while (dr.Read())
                    {
                        //Get Player info
                        string name = dr["Team"].ToString();

                        //Check if player is null
                        if (name != null)
                        {
                            Team team = new Team();
                            team.Name = name;
                            teams.Add(team);
                        }
                    }
                    dr.Close();
                }
            }
            return teams;
        }

        private void GetQBs(List<Player> players, List<Team> teams)
        {
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
                            string teamName = dr["Team"].ToString();
                            foreach (var team in teams)
                            {
                                if (team.Name == teamName)
                                {
                                    player.Team = team;
                                }
                            }
                            player.Position = "QB";
                            players.Add(player);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void GetRBs(List<Player> players, List<Team> teams)
        {
            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM RBs";
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
                            string teamName = dr["Team"].ToString();
                            foreach (var team in teams)
                            {
                                if (team.Name == teamName)
                                {
                                    player.Team = team;
                                }
                            }
                            player.Position = "RB";
                            players.Add(player);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void GetWRs(List<Player> players, List<Team> teams)
        {
            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM WRs";
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
                            string teamName = dr["Team"].ToString();
                            foreach (var team in teams)
                            {
                                if (team.Name == teamName)
                                {
                                    player.Team = team;
                                }
                            }
                            player.Position = "WR";
                            players.Add(player);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void GetTEs(List<Player> players, List<Team> teams)
        {
            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM TEs";
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
                            string teamName = dr["Team"].ToString();
                            foreach (var team in teams)
                            {
                                if (team.Name == teamName)
                                {
                                    player.Team = team;
                                }
                            }
                            player.Position = "TE";
                            players.Add(player);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void GetDPs(List<Player> players, List<Team> teams)
        {
            //Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM DPs";
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
                            string teamName = dr["Team"].ToString();
                            foreach (var team in teams)
                            {
                                if (team.Name == teamName)
                                {
                                    player.Team = team;
                                }
                            }
                            player.Position = "DP";
                            players.Add(player);
                        }
                    }
                    dr.Close();
                }
            }
        }
    }
}
