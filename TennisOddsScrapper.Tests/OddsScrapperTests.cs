using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TennisOddsScrapper.BL;

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
    }
}
