namespace DependencyInjectionWorkshop.Models
{
    public class BaseAuthenticateDecorator : IAuthentication
    {
        private readonly IAuthentication _authentication;

        public BaseAuthenticateDecorator(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public virtual bool Verify(string accountId, string password, string otp)
        {
            return _authentication.Verify(accountId, password, otp);
        }
    }
}