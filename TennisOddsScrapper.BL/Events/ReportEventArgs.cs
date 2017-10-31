using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisOddsScrapper.BL.Events
{
    public class ReportEventArgs : EventArgs
    {
        public int ProgressPercentage { get; set; }
    }
}
