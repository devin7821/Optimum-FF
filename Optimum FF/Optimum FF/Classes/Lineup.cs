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

        public Lineup(Settings settings)
        {
            Settings = settings;
            Players = new List<Player>();
            for (int i = 0; i <= Settings.TotalCount; i++)
            {
                Player player = new Player();
                Players.Add(player);
            }
        }

        public void Swap(Player player1, Player player2)
        {
            var index = Players.IndexOf(player2);
            Players[Players.IndexOf(player1)] = player2;
            Players[index] = player1;
        }

        public List<ListBoxItem> Display()
        {
            List<ListBoxItem> playerListBox = new List<ListBoxItem>();
            foreach (var player in Players)
            {
                playerListBox.Add(player.Display());
            } 
            return playerListBox;
        }
    }
}
