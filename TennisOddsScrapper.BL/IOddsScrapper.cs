using System;
using System.Collections.Generic;
using TennisOddsScrapper.BL.Events;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL
{
    public interface IOddsScrapper
    {
        EventHandler<ReportEventArgs> ProgressReportedEvent { get; set; }
            
        List<OddValue> OddValues { get; set; }

        void Delay();
        void Initialize();
        void LogIn(string loging, string b);
        void StartScraping();
    }
}