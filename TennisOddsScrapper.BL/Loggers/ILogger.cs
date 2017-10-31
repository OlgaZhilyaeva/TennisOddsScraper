using System;

namespace TennisOddsScrapper.BL.Loggers
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception e);
    }
}
