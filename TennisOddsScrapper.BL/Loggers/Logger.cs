using System;
using System.IO;

namespace TennisOddsScrapper.BL.Loggers
{
    public class Logger : ILogger
    {
        private const string LogFileName = "log.txt";

        public void Log(string message)
        {
            var content = $"{DateTime.Now}: {message}\n";
            File.AppendAllText(LogFileName, content);
        }

        public void Log(Exception e)
        {
            Log($"{e.Message}\n{e.StackTrace}");
        }

        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(() => new Logger());
        public static ILogger I => _logger.Value;
    }
}
