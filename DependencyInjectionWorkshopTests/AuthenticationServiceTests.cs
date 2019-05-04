using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private AuthenticationService _authenticationService;
        private string _defaultAccountId = "lulu";
        private string _defaultHashPassword = "hash password";
        private string _defaultOtp = "123456";
        private string _defaultPassword = "ps";
        private IFailedCounter _failedCounter;
        private IHash _hash;
        private ILogger _logger;
        private INotification _notification;
        private IOtp _otpService;
        private IProfile _profile;
        private int _failedCount = 22;

        [SetUp]
        public void SetUp()
        {
            _profile = Substitute.For<IProfile>();
            _otpService = Substitute.For<IOtp>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _logger = Substitute.For<ILogger>();
            _hash = Substitute.For<IHash>();
            _authenticationService = new AuthenticationService(_profile, _failedCounter, _hash, _otpService, _logger, _notification);
        }

        [Test]
        public void is_valid()
        {
            GivenPassword(_defaultAccountId, _defaultHashPassword);
            GivenHashPassword(_defaultPassword, _defaultHashPassword);
            GivenOtp(_defaultAccountId, _defaultOtp);

            var isVerify = WhenVerify(_defaultAccountId, _defaultPassword, _defaultOtp);
            ShouldBeValid(isVerify);
        }


        [Test]
        public void is_invalid_when_opt_wrong()
        {
            GivenPassword(_defaultAccountId, _defaultHashPassword);
            GivenHashPassword(_defaultPassword, _defaultHashPassword);
            GivenOtp(_defaultAccountId, _defaultOtp);

            var isVerify = WhenVerify(_defaultAccountId, _defaultPassword, "wrong otp");
            ShouldBeInvalid(isVerify);

        }

        [Test]
        public void notify_user_when_invalid()
        {
            WhenInvalid();
            ShouldNotifyUser();
        }

        [Test]
        public void logger_account_failed_count_when_invalid()
        {
            GivenFailedCount(_defaultAccountId, _failedCount);
            WhenInvalid();
            LogShouldContains(_defaultAccountId, _failedCount);
        }

        private void GivenFailedCount(string accountId, int failedCount)
        {
             _failedCounter.Get(accountId).ReturnsForAnyArgs(failedCount);
        }

        private void LogShouldContains(string accountId, int failedCount)
        {
            _logger.Received(1).Info(Arg.Is<string>(x => x.Contains(accountId) && x.Contains(failedCount.ToString())));
        }

        private void ShouldNotifyUser()
        {
            _notification.Received(1).PushMessage(Arg.Any<string>());
        }

        private bool WhenInvalid()
        {
            GivenPassword(_defaultAccountId, _defaultHashPassword);
            GivenHashPassword(_defaultPassword, _defaultHashPassword);
            GivenOtp(_defaultAccountId, _defaultOtp);
            var isVerify = WhenVerify(_defaultAccountId, _defaultPassword, "wrong otp");
            return isVerify;
        }

        private void ShouldBeInvalid(bool isVerify)
        {
            Assert.IsFalse(isVerify);
        }

        private static void ShouldBeValid(bool isVerify)
        {
            Assert.IsTrue(isVerify);
        }

        private bool WhenVerify(string accountId, string password, string otp)
        {
            return _authenticationService.Verify(accountId, password, otp);
        }

        private void GivenHashPassword(string password, string hashPassword)
        {
            _hash.GetHash(password).ReturnsForAnyArgs(hashPassword);
        }

        private void GivenOtp(string accountId, string otp)
        {
            _otpService.GetCurrentOtp(accountId).ReturnsForAnyArgs(otp);
        }

        private void GivenPassword(string accountId, string password)
        {
            _profile.GetPassword(accountId).ReturnsForAnyArgs(password);
        }
    }
}