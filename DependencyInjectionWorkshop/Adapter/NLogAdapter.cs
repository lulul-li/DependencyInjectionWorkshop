using NLog;

namespace DependencyInjectionWorkshop.Adapter
{
    public class NLogAdapter
    {
        public void log(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(
                message);
        }
    }
}