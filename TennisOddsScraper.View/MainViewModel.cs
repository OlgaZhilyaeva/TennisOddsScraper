using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScraper.View
{
    class MainViewModel : INotifyPropertyChanged
    {
        public String WindowTitle { get; set; }

        private String text;
        public String MainText
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                NotifyPropertyChanged("MainText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
