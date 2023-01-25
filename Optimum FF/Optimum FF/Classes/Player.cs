using System;
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

        public Player()
        {
            Name = "";
            Position = "";
            Team = new Team();
            Value = 0;
        }

        public override string ToString()
        {
            return this.Position + " " + this.Name + " " + this.Team.Name + "\nValue: " + this.Value + " Rank: " + this.Rank;
        }

        public ListBoxItem Display()
        {
            ListBoxItem item = new ListBoxItem();
            TextBlock playerBlock = new TextBlock();
            playerBlock.Text = Name;
            playerBlock.Text += " ";
            playerBlock.Text += Team;
            item.Content = playerBlock;

            return (item);
        }
    }
}
