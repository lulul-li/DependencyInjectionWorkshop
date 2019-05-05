using DependencyInjectionWorkshop.Exception;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounterDecorator : BaseAuthenticateDecorator
    {
        private readonly IAuthentication _authentication;
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthentication authentication, IFailedCounter failedCounter) : base(authentication)
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

        public override bool Verify(string accountId, string password, string otp)
        {
            CheckAccountIsLock(accountId);
            var isVerify = _authentication.Verify(accountId, password, otp);
            if (isVerify)
            {
                Reset(accountId);
            }
            else
            {
                Add(accountId);
            }

            return isVerify;
        }

        public void Reset(string accountId)
        {
            _failedCounter.Reset(accountId);
        }

        public void Add(string accountId)
        {
            _failedCounter.Add(accountId);
        }
    }
}