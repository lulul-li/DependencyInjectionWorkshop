using DependencyInjectionWorkshop.Adapter;

namespace DependencyInjectionWorkshop.Models
{
    public class NotifyDecorator:IAuthentication
    {
        private readonly IAuthentication _authentication;
        private readonly INotification _notification;

        public NotifyDecorator(IAuthentication authentication, INotification notification)
        {
            _authentication = authentication;
            _notification = notification;
        }

        public void notify()
        {
            _notification.PushMessage("my message");
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var isVerify = _authentication.Verify(accountId, password, otp);
            if (!isVerify)
            {
                notify();
            }

            return isVerify;
        }
    }
}