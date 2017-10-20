using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TennisOddsScrapper.BL;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScraper.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OddsScrapper _scrapper;
        private ObservableCollection<OddValue> _oddsValuesList;

        public MainWindow()
        {
            InitializeComponent();

            _scrapper = new OddsScrapper();
            _scrapper.ItemAddedEvent = ItemAddedEvent;
        }

        private void ItemAddedEvent(OddValue value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OddsValuesList.Add(value);
            });
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            MessageBox.Show(selectedItem.Content.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    IWebDriver driver = new ChromeDriver();
        //    driver.Navigate().GoToUrl("http://google.com");

        //    driver.FindElement(By.Id("lst-ib")).SendKeys("I love Alex");
        //    driver.FindElement(By.Id("lst-ib")).Submit();
        //}


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {


        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            OddsDbContext db = new OddsDbContext();

            foreach (var dbDuelLink in db.DuelLinks)
            {
                System.Windows.MessageBox.Show(dbDuelLink.Name);

            }
        }

        private void BtnGetNewInfo_OnClick(object sender, RoutedEventArgs e)
        {
            //OddsDbContext context = new OddsDbContext();
            List<OddValue> list = new List<OddValue>()
            {
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 = "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Home/Away"
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 = "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Asian Handicap"
                },
                new OddValue()
                {
                    Average1 = "av1",
                    Average2 = "av2",
                    AveragePayout = "avPay",
                    GameValue = "svsv",
                    Highest1 = "nfkdnv",
                    Highest2 = "mlvkdm",
                    HighestPayout = "mkvm",
                    Tab = "Over/Under"
                }
            };

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000);

            //        //_scrapper.Initialize();
            //        //_scrapper.LogIn();
            //        //_scrapper.StartScraping();

            //        Application.Current.Dispatcher.Invoke(() =>
            //        {
            //            OddsValuesList.Add(new OddValue()
            //            {
            //                Average1 = "average1"
            //            });
            //            // System.Windows.MessageBox.Show("Test");
            //        });
            //    }
            //});

            Task.Run(() =>
            {
                _scrapper.Initialize();
                _scrapper.LogIn();
                _scrapper.StartScraping();
            });


            List<OddValue> oddsValues = _scrapper.OddValues;

            //OddsValuesList.Add(new OddValue()
            //{
            //    Average1 = "average1"
            //});
        }

        public ObservableCollection<OddValue> OddsValuesList
        {
            get
            {
                if (_oddsValuesList == null)
                {
                    _oddsValuesList = new ObservableCollection<OddValue>();
                }
                return _oddsValuesList;
            }
            set { _oddsValuesList = value; }
        }

        private void BtnCreateXml_OnClick(object sender, RoutedEventArgs e)
        {
            _scrapper.StartScraping();
            _scrapper.SaveDataToXML();
        }

        private void BtnPutToDb_OnClick(object sender, RoutedEventArgs e)
        {
            _scrapper.PutDataToDb();
        }

        private void Login_OnClick(object sender, RoutedEventArgs e)
        {
            _scrapper.LogIn();
        }
    }
}
