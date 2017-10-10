using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TennisOddsScrapper.BL;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.Tests
{
    [TestClass]
    public class OddsScrapperTests
    {
        [TestMethod]
        public void TestLogIn()
        {
            OddsScrapper bl = new OddsScrapper();
            bl.LogIn();
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

    }
}
