using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using Dapper;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        public bool Verify(string accountId, string password, string otp)
        {
            string dbPassword;
            using (var connection = new SqlConnection("my connection string"))
            {
                var password1 = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
                dbPassword = password1;
            }

            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hashPassword = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hashPassword.Append(theByte.ToString("x2"));
            }


            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
            var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"web api error, accountId:{accountId}");

            }

            var correctOtp = response.Content.ReadAsAsync<string>().Result;

            return hashPassword.ToString() == dbPassword && correctOtp == otp;
        }
    }
}