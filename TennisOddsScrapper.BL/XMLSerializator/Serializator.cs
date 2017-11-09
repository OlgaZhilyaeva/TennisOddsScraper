using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL.XMLSerializator
{
    public class SerializatorStub : ISerializator
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private OddSerializationModel CreateModel()
        {
            return new OddSerializationModel()
            {
                AHeq_BET = RandomString(5),
                AverOddsAHeqAway = RandomString(5),
                AverOddsAHeqHome = RandomString(5),
                AverOddsAway = RandomString(5),
                AverOddsHome = RandomString(5),
                AverOddsOUeqOVER = RandomString(5),
                AverOddsOUeqUNDER = RandomString(5),
                AwayTeam = RandomString(5),
                Date = DateTime.Now.ToShortDateString(),
                HomeTeam = RandomString(5),
                OUeq_BET = RandomString(5),
                OddsAHeqAway = RandomString(5),
                OddsAHeqHome = RandomString(5),
                OddsAway = RandomString(5),
                OddsHome = RandomString(5),
                OddsOUeqOVER = RandomString(5),
                OddsOUeqUNDER = RandomString(5),
                PayoutAverOddsHomeAway = RandomString(5),
                PayoutAverOddsAHeqHomeAway = RandomString(5),
                PayoutAverOddsOUeqOVER = RandomString(5),
                PayoutOddsAHeqHomeAway = RandomString(5),
                PayoutOddsOUeqUNDER = RandomString(5),
                PayoutOddsHomeAway = RandomString(5),
                Id = random.Next(0, 500)
            };
        }

        public string Path { get; set; }
        public OddSerializationList TransformData(List<OddValue> oddsValues)
        {
            OddSerializationList list = new OddSerializationList();
            for (int i = 0; i < 50; i++)
            {
                list.Add(CreateModel());
            }

            return list;
        }

        public void Serialize(OddSerializationList oddSerialization)
        {
            Debug.WriteLine(nameof(Serialize));
        }
    }

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
                    tempModel.HomeTeam = oddValue.TeamsLink.HomeTeam;
                    tempModel.AwayTeam = oddValue.TeamsLink.AwayTeam;
                    tempModel.Date = oddValue.Date;
                    tempModel.Id = oddValue.OddValueId;
                    tempModel.Country = oddValue.DuelLink.MatchLink.CountryLink.Name;
                    tempModel.Match = oddValue.DuelLink.MatchLink.Name;

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
