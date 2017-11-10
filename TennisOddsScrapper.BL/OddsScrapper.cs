using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TennisOddsScrapper.BL.Events;
using TennisOddsScrapper.BL.Loggers;
using TennisOddsScrapper.BL.Models;
using TennisOddsScrapper.BL.XMLSerializator;

namespace TennisOddsScrapper.BL
{
    public class OddsScrapperStub : IOddsScrapper
    {
        public EventHandler<ReportEventArgs> ProgressReportedEvent { get; set; }
        public List<OddValue> OddValues { get; set; }

        public OddsScrapperStub()
        {
            OddValues = new List<OddValue>();
        }

        public void Delay()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            Debug.WriteLine(nameof(Initialize));
        }

        public void LogIn(string loging, string b)
        {
            Debug.WriteLine(nameof(LogIn));
        }

        public void PutDataToDb()
        {
            Debug.WriteLine(nameof(PutDataToDb));
        }

        public void StartScraping()
        {
            Debug.WriteLine(nameof(StartScraping));
        }
    }

    public class OddsScrapper : IOddsScrapper
    {
        
        private List<OddValue> _oddsValues = new List<OddValue>();
        private IWebDriver _driver;
        private Random _random;
        private ILogger _logger = new Loggers.Logger();

        public EventHandler<ReportEventArgs> ProgressReportedEvent { get; set; }

        public List<OddValue> OddValues
        {
            get { return _oddsValues; }
            set { _oddsValues = value; }
        }

        private int _attemptsCount = 3;

        public OddsScrapper()
        {
            _random = new Random();
        }

        public void Initialize()
        {
            _driver = new ChromeDriver();
            _oddsValues = new List<OddValue>();
        }

        public void LogIn(string userName, string pass)
        {
            IWebDriver driver = _driver;
            driver.Navigate().GoToUrl("http://www.oddsportal.com/login/");

            var js = $@"
                document.querySelectorAll(""[name='login-username']"")[0].value = '{userName}';
                document.querySelectorAll(""[name='login-password']"")[0].value = '{pass}';

                document.querySelectorAll(""[name='login-submit']"")[1].click();";
            
            IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
            executor.ExecuteScript(js);
        }

        private void ReportProgress(int progress)
        {
            ProgressReportedEvent?.Invoke(this, new ReportEventArgs() {ProgressPercentage = progress});
        }

        public void StartScraping()
        {
            _oddsValues.Clear();
            _driver.Navigate().GoToUrl("http://www.oddsportal.com/tennis/");
            Delay();

            List<CountryLink> countriesLinks = GetCountriesLinks();

            ReportProgress(0);
            var countriesCount = countriesLinks.Count;
            var cI = 0;

            foreach (var countryLink in countriesLinks)
            {
                ReportProgress((int) (cI / (double)countriesCount * 100));
                cI++;

                //// TODO: remove from production.
                //if (countryLink.Name != "Canada")
                //{
                //    continue;
                //}

                _driver.Navigate().GoToUrl(countryLink.Url);
                Delay();

                List<MatchLink> matchLinks = GetMatchesLinks(countryLink);
                foreach (var matchLink in matchLinks)
                {
                    _driver.Navigate().GoToUrl(matchLink.Url);
                    Delay();

                    List<DuelLink> duelsList = GetDuelsLinks(matchLink);
                    List<Teams> teams = new List<Teams>();
                    int group = 0;
                    foreach (var duelLink in duelsList)
                    {
                        try
                        {
                            String[] words = duelLink.Name.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                            teams.Add(new Teams()
                            {
                                HomeTeam = words[1],
                                AwayTeam = words[0]
                            });

                            _driver.Navigate().GoToUrl(duelLink.Url);
                            Delay();

                            var oddVal = SetOddValuesHa(duelLink, group);
                            if (oddVal != null)
                                AddOddValue(oddVal);

                            if (NaviagateToTab(duelLink, "Asian Handicap"))
                            {
                                oddVal = SetOddValuesAh(duelLink, group);
                                if (oddVal != null)
                                    AddOddValue(oddVal);
                            }

                            if (NaviagateToTab(duelLink, "Over/Under"))
                            {
                                oddVal = SetOddValuesAh(duelLink, group);
                                if (oddVal != null)
                                    AddOddValue(oddVal);
                            }
                            group++;
                        }
                        catch (Exception e)
                        {
                            Logger.I.Log(e);
                        }
                    }
                }
            }

            ReportProgress(0);

            _driver.Close();
            _driver.Quit();
        }

        private void AddOddValue(OddValue value)
        {
            _oddsValues.Add(value);
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

        private OddValue SetOddValuesHa(DuelLink duelLink, int group)
        {

            IWebElement pathAver = _driver.FindElement(By.CssSelector(".table-main .aver"));
            IWebElement pathHigh = _driver.FindElement(By.CssSelector(".table-main .highest"));

            return new OddValue()
            {
                DuelLink = duelLink,
                Group = group,
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

        private OddValue SetOddValuesAh(DuelLink duelLink, int group)
        {
            List<Counter> counters = new List<Counter>();
            string gameValue = "";

            IList<IWebElement> containers = SafeTryFindElements(_attemptsCount, ".table-container");
            string date = _driver.FindElement(By.CssSelector(".wrap .date")).Text;// get date
            

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
                List<IWebElement> gameValueList = container.FindElements(By.CssSelector("strong")).ToList();
                List<string> games = new List<string>();


                foreach (var game in gameValueList)
                {

                    if (game.Text.Contains("Asian handicap "))
                    {
                        games.Add(game.Text.Replace("Asian handicap ", ""));
                    }

                    if (game.Text.Contains("Over/Under "))
                    {
                        games.Add(game.Text.Replace("Over/Under ", ""));
                    }

                }
                if (linksList.Count > 0 && countersList.Count > 0)
                {
                    IWebElement link = linksList.FirstOrDefault(); // SafeTryFindElements(_attemptsCount, ".odds-co a").FirstOrDefault();

                    IWebElement counterElement = countersList.FirstOrDefault(); //SafeTryFindElements(_attemptsCount, ".odds-cnt").FirstOrDefault();

                    gameValue = games.FirstOrDefault();

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
                        Delay();
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

            FindHigh finalElement = null;

            if (maxCounters.Count == pathHigh.Count && maxCounters.Count == pathAver.Count)
            {
                int z = 0;
                List<FindHigh> findHighs = new List<FindHigh>(); 
                foreach (var high in pathHigh)
                {
                    List<IWebElement> arrayHighs = high.FindElements(By.ClassName("no-border-right-highest")).ToList();

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

            if (finalElement != null)
            {
                OddValue value = new OddValue()
                {
                    DuelLink = duelLink,
                    Group = group,
                    GameValue = finalElement.GameValue,
                    Average1 = finalElement.AverageString.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                    Average2 = finalElement.AverageString.FindElement(By.CssSelector(".right:nth-child(4)")).Text,
                    AveragePayout = finalElement.AverageString.FindElement(By.CssSelector(".aver .center")).Text,
                    Highest1 = finalElement.HighString.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                    Highest2 = finalElement.HighString.FindElement(By.CssSelector(".right:nth-child(4)")).Text,
                    HighestPayout = finalElement.HighString.FindElement(By.CssSelector(".center")).Text,
                    Tab = _driver.FindElement(By.CssSelector(".ul-nav>.active")).Text,
                    Date = date
                };
                return value;
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