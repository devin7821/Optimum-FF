﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Optimum_FF
{
    public class Player
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public Team? Team { get; set; }
        public float? Value { get; set; }
        public int? Rank { get; set; }

       /* public Player(string name, string position, Team team, float value)
        {
            Name = name;
            Position = position;
            Team = team;
            Value = value;
        }*/

        //public ListBoxItem Display()
        //{
        //    ListBoxItem item = new ListBoxItem();
        //    TextBlock playerBlock = new TextBlock();
        //    playerBlock.Text = Name;
        //    //playerBlock.Text += (" TDs: " + tds);
        //    //playerBlock.Text += (" Int: " + interceptions);
        //    item.Content = playerBlock;
        //    //Add to list
        //    //playerList.Items.Add(item);
        //    Debug.WriteLine(Name);

        //    return (item);
        //}
    }
}