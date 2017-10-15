using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL.XMLSerializator
{
    public class Serializator: ISerializator
    {
        public string Path { get; set; }

        public OddSerializationList TransformData(List<OddValue> oddsValues)
        {

            OddSerializationList temperaryModels = new OddSerializationList();
            foreach (var oddsValue in oddsValues)
            {
                OddSerializationModel tempModel = new OddSerializationModel();

                if (oddsValue.Tab == "Home/Away")
                {
                    tempModel.OddsHome = oddsValue.Highest1;
                    tempModel.OddsAway = oddsValue.Highest2;
                    tempModel.AverOddsHome = oddsValue.Average1;
                    tempModel.AverOddsAway = oddsValue.Average2;
                    tempModel.PayoutAverOddsHomeAway = oddsValue.AveragePayout;
                    tempModel.PayoutOddsHomeAway = oddsValue.HighestPayout;
                }

                if (oddsValue.Tab == "Asian Handicap")
                {
                    tempModel.AHeq_BET = oddsValue.GameValue;
                    tempModel.OddsAHeqHome = oddsValue.Highest1;
                    tempModel.OddsAHeqAway = oddsValue.Highest2;
                    tempModel.AverOddsAHeqHome = oddsValue.Average1;
                    tempModel.AverOddsAHeqAway = oddsValue.Average2;
                    tempModel.PayoutAverOddsHomeAway = oddsValue.AveragePayout;
                    tempModel.PayoutOddsHomeAway = oddsValue.HighestPayout;
                }

                if (oddsValue.Tab == "Over/Under")
                {
                    tempModel.OUeq_BET = oddsValue.GameValue;
                    tempModel.OddsOUeqOVER = oddsValue.Highest1;
                    tempModel.OddsOUeqUNDER = oddsValue.Highest2;
                    tempModel.AverOddsOUeqOVER = oddsValue.Average1;
                    tempModel.AverOddsOUeqUNDER = oddsValue.Average2;
                    tempModel.PayoutAverOddsOUeq = oddsValue.AveragePayout;
                    tempModel.PayoutOddsOUeq = oddsValue.HighestPayout;
                }

                temperaryModels.Add(tempModel);
            }
            return temperaryModels;
        }

        public void Serialize(OddSerializationList oddSerialization)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OddSerializationList));
            using (TextWriter writer = new StreamWriter(Path))
            {
                serializer.Serialize(writer, oddSerialization);
            }
        }
    }
}
