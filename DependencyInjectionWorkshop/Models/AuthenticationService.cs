using System.Reflection.Emit;
using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IProfile _profile;
        private readonly IFailedCounter _failedCounter;
        private readonly IHash _hash;
        private readonly IOtp _otp;
        private readonly ILogger _logger;

        public AuthenticationService(IProfile profile, IFailedCounter failedCounter, IHash hash, IOtp otp, ILogger logger)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otp = otp;
            _logger = logger;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var dbPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.GetHash(password);

            var currentOtp = _otp.GetCurrentOtp(accountId);

            if (hashPassword == dbPassword && currentOtp == otp)
            {
                return true;
            }
            else
            {
                _failedCounter.Add(accountId);

                var failedCount = _failedCounter.Get(accountId);

                _logger.Info($"verify failed account : {accountId} ,failed count {failedCount}");

                return false;
            }
        }
    }
}