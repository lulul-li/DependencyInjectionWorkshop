using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Decorate;
using DependencyInjectionWorkshop.Exception;
using DependencyInjectionWorkshop.Models;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private const string DefaultAccountId = "lulu";
        private const string DefaultHashPassword = "hash password";
        private const string DefaultOtp = "123456";
        private const string DefaultPassword = "ps";
        private const int FailedCount = 22;
        private IAuthentication _authenticationService;
        private IFailedCounter _failedCounter;
        private IHash _hash;
        private ILogger _logger;
        private INotification _notification;
        private IOtp _otpService;
        private IProfile _profile;

        [SetUp]
        public void SetUp()
        {
            _profile = Substitute.For<IProfile>();
            _otpService = Substitute.For<IOtp>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _logger = Substitute.For<ILogger>();
            _hash = Substitute.For<IHash>();
            var authenticationService = new AuthenticationService(_profile, _hash, _otpService);
            var failedCounterDecorator = new FailedCounterDecorator(authenticationService, _failedCounter);
            var logDecorator = new LogDecorate(failedCounterDecorator,_failedCounter,_logger);
            _authenticationService = new NotifyDecorator(logDecorator, _notification);
        }

        [Test]
        public void is_valid()
        {
            var isVerify = WhenValid();
            ShouldBeValid(isVerify);
        }

        [Test]
        public void is_invalid_when_opt_wrong()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHashPassword(DefaultPassword, DefaultHashPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isVerify = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
            ShouldBeInvalid(isVerify);
        }

        [Test]
        public void is_invalid_when_password_wrong()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHashPassword("wrong password", "wrong hash password");
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isVerify = WhenVerify(DefaultAccountId, "wrong password", DefaultOtp);
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
            GivenFailedCount(DefaultAccountId, FailedCount);
            WhenInvalid();
            LogShouldContains(DefaultAccountId, FailedCount);
        }

        [Test]
        public void reset_failed_count_when_valid()
        {
            WhenValid();
            _failedCounter.Received(1).Reset(Arg.Any<string>());
        }

        [Test]
        public void add_failed_count_when_invalid()
        {
            WhenInvalid();
            _failedCounter.Received(1).Add(Arg.Any<string>());
        }

        [Test]
        public void throw_failed_too_many_times_exception_when_account_lock()
        {
            GivenAccountIsLock(DefaultAccountId, true);

            TestDelegate verify = () => _authenticationService.Verify(DefaultAccountId, DefaultPassword, DefaultOtp);
            ShouldThrow<FailedTooManyTimesException>(verify);
        }

        private static TResult ShouldThrow<TResult>(TestDelegate verify) where TResult : Exception
        {
            return Assert.Throws<TResult>(verify);
        }

        private static void ShouldBeValid(bool isVerify)
        {
            Assert.IsTrue(isVerify);
        }

        private void GivenAccountIsLock(string accountId, bool isLock)
        {
            _failedCounter.CheckAccountIsLock(accountId).ReturnsForAnyArgs(isLock);
        }

        private bool WhenValid()
        {
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHashPassword(DefaultPassword, DefaultHashPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var isVerify = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);
            return isVerify;
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
            GivenPassword(DefaultAccountId, DefaultHashPassword);
            GivenHashPassword(DefaultPassword, DefaultHashPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);
            return WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
        }

        private void ShouldBeInvalid(bool isVerify)
        {
            Assert.IsFalse(isVerify);
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