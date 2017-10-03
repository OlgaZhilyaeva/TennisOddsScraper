using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TestSElenium
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        IWebDriver driver = new ChromeDriver();

        private void button1_Click(object sender, EventArgs e)
        {
            driver.Navigate().GoToUrl("http://www.oddsportal.com/tennis/");
            Delay();
            List<string> URL1 = new List<string>();
            List<string> URL2 = new List<string>();
            List<string> URL3 = new List<string>();
            List<string> NamesCountries = new List<string>();
            List<string> NamesCups = new List<string>();
            List<string> NamesMatches = new List<string>();

            IList<IWebElement> selectElements = driver.FindElements(By.ClassName("bfl"));
            AddHref(URL1, selectElements);//get hrefs
            IList<HAItem> XMLObjects = new List<HAItem>();

            AddNames(NamesCountries, selectElements);//get Names

            for (int i = 1; i < URL1.Count; i++)
            {
                HAItem item = new HAItem();

                string country = NamesCountries[i];
                item.Country = selectElements[i].Text;

                driver.Navigate().GoToUrl(URL1[i]);
                Delay();
                IList<IWebElement> selectURLs1 = driver.FindElements(By.CssSelector("#country-tournaments-table tbody tr td>a"));
                AddHref(URL2, selectURLs1);//get hrefs
                AddNames(NamesCups, selectURLs1);

                for (int j = 0; j < URL2.Count; j++)
                {
                    string cup = URL2[j];
                    if (cup != "")
                    {
                        item.Cup = NamesCups[j];

                        driver.Navigate().GoToUrl(URL2[j]);
                        Delay();
                        IList<IWebElement> selectURLs2 = driver.FindElements(By.CssSelector(".table-participant > a"));
                        AddHref(URL3, selectURLs2);//get hrefs
                        AddNames(NamesMatches, selectURLs2);

                        for (int z = 0; z < URL3.Count; z++)
                        {
                            string match = URL3[z];
                            item.Match = NamesMatches[z];
                            driver.Navigate().GoToUrl(URL3[z]);// Goto Home/away
                            Delay();
                            GetValues(item, 1);

                            driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Asian Handicap']")).Click();//goto АН
                            Delay();
                            FindMax(item, 2);

                            driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Over/Under']")).Click();//goto O/U
                            Delay();
                            FindMax(item, 3);

                            XMLObjects.Add(item);

                        }
                    }
                }
            }

            XmlSerializer formatter = new XmlSerializer(typeof(HAItem[]));

            using (FileStream fs = new FileStream("Odds.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, XMLObjects);
            }
        }

        public void FindMax(HAItem itemX, int val)
        {
            //get containers with display != none
            IList<IWebElement> containers = driver.FindElements(By.CssSelector(".table-container"))
                .Where(x => x.GetCssValue("display").Equals("none") == false).ToList();
            Delay();
            List<IWebElement> links = new List<IWebElement>();
            List<IWebElement> odds = new List<IWebElement>();

            foreach (var item in containers)
            {
                links.AddRange(item.FindElements(By.CssSelector(".odds-co a")));
                odds.AddRange(item.FindElements(By.CssSelector(".odds-cnt")));
            }
            // get numbers of odds
            List<LineObj> LineArray = new List<LineObj>();
            double[] mas = new double[odds.Count];
            Delay();
            for (int d = 0; d < odds.Count; d++)
            {
                string a = odds[d].Text.Replace("(", "").Replace(")", "");
                if (a != "")
                {
                    mas[d] = Convert.ToDouble(a);
                }

                LineArray.Add(new LineObj()
                {
                    CompareOdds = links[d],
                    Value = mas[d]
                });
            }
            //if there are more the one max value
            List<LineObj> max = LineArray.OrderByDescending(x => x.Value).ToList();//начало правок когда несколько одинаковых в скобках
            double firstEl = max.First().Value;
            max = max.Where(x => x.Value == firstEl).ToList();
            LineObj FinalMax = new LineObj();

            if (max.Count > 1)
            {
                for (int i = 0; i < max.Count; i++)
                {
                    max[i].CompareOdds.Click();
                    string hPayout = driver.FindElement(By.CssSelector(".table-main .highest .center")).Text;
                    Delay();
                    hPayout = hPayout.Replace("%", "");
                    double hPay = Convert.ToDouble(hPayout);
                    max[i].hPayout = hPay;
                    max[i].CompareOdds.Click();
                }
                max = max.OrderByDescending(x => x.hPayout).ToList();
            }
            FinalMax = max.First();
            //get max value and goto function which finds values
            if (FinalMax != null)
            {
                FinalMax.CompareOdds.Click();
                Delay();
                GetValues(itemX, val);
            }
        }

        public void AddNames(List<string> A, IList<IWebElement> B)
        {
            for (int i = 0; i < B.Count; i++)
            {
                A.Add(B[i].Text);
            }
        }

        public void AddHref(List<string> A, IList<IWebElement> B)
        {
            for (int i = 0; i < B.Count; i++)
            {
                if (B[i].Text != " ")
                {
                    A.Add(B[i].GetAttribute("href"));
                }
            }
        }

        public void GetValues(HAItem item, int val)
        {

            string averPayout = driver.FindElement(By.CssSelector(".table-main .aver .center")).Text;
            Delay();
            string hPayout = driver.FindElement(By.CssSelector(".table-main .highest .center")).Text;
            Delay();
            if (val == 1)
            {
                string aver1 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(2)")).Text;
                Delay();
                string aver2 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(3)")).Text;
                Delay();
                string h1 = driver.FindElement(By.CssSelector(".table-main .highest .right:nth-child(2)")).Text;
                Delay();
                string h2 = driver.FindElement(By.CssSelector(".table-main .highest .right:nth-child(3)")).Text;
                Delay();

                item.Av1 = Convert.ToDouble(aver1);
                item.Av2 = Convert.ToDouble(aver2);
                item.AvPayout = averPayout;
                item.H1 = Convert.ToDouble(h1);
                item.H2 = Convert.ToDouble(h2);
                item.HPayout = hPayout;
            }
            if (val == 2)
            {
                string aver1 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(3)")).Text;
                string aver2 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(4)")).Text;
                string h1 = driver.FindElement(By.CssSelector(".table-main .highest .right:nth-child(3)")).Text;
                string h2 = driver.FindElement(By.CssSelector(".table-main .highest .right:nth-child(4)")).Text;

                item.AHAv1 = Convert.ToDouble(aver1);
                item.AHAv2 = Convert.ToDouble(aver2);
                item.AHAvPayout = averPayout;
                item.AHH1 = Convert.ToDouble(h1);
                item.AHH2 = Convert.ToDouble(h2);
                item.AHHPayout = hPayout;
            }
            if (val == 3)
            {
                string aver1 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(3)")).Text;
                string aver2 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(4)")).Text;
                string h1 = driver.FindElement(By.CssSelector(".table-main .highest .right:nth-child(3)")).Text;
                string h2 = driver.FindElement(By.CssSelector(".table-main .highest .right:nth-child(4)")).Text;

                item.OUAv1 = Convert.ToDouble(aver1);
                item.OUAv2 = Convert.ToDouble(aver2);
                item.OUAvPayout = averPayout;
                item.OUH1 = Convert.ToDouble(h1);
                item.OUH2 = Convert.ToDouble(h2);
                item.OUHPayout = hPayout;

            }

        }


        public void Delay()
        {
            Thread.Sleep(400);
        }



    }
}
