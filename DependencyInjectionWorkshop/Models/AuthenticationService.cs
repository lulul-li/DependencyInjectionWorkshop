using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IProfile _profile;
        private readonly IFailedCounter _failedCounter;
        private readonly IHash _hash;
        private readonly IOtp _otp;
        private readonly ILogger _logger;
        private readonly INotification _notification;
        
        public AuthenticationService(IProfile profile, IFailedCounter failedCounter, IHash hash, IOtp otp, ILogger logger, INotification notification)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otp = otp;
            _logger = logger;
            _notification = notification;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.GetHash(password);

            var currentOtp = _otp.GetCurrentOtp(accountId);

            if (hashPassword.ToString() == dbPassword && currentOtp == otp)
            {
                _failedCounter.Reset(accountId);

                return true;
            }
            else
            {
                _failedCounter.Add(accountId);

                var failedCount = _failedCounter.Get(accountId);

                _logger.Info($"verify failed account : {accountId} ,failed count {failedCount}");

                _notification.PushMessage("my message");

                return false;
            }
        }
    }
}