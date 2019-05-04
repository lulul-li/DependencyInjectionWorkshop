using NLog;

namespace DependencyInjectionWorkshop.Adapter
{
    public interface ILog
    {
        void Info(string message);
    }

    public class NLogAdapter : ILog
    {
        public void Info(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(
                message);
        }
    }
}