using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimum_FF
{
    public class Settings
    {
        public string LeagueType { get; set; }
        public int TotalCount { get; set; }
        public int QBCount { get; set; }
        public int WRCount { get; set; }
        public int RBCount { get; set; }
        public int TECount { get; set; }
        public int KCount { get; set; }
        public int DEFCount { get; set; }
        public int DPCount { get; set; }
        public int FlexCount { get; set; }
        public int BenchCount { get; set; }

        public Settings()
        {
            LeagueType = "Standard";
            QBCount = 1;
            WRCount = 3;
            RBCount = 2;
            TECount = 1;
            FlexCount = 0;
            KCount = 1;
            DEFCount = 1;
            DPCount = 1;
            BenchCount = 6;
            TotalCount = 15;
        }
    }
}
