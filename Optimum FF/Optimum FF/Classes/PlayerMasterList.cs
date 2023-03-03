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
        // Default constructor
        public PlayerMasterList()
        {
            // Get all players from database
            Players = new List<Player>();
            Teams = GetTeams();
            GetQBs(Players, Teams);
            GetRBs(Players, Teams);
            GetWRs(Players, Teams);
            GetTEs(Players, Teams);
            GetDPs(Players, Teams);
            GetKs(Players, Teams);
            // Put into last-first format
            foreach (var player in Players)
            {
                var names = player.Name.Split(' ');
                player.Name = (names[1] + " " + names[0]);
            }
            // Create all teams
            foreach (var team in Teams)
            {
                Player player = new Player();
                player.Name = team.Name;
                player.Position = "DEF";
                Players.Add(player);
            }
        }
        // Get teams
        private List<Team> GetTeams()
        {
            List<Team> teams = new List<Team>();
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM Teams";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read team data
                    while (dr.Read())
                    {
                        // Get team name
                        string name = dr["Team"].ToString();

                        // Check if teams is null
                        if (name != null)
                        {
                            // Set team info
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
        // Get all QBs
        private void GetQBs(List<Player> players, List<Team> teams)
        {
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM QBs";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read QB Data
                    while (dr.Read())
                    {
                        // Get player name
                        string name = dr["Player"].ToString();                   

                        // Check if player is null
                        if (name != null)
                        {
                            // Get player info
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
        // Get all RBs
        private void GetRBs(List<Player> players, List<Team> teams)
        {
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM RBs";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read RB Data
                    while (dr.Read())
                    {
                        // Get player name
                        string name = dr["Player"].ToString();

                        // Check if player is null
                        if (name != null)
                        {
                            // Get player info
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
        // Get all WRs
        private void GetWRs(List<Player> players, List<Team> teams)
        {
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM WRs";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read WR Data
                    while (dr.Read())
                    {
                        // Get player name
                        string name = dr["Player"].ToString();

                        // Check if player is null
                        if (name != null)
                        {
                            // Get player info
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
        // Get all TEs
        private void GetTEs(List<Player> players, List<Team> teams)
        {
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM TEs";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read TE Data
                    while (dr.Read())
                    {
                        // Get player name
                        string name = dr["Player"].ToString();

                        // Check if player is null
                        if (name != null)
                        {
                            // Get player info
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
        // Get all DPs
        private void GetDPs(List<Player> players, List<Team> teams)
        {
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM DPs";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read DP Data
                    while (dr.Read())
                    {
                        // Get name
                        string name = dr["Player"].ToString();

                        // Check if player is null
                        if (name != null)
                        {
                            // Get player info
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
        // Get all Ks
        private void GetKs(List<Player> players, List<Team> teams)
        {
            // Create SQL connection
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT * FROM Ks";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Read K Data
                    while (dr.Read())
                    {
                        // Get name
                        string name = dr["Player"].ToString();

                        // Check if player is null
                        if (name != null)
                        {
                            // Get player info
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
                            player.Position = "K";
                            players.Add(player);
                        }
                    }
                    dr.Close();
                }
            }
        }
    }
}
