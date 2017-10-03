using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
    public class MatchLink
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public CountryLink CountryLink { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Url}";
        }
    }
}
