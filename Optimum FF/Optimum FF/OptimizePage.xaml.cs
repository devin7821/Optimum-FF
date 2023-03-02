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
using System.IO;
using CsvHelper;
using System.Globalization;
using Microsoft.Win32;

namespace Optimum_FF
{
    /// <summary>
    /// Interaction logic for OptimizePage.xaml
    /// </summary>
    public partial class OptimizePage : Page
    {
        PlayerMasterList masterList = new PlayerMasterList();
        Lineup lineup;
        Player[] trades = new Player[4];
        List<Player> qbsList = new List<Player>();
        List<Player> wrsList = new List<Player>();
        List<Player> rbsList = new List<Player>();
        List<Player> tesList = new List<Player>();
        List<Player> ksList = new List<Player>();
        List<Player> defsList = new List<Player>();
        List<Player> dpsList = new List<Player>();
        double? qbValue = 0;
        double? wrValue = 0;
        double? rbValue = 0;
        double? teValue = 0;

        public OptimizePage(Settings settings)
        {
            InitializeComponent();

            lineup = new Lineup(settings);
            this.playerList.ItemsSource = lineup.Players;
            for (int k = 0; k < 4; k++)
            {
                trades[k] = new Player();
                trades[k].Name = "TradeSlot";
            }
            this.tradesList.ItemsSource = trades;


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
                while (j < lineup.Settings.FlexCount)
                {
                    lineup.Players[i + j].Position = "Flex";
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
        public OptimizePage(Lineup Lineup)
        {
            InitializeComponent();

            lineup = Lineup;
            this.playerList.ItemsSource = lineup.Players;
            for (int k = 0; k < 4; k++)
            {
                trades[k] = new Player();
                trades[k].Name = "TradeSlot";
            }
            this.tradesList.ItemsSource = trades;

            playerList.Items.Refresh();
            tradesList.Items.Refresh();

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
                                if (player.Name.Length > 3)
                                {
                                    var names = player.Name.Split(' ');
                                    lineup.Players[i].Name = names[1] + " " + names[0];
                                    lineup.Players[i].Team = player.Team;
                                }
                                else
                                {
                                    lineup.Players[i].Name = player.Name;
                                }
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

        private void OptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var player in masterList.Players) if (player.Name.Length > 3)
                {
                    var names = player.Name.Split(' ');
                    player.Name = (names[1] + " " + names[0]);
                }
            RankQBs();
            RankWRs();
            RankRBs();
            RankTEs();
            RankDEFs();
            RankDPs();
            RankKs();
            GetTrades();
            foreach (var player in masterList.Players) if (player.Name.Length > 3)
                {
                    var names = player.Name.Split(' ');
                    player.Name = (names[1] + " " + names[0]);
                }
        }

        private void RankQBs()
        {
            List<Player> qbs;
            qbs = masterList.Players.FindAll(x => x.Position == "QB");

            foreach (var player in qbs)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM QBs WHERE Player=@Player";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player", player.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Player"].ToString();

                            //Check if player is null
                            if (name == player.Name)
                            {
                                double value = 0;

                                int games = (int)dr["Games"];
                                double att = (int)dr["Att"];
                                double tds = (int)dr["TDs"];
                                double interceptions = (int)dr["Int"];
                                double ypg = (double)dr["YPG"];

                                value = (att / games) * .1;
                                value += ((tds / games) * 6);
                                value -= ((interceptions / games) * 2);
                                value += (ypg / 20);

                                player.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            qbs = qbs.OrderBy(x => x.Value).ToList();
            for (int i = qbs.Count() - 1; i >= 0; i--)
            {
                qbs[i].Rank = qbs.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("QB"))
                {
                    player.Value = qbs.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = qbs.Find(x => x.Name.Contains(player.Name)).Rank;
                    qbValue += player.Value;
                    qbsList.Add(player);
                }
            for (int i = 0; i < lineup.Settings.QBCount; i++)
            {
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: QB"))
                    {
                        if (player.Rank < lineup.Players[i].Rank)
                        {
                            player.Position = "QB";
                            lineup.Players[i].Position = "Bench: QB";
                            lineup.Swap(player, lineup.Players[i]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void RankWRs()
        {
            List<Player> wrs;
            wrs = masterList.Players.FindAll(x => x.Position == "WR");

            foreach (var player in wrs)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM WRs WHERE Player=@Player";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player", player.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Player"].ToString();

                            //Check if player is null
                            if (name == player.Name)
                            {
                                double value = 0;

                                int games = (int)dr["Games"];
                                double rcpg = (double)dr["RCPG"];
                                double rctds = (int)dr["RCTDs"];
                                double rutds = (int)dr["RUTDs"];
                                double fmb = (int)dr["RCFmb"];
                                if (fmb == 0)
                                {
                                    fmb = (int)dr["RUFmb"];
                                }
                                double rcypg = (double)dr["RCYPG"];
                                double ruypg = (double)dr["RUYPG"];

                                if (lineup.Settings.LeagueType == "PPR")
                                {
                                    value += rcpg;
                                }
                                value += (((rctds + rutds) / games) * 6);
                                value -= ((fmb / games) * 2);
                                value += ((rcypg + ruypg) / 10);

                                player.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            wrs = wrs.OrderBy(x => x.Value).ToList();
            for (int i = wrs.Count() - 1; i >= 0; i--)
            {
                wrs[i].Rank = wrs.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("WR"))
                {
                    player.Value = wrs.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = wrs.Find(x => x.Name.Contains(player.Name)).Rank;
                    wrValue += player.Value;
                    wrsList.Add(player);
                }
            for (int i = 0; i < lineup.Settings.WRCount; i++)
            {
                int currentPlayer = lineup.Settings.QBCount + i;
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: WR"))
                    {
                        if (player.Rank < lineup.Players[currentPlayer].Rank)
                        {
                            player.Position = "WR";
                            lineup.Players[currentPlayer].Position = "Bench: WR";
                            lineup.Swap(player, lineup.Players[currentPlayer]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void RankRBs()
        {
            List<Player> rbs;
            rbs = masterList.Players.FindAll(x => x.Position == "RB");

            foreach (var player in rbs)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM RBs WHERE Player=@Player";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player", player.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Player"].ToString();

                            //Check if player is null
                            if (name == player.Name)
                            {
                                double value = 0;

                                int games = (int)dr["Games"];
                                double rcpg = (double)dr["RCPG"];
                                double rctds = (int)dr["RCTDs"];
                                double rutds = (int)dr["RUTDs"];
                                double fmb = (int)dr["RCFmb"];
                                if (fmb == 0)
                                {
                                    fmb = (int)dr["RUFmb"];
                                }
                                double rcypg = (double)dr["RCYPG"];
                                double ruypg = (double)dr["RUYPG"];

                                if (lineup.Settings.LeagueType == "PPR")
                                {
                                    value += rcpg;
                                }
                                value += (((rctds + rutds) / games) * 6);
                                value -= ((fmb / games) * 2);
                                value += ((rcypg + ruypg) / 10);

                                player.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            rbs = rbs.OrderBy(x => x.Value).ToList();
            for (int i = rbs.Count() - 1; i >= 0; i--)
            {
                rbs[i].Rank = rbs.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("RB"))
                {
                    player.Value = rbs.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = rbs.Find(x => x.Name.Contains(player.Name)).Rank;
                    rbValue += player.Value;
                    rbsList.Add(player);
                }
            for (int i = 0; i < lineup.Settings.RBCount; i++)
            {
                int currentPlayer = lineup.Settings.QBCount + lineup.Settings.WRCount + i;
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: RB"))
                    {
                        if (player.Rank < lineup.Players[currentPlayer].Rank)
                        {
                            player.Position = "RB";
                            lineup.Players[currentPlayer].Position = "Bench: RB";
                            lineup.Swap(player, lineup.Players[currentPlayer]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void RankTEs()
        {
            List<Player> tes;
            tes = masterList.Players.FindAll(x => x.Position == "TE");

            foreach (var player in tes)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM TEs WHERE Player=@Player";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player", player.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Player"].ToString();

                            //Check if player is null
                            if (name == player.Name)
                            {
                                double value = 0;

                                int games = (int)dr["Games"];
                                double rcpg = (double)dr["RCPG"];
                                double rctds = (int)dr["RCTDs"];
                                double fmb = (int)dr["RCFmb"];
                                double rcypg = (double)dr["RCYPG"];

                                if (lineup.Settings.LeagueType == "PPR")
                                {
                                    value += rcpg;
                                }
                                value += ((rctds / games) * 6);
                                value -= ((fmb / games) * 2);
                                value += (rcypg / 10);

                                player.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            tes = tes.OrderBy(x => x.Value).ToList();
            for (int i = tes.Count() - 1; i >= 0; i--)
            {
                tes[i].Rank = tes.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("TE"))
                {
                    player.Value = tes.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = tes.Find(x => x.Name.Contains(player.Name)).Rank;
                    teValue += player.Value;
                    tesList.Add(player);
                }
            for (int i = 0; i < lineup.Settings.TECount; i++)
            {
                int currentPlayer = lineup.Settings.QBCount + lineup.Settings.WRCount + lineup.Settings.RBCount + i;
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: TE"))
                    {
                        if (player.Rank < lineup.Players[currentPlayer].Rank)
                        {
                            player.Position = "TE";
                            lineup.Players[currentPlayer].Position = "Bench: TE";
                            lineup.Swap(player, lineup.Players[currentPlayer]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void RankKs()
        {
            List<Player> ks;
            ks = masterList.Players.FindAll(x => x.Position == "K");

            foreach (var player in ks)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM Ks WHERE Player=@Player";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player", player.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Player"].ToString();

                            //Check if player is null
                            if (name == player.Name)
                            {
                                double value = 0;

                                double games = (int)dr["Games"];
                                double twfga = (int)dr["TwentyFGA"];
                                double twfgm = (int)dr["TwentyFGM"];
                                double thfg = (int)dr["ThirtyFG"];
                                double frfg = (int)dr["FourtyFG"];
                                double fvfg = (int)dr["FiftyFG"];
                                double xp = (int)dr["XP"];

                                if (twfgm / twfga > 1)
                                {
                                    value -= ((twfga / twfgm) / games) * 2;
                                }

                                value += (twfgm / games) * 3;
                                value += (thfg / games) * 3;
                                value += (frfg / games) * 4;
                                value += (fvfg / games) * 5;
                                value += (xp / games);

                                player.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            ks = ks.OrderBy(x => x.Value).ToList();
            for (int i = ks.Count() - 1; i >= 0; i--)
            {
                ks[i].Rank = ks.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("K"))
                {
                    player.Value = ks.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = ks.Find(x => x.Name.Contains(player.Name)).Rank;
                }
            for (int i = 0; i < lineup.Settings.KCount; i++)
            {
                int currentPlayer = lineup.Settings.QBCount + lineup.Settings.WRCount + lineup.Settings.RBCount + lineup.Settings.TECount + i;
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: K"))
                    {
                        if (player.Rank < lineup.Players[currentPlayer].Rank)
                        {
                            player.Position = "K";
                            lineup.Players[currentPlayer].Position = "Bench: K";
                            lineup.Swap(player, lineup.Players[currentPlayer]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void RankDEFs()
        {
            List<Team> defs;
            defs = masterList.Teams;

            foreach (var team in defs)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM Teams WHERE Team=@Team";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Team", team.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Team"].ToString();

                            //Check if player is null
                            if (name == team.Name)
                            {
                                double value = 0;

                                int games = (int)dr["Games"];
                                double ptd = (int)dr["PTD"];
                                double rtd = (int)dr["RTD"];
                                double to = (int)dr["Turnovers"];
                                double interceptions = (int)dr["Int"];
                                double fumbles = to - interceptions;

                                if (((ptd + rtd) / games) > 2)
                                {
                                    value -= (((ptd + rtd) / games) * 2);
                                }
                                else
                                {
                                    value += 15;
                                }
                                double temp = (to / games) * 2.0;
                                value += ((to / games) * 2);

                                team.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            defs = defs.OrderBy(x => x.Value).ToList();
            for (int i = defs.Count() - 1; i >= 0; i--)
            {
                defs[i].Rank = defs.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("DEF"))
                {
                    player.Value = defs.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = defs.Find(x => x.Name.Contains(player.Name)).Rank;
                }
            for (int i = 0; i < lineup.Settings.DEFCount; i++)
            {
                int currentPlayer = lineup.Settings.QBCount + lineup.Settings.WRCount + lineup.Settings.RBCount + lineup.Settings.TECount + lineup.Settings.KCount + i;
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: DEF"))
                    {
                        if (player.Rank < lineup.Players[currentPlayer].Rank)
                        {
                            player.Position = "DEF";
                            lineup.Players[currentPlayer].Position = "Bench: DEF";
                            lineup.Swap(player, lineup.Players[currentPlayer]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void RankDPs()
        {
            List<Player> dps;
            dps = masterList.Players.FindAll(x => x.Position == "DP");

            foreach (var player in dps)
            {
                //Create SQL connection
                string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var sql = "SELECT * FROM DPs WHERE Player=@Player";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Player", player.Name);
                        SqlDataReader dr = cmd.ExecuteReader();

                        //Read QB Data
                        while (dr.Read())
                        {
                            //Get Player info
                            string name = dr["Player"].ToString();

                            //Check if player is null
                            if (name == player.Name)
                            {
                                double value = 0;

                                int games = (int)dr["Games"];
                                double interceptions = (int)dr["Int"];
                                double ff = (int)dr["FF"];
                                double fr = (int)dr["FR"];
                                double sk = (double)dr["Sk"];
                                double solo = (int)dr["Solo"];

                                value += ((interceptions + ff) / games) * 2;
                                value += ((fr + sk + solo) / games);

                                player.Value = Math.Round(value, 3);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            dps = dps.OrderBy(x => x.Value).ToList();
            for (int i = dps.Count() - 1; i >= 0; i--)
            {
                dps[i].Rank = dps.Count() - i;
            }
            foreach (var player in lineup.Players) if (player.Position.EndsWith("DP"))
                {
                    player.Value = dps.Find(x => x.Name.Contains(player.Name)).Value;
                    player.Rank = dps.Find(x => x.Name.Contains(player.Name)).Rank;
                }
            for (int i = 0; i < lineup.Settings.TECount; i++)
            {
                int currentPlayer = lineup.Settings.QBCount + lineup.Settings.WRCount + lineup.Settings.RBCount + lineup.Settings.TECount + lineup.Settings.KCount + lineup.Settings.DEFCount + i;
            restart:
                foreach (var player in lineup.Players) if (player.Position.Contains("Bench: DP"))
                    {
                        if (player.Rank < lineup.Players[currentPlayer].Rank)
                        {
                            player.Position = "DP";
                            lineup.Players[currentPlayer].Position = "Bench: DP";
                            lineup.Swap(player, lineup.Players[currentPlayer]);
                            goto restart;
                        }
                    }
            }
            playerList.ItemsSource = lineup.Players;
            playerList.Items.Refresh();
        }

        private void GetTrades()
        {
            double?[] arr = new double?[] { qbValue, wrValue, rbValue, teValue };
            Array.Sort(arr);
            qbsList = qbsList.OrderBy(x => x.Value).ToList();
            wrsList = wrsList.OrderBy(x => x.Value).ToList();
            rbsList = rbsList.OrderBy(x => x.Value).ToList();
            tesList = tesList.OrderBy(x => x.Value).ToList();
            if (arr[arr.Length - 1] == qbValue || arr[arr.Length - 2] == qbValue)
            {
                Player receive = new Player();
                Player send = qbsList[0];
                string trade;
                if (arr[arr.Length - 1] == qbValue)
                {
                    trade = "first";
                }
                else
                {
                    trade = "second";
                }

                if ((arr[0] == wrValue && trade == "first") || (arr[1] == wrValue && trade == "second"))
                {
                    foreach (Player wr in masterList.Players.FindAll(x => x.Position == "WR"))
                    {
                        if (wrsList.Find(x => x.Name == wr.Name) == null)
                        {
                            double? diff = send.Value - wr.Value;
                            if (receive.Value == 0)
                            {
                                receive = wr;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = wr;
                                }
                            }
                        }
                    }      
                }
                if ((arr[0] == rbValue && trade == "first") || (arr[1] == rbValue && trade == "second"))
                {
                    foreach (Player rb in masterList.Players.FindAll(x => x.Position == "RB"))
                    {
                        if (rbsList.Find(x => x.Name == rb.Name) == null)
                        {
                            double? diff = send.Value - rb.Value;
                            if (receive.Value == 0)
                            {
                                receive = rb;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = rb;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == teValue && trade == "first") || (arr[1] == teValue && trade == "second"))
                {
                    foreach (Player te in masterList.Players.FindAll(x => x.Position == "TE"))
                    {
                        if (tesList.Find(x => x.Name == te.Name) == null)
                        {
                            double? diff = send.Value - te.Value;
                            if (receive.Value == 0)
                            {
                                receive = te;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = te;
                                }
                            }
                        }
                    }
                }
                send.Position = "Send: " + send.Position;
                receive.Position = "Receive: " + receive.Position;
                if (trade == "first")
                {
                    trades[0] = send;
                    trades[1] = receive;
                }
                else
                {
                    trades[2] = send;
                    trades[3] = receive;
                }
            }
            if (arr[arr.Length - 1] == wrValue || arr[arr.Length - 2] == wrValue)
            {
                Player receive = new Player();
                Player send = wrsList[0];
                string trade;
                if (arr[arr.Length - 1] == wrValue)
                {
                    trade = "first";
                }
                else
                {
                    trade = "second";
                }

                if ((arr[0] == qbValue && trade == "first") || (arr[1] == qbValue && trade == "second"))
                {
                    foreach (Player qb in masterList.Players.FindAll(x => x.Position == "QB"))
                    {
                        if (qbsList.Find(x => x.Name == qb.Name) == null)
                        {
                            double? diff = send.Value - qb.Value;
                            if (receive.Value == 0)
                            {
                                receive = qb;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = qb;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == rbValue && trade == "first") || (arr[1] == rbValue && trade == "second"))
                {
                    foreach (Player rb in masterList.Players.FindAll(x => x.Position == "RB"))
                    {
                        if (rbsList.Find(x => x.Name == rb.Name) == null)
                        {
                            double? diff = send.Value - rb.Value;
                            if (receive.Value == 0)
                            {
                                receive = rb;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = rb;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == teValue && trade == "first") || (arr[1] == teValue && trade == "second"))
                {
                    foreach (Player te in masterList.Players.FindAll(x => x.Position == "TE"))
                    {
                        if (tesList.Find(x => x.Name == te.Name) == null)
                        {
                            double? diff = send.Value - te.Value;
                            if (receive.Value == 0)
                            {
                                receive = te;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = te;
                                }
                            }
                        }
                    }
                }
                send.Position = "Send: " + send.Position;
                receive.Position = "Receive: " + receive.Position;
                if (trade == "first")
                {
                    trades[0] = send;
                    trades[1] = receive;
                }
                else
                {
                    trades[2] = send;
                    trades[3] = receive;
                }
            }
            if (arr[arr.Length - 1] == rbValue || arr[arr.Length - 2] == rbValue)
            {
                Player receive = new Player();
                Player send = rbsList[0];
                string trade;
                if (arr[arr.Length - 1] == rbValue)
                {
                    trade = "first";
                }
                else
                {
                    trade = "second";
                }

                if ((arr[0] == wrValue && trade == "first") || (arr[1] == wrValue && trade == "second"))
                {
                    foreach (Player wr in masterList.Players.FindAll(x => x.Position == "WR"))
                    {
                        if (wrsList.Find(x => x.Name == wr.Name) == null)
                        {
                            double? diff = send.Value - wr.Value;
                            if (receive.Value == 0)
                            {
                                receive = wr;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = wr;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == qbValue && trade == "first") || (arr[1] == qbValue && trade == "second"))
                {
                    foreach (Player qb in masterList.Players.FindAll(x => x.Position == "QB"))
                    {
                        if (qbsList.Find(x => x.Name == qb.Name) == null)
                        {
                            double? diff = send.Value - qb.Value;
                            if (receive.Value == 0)
                            {
                                receive = qb;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = qb;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == teValue && trade == "first") || (arr[1] == teValue && trade == "second"))
                {
                    foreach (Player te in masterList.Players.FindAll(x => x.Position == "TE"))
                    {
                        if (tesList.Find(x => x.Name == te.Name) == null)
                        {
                            double? diff = send.Value - te.Value;
                            if (receive.Value == 0)
                            {
                                receive = te;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = te;
                                }
                            }
                        }
                    }
                }
                send.Position = "Send: " + send.Position;
                receive.Position = "Receive: " + receive.Position;
                if (trade == "first")
                {
                    trades[0] = send;
                    trades[1] = receive;
                }
                else
                {
                    trades[2] = send;
                    trades[3] = receive;
                }
            }
            if (arr[arr.Length - 1] == teValue || arr[arr.Length - 2] == teValue)
            {
                Player receive = new Player();
                Player send = tesList[0];
                string trade;
                if (arr[arr.Length - 1] == teValue)
                {
                    trade = "first";
                }
                else
                {
                    trade = "second";
                }

                if ((arr[0] == wrValue && trade == "first") || (arr[1] == wrValue && trade == "second"))
                {
                    foreach (Player wr in masterList.Players.FindAll(x => x.Position == "WR"))
                    {
                        if (wrsList.Find(x => x.Name == wr.Name) == null)
                        {
                            double? diff = send.Value - wr.Value;
                            if (receive.Value == 0)
                            {
                                receive = wr;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = wr;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == qbValue && trade == "first") || (arr[1] == qbValue && trade == "second"))
                {
                    foreach (Player qb in masterList.Players.FindAll(x => x.Position == "QB"))
                    {
                        if (qbsList.Find(x => x.Name == qb.Name) == null)
                        {
                            double? diff = send.Value - qb.Value;
                            if (receive.Value == 0)
                            {
                                receive = qb;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = qb;
                                }
                            }
                        }
                    }
                }
                if ((arr[0] == rbValue && trade == "first") || (arr[1] == rbValue && trade == "second"))
                {
                    foreach (Player rb in masterList.Players.FindAll(x => x.Position == "RB"))
                    {
                        if (rbsList.Find(x => x.Name == rb.Name) == null)
                        {
                            double? diff = send.Value - rb.Value;
                            if (receive.Value == 0)
                            {
                                receive = rb;
                            }
                            else if ((send.Value - receive.Value) > diff)
                            {
                                if (diff > 0)
                                {
                                    receive = rb;
                                }
                            }
                        }
                    }
                }
                send.Position = "Send: " + send.Position;
                receive.Position = "Receive: " + receive.Position;
                if (trade == "first")
                {
                    trades[0] = send;
                    trades[1] = receive;
                }
                else
                {
                    trades[2] = send;
                    trades[3] = receive;
                }
            }
            tradesList.Items.Refresh();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new SettingsPage());
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV Spreadsheet|*.csv";
            dialog.Title = "Save Settings";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                FileStream fs = (FileStream)dialog.OpenFile();

                string columns = "QBCount,WRCount,RBCount,TECount,FlexCount,KCount,DEFCount,DPCount,BenchCount,TotalCount,LeagueType\n";
                fs.Write(Encoding.ASCII.GetBytes(columns), 0, ASCIIEncoding.ASCII.GetByteCount(columns));
                string input = lineup.Settings.QBCount + "," + lineup.Settings.WRCount + "," + lineup.Settings.RBCount + "," +
                    lineup.Settings.TECount + "," + lineup.Settings.FlexCount + "," + lineup.Settings.KCount + "," + 
                    lineup.Settings.DEFCount + "," + lineup.Settings.DPCount + "," + lineup.Settings.BenchCount + "," + 
                    lineup.Settings.TotalCount + "," + lineup.Settings.LeagueType + "\n";
                fs.Write(Encoding.ASCII.GetBytes(input), 0, ASCIIEncoding.ASCII.GetByteCount(input));
                foreach (var player in lineup.Players)
                {
                    input = player.Name + "," + player.Position + "," + player.Team.Name + "\n";
                    fs.Write(Encoding.ASCII.GetBytes(input), 0, ASCIIEncoding.ASCII.GetByteCount(input));
                }
                fs.Close();
            }
        }
    }
}
