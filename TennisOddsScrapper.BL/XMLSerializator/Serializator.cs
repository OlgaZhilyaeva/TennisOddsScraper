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
            List<IGrouping<int, OddValue>> a = oddsValues.GroupBy(x => x.Group).ToList();
            foreach (var oddsValue in a)
            {
                OddSerializationModel tempModel = new OddSerializationModel();
                foreach (var oddValue in oddsValue)
                {
                    if (oddValue.Tab == "Home/Away")
                    {
                        tempModel.OddsHome = oddValue.Highest1;
                        tempModel.OddsAway = oddValue.Highest2;
                        tempModel.AverOddsHome = oddValue.Average1;
                        tempModel.AverOddsAway = oddValue.Average2;
                        tempModel.PayoutAverOddsHomeAway = oddValue.AveragePayout;
                        tempModel.PayoutOddsHomeAway = oddValue.HighestPayout;
                    }

                    if (oddValue.Tab == "Asian Handicap")
                    {
                        tempModel.AHeq_BET = oddValue.GameValue;
                        tempModel.OddsAHeqHome = oddValue.Highest1;
                        tempModel.OddsAHeqAway = oddValue.Highest2;
                        tempModel.AverOddsAHeqHome = oddValue.Average1;
                        tempModel.AverOddsAHeqAway = oddValue.Average2;
                        tempModel.PayoutAverOddsAHeqHomeAway = oddValue.AveragePayout;
                        tempModel.PayoutOddsAHeqHomeAway = oddValue.HighestPayout;
                    }

                    if (oddValue.Tab == "Over/Under")
                    {
                        tempModel.OUeq_BET = oddValue.GameValue;
                        tempModel.OddsOUeqOVER = oddValue.Highest1;
                        tempModel.OddsOUeqUNDER = oddValue.Highest2;
                        tempModel.AverOddsOUeqOVER = oddValue.Average1;
                        tempModel.AverOddsOUeqUNDER = oddValue.Average2;
                        tempModel.PayoutAverOddsOUeqOVER = oddValue.AveragePayout;
                        tempModel.PayoutOddsOUeqUNDER = oddValue.HighestPayout;
                    }
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
                var xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");

                serializer.Serialize(writer, oddSerialization, xmlnsEmpty);
            }
        }
    }
}
