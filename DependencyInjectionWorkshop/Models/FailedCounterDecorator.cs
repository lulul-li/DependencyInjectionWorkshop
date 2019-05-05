using DependencyInjectionWorkshop.Exception;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounterDecorator : IAuthentication
    {
        private readonly IAuthentication _authentication;
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthentication authentication, IFailedCounter failedCounter)
        {
            _authentication = authentication;
            _failedCounter = failedCounter;
        }

        public void CheckAccountIsLock(string accountId)
        {
            var isLock = _failedCounter.CheckAccountIsLock(accountId);
            if (isLock)
            {
                throw new FailedTooManyTimesException();
            }
        }

        public bool Verify(string accountId, string password, string otp)
        {
            CheckAccountIsLock(accountId);
            return _authentication.Verify(accountId, password, otp);
        }
    }
}