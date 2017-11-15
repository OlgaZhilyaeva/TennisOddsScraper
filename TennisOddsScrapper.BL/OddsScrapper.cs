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
            ProgressReportedEvent?.Invoke(this, new ReportEventArgs() { ProgressPercentage = progress });
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

            int group = 0;

            foreach (var countryLink in countriesLinks)
            {
                ReportProgress((int)(cI / (double)countriesCount * 100));
                cI++;

<<<<<<< HEAD
                //// TODO: remove from production.
=======
                // TODO: remove from production.
>>>>>>> b2ecb801363465117fef5424f2fbdd6d7bba7c6b
                //if (countryLink.Name != "Chile")
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

                    List<DuelLink> duelsList = GetDuelsLinks(matchLink).Where(x => !String.IsNullOrEmpty(x.Name)).ToList();
                    List<Teams> teams = new List<Teams>();
                    int group = 0;

                    foreach (var duelLink in duelsList)
                    {
                        try
                        {
                            String[] words = duelLink.Name.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                            var team = new Teams()
                            {
                                HomeTeam = words[1].Trim(),
                                AwayTeam = words[0].Trim()
                            };

                            teams.Add(team);

                            _driver.Navigate().GoToUrl(duelLink.Url);
                            Delay();

                            var oddVal = SetOddValuesHa(duelLink, group);
                            if (oddVal != null)
                                AddOddValue(oddVal, team);

                            if (NaviagateToTab(duelLink, "Asian Handicap"))
                            {
                                oddVal = SetOddValuesAh(duelLink, group);
                                if (oddVal != null)
                                    AddOddValue(oddVal, team);
                            }

                            if (NaviagateToTab(duelLink, "Over/Under"))
                            {
                                oddVal = SetOddValuesAh(duelLink, group);
                                if (oddVal != null)
                                    AddOddValue(oddVal, team);
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

        private void AddOddValue(OddValue value, Teams teams)
        {
            value.TeamsLink = teams;
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
                AveragePayout = pathAver.FindElement(By.CssSelector(".aver .center")).Text.Trim(),
                Highest1 = pathHigh.FindElement(By.CssSelector(".right:nth-child(2)")).Text,
                Highest2 = pathHigh.FindElement(By.CssSelector(".right:nth-child(3)")).Text,
                HighestPayout = pathHigh.FindElement(By.CssSelector(".center")).Text,
                Tab = _driver.FindElement(By.CssSelector(".first strong")).Text,
                Date = GetDate()
            };
        }

        private string GetDate()
        {
            string date = _driver.FindElement(By.CssSelector(".wrap .date")).Text;// get date

            string[] dataArray = date.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (dataArray.Length == 3)
            {
                date = dataArray[1].Trim();
            }

            return date;
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

            IList<IWebElement> containers = SafeTryFindElements(_attemptsCount, ".table-container").Where(x => !x.GetCssValue("display").Equals("none")).ToList();

            if (!containers.Any())
                return null;

            foreach (var container in containers)
            {
                //Delay();
                List<IWebElement> linksList = container.FindElements(By.CssSelector(".odds-co a")).ToList();
                List<string> countersList = container.FindElements(By.CssSelector(".odds-cnt")).Select(x => x.Text).ToList();
                List<string> gameValueList = container.FindElements(By.CssSelector("strong")).Select(x => x.Text).ToList();
                List<string> games = new List<string>();

                foreach (var game in gameValueList)
                {

                    if (game.Contains("Asian handicap "))
                    {
                        games.Add(game.Replace("Asian handicap ", ""));
                    }

                    if (game.Contains("Over/Under "))
                    {
                        games.Add(game.Replace("Over/Under ", ""));
                    }

                }
                if (linksList.Count > 0 && countersList.Count > 0)
                {
                    IWebElement link = linksList.FirstOrDefault(); // SafeTryFindElements(_attemptsCount, ".odds-co a").FirstOrDefault();

                    string counterElement = countersList.FirstOrDefault(); //SafeTryFindElements(_attemptsCount, ".odds-cnt").FirstOrDefault();

                    gameValue = games.FirstOrDefault();

                    string counterString = counterElement.Replace("(", "")
                        .Replace(")", "");

                    int counter = 0;
                    if (Int32.TryParse(counterString, out counter))
                    {
                        counters.Add(new Counter
                        {
                            Value = counter,
                            Link = link,
                            ElementContext = container
                        });
                    }
                }
            }

            int max = counters.Max(x => x.Value);
            List<Counter> maxCounters = counters.Where(x => x.Value == max).ToList();

            bool actionsSucceeded = true;

            CloseAllTableContainers();

            FindHigh finalElement = FindFinalElement(maxCounters, gameValue);

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
                    Tab = _driver.FindElement(By.CssSelector(".ul-nav>.active")).Text
                };
                return value;
            }
            return null;
        }

        private FindHigh FindFinalElement(List<Counter> maxCounters, string gameValue)
        {
            List<FindHigh> highestValues = new List<FindHigh>();

            foreach (var maxCounter in maxCounters)
            {
                if (maxCounter.Link.Text.ToLower().Contains("compare"))
                {
                    Delay();
                    maxCounter.Link.Click();
                }

                List<IWebElement> avgElements = maxCounter.ElementContext.FindElements(By.CssSelector(".table-main .aver")).ToList();
                List<IWebElement> highestElements = maxCounter.ElementContext.FindElements(By.CssSelector(".table-main .highest")).ToList();

                if (!avgElements.Any() || avgElements.Count != 1 || highestElements.Count != 1 || avgElements.Count != highestElements.Count)
                    throw new InvalidOperationException("Unable to find avg and highest data.");


                var highestElement = highestElements.FirstOrDefault();

                var elems = highestElement.FindElements(By.ClassName("no-border-right-highest")).ToList();
                var arrayHighs = elems.FirstOrDefault()?.Text.Replace("%", "");

                double counter = 0;
                if (String.IsNullOrWhiteSpace(arrayHighs) || !Double.TryParse(arrayHighs, out counter))
                    throw new NullReferenceException("Unable to get % payout value of highest bid.");

                highestValues.Add(new FindHigh()
                {
                    AverageString = avgElements.FirstOrDefault(),
                    GameValue = gameValue,
                    HighInt = counter,
                    HighString = highestElements.FirstOrDefault()
                });
            }

            return highestValues.OrderByDescending(x => x.HighInt).FirstOrDefault();
        }

        private void CloseAllTableContainers()
        {
            var elements = _driver.FindElements(By.PartialLinkText("Hide odds"));

            foreach (var webElement in elements)
            {
                webElement.Click();
            }
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
            Thread.Sleep(_random.Next(200, 800));
            Thread.Sleep(_random.Next(100, 600));
        }
    }
}