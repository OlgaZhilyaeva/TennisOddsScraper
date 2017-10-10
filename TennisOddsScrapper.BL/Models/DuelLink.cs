using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
     public class DuelLink: IEquatable<DuelLink>
    {
        public int DuelLinkId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public MatchLink MatchLink { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Url}";
        }

        public bool Equals(DuelLink other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DuelLink) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 1244688004;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
