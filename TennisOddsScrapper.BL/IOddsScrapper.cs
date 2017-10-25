using System.Collections.Generic;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL
{
    public interface IOddsScrapper
    {
        List<OddValue> OddValues { get; }

        void Delay();
        void Initialize();
        void LogIn();
        void StartScraping();
    }
}