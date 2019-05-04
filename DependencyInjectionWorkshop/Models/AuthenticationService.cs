using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IProfile _profile;
        private readonly IFailedCounter _failedCounter;
        private readonly IHash _hash;
        private readonly IOtpService _otpService;
        private readonly ILogger _logger;
        private readonly INotification _notification;
        
        public AuthenticationService(IProfile profile, IFailedCounter failedCounter, IHash hash, IOtpService otpService, ILogger logger, INotification notification)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otpService = otpService;
            _logger = logger;
            _notification = notification;
        }

        public AuthenticationService()
        {
            _profile = new Profile();
            _failedCounter = new FailedCounter();
            _hash = new SHA256Adapter();
            _otpService = new OtpService();
            _logger = new NLoggerAdapter();
            _notification = new SlackAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.GetHash(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

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