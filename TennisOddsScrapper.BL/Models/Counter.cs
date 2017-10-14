using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TennisOddsScrapper.BL.Models
{
    public class Counter
    {
        public string GameValue { get; set; }
        public int Value { get; set; }
        public IWebElement Link { get; set; }
    }
}
