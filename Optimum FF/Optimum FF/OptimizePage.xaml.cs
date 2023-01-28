﻿using System;
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

        private void OptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            RankQBs();
            RankWRs();
            RankRBs();
            RankTEs();
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
                                int att = (int)dr["Att"];
                                int tds = (int)dr["TDs"];
                                int interceptions = (int)dr["Int"];
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
                                int rctds = (int)dr["RCTDs"];
                                int rutds = (int)dr["RUTDs"];
                                int fmb = (int)dr["RCFmb"];
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
                                int rctds = (int)dr["RCTDs"];
                                int rutds = (int)dr["RUTDs"];
                                int fmb = (int)dr["RCFmb"];
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
                                int rctds = (int)dr["RCTDs"];
                                int fmb = (int)dr["RCFmb"];
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
    }
}
