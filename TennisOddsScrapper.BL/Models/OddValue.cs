using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Models
{
    public class OddValue: IEquatable<OddValue>
    {
        public int OddValueId { get; set; }
        public string Tab { get; set; }
        public string Average1 { get; set; }
        public string Average2 { get; set; }
        public string AveragePayout { get; set; }
        public string Highest1 { get; set; }
        public string Highest2 { get; set; }
        public string HighestPayout { get; set; }

        public string GameValue { get; set; }
        public string Date { get; set; }

        public DuelLink DuelLink { get; set; }

        public bool Equals(OddValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(AveragePayout, other.AveragePayout) &&
                   string.Equals(HighestPayout, other.HighestPayout);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OddValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = OddValueId;
                hashCode = (hashCode * 397) ^ (AveragePayout != null ? AveragePayout.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HighestPayout != null ? HighestPayout.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
