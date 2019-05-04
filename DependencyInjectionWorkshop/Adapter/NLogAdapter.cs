﻿using NLog;

namespace DependencyInjectionWorkshop.Adapter
{
    public interface ILogger
    {
        void Info(string message);
    }

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