using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TennisOddsScrapper.BL.XMLSerializator
{
    [XmlRoot("Matches", Namespace = "")]
    public class OddSerializationList : List<OddSerializationModel>
    {
        public OddSerializationList()
        {
            Matches = new string[] { "123", "234", "456" };
        }

        public string[] Matches { get; set; }
    }
}