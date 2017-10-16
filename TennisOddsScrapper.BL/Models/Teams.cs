using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
    public class Teams
    {
        public int TeamsId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public virtual List<OddValue> OddValues { get; set; }
    }
}
