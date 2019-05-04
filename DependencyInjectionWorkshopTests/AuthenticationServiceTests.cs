using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;
using NSubstitute;
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
        
        [Test]
        public void is_valid()
        {
            GivenPassword(_defaultAccountId, _defaultHashPassword);
            GivenHashPassword(_defaultPassword, _defaultHashPassword);
            GivenOtp(_defaultAccountId, _defaultOtp);

            var isVerify = _authenticationService.Verify(_defaultAccountId, _defaultPassword, _defaultOtp);
            Assert.IsTrue(isVerify);
        }

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