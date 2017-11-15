using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL
{
    public class OddsDbContext: DbContext
    {
        public OddsDbContext() 
            :base("OddsData")
        { }

        public DbSet<CountryLink> CountryLinks { get; set; }
        public DbSet<DuelLink> DuelLinks { get; set; }
        public DbSet<MatchLink> MatchLinks { get; set; }
        public DbSet<OddValue> OddValues { get; set; }

    }
}
