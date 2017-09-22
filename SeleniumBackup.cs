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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestSElenium
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://www.oddsportal.com/tennis/");
            List<string> URL1 = new List<string>();
            List<string> URL2 = new List<string>();
            List<string> URL3 = new List<string>();

            IList<IWebElement> selectElements = driver.FindElements(By.ClassName("bfl"));
            AddHref(URL1, selectElements);//get hrefs


            for (int i = 1; i < URL1.Count; i++)
            {
                string country = selectElements[i].Text;
                driver.Navigate().GoToUrl(URL1[i]);
                IList<IWebElement> selectURLs1 = driver.FindElements(By.CssSelector("#country-tournaments-table tbody tr td>a"));
                AddHref(URL2, selectURLs1);//get hrefs

                    for (int j = 0; j < URL2.Count; j++)
                    {
                        string cup = selectURLs1[j].Text;
                        driver.Navigate().GoToUrl(URL2[j]);
                        IList<IWebElement> selectURLs2 = driver.FindElements(By.CssSelector(".table-participant > a"));
                        AddHref(URL3, selectURLs2);//get hrefs

                        for (int z = 0; z < selectURLs2.Count; z++)
                        {
                            string mutch = selectURLs2[z].Text;
                            driver.Navigate().GoToUrl(URL3[z]);// дошел до корня со значениями Home/away!!!!!!!!!!!!!!!!!!

                        string averName = driver.FindElements(By.CssSelector(".table-main .aver .name")).FirstOrDefault().Text;
                        string aver1 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(2)")).Text;
                        string aver2 = driver.FindElement(By.CssSelector(".table-main .aver .right:nth-child(3)")).Text;
                        string averPayout = driver.FindElement(By.CssSelector(".table-main .aver .center")).Text;

                        //string AH = driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Asian Handicap']")).GetAttribute("href");
                        //string OU = driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Over/Under']")).GetAttribute("href");

                        driver.FindElement(By.CssSelector("#tab-nav-main ul > li a[title='Asian Handicap']")).Click();//переход на АН
                        
                        IList<IWebElement> selectBigger = driver.FindElements(By.CssSelector(".table-container .odds-cnt"));
                        int [] mas = new int[selectBigger.Count];

                        for (int s = 0; s < selectBigger.Count; s++)//поиск среди всех вариантов
                        {
                            string a = selectBigger[s].Text.Replace("(", "");
                            a = a.Replace(")", "");
                            if (a!="")
                            {
                                mas[s] = Convert.ToInt32(a);
                            }
                        }


                        int max = 0;
                        for (int m = 0; m < mas.Length; m++)
                        {
                            if (mas[m]>max)
                            {
                                max = mas[m];//находит максимальный элемент
                            }
                        }




                    }
                }
            }

            void AddHref( List<string> A, IList<IWebElement> B)
            {
                for (int i = 0; i < B.Count; i++)
                {
                    A.Add(B[i].GetAttribute("href"));
                }
            }

            ////написать функцию поиска среднего и высокого на странице по URL
            //void FindValues(string URL)
            //{
            //    driver.Navigate().GoToUrl(URL);
            //    IList<IWebElement> AHOUurls = driver.FindElements(By.CssSelector(".table-main .aver td"));
            
            //}

            //void SetHAItem ( string CountryName, string CupName, string MutchName, string HomeAwayURL)// home/avay
            //{

            //}


        }
    }
}
