using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            MessageBox.Show(selectedItem.Content.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainViewModel viewModel = new MainViewModel();
            viewModel.WindowTitle = "I Love You";
            viewModel.MainText = "Olya";

            this.DataContext = viewModel;
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

            //DuelLink duelLink = new DuelLink() { MatchLink = null, Url = "dfdf", DuelLinkId = 6, Name = "Name" };
            //OddsDbContext db = new OddsDbContext();
            //foreach (var dbDuelLink in db.DuelLinks)
            //{
            //    System.Windows.MessageBox.Show("Test" + dbDuelLink.Name);

            //}
            //db.DuelLinks.Add(duelLink);
            //db.SaveChanges();

            //foreach (var dbDuelLink in db.DuelLinks)
            //{
            //    System.Windows.MessageBox.Show("Test"+ dbDuelLink.Name);
                
            //}

            OddsScrapper scrapper = new OddsScrapper();

            scrapper.LogIn();
            scrapper.StartScraping();
            



        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            OddsDbContext db = new OddsDbContext();

            foreach (var dbDuelLink in db.DuelLinks)
            {
                System.Windows.MessageBox.Show(dbDuelLink.Name);

            }
        }
    }
}
