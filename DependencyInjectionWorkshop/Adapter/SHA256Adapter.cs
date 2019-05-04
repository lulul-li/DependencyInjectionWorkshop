using System.Security.Cryptography;
using System.Text;

namespace DependencyInjectionWorkshop.Adapter
{
    public class SHA256Adapter
    {
        public StringBuilder GetHashPassword(string password)
        {
            var hashPassword = new StringBuilder();
            var crypto = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hashPassword.Append(theByte.ToString("x2"));
            }

            return hashPassword;
        }
    }
}