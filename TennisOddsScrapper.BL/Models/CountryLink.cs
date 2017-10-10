using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
    public class CountryLink: IEquatable<CountryLink>
    {
        public int CountryLinkId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public virtual List<MatchLink> MatchLinks { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Url}";
        }

        public bool Equals(CountryLink other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Url, other.Url) && Equals(MatchLinks, other.MatchLinks);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CountryLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CountryLinkId;
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MatchLinks != null ? MatchLinks.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
