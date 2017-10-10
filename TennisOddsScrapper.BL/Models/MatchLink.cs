using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
    public class MatchLink: IEquatable<MatchLink>
    {
        public int MatchLinkId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public CountryLink CountryLink { get; set; }

        public virtual List<DuelLink> DieDuelLinks{ get; set; }

        public override string ToString()
        {
            return $"{Name}: {Url}";
        }

        public bool Equals(MatchLink other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Url, other.Url) && Equals(CountryLink, other.CountryLink);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MatchLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MatchLinkId;
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CountryLink != null ? CountryLink.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
