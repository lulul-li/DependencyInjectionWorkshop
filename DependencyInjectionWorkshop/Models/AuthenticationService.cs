using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly Profile _profile = new Profile();
        private readonly SHA256Adapter _sha256Adapter = new SHA256Adapter();
        private readonly OtpService _otpService = new OtpService();
        private readonly FailedCounter _failedCounter = new FailedCounter();
        private readonly NLogAdapter _nLogAdapter = new NLogAdapter();
        private readonly SlackAdapter _slackAdapter = new SlackAdapter();

        public bool Verify(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profile.GetDbPassword(accountId);

            var hashPassword = _sha256Adapter.GetHashPassword(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

            if (hashPassword.ToString() == dbPassword && currentOtp == otp)
            {
                _failedCounter.ResetFailedCounter(accountId);

                return true;
            }
            else
            {
                _failedCounter.Add(accountId);

                var failedCount = _failedCounter.Get(accountId);

                _nLogAdapter.log($"verify failed account : {accountId} ,failed count {failedCount}");

                _slackAdapter.Notify("my message");

                return false;
            }
        }
    }
}