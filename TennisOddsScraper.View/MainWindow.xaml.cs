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
using TennisOddsScrapper.BL.Events;
using TennisOddsScrapper.BL.Models;
using TennisOddsScrapper.BL.XMLSerializator;

namespace TennisOddsScraper.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IOddsScrapper _scrapper;
       // private List<OddValue> Oddslist;
        private ISerializator _serializator;
        private OddSerializationList _resultList;

        public MainWindow()
        {
            InitializeComponent();

            // For tests only.
            //_serializator = new SerializatorStub();
            //_scrapper = new OddsScrapperStub();

            // Uncomment for production.
            _serializator = new Serializator();
            _scrapper = new OddsScrapper();

            _scrapper.ProgressReportedEvent +=ProgressReportedEvent;

            //Oddslist = _scrapper.OddValues;
            OddsValuesList = new ObservableCollection<OddSerializationModel>();
        }

        private void ProgressReportedEvent(object sender, ReportEventArgs reportEventArgs)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PbProgress.Value = reportEventArgs.ProgressPercentage;
                PbProgress.IsIndeterminate = PbProgress.Value == 0;
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

        private void BtnGetNewInfo_OnClick(object sender, RoutedEventArgs e)
        {
            string login = this.login.Text;
            string password = this.password.Password;

            Task.Run(() =>
            {
                _scrapper.Initialize();
                _scrapper.LogIn(login, password);

                _scrapper.StartScraping();
                _resultList = _serializator.TransformData(_scrapper.OddValues);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    OddsValuesList.Clear();

                    foreach (OddSerializationModel model in _resultList)
                    {
                        OddsValuesList.Add(model);
                    }

                    PbProgress.IsIndeterminate = false;
                    PbProgress.Value = 0;

                    MessageBox.Show("Scraping ended!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }

        public ObservableCollection<OddSerializationModel> OddsValuesList { get; set; }

        private void BtnCreateXml_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
                SaveDataToXml()
            );
        }
        public void SaveDataToXml()
        {
            _serializator.Serialize(_resultList);
            MessageBox.Show("Export to XML complete!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnPutToDb_OnClick(object sender, RoutedEventArgs e)
        {
            PutDataToDb();
            MessageBox.Show("Export to database complete!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PutDataToDb()
        {
            using (OddsDbContext db = new OddsDbContext())
            {
                List<OddValue> valuesList = _scrapper.OddValues;

                //TODO: teams to db
                var duelLinks = valuesList.Select(x => x.DuelLink).Distinct();
                foreach (var duelLink in duelLinks)
                {
                    db.DuelLinks.Add(duelLink);
                }

                var matchLinks = valuesList.Select(x => x.DuelLink.MatchLink).Distinct();
                foreach (var matchLink in matchLinks)
                {
                    db.MatchLinks.Add(matchLink);
                }

                var countryLinks = valuesList.Select(x => x.DuelLink.MatchLink.CountryLink).Distinct();
                foreach (var countryLink in countryLinks)
                {
                    db.CountryLinks.Add(countryLink);
                }

                foreach (var oddValue in valuesList)
                {
                    db.OddValues.Add(oddValue);
                }

                db.SaveChanges();
            }
        }
    }
}
