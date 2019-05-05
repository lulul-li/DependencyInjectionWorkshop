using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class NotifyDecorator : BaseAuthenticateDecorator
    {
        private readonly IAuthentication _authentication;
        private readonly INotification _notification;

        public NotifyDecorator(IAuthentication authentication, INotification notification) : base(authentication)
        {
            _authentication = authentication;
            _notification = notification;
        }

        public void PushMsg()
        {
            _notification.PushMessage("my message");
        }
        public override bool Verify(string accountId, string password, string otp)
        {
            var isVerify = _authentication.Verify(accountId, password, otp);
            if (!isVerify)
            {
                PushMsg();
            }

            return isVerify;
        }
    }
}