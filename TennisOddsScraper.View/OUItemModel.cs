using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScraper.View
{
    public class OUItemModel
    {
        public int ID { get; set; }

        //public DateTime Date { get; set; }

        public string Country { get; set; }

        public string Cup { get; set; }

        public string Mutch { get; set; }

        public string Handicap { get; set; }

        public float Av1 { get; set; }

        public float Av2 { get; set; }

        public float AvPayout { get; set; }

        public float H1 { get; set; }

        public float H2 { get; set; }

        public float HPayout { get; set; }
    }
}
