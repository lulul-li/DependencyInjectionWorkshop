using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IProfile _profile;
        private readonly IFailedCounter _failedCounter;
        private readonly IHash _sha256Adapter;
        private readonly IOtpService _otpService;
        private readonly ILog _nLogAdapter;
        private readonly INotify _slackAdapter;

        public AuthenticationService(IProfile profile, IFailedCounter failedCounter, IHash sha256Adapter, IOtpService otpService, ILog nLogAdapter, INotify slackAdapter)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _sha256Adapter = sha256Adapter;
            _otpService = otpService;
            _nLogAdapter = nLogAdapter;
            _slackAdapter = slackAdapter;
        }

        public AuthenticationService()
        {
            _profile = new Profile();
            _failedCounter = new FailedCounter();
            _sha256Adapter = new SHA256Adapter();
            _otpService = new OtpService();
            _nLogAdapter = new NLogAdapter();
            _slackAdapter = new SlackAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profile.GetPassword(accountId);

            var hashPassword = _sha256Adapter.GetHash(password);

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

                _nLogAdapter.Info($"verify failed account : {accountId} ,failed count {failedCount}");

                _slackAdapter.PushMessage("my message");

                return false;
            }
        }
    }
}