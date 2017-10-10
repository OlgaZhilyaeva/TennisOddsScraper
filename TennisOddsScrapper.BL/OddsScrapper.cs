﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
                if (countryLink.Name == "China" || countryLink.Name == "Austria" || countryLink.Name == "Germany" || countryLink.Name == "Hong Kong" || countryLink.Name == "Italy" || countryLink.Name == "Spain" || countryLink.Name == "USA" || countryLink.Name == "Uzbekistan" || countryLink.Name == "World")
                {
                    continue;
                }
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
                        if (oddVal != null)
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
            // Create and insert into DB
            using (OddsDbContext db = new OddsDbContext())
            {

                var duelLinks = oddsValues.Select(x => x.DuelLink).Distinct();
                foreach (var duelLink in duelLinks)
                {
                    db.DuelLinks.Add(duelLink);
                }

                var matchLinks = oddsValues.Select(x => x.DuelLink.MatchLink).Distinct();
                foreach (var matchLink in matchLinks)
                {
                    db.MatchLinks.Add(matchLink);
                }

                var countryLinks = oddsValues.Select(x => x.DuelLink.MatchLink.CountryLink).Distinct();
                foreach (var countryLink in countryLinks)
                {
                    db.CountryLinks.Add(countryLink);
                }

                foreach (var oddValue in oddsValues)
                {
                    db.OddValues.Add(oddValue);
                }
                
                db.SaveChanges();
            }
        }

        private List<CountryLink> GetCountriesLinks()
        {
            return _driver.FindElements(By.CssSelector(".table-main .bfl")).Select(x => new CountryLink()
            {
                Name = x.Text.Trim(),
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
            return _driver.FindElements(By.CssSelector(".table-participant > a")).Where(x => x.GetAttribute("href").Contains("javascript:") == false).Select(x => new DuelLink()
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
            List<IWebElement> arrayLi = _driver.FindElements(By.CssSelector($"#tab-nav-main ul > li")).ToList();
            foreach (var li in arrayLi)
            {
                List<IWebElement> list = li.FindElements(By.CssSelector($"a[title='{tabName}']")).ToList();
                if (list.Count > 0)
                {
                    if (li.GetCssValue("display").Contains("none"))
                    {
                        return false;
                    }
                    else
                    {
                        string js = _driver.FindElement(By.CssSelector($"#tab-nav-main ul > li a[title='{tabName}']"))
                            .GetAttribute("onmousedown").Replace("return false;", ""); //goto АН
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
                        executor.ExecuteScript(js);

                        Delay();
                        return true;
                    }
                }
            }
            return false;
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
            string date = _driver.FindElement(By.CssSelector(".wrap .date")).Text;// get date
            string gameValue = "";

            int startIndex = date.IndexOf(' ');
            int endIndex = date.LastIndexOf(',');
            int length = endIndex - startIndex;
            date = date.Substring(startIndex, length);


            if (containers == null)
                return null;

            foreach (var container in containers)
            {
                //Delay();
                List<IWebElement> linksList = container.FindElements(By.CssSelector(".odds-co a")).ToList();
                List<IWebElement> countersList = container.FindElements(By.CssSelector(".odds-cnt")).ToList();
                

                if (linksList.Count > 0 && countersList.Count > 0)
                {
                    IWebElement link = linksList.FirstOrDefault(); // SafeTryFindElements(_attemptsCount, ".odds-co a").FirstOrDefault();

                    IWebElement counterElement = countersList.FirstOrDefault(); //SafeTryFindElements(_attemptsCount, ".odds-cnt").FirstOrDefault();

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
            }

            int max = counters.Max(x => x.Value);
            List<Counter> maxCounters = counters.Where(x => x.Value == max).ToList();

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
                           _driver.FindElements(By.CssSelector(".table-main .highest")).Count > 0 &&
                           _driver.FindElements(By.CssSelector(".table-header-light strong")).Count > 0;
                           ;
                });
            }

            if (actionsSucceeded == false)
            {
                return null;
            }

            List<IWebElement> pathHigh = _driver.FindElements(By.CssSelector(".table-main .highest")).ToList();
            List<IWebElement> pathAver = _driver.FindElements(By.CssSelector(".table-main .aver")).ToList();
            List<IWebElement> gameValueList = _driver.FindElements(By.CssSelector(".table-header-light strong")).ToList();

            FindHigh finalElement = null;

            if (maxCounters.Count == pathHigh.Count && maxCounters.Count == pathAver.Count)
            {
                int z = 0;
                List<FindHigh> findHighs = new List<FindHigh>(); 
                foreach (var high in pathHigh)
                {
                    List<IWebElement> arrayHighs = high.FindElements(By.ClassName("no-border-right-highest")).ToList();

                    if (gameValueList[z].Text.Contains("Asian handicap "))
                    {
                        gameValue = gameValueList[z].Text.Replace("Asian handicap ", "");
                    }

                    if (gameValueList[z].Text.Contains("Over/Under "))
                    {
                        gameValue = gameValueList[z].Text.Replace("Over/Under ", "");
                    }

                    if (arrayHighs.Count == 1)
                    {
                        string arrayHighsString = arrayHighs.FirstOrDefault().Text.Replace("%", "");
                        double counter = 0;
                        if (Double.TryParse(arrayHighsString, out counter))
                        {
                            findHighs.Add(new FindHigh(){HighInt = counter,  HighString = high, AverageString = pathAver[z], GameValue = gameValue});
                        }
                    }
                    z++;
                }
                finalElement = findHighs.OrderByDescending(x => x.HighInt).FirstOrDefault();
            }

            //вылетает когда finalElement = NULL
            if (finalElement != null)
            {
                return new OddValue()
                {
                    DuelLink = duelLink,
                    Average1 = finalElement.AverageString.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                    Average2 = finalElement.AverageString.FindElement(By.CssSelector(".right:nth-child(4)")).Text,
                    AveragePayout = finalElement.AverageString.FindElement(By.CssSelector(".aver .center")).Text,
                    Highest1 = finalElement.HighString.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                    Highest2 = finalElement.HighString.FindElement(By.CssSelector(".right:nth-child(4)")).Text,
                    HighestPayout = finalElement.HighString.FindElement(By.CssSelector(".center")).Text,
                    Tab = _driver.FindElement(By.CssSelector(".ul-nav>.active")).Text,
                    GameValue = gameValue,
                    Date = date
                };
            }
            return null;
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