using System.Collections.Generic;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL
{
    public interface IOddsScrapper
    {
        List<OddValue> OddValues { get; set; }

        void Delay();
        void Initialize();
        void LogIn(string a, string b);
        void StartScraping();
    }
}