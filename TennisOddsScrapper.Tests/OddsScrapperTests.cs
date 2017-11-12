using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TennisOddsScrapper.BL;
using TennisOddsScrapper.BL.Models;
using TennisOddsScrapper.BL.XMLSerializator;


namespace TennisOddsScrapper.Tests
{
    [TestClass]
    public class OddsScrapperTests
    {
        [TestMethod]
        public void TestLogIn()
        {
            OddsScrapper bl = new OddsScrapper();
            bl.LogIn("", "");
            Thread.Sleep(5000);
            bl.StartScraping();
        }

        [TestMethod]
        public void TestDbContext()
        {
            DuelLink duelLink = new DuelLink() { MatchLink = null, Url = "dfdf", DuelLinkId = 6, Name = "Name" };
            OddsDbContext db = new OddsDbContext();
            db.DuelLinks.Add(duelLink);
        }

        [TestMethod]
        public void XML()
        {
            ISerializator serializator = new Serializator();
            serializator.Path = @"D:\UpWork\Xml.xml";
            OddSerializationList list = serializator.TransformData(new List<OddValue>()
            {
               new OddValue()
               {
                   Average1 = "av1",
                   Average2 =  "av2",
                   AveragePayout = "avPay",
                   GameValue = "svsv",
                   Highest1 = "nfkdnv",
                   Highest2 = "mlvkdm",
                   HighestPayout = "mkvm",
                   Tab = "Home/Away"
               },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Asian Handicap"
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Over/Under"
                }
            });

            serializator.Serialize(list);
        }

        [TestMethod]
        public void ArrayLinkTest()
        {
            var oddsValues = new OddValue[]
            {
                new OddValue()
                {
                    DuelLink = new DuelLink()
                    {
                        DuelLinkId = 0,
                        Name = "test",
                        Url = "google.com"
                    }
                },
                new OddValue()
                {
                    DuelLink = new DuelLink()
                    {
                        DuelLinkId = 1,
                        Name = "test1",
                        Url = "google.com"
                    }
                },
                new OddValue()
                {
                    DuelLink = new DuelLink()
                    {
                        DuelLinkId = 2,
                        Name = "test",
                        Url = "google.com"
                    }
                }
            };
            var duelLinks = oddsValues.Select(x => x.DuelLink).Distinct().ToArray();

            
        }

        [TestMethod]
        public void khsdvcdysvb()
        {
            Serializator serializator = new Serializator();

            List<OddValue> list = new List<OddValue>()
            {
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Home/Away",
                    Group = 0
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Asian Handicap",
                    Group = 0
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Over/Under",
                    Group = 0
                },
                new OddValue()
                    {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Home/Away",
                    Group = 1
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Asian Handicap",
                    Group = 1
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 =  "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Over/Under",
                    Group = 2
                }
            };
            OddSerializationList z = serializator.TransformData(list);
            serializator.Path = @"D:\UpWork\Xml.xml";
            serializator.Serialize(z);

        }

    }
}
