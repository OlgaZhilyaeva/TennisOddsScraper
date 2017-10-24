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
using TennisOddsScrapper.BL.XMLSerializator;

namespace TennisOddsScraper.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OddsScrapper _scrapper;
        private ObservableCollection<OddValue> _oddsValuesList;
        private List<OddValue> Oddslist;
        public MainWindow()
        {
            InitializeComponent();

            _scrapper = new OddsScrapper();
            _scrapper.ItemAddedEvent = ItemAddedEvent;
            Oddslist = _scrapper.OddValues;

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
            List<>
            Serializator serializator = new Serializator();
            Task.Run(() =>
            {
                _scrapper.Initialize();
                _scrapper.LogIn();
                _scrapper.StartScraping();
                serializator.TransformData(Oddslist);
            });

            List<OddValue> oddsValues = _scrapper.OddValues;
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
           
            Task.Run(() =>
                SaveDataToXML(Oddslist)
            );

        }
        public void SaveDataToXML(List<OddValue> _oddsValues)
        {
            ISerializator serializator = null;
            OddSerializationList oddSerialization = serializator.TransformData(_oddsValues);
            serializator.Serialize(oddSerialization);
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
