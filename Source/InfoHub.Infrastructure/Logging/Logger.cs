using NLog;

namespace InfoHub.Infrastructure.Logging
{
    public static class Logger
    {
        public static void Log(LogEventType eventType, string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Log(LogLevel.FromOrdinal((int) eventType), message);
        }
    }
}
