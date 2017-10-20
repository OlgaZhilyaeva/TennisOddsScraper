using System;
using System.Xml.Serialization;

namespace TennisOddsScrapper.BL.XMLSerializator
{
    [XmlType("Match")]
    public class OddSerializationModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public string OddsHome { get; set; }
        public string OddsAway { get; set; }
        public string AverOddsHome { get; set; }
        public string AverOddsAway { get; set; }
        public string PayoutAverOddsHomeAway { get; set; }
        public string PayoutOddsHomeAway { get; set; }

        [XmlElement("AHeq-BET")]
        public string AHeq_BET { get; set; }
        public string OddsAHeqHome { get; set; }
        public string OddsAHeqAway { get; set; }
        public string AverOddsAHeqHome { get; set; }
        public string AverOddsAHeqAway { get; set; }
        public string PayoutAverOddsAHeqHomeAway { get; set; }
        public string PayoutOddsAHeqHomeAway { get; set; }

        [XmlElement("OUeq-BET")]
        public string OUeq_BET { get; set; }
        public string OddsOUeqOVER { get; set; }
        public string OddsOUeqUNDER { get; set; }
        public string AverOddsOUeqOVER { get; set; }
        public string AverOddsOUeqUNDER { get; set; }
        public string PayoutAverOddsOUeqOVER { get; set; }
        public string PayoutOddsOUeqUNDER { get; set; }

    }
}
