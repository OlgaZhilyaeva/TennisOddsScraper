using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TennisOddsScrapper.BL.Models
{
    class FindHigh
    {
        public double HighInt { get; set; }

        public IWebElement AverageString { get; set; }

        public IWebElement HighString { get; set; }

        public string GameValue { get; set; }
    }
}
