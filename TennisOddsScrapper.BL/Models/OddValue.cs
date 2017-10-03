using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
    public class OddValue
    {
        public string Tab { get; set; }
        public string Average1 { get; set; }
        public string Average2 { get; set; }
        public string AveragePayout { get; set; }
        public string Highest1 { get; set; }
        public string Highest2 { get; set; }
        public string HighestPayout { get; set; }

        public DuelLink DuelLink { get; set; }
    }
}
