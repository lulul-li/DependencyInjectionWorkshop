﻿using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otp;

        public AuthenticationService(IProfile profile, IHash hash, IOtp otp)
        {
            _profile = profile;
            _hash = hash;
            _otp = otp;
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var dbPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.GetHash(password);

            var currentOtp = _otp.GetCurrentOtp(accountId);
            return hashPassword == dbPassword && currentOtp == otp;
        }
    }
}