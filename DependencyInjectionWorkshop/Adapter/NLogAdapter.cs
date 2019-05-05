using NLog;

namespace DependencyInjectionWorkshop.Adapter
{
    public class NLoggerAdapter : ILogger
    {
        public void Info(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(
                message);
        }
    }
}