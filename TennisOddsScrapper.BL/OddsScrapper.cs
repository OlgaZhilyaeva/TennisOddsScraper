using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TennisOddsScrapper.BL.Logger;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL
{
    public class OddsScrapper
    {
        private IWebDriver _driver;
        private Random _random;
        private ILogger _logger = new Logger.Logger();

        private int _attemptsCount = 3;

        public OddsScrapper()
        {
            _driver = new ChromeDriver();
            _random = new Random();
        }

        public void LogIn()
        {
            _driver.Navigate().GoToUrl("http://www.oddsportal.com/login/");
            IWebElement login = _driver.FindElement(By.Id("login-username1"));
            IWebElement password = _driver.FindElement(By.Id("login-password1"));

            login.SendKeys("Statstat1");
            Delay();
            password.SendKeys("Memorex1");
            Delay();
        }

        public void StartScraping()
        {
            _driver.Navigate().GoToUrl("http://www.oddsportal.com/tennis/");
            Delay();

            List<OddValue> oddsValues = new List<OddValue>();
            List<CountryLink> countriesLinks = GetCountriesLinks();

            foreach (var countryLink in countriesLinks)
            {
                _driver.Navigate().GoToUrl(countryLink.Url);
                Delay();

                List<MatchLink> matchLinks = GetMatchesLinks(countryLink);
                foreach (var matchLink in matchLinks)
                {
                    _driver.Navigate().GoToUrl(matchLink.Url);
                    Delay();

                    List<DuelLink> duelsList = GetDuelsLinks(matchLink);
                    foreach (var duelLink in duelsList)
                    {
                        _driver.Navigate().GoToUrl(duelLink.Url);
                        Delay();

                        var oddVal = SetOddValuesHa(duelLink);
                        if(oddVal != null)
                            oddsValues.Add(oddVal);


                        if (NaviagateToTab(duelLink, "Asian Handicap"))
                        {
                            oddVal = SetOddValuesAh(duelLink);
                            if (oddVal != null)
                                oddsValues.Add(oddVal);
                        }

                        if (NaviagateToTab(duelLink, "Over/Under"))
                        {
                            oddVal = SetOddValuesAh(duelLink);
                            if (oddVal != null)
                                oddsValues.Add(oddVal);
                        }


                    }
                }
            }
        }

        private List<CountryLink> GetCountriesLinks()
        {
            return _driver.FindElements(By.CssSelector(".table-main .bfl")).Select(x => new CountryLink()
            {
                Name = x.Text,
                Url = x.GetAttribute("href")
            }).Skip(1).ToList();
        }

        private List<MatchLink> GetMatchesLinks(CountryLink country)
        {
            return _driver.FindElements(By.CssSelector(".table-main tbody>tr>td>a")).Select(x => new MatchLink()
            {
                Name = x.Text,
                Url = x.GetAttribute("href"),
                CountryLink = country
            }).ToList();
        }

        private List<DuelLink> GetDuelsLinks(MatchLink match)
        {
            return _driver.FindElements(By.CssSelector(".table-participant > a")).Select(x => new DuelLink()
            {
                Name = x.Text,
                Url = x.GetAttribute("href"),
                MatchLink = match
            }).ToList();
        }

        private OddValue SetOddValuesHa(DuelLink duelLink)
        {

            IWebElement pathAver = _driver.FindElement(By.CssSelector(".table-main .aver"));
            IWebElement pathHigh = _driver.FindElement(By.CssSelector(".table-main .highest"));

            return new OddValue()
            {
                DuelLink = duelLink,
                Average1 = pathAver.FindElement(By.CssSelector(".right:nth-child(2)")).Text,
                Average2 = pathAver.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                AveragePayout = pathAver.FindElement(By.CssSelector(".aver .center")).Text,
                Highest1 = pathHigh.FindElement(By.CssSelector(".right:nth-child(2)")).Text,
                Highest2 = pathHigh.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                HighestPayout = pathHigh.FindElement(By.CssSelector(".center")).Text,
                Tab = _driver.FindElement(By.CssSelector(".first strong")).Text
            };
        }

        private bool NaviagateToTab(DuelLink duelLink, string tabName)
        {
            IWebElement tab = _driver
                .FindElements(By.CssSelector($"#tab-nav-main ul > li a[title='{tabName}']"))
                .FirstOrDefault(x => x.GetCssValue("display").Contains("none")==false);
            if (tab != null )
            {
                string js = _driver.FindElement(By.CssSelector($"#tab-nav-main ul > li a[title='{tabName}']"))
                    .GetAttribute("onmousedown").Replace("return false;", ""); //goto АН
                IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
                executor.ExecuteScript(js);

                Delay();
                return true;
            }
            else
            {
                return false;
            }
        }

        private IList<IWebElement> SafeTryFindElements(int attemptsCount, string cssSelector)
        {
            for (int i = 0; i < attemptsCount; i++)
            {
                var results = _driver.FindElements(By.CssSelector(".table-container"));
                if (results.Count > 0)
                {
                    return results;
                }
                Delay();
            }

            _logger.Log($"Safe add elemets failed. Selector: {cssSelector}");
            return null;
        }

        [Obsolete]
        private void GotoTabAh(DuelLink duelLink)
        {
            _driver.Navigate().GoToUrl(duelLink.Url);
            string js = _driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Asian Handicap']")).GetAttribute("onmousedown").Replace("return false;", ""); //goto АН
            IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
            executor.ExecuteScript(js);
        }

        [Obsolete]
        private void GoToTabOu(DuelLink duelLink)
        {
            _driver.Navigate().GoToUrl(duelLink.Url);
            _driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Over/Under']")).Click(); //goto O/U
        }

        private OddValue SetOddValuesAh(DuelLink duelLink)
        {
            List<Counter> counters = new List<Counter>();

            IList<IWebElement> containers = SafeTryFindElements(_attemptsCount, ".table-container");

            if (containers == null)
                return null;

            foreach (var container in containers)
            {
                //Delay();

                IWebElement link = container.FindElement(By.CssSelector(".odds-co a")); // SafeTryFindElements(_attemptsCount, ".odds-co a").FirstOrDefault();

                //if (link == null)
                //   continue;

                IWebElement counterElement = container.FindElement(By.CssSelector(".odds-cnt")); //SafeTryFindElements(_attemptsCount, ".odds-cnt").FirstOrDefault();

                //if (counterElement == null)
                //    continue;

                string counterString = counterElement.Text.Replace("(", "")
                    .Replace(")", "");
                int counter = 0;
                if (Int32.TryParse(counterString, out counter))
                {
                    counters.Add(new Counter
                    {
                        Value = counter,
                        Link = link
                    });
                }
            }

            int max = counters.Max(x => x.Value);
            IEnumerable<Counter> maxCounters = counters.Where(x => x.Value == max);

            bool actionsSucceeded = true;

            foreach (var maxCounter in maxCounters)
            {
                actionsSucceeded = DoSafeAction(_attemptsCount, () =>
                {
                    if (maxCounter.Link.Text.ToLower().Contains("compare"))
                    {
                        maxCounter.Link.Click();
                    }
                }, () =>
                {
                    return _driver.FindElements(By.CssSelector(".table-main .aver")).Count > 0 &&
                           _driver.FindElements(By.CssSelector(".table-main .highest")).Count > 0;
                });
            }

            if (actionsSucceeded == false)
            {
                return null;
            }

            IWebElement pathAver = _driver.FindElement(By.CssSelector(".table-main .aver"));
            IWebElement pathHigh = _driver.FindElement(By.CssSelector(".table-main .highest"));

            return new OddValue()
            {
                DuelLink = duelLink,
                Average1 = pathAver.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                Average2 = pathAver.FindElement(By.CssSelector(".right:nth-child(4)")).Text,
                AveragePayout = pathAver.FindElement(By.CssSelector(".aver .center")).Text,
                Highest1 = pathHigh.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                Highest2 = pathHigh.FindElement(By.CssSelector(".right:nth-child(4)")).Text,
                HighestPayout = pathHigh.FindElement(By.CssSelector(".center")).Text,
                Tab = _driver.FindElement(By.CssSelector(".ul-nav>.active")).Text

            };
        }

        private bool DoSafeAction(int attemptsCount, Action targetAction, Func<bool> successCriteria)
        {
            for (int i = 0; i < attemptsCount; i++)
            {
                targetAction();

                if (successCriteria() == true)
                    return true;
            }

            return false;
        }

        public void Delay()
        {
            Thread.Sleep(_random.Next(1500, 1500));
        }
    }
}