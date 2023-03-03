using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimum_FF
{
    public class Team
    {
        public string? Name { get; set; }
        public string? Opponent { get; set; }
        public double? Value { get; set; }
        public int? Rank { get; set; } 
        // Default constructor
        public Team()
        {
            Name = "";
            Opponent = "";
        }
    }
}
