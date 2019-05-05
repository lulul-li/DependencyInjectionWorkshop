using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class LogDecorate: BaseAuthenticateDecorator
    {
        private readonly IAuthentication _authentication;
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogDecorate(IAuthentication authentication, IFailedCounter failedCounter, ILogger logger) : base(authentication)
        {
            _authentication = authentication;
            _failedCounter = failedCounter;
            _logger = logger;
        }

        public void LogFailedCount(string accountId)
        {
            var failedCount = _failedCounter.Get(accountId);
            _logger.Info($"verify failed account : {accountId} ,failed count {failedCount}");
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var isVerify = _authentication.Verify(accountId, password, otp);
            if (!isVerify)
            {
                LogFailedCount(accountId);
            }
            return isVerify;
        }
    }
}