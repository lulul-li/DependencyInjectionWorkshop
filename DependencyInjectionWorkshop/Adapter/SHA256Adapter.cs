using System.Security.Cryptography;
using System.Text;

namespace DependencyInjectionWorkshop.Adapter
{
    public interface IHash
    {
        StringBuilder GetHash(string paintText);
    }

    public class SHA256Adapter : IHash
    {
        public StringBuilder GetHash(string paintText)
        {
            var hashPassword = new StringBuilder();
            var crypto = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(paintText));
            foreach (var theByte in crypto)
            {
                hashPassword.Append(theByte.ToString("x2"));
            }

            return hashPassword;
        }
    }
}