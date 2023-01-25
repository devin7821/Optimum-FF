using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Optimum_FF
{
    public class Lineup
    {
        public Settings Settings { get; set; }
        public List<Player> Players { get; set; }

        public Lineup()
        {
            Settings = new Settings();
            Settings.LeagueType = "Standard";
            Settings.QBCount = 1;
            Settings.WRCount = 3;
            Settings.RBCount = 2;
            Settings.TECount = 1;
            Settings.FlexCount = 0;
            Settings.KCount = 1;
            Settings.DEFCount = 1;
            Settings.DPCount = 1;
            Settings.BenchCount = 6;
            Settings.TotalCount = 15;
            Players = new List<Player>();
        }

        //public void Display(ListBox playerListBox)
        //{
        //    //ListBox playerListBox = new ListBox();
        //    foreach (var player in Players)
        //    {              
        //        playerListBox

        //    }
        //}
    }
}
